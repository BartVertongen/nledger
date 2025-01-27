﻿// **********************************************************************************
// Copyright (c) 2015-2021, Dmitry Merzlyakov.  All rights reserved.
// Licensed under the FreeBSD Public License. See LICENSE file included with the distribution for details and disclaimer.
// 
// This file is part of NLedger that is a .Net port of C++ Ledger tool (ledger-cli.org). Original code is licensed under:
// Copyright (c) 2003-2021, John Wiegley.  All rights reserved.
// See LICENSE.LEDGER file included with the distribution for details and disclaimer.
// **********************************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace NLedger.Utility
{
    public static class StringExtensions
    {
		/// <summary>
		/// First parse of a line in REPL or the arguments in the Commandline.
		/// </summary>
		/// <param name="line">input line or the arguments</param>
		/// <returns>A list of strings</returns>
		/// <exception cref="LogicError"></exception>
		public static IEnumerable<string> SplitArguments(string line)
        {
            line = line ?? String.Empty;
            IList<string> args = new List<string>();

            char[] buf = new char[4096];    //maximum line length
            int q = 0; // current position in the buffer
            char inQuotedString = default(char);    //default char is the nul-char

            for (int p = 0; p < line.Length; p++)
            {
                if (inQuotedString == default(char) && char.IsWhiteSpace(line[p]))
                {
                    if (q != 0)
                    {
                        args.Add(new string(buf, 0, q));
                        q = 0;
                    }
                }
                else if (inQuotedString != '\'' && line[p] == '\\')
                {
                    p++;
                    if (p == line.Length)
                        throw new LogicError(LogicError.ErrorMessageInvalidUseOfBackslash);
                    buf[q++] = line[p];
                }
                else if (inQuotedString != '"' && line[p] == '\'')
                {
                    if (inQuotedString == '\'')
                        inQuotedString = default(char);
                    else
                        inQuotedString = '\'';
                }
                else if (inQuotedString != '\'' && line[p] == '"')
                {
                    if (inQuotedString == '"')
                        inQuotedString = default(char);
                    else
                        inQuotedString = '"';
                }
                else
                {
                    buf[q++] = line[p];
                }
            }

            if (inQuotedString != default(char))
                throw new LogicError(String.Format(LogicError.ErrorMessageUnterminatedStringExpectedSmth, inQuotedString));
            // We put away the last part
            if (q != 0)
                args.Add(new string(buf, 0, q));

            return args;
        }

        /// <summary>
        /// Just a substring but with extra checks
        /// </summary>
        /// <param name="s"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string SafeSubstring(this string s, int startIndex, int length)
        {
            if (String.IsNullOrEmpty(s))
                return String.Empty;

            return s.Length > startIndex ? s.Substring(startIndex, length) : String.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static string SafeSubstring(this string s, int startIndex)
        {
            if (String.IsNullOrEmpty(s))
                return String.Empty;

            return s.Length > startIndex ? s.Substring(startIndex) : String.Empty;
        }

        /// <summary>
        /// Ported from next_element
        /// </summary>
        public static string NextElement(ref string s, bool variable = false)
        {
            if (String.IsNullOrEmpty(s))
                return String.Empty;

            int startIndex = 0;
            while (true)
            {
                int pos = s.IndexOfAny(CharExtensions.WhitespaceChars, startIndex);
                if (pos == -1)
                    return String.Empty;

                if (!variable || s[pos] == '\t' || pos == s.Length - 1 || s[pos + 1] == ' ')
                {
                    string element = s.Substring(pos).TrimStart();
                    s = s.Substring(0, pos);
                    return element;
                }
                else
                {
                    startIndex = pos + 2;
                }
            }
        }

        public static string SplitBySeparator(ref string s, char separator, bool variable = false)
        {
            if (String.IsNullOrEmpty(s))
                return String.Empty;

            int startIndex = 0;
            while (true)
            {
                int pos = s.IndexOf(separator, startIndex);

                if (pos == -1)
                    return String.Empty;

                if (!variable || (pos > 2 && (s[pos-1] == '\t' || String.IsNullOrWhiteSpace(s.Substring(pos-2, 2)))))
                {
                    string element = s.Substring(pos + 1);
                    s = s.Substring(0, pos).TrimEnd();
                    return element;
                }
                else
                {
                    startIndex = pos + 1;
                }
            }
        }

        public static string ReadInto(ref string s, Func<char, bool> condition)
        {
            if (condition == null)
                throw new ArgumentNullException("condition");

            if (String.IsNullOrEmpty(s))
                return String.Empty;

            for (int i = 0; i < s.Length; i++)
            {
                if (!condition(s[i]))
                {
                    string result = s.Substring(0, i).EncodeEscapeSequenecs();
                    s = s.Substring(i);
                    return result;
                }
            }

            string finalResult = s;
            s = string.Empty;
            return finalResult;
        }

        public static string EncodeEscapeSequenecs(this string s)
        {
            if (!String.IsNullOrEmpty(s) && s.IndexOf('\\') >= 0)
                return s.
                        Replace(@"\b", "\b").   //bell
                        Replace(@"\f", "\f").   //feed
                        Replace(@"\n", "\n").   //newline
                        Replace(@"\r", "\r").   //return
                        Replace(@"\t", "\t").   //tab
                        Replace(@"\v", "\v");   //vertical tab
            else
                return s ?? String.Empty;
        }

        public static bool StartsWithDigit(this string s)
        {
            return !String.IsNullOrEmpty(s) && Char.IsDigit(s[0]);
        }

        public static bool StartsWithWhiteSpace(this string s)
        {
            return !String.IsNullOrEmpty(s) && Char.IsWhiteSpace(s[0]);
        }

        public static string GetWidthAlignFormatString(int width = 0, bool alignRight = false)
        {
            int key = alignRight ? width : -width;

            return WidthAlignFormatStrings.GetOrAdd(key, w =>
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("{0");
                if (width > 0)
                {
                    sb.Append(",");
                    if (alignRight)
                        sb.Append("-");
                    sb.Append(width);
                }
                sb.Append("}");

                return sb.ToString();
            });
        }

        public static string GetFirstLine(this string s)
        {
            return String.IsNullOrEmpty(s) ? s : s.Split(new[] { '\r', '\n' }).FirstOrDefault();
        }

        public static string GetWord(ref string s)
        {
            s = s?.TrimStart();
            if (String.IsNullOrEmpty(s))
                return String.Empty;

            var pos = s.IndexOfAny(CharExtensions.WhitespaceChars);
            if (pos >= 0)
            {
                var word = s.Substring(0, pos);
                s = s.Substring(pos).TrimStart();
                return word;
            }
            else
            {
                var word = s;
                s = String.Empty;
                return word;
            }
        }

        private static readonly ConcurrentDictionary<int, string> WidthAlignFormatStrings = new ConcurrentDictionary<int, string>();
    }
}