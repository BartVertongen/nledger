﻿// **********************************************************************************
// Copyright (c) 2015-2017, Dmitry Merzlyakov.  All rights reserved.
// Licensed under the FreeBSD Public License. See LICENSE file included with the distribution for details and disclaimer.
// 
// This file is part of NLedger that is a .Net port of C++ Ledger tool (ledger-cli.org). Original code is licensed under:
// Copyright (c) 2003-2017, John Wiegley.  All rights reserved.
// See LICENSE.LEDGER file included with the distribution for details and disclaimer.
// **********************************************************************************
using NLedger.Accounts;
using NLedger.Chain;
using NLedger.Expressions;
using NLedger.Output;
using NLedger.Print;
using NLedger.Scopus;
using NLedger.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLedger
{
    public static class Select
    {
        public static bool GetPrincipalIdentifiers(ExprOp expr, ref string ident, bool doTransforms = false)
        {
            bool result = true;

            if (expr.IsIdent)
            {
                string name = expr.AsIdent;

                if (name == "date" || name == "aux_date" || name == "payee")
                {
                    if (!String.IsNullOrEmpty(ident) &&
                        !(name == "date" || name == "aux_date" || name == "payee"))  // TODO - should it be "ident"?
                        result = false;
                    ident = name;
                }
                else if (name == "account")
                {
                    if (!String.IsNullOrEmpty(ident) && !(name == "account"))  // TODO - the same
                        result = false;
                    ident = name;
                    if (doTransforms)
                        expr.AsIdent = "display_account";
                }
                else if (name == "amount")
                {
                    if (!String.IsNullOrEmpty(ident) && !(name == "amount"))    // TODO - the same
                        result = false;
                    ident = name;
                    if (doTransforms)
                        expr.AsIdent = "display_amount";
                }
                else if (name == "total")
                {
                    if (!String.IsNullOrEmpty(ident) && !(name == "total"))    // TODO - the same
                        result = false;
                    ident = name;
                    if (doTransforms)
                        expr.AsIdent = "display_total";
                }
            }

            if (expr.Kind > OpKindEnum.TERMINALS || expr.IsScope)
            {
                if (expr.Left != null)
                {
                    if (!GetPrincipalIdentifiers(expr.Left, ref ident, doTransforms))
                        result = false;
                    if (expr.Kind > OpKindEnum.UNARY_OPERATORS && expr.HasRight)
                    {
                        if (!GetPrincipalIdentifiers(expr.Right, ref ident, doTransforms))
                            result = false;
                    }
                }
            }

            return result;
        }

        public static Value SelectCommand(CallScope args)
        {
            string text = "select " + CallScope.JoinArgs(args);
            if (String.IsNullOrEmpty(text))
                throw new LogicError(LogicError.ErrorMessageUsageSelectText);

            Report report = args.FindScope<Report>();

            // Our first step is to divide the select statement into its principal
            // parts:
            //
            //   SELECT <VALEXPR-LIST>
            //   FROM <NAME>
            //   WHERE <VALEXPR>
            //   DISPLAY <VALEXPR>
            //   COLLECT <VALEXPR>
            //   GROUP BY <VALEXPR>
            //   STYLE <NAME>

            Mask selectRe = new Mask(SelectRe);

            Mask fromAccountsRe = new Mask(FromAccountsRe);
            bool accountsReport = fromAccountsRe.Match(text);

            ExprOp reportFunctor = null;
            StringBuilder formatter = new StringBuilder();

            foreach (var match in selectRe.Matches(text))
            {
                string keyword = match.Groups[1].Value;
                string arg = match.Groups[2].Value;

                if (keyword == "select")
                {
                    Expr argsExpr = new Expr(arg);
                    Value columns = ExprOp.SplitConsExpr(argsExpr.Op);
                    bool first = true;
                    string thusFar = String.Empty;

                    int cols = 0;

                    if (report.ColumnsHandler.Handled)
                        cols = Int32.Parse(report.ColumnsHandler.Value);
                    else
                    {
                        string columnsEnv = Environment.GetEnvironmentVariable("COLUMNS");  // TODO - use list of variables
                        if (!Int32.TryParse(columnsEnv, out cols))
                        {
                            try { cols = Console.WindowWidth; } catch { cols = 0; }   // TODO - virtualize getter.
                            if (cols == 0)
                                cols = 80;
                        }
                    }

                    int dateWidth = report.DateWidthHandler.Handled ? Int32.Parse(report.DateWidthHandler.Str()) :
                        Times.TimesCommon.Current.FormatDate(Times.TimesCommon.Current.CurrentDate, Times.FormatTypeEnum.FMT_PRINTED).Length;
                    int payeeWidth = report.PayeeWidthHandler.Handled ? Int32.Parse(report.PayeeWidthHandler.Str()) :
                        (int)(((double)cols) * 0.263157);
                    int accountWidth = report.AccountWidthHandler.Handled ? Int32.Parse(report.AccountWidthHandler.Str()) :
                        (int)(((double)cols) * 0.302631);
                    int amountWidth = report.AmountWidthHandler.Handled ? Int32.Parse(report.AmountWidthHandler.Str()) :
                        (int)(((double)cols) * 0.157894);
                    int totalWidth = report.TotalWidthHandler.Handled ? Int32.Parse(report.TotalWidthHandler.Str()) :
                        amountWidth;
                    int metaWidth = report.MetaWidthHandler.Handled ? Int32.Parse(report.MetaWidthHandler.Str()) :
                        10;

                    bool sawPayee = false;
                    bool sawAccount = false;

                    int colsNeeded = 0;
                    foreach(Value column in columns.AsSequence)
                    {
                        string ident = null;
                        if (GetPrincipalIdentifiers(Expr.AsExpr(column), ref ident))
                        {
                            if (ident == "date" || ident == "aux_date")
                            {
                                colsNeeded += dateWidth + 1;
                            }
                            else if (ident == "payee")
                            {
                                colsNeeded += payeeWidth + 1;
                                sawPayee = true;
                            }
                            else if (ident == "account")
                            {
                                colsNeeded += accountWidth + 1;
                                sawAccount = true;
                            }
                            else if (ident == "amount")
                            {
                                colsNeeded += amountWidth + 1;
                            }
                            else if (ident == "total")
                            {
                                colsNeeded += totalWidth + 1;
                            }
                            else
                            {
                                colsNeeded += metaWidth + 1;
                            }
                        }
                    }

                    while ((sawAccount || sawPayee) && colsNeeded < cols)
                    {
                        if (sawAccount && colsNeeded < cols)
                        {
                            ++accountWidth;
                            ++colsNeeded;
                            if (colsNeeded < cols)
                            {
                                ++accountWidth;
                                ++colsNeeded;
                            }
                        }
                        if (sawPayee && colsNeeded < cols)
                        {
                            ++payeeWidth;
                            ++colsNeeded;
                        }
                    }

                    while ((sawAccount || sawPayee) && colsNeeded > cols &&
                           accountWidth > 5 && payeeWidth > 5)
                    {
                        if (sawAccount && colsNeeded > cols)
                        {
                            --accountWidth;
                            --colsNeeded;
                            if (colsNeeded > cols)
                            {
                                --accountWidth;
                                --colsNeeded;
                            }
                        }
                        if (sawPayee && colsNeeded > cols)
                        {
                            --payeeWidth;
                            --colsNeeded;
                        }
                    }

                    if (!report.DateWidthHandler.Handled)
                        report.DateWidthHandler.Value = dateWidth.ToString();
                    if (!report.PayeeWidthHandler.Handled)
                        report.PayeeWidthHandler.Value = payeeWidth.ToString();
                    if (!report.AccountWidthHandler.Handled)
                        report.AccountWidthHandler.Value = accountWidth.ToString();
                    if (!report.AmountWidthHandler.Handled)
                        report.AmountWidthHandler.Value = amountWidth.ToString();
                    if (!report.TotalWidthHandler.Handled)
                        report.TotalWidthHandler.Value = totalWidth.ToString();

                    foreach (Value column in columns.AsSequence)
                    {
                        if (first)
                            first = false;
                        else
                            formatter.Append(" ");

                        formatter.Append("%(");

                        string ident = null;
                        if (GetPrincipalIdentifiers(Expr.AsExpr(column), ref ident, true))
                        {
                            if (ident == "date" || ident == "aux_date")
                            {
                                formatter.Append("ansify_if(");
                                formatter.Append("ansify_if(justify(format_date(");

                                PrintColumn(column, formatter);

                                formatter.Append("), int(date_width)),");
                                formatter.Append("green if color and date > today),");
                                formatter.Append("bold if should_bold)");

                                if (!String.IsNullOrEmpty(thusFar))
                                    thusFar += " + ";
                                thusFar += "int(date_width) + 1";
                            }
                            else if (ident == "payee")
                            {
                                formatter.Append("ansify_if(");
                                formatter.Append("ansify_if(justify(truncated(");

                                PrintColumn(column, formatter);

                                formatter.Append(", int(payee_width)), int(payee_width)),");
                                formatter.Append("bold if color and !cleared and actual),");
                                formatter.Append("bold if should_bold)");

                                if (!String.IsNullOrEmpty(thusFar))
                                    thusFar += " + ";
                                thusFar += "int(payee_width) + 1";
                            }
                            else if (ident == "account")
                            {
                                formatter.Append("ansify_if(");

                                if (accountsReport)
                                {
                                    formatter.Append("ansify_if(");
                                    formatter.Append("partial_account(options.flat), blue if color),");
                                }
                                else
                                {
                                    formatter.Append("justify(truncated(");
                                    PrintColumn(column, formatter);
                                    formatter.Append(", int(account_width), int(abbrev_len)),");
                                    formatter.Append("int(account_width), -1, ");
                                    formatter.Append("false, color),");

                                    if (!String.IsNullOrEmpty(thusFar))
                                        thusFar += " + ";
                                    thusFar += "int(account_width) + 1";
                                }

                                formatter.Append(" bold if should_bold)");
                            }
                            else if (ident == "amount" || ident == "total")
                            {
                                formatter.Append("ansify_if(");
                                formatter.Append("justify(scrub(");

                                PrintColumn(column, formatter);

                                formatter.Append("), ");

                                if (ident == "amount")
                                    formatter.Append("int(amount_width),");
                                else
                                    formatter.Append("int(total_width),");

                                if (!String.IsNullOrEmpty(thusFar))
                                    thusFar += " + ";

                                if (ident == "amount")
                                    thusFar += "int(amount_width)";
                                else
                                    thusFar += "int(total_width)";

                                if (String.IsNullOrEmpty(thusFar))
                                    formatter.Append("-1");
                                else
                                    formatter.Append(thusFar);

                                formatter.Append(", true, color),");
                                formatter.Append(" bold if should_bold)");

                                thusFar += " + 1";
                            }
                            else
                            {
                                formatter.Append("ansify_if(");
                                formatter.Append("justify(truncated(");

                                PrintColumn(column, formatter);

                                formatter.Append(", int(meta_width or 10)), int(meta_width) or 10),");
                                formatter.Append("bold if should_bold)");

                                if (!String.IsNullOrEmpty(thusFar))
                                    thusFar += " + ";
                                thusFar += "(int(meta_width) or 10) + 1";
                            }

                        }
                        formatter.Append(")");
                    }
                    formatter.AppendLine();
                }
                else if (keyword == "from")
                {
                    if (arg == "xacts" || arg == "txns" || arg == "transactions")
                    {
                        var reporter = new Reporter<Post, PostHandler>(new PrintXacts(report, report.RawHandler.Handled), report, "#select", report.PostsReport);
                        reportFunctor = ExprOp.WrapFunctor(scope => reporter.Handle((CallScope)scope));
                    }
                    else if (arg == "posts" || arg == "postings")
                    {
                        var reporter = new Reporter<Post, PostHandler>(new FormatPosts(report, formatter.ToString()), report, "#select", report.PostsReport);
                        reportFunctor = ExprOp.WrapFunctor(scope => reporter.Handle((CallScope)scope));
                    }
                    else if (arg == "accounts")
                    {
                        var reporter = new Reporter<Account, AccountHandler>(new FormatAccounts(report, formatter.ToString()), report, "#select", report.AccountsReport);
                        reportFunctor = ExprOp.WrapFunctor(scope => reporter.Handle((CallScope)scope));
                    }
                    else if (arg == "commodities")
                    {
                        var reporter = new Reporter<Post, PostHandler>(new FormatPosts(report, formatter.ToString()), report, "#select", report.CommoditiesReport);
                        reportFunctor = ExprOp.WrapFunctor(scope => reporter.Handle((CallScope)scope));
                    }
                }
                else if (keyword == "where")
                {
                    report.LimitHandler.On("#select", arg);
                }
                else if (keyword == "display")
                {
                    report.DisplayHandler.On("#select", arg);
                }
                else if (keyword == "collect")
                {
                    report.AmountHandler.On("#select", arg);
                }
                else if (keyword == "group by")
                {
                    report.GroupByHandler.On("#select", arg);
                }
                else if (keyword == "style")
                {
                    // TODO - something has not been done in the original code...
                    if (arg == "csv")
                    {
                    }
                    else if (arg == "xml")
                    {
                    }
                    else if (arg == "json")
                    {
                    }
                    else if (arg == "emacs")
                    {
                    }
                    else if (arg == "org")
                    {
                    }
                }
            }

            if (reportFunctor == null)
            {
                var reporter = new Reporter<Post, PostHandler>(new FormatPosts(report, formatter.ToString()), report, "#select", report.PostsReport);
                reportFunctor = ExprOp.WrapFunctor(scope => reporter.Handle((CallScope)scope));
            }

            CallScope callArgs = new CallScope(report);
            return reportFunctor.AsFunction(callArgs);
        }

        private static void PrintColumn(Value column, StringBuilder formatter)
        {
            string s = null;
            Expr.AsExpr(column).Print(ref s);
            formatter.Append(s);
        }

        private const string SelectRe =
            @"(select|from|where|display|collect|group\s+by|style)\s+(.+?)(?=(\s+(from|where|display|collect|group\s+by|style)\s+|$))";
        private const string FromAccountsRe = @"from\s+accounts\b"; 
    }
}
