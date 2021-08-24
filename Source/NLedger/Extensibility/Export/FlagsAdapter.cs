﻿using NLedger.Commodities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLedger.Extensibility.Export
{
    public static class FlagsAdapter
    {
        public static FlagsConverter<Annotate.Annotation> AnnotationFlagsAdapter()
        {
            return new FlagsConverter<Annotate.Annotation>().
                AddMapping(Annotation.ANNOTATION_PRICE_CALCULATED, a => a.IsPriceCalculated, (a, v) => a.IsPriceCalculated = v).
                AddMapping(Annotation.ANNOTATION_PRICE_FIXATED, a => a.IsPriceFixated, (a, v) => a.IsPriceFixated = v).
                AddMapping(Annotation.ANNOTATION_PRICE_NOT_PER_UNIT, a => a.IsPriceNotPerUnit, (a, v) => a.IsPriceNotPerUnit = v).
                AddMapping(Annotation.ANNOTATION_DATE_CALCULATED, a => a.IsDateCalculated, (a, v) => a.IsDateCalculated = v).
                AddMapping(Annotation.ANNOTATION_TAG_CALCULATED, a => a.IsTagCalculated, (a, v) => a.IsTagCalculated = v).
                AddMapping(Annotation.ANNOTATION_VALUE_EXPR_CALCULATED, a => a.IsValueExprCalculated, (a, v) => a.IsValueExprCalculated = v);
        }

        public static FlagsConverter<Accounts.Account> AccountFlagsAdapter()
        {
            return new FlagsConverter<Accounts.Account>().
                AddMapping(Account.ACCOUNT_KNOWN, a => a.IsKnownAccount, (a, v) => a.IsKnownAccount = v).
                AddMapping(Account.ACCOUNT_TEMP, a => a.IsTempAccount, (a, v) => a.IsTempAccount = v).
                AddMapping(Account.ACCOUNT_GENERATED, a => a.IsGeneratedAccount, (a, v) => a.IsGeneratedAccount = v);
        }

        public static FlagsConverter<NLedger.PostXData> PostXDataFlagsAdapter()
        {
            return new FlagsConverter<NLedger.PostXData>().
                AddMapping(PostingXData.POST_EXT_RECEIVED, x => x.Received, (x, v) => x.Received = v).
                AddMapping(PostingXData.POST_EXT_HANDLED, x => x.Handled, (x, v) => x.Handled = v).
                AddMapping(PostingXData.POST_EXT_DISPLAYED, x => x.Displayed, (x, v) => x.Displayed = v).
                AddMapping(PostingXData.POST_EXT_DIRECT_AMT, x => x.DirectAmt, (x, v) => x.DirectAmt = v).
                AddMapping(PostingXData.POST_EXT_SORT_CALC, x => x.SortCalc, (x, v) => x.SortCalc = v).
                AddMapping(PostingXData.POST_EXT_COMPOUND, x => x.Compound, (x, v) => x.Compound = v).
                AddMapping(PostingXData.POST_EXT_VISITED, x => x.Visited, (x, v) => x.Visited = v).
                AddMapping(PostingXData.POST_EXT_MATCHES, x => x.Matches, (x, v) => x.Matches = v).
                AddMapping(PostingXData.POST_EXT_CONSIDERED, x => x.Considered, (x, v) => x.Considered = v);
        }

        public static int CommodityFlagsToInt(CommodityFlagsEnum flags) => (int)flags;

        public static int SupportsFlagsToInt(SupportsFlagsEnum flags) => (int)flags;
    }
}
