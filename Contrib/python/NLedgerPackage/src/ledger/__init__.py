import ledger.conf
from ledger.conf import clrRuntime
from ledger.conf import ClrRuntime

from clr_loader import get_coreclr
from clr_loader import get_mono
from clr_loader import get_netfx

from pythonnet import set_runtime

# Set preferrable runtime and load clr

if clrRuntime == ClrRuntime.core:
    set_runtime(get_coreclr(ledger.conf.corefile))
elif clrRuntime == ClrRuntime.mono:
    set_runtime(get_mono())
elif clrRuntime == ClrRuntime.netfx:
    set_runtime(get_netfx())

import clr

# Load library dll

#Adding path to runtime dll to PATH and sending only name fixes a problem in pythonnet:
#it tries to use Assembly.Load for loading an assembly specified by path and name
import ntpath
import sys

dllFolder = ntpath.dirname(ledger.conf.runtimefile)
dllName = ntpath.splitext(ntpath.basename(ledger.conf.runtimefile))[0]

if not(dllFolder in sys.path):
   sys.path.append(dllFolder)

clr.AddReference(dllName)

# Import library classes and specifying custom helper methods

# Module initialization

from NLedger.Extensibility.Python import PythonSession
PythonSession.PythonModuleInitialization()

# Export Ledger classes and globals

from NLedger.Extensibility.Export import CommodityPool as ExportedCommodityPool
from NLedger.Extensibility.Export import Commodity as ExportedCommodity
from NLedger.Extensibility.Export import KeepDetails as ExportedKeepDetails
from NLedger.Extensibility.Export import PricePoint
from NLedger.Extensibility.Export import Annotation as ExportedAnnotation
from NLedger.Extensibility.Export import AnnotatedCommodity
from NLedger.Extensibility.Export import Amount as ExportedAmount
from NLedger.Extensibility.Export import ParseFlags
from NLedger.Extensibility.Export import ValueType
from NLedger.Extensibility.Export import Value
from NLedger.Extensibility.Export import ValueType
from NLedger.Extensibility.Export import Account
from NLedger.Extensibility.Export import AccountXData
from NLedger.Extensibility.Export import AccountXDataDetails
from NLedger.Extensibility.Export import Balance
from NLedger.Extensibility.Export import Expr
from NLedger.Extensibility.Export import FileInfo
from NLedger.Extensibility.Export import Position
from NLedger.Extensibility.Export import Journal
from NLedger.Extensibility.Export import JournalItem
from NLedger.Extensibility.Export import State
from NLedger.Extensibility.Export import Mask
from NLedger.Extensibility.Export import Posting
from NLedger.Extensibility.Export import PostingXData
from NLedger.Extensibility.Export import SymbolKind
from NLedger.Extensibility.Export import Session
from NLedger.Extensibility.Export import SessionScopeAttributes
from NLedger.Extensibility.Export import SortValue
from NLedger.Extensibility.Export import Times
from NLedger.Extensibility.Export import Transaction
from NLedger.Extensibility.Export import AutomatedTransaction
from NLedger.Extensibility.Export import PeriodicTransaction

#commodities = ExportedCommodityPool.commodities
COMMODITY_STYLE_DEFAULTS = ExportedCommodityPool.COMMODITY_STYLE_DEFAULTS
COMMODITY_STYLE_SUFFIXED = ExportedCommodityPool.COMMODITY_STYLE_SUFFIXED
COMMODITY_STYLE_SEPARATED = ExportedCommodityPool.COMMODITY_STYLE_SEPARATED
COMMODITY_STYLE_DECIMAL_COMMA = ExportedCommodityPool.COMMODITY_STYLE_DECIMAL_COMMA
COMMODITY_STYLE_TIME_COLON = ExportedCommodityPool.COMMODITY_STYLE_TIME_COLON
COMMODITY_STYLE_THOUSANDS = ExportedCommodityPool.COMMODITY_STYLE_THOUSANDS
COMMODITY_NOMARKET = ExportedCommodityPool.COMMODITY_NOMARKET
COMMODITY_BUILTIN = ExportedCommodityPool.COMMODITY_BUILTIN
COMMODITY_WALKED = ExportedCommodityPool.COMMODITY_WALKED
COMMODITY_KNOWN = ExportedCommodityPool.COMMODITY_KNOWN
COMMODITY_PRIMARY = ExportedCommodityPool.COMMODITY_PRIMARY

ANNOTATION_PRICE_CALCULATED = ExportedAnnotation.ANNOTATION_PRICE_CALCULATED
ANNOTATION_PRICE_FIXATED = ExportedAnnotation.ANNOTATION_PRICE_FIXATED
ANNOTATION_PRICE_NOT_PER_UNIT = ExportedAnnotation.ANNOTATION_PRICE_NOT_PER_UNIT
ANNOTATION_DATE_CALCULATED = ExportedAnnotation.ANNOTATION_DATE_CALCULATED
ANNOTATION_TAG_CALCULATED = ExportedAnnotation.ANNOTATION_TAG_CALCULATED
ANNOTATION_VALUE_EXPR_CALCULATED = ExportedAnnotation.ANNOTATION_VALUE_EXPR_CALCULATED

NULL_VALUE = Value.NULL_VALUE

ACCOUNT_NORMAL = Account.ACCOUNT_NORMAL
ACCOUNT_KNOWN = Account.ACCOUNT_KNOWN
ACCOUNT_TEMP = Account.ACCOUNT_TEMP
ACCOUNT_GENERATED = Account.ACCOUNT_GENERATED

ACCOUNT_EXT_SORT_CALC = AccountXData.ACCOUNT_EXT_SORT_CALC
ACCOUNT_EXT_HAS_NON_VIRTUALS = AccountXData.ACCOUNT_EXT_HAS_NON_VIRTUALS
ACCOUNT_EXT_HAS_UNB_VIRTUALS = AccountXData.ACCOUNT_EXT_HAS_UNB_VIRTUALS
ACCOUNT_EXT_AUTO_VIRTUALIZE = AccountXData.ACCOUNT_EXT_AUTO_VIRTUALIZE
ACCOUNT_EXT_VISITED = AccountXData.ACCOUNT_EXT_VISITED
ACCOUNT_EXT_MATCHING = AccountXData.ACCOUNT_EXT_MATCHING
ACCOUNT_EXT_TO_DISPLAY = AccountXData.ACCOUNT_EXT_TO_DISPLAY
ACCOUNT_EXT_DISPLAYED = AccountXData.ACCOUNT_EXT_DISPLAYED

ITEM_NORMAL = JournalItem.ITEM_NORMAL
ITEM_GENERATED = JournalItem.ITEM_GENERATED
ITEM_TEMP = JournalItem.ITEM_TEMP

POST_VIRTUAL = Posting.POST_VIRTUAL
POST_MUST_BALANCE = Posting.POST_MUST_BALANCE
POST_CALCULATED = Posting.POST_CALCULATED
POST_COST_CALCULATED = Posting.POST_COST_CALCULATED

POST_EXT_RECEIVED = PostingXData.POST_EXT_RECEIVED
POST_EXT_HANDLED = PostingXData.POST_EXT_HANDLED
POST_EXT_DISPLAYED = PostingXData.POST_EXT_DISPLAYED
POST_EXT_DIRECT_AMT = PostingXData.POST_EXT_DIRECT_AMT
POST_EXT_SORT_CALC = PostingXData.POST_EXT_SORT_CALC
POST_EXT_COMPOUND = PostingXData.POST_EXT_COMPOUND
POST_EXT_VISITED = PostingXData.POST_EXT_VISITED
POST_EXT_MATCHES = PostingXData.POST_EXT_MATCHES
POST_EXT_CONSIDERED = PostingXData.POST_EXT_CONSIDERED


def string_value(str):
    return Value.string_value(str)

def mask_value(str):
    return Value.mask_value(str)

def value_context(str):
    return Value.value_context(str)

# Session scope attributes and functions

session = SessionScopeAttributes.session

def read_journal(pathName):
    return SessionScopeAttributes.read_journal(pathName)

def read_journal_from_string(data):
    return SessionScopeAttributes.read_journal_from_string(data)

# Times functions

def parse_datetime(str):
    return Times.parse_datetime(str)

def parse_date(str):
    return Times.parse_date(str)

def times_initialize():
    Times.times_initialize()

def times_shutdown():
    Times.times_shutdown()


# Wrappers for NLedger exported classes
# They extend base export definitions with Python-related specifics
# Basically, python code is needed to manage two cases: 
# a) when operation result is an object of .Net class and we need to wrap it up (e.g. overloaded operators like +); 
# b) when an operation parameter is a python type and cannot be implicitly associated with a .Net method (e.g. datetime.datetime vs DateTime)

from NLedger.Extensibility.Export import FlagsAdapter
from NLedger.Amounts import Amount as OriginAmount
from NLedger.Commodities import CommodityPool as OriginCommodityPool
from NLedger.Commodities import Commodity as OriginCommodity
from NLedger.Annotate import Annotation as OriginAnnotation
from NLedger.Annotate import AnnotationKeepDetails as OriginAnnotationKeepDetails

# Manage date conversions

from datetime import datetime
from datetime import date

from NLedger.Utility import Date
from System import DateTime
from System.Globalization import DateTimeStyles

# Converts to Python date
def to_pdate(value) -> date:
    if value is None:
        return None
    elif isinstance(value, DateTime):
        return date(value.Year, value.Month, value.Day)
    elif isinstance(value, Date):
        return date(value.Year, value.Month, value.Day)
    elif isinstance(value, datetime):
        return value.date()
    elif isinstance(value, date):
        return value
    else:
        raise Exception("Date value is expected")

# Converts to Python datetime
def to_pdatetime(value) -> datetime:
    if value is None:
        return None
    elif isinstance(value, DateTime):
        return datetime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond)
    elif isinstance(value, Date):
        return datetime(value.Year, value.Month, value.Day, 0, 0, 0, 0)
    elif isinstance(value, datetime):
        return value
    elif isinstance(value, date):
        return datetime.combine(value, datetime.min.time())
    else:
        raise Exception("Date value is expected")

# Converts to .Net Ledger Date
def to_ndate(value) -> Date:
    if value is None:
        return None
    elif isinstance(value, DateTime):
        return Date(value.Year, value.Month, value.Day)
    elif isinstance(value, Date):
        return value
    elif isinstance(value, datetime):
        return Date(value.year, value.month, value.day)
    elif isinstance(value, date):
        return Date(value.year, value.month, value.day)
    else:
        raise Exception("Date value is expected")

# Converts to .Net DateTime
def to_ndatetime(value) -> DateTime:
    if value is None:
        return None
    elif isinstance(value, DateTime):
        return value
    elif isinstance(value, Date):
        return DateTime(value.Year, value.Month, value.Day)
    elif isinstance(value, datetime):
        return DateTime(value.year, value.month, value.day, value.hour, value.minute, value.second, value.microsecond)
    elif isinstance(value, date):
        return DateTime(value.year, value.month, value.day)
    else:
        raise Exception("Date value is expected")

def to_amount(value):
    return value if type(value) == Amount else Amount(value)

# Amounts

class Amount(OriginAmount):

    def exact(value):
        return Amount(OriginAmount.Exact(value))

    def __eq__(self, o: object) -> bool:
        return super().__eq__(to_amount(o))

    def __ne__(self, o: object) -> bool:
        return super().__ne__(to_amount(o))

    def __lt__(self, o: object) -> bool:
        return super().__lt__(to_amount(o))

    def __le__(self, o: object) -> bool:
        return super().__le__(to_amount(o))

    def __gt__(self, o: object) -> bool:
        return super().__gt__(to_amount(o))

    def __ge__(self, o: object) -> bool:
        return super().__ge__(to_amount(o))

    def __neg__(self):
        return Amount(super().__neg__())

    def __bool__(self) -> bool:
        return self.IsNonZero

    __nonzero__ = __bool__

    def __add__(self, o: object):
        return Amount(super().__add__(to_amount(o)))

    def __radd__(self, o: object):
        return Amount(super().__radd__(to_amount(o)))

    def __sub__(self, o: object):
        return Amount(super().__sub__(to_amount(o)))

    def __rsub__(self, o: object):
        return Amount(super().__rsub__(to_amount(o)))

    def __mul__(self, o: object):
        return Amount(super().__mul__(to_amount(o)))

    def __rmul__(self, o: object):
        return Amount(super().__rmul__(to_amount(o)))

    def __truediv__(self, o: object):
        return Amount(super().__truediv__(to_amount(o)))

    def __rtruediv__(self, o: object):
        return Amount(super().__rtruediv__(to_amount(o)))

    @property
    def precision(self):
        return super().Precision

    @property
    def display_precision(self):
        return super().DisplayPrecision

    @property
    def keep_precision(self):
        return super().KeepPrecision

    @keep_precision.setter
    def keep_precision(self, value):
        return super().SetKeepPrecision(value)

    def negated(self):
        return Amount(self.Negated())

    def in_place_negate(self):
        self.InPlaceNegate()
        return self

    def abs(self):
        return Amount(self.Abs())

    __abs__ = abs

    def inverted(self):
        return Amount(self.Inverted())

    def rounded(self):
        return Amount(self.Rounded())

    def in_place_round(self):
        self.InPlaceRound()
        return self

    def truncated(self):
        return Amount(self.Truncated())

    def in_place_truncate(self):
        self.InPlaceTruncate()
        return self

    def floored(self):
        return Amount(self.Floored())

    def in_place_floor(self):
        self.InPlaceFloor()
        return self

    def unrounded(self):
        return Amount(self.Unrounded())

    def in_place_unround(self):
        self.InPlaceUnround()
        return self

    def reduced(self):
        return Amount(self.Reduced())

    def in_place_reduce(self):
        self.InPlaceReduce()
        return self

    def unreduced(self):
        return Amount(self.Unreduced())

    def in_place_unreduce(self):
        self.InPlaceUnreduce()
        return self

    # TBC

# Commodities

class CommodityPool:

    origin: None

    def __init__(self,origin) -> None:
        assert isinstance(origin, OriginCommodityPool)
        self.origin = origin

    @property
    def null_commodity(self):
        return Commodity(self.origin.NullCommodity)

    @property
    def default_commodity(self):
        return Commodity.from_origin(self.origin.DefaultCommodity)

    @default_commodity.setter
    def set_default_commodity(self, value):
        self.origin.DefaultCommodity = value

    @property
    def keep_base(self):
        return self.origin.KeepBase

    @keep_base.setter
    def keep_base(self, value):
        self.origin.KeepBase = value

    @property
    def price_db(self):
        return self.origin.PriceDb

    @price_db.setter
    def price_db(self, value):
        self.origin.PriceDb = value

    @property
    def quote_leeway(self):
        return self.origin.QuoteLeeway

    @quote_leeway.setter
    def quote_leeway(self, value):
        self.origin.QuoteLeeway = value

    @property
    def get_quotes(self):
        return self.origin.GetQuotes

    @get_quotes.setter
    def get_quotes(self, value):
        self.origin.GetQuotes = value

    def create(self, symbol):
        return Commodity(self.origin.Create(symbol))

    def find_or_create(self, symbol):
        return Commodity(self.origin.FindOrCreate(symbol))

    # TBC

commodities = CommodityPool(OriginCommodityPool.Current)

class Annotation:

    origin: None
    flags_adapter = FlagsAdapter.AnnotationFlagsAdapter()

    def __init__(self,origin) -> None:
        assert isinstance(origin, OriginAnnotation)
        self.origin = origin

    def __eq__(self, o: object) -> bool:
        return self.origin.Equals(o.origin)

    def __ne__(self, o: object) -> bool:
        return not self.origin.Equals(o.origin)

    def __bool__(self) -> bool:
        return not OriginAnnotation.IsNullOrEmpty(self.origin)

    __nonzero__ = __bool__

    @property
    def flags(self):
        return self.flags_adapter.GetFlags(self.origin)

    @flags.setter
    def flags(self,value):
        return self.flags_adapter.SetFlags(self.origin, value)

    def has_flags(self,flag):
        return self.flags_adapter.HasFlags(self.origin, flag)

    def clear_flags(self):
        return self.flags_adapter.ClearFlags(self.origin)

    def add_flags(self,flag):
        return self.flags_adapter.AddFlags(self.origin,flag)

    def drop_flags(self,flag):
        return self.flags_adapter.DropFlags(self.origin,flag)

    @property
    def price(self):
        return Amount(self.origin.Price)

    @price.setter
    def price(self,value):
        self.origin.Price = value

    @property
    def date(self):
        return to_pdate(self.origin.Date)

    @date.setter
    def date(self,value):
        self.origin.Date = to_ndate(value)

    @property
    def tag(self):
        return self.origin.Tag

    @tag.setter
    def tag(self,value):
        self.origin.Tag = value

    def valid(self) -> bool:
        return True # valid() is not implemented in NLedger's annotation

class KeepDetails:

    origin: None

    def __init__(self, keepPrice = False, keepDate = False, keepTag = False, onlyActuals = False, origin = None) -> None:

        if not(origin is None):
            assert isinstance(origin, OriginAnnotationKeepDetails)
            self.origin = origin
        else:
            self.origin = OriginAnnotationKeepDetails(keepPrice, keepDate, keepTag, onlyActuals)

    @classmethod
    def from_origin(cls, origin):
        return KeepDetails(origin=origin) if not origin is None else None

    @property
    def keep_price(self) -> bool:
        return self.origin.KeepPrice

    @keep_price.setter
    def keep_price(self,value):
        self.origin.KeepPrice = value

    @property
    def keep_date(self) -> bool:
        return self.origin.KeepDate

    @keep_date.setter
    def keep_date(self,value):
        self.origin.KeepDate = value

    @property
    def keep_tag(self) -> bool:
        return self.origin.KeepTag

    @keep_tag.setter
    def keep_tag(self,value):
        self.origin.KeepTag = value

    @property
    def only_actuals(self) -> bool:
        return self.origin.OnlyActuals

    @only_actuals.setter
    def only_actuals(self,value):
        self.origin.OnlyActuals = value

    def keep_all(self, comm = None):
        return self.origin.KeepAll() if comm is None else self.origin.KeepAll(comm.origin)

    def keep_any(self, comm = None):
        return self.origin.KeepAny() if comm is None else self.origin.KeepAny(comm.origin)


class Commodity:

    origin: None

    def __init__(self,origin) -> None:
        assert isinstance(origin, OriginCommodity)
        self.origin = origin

    @classmethod
    def from_origin(cls, origin):
        return Commodity(origin=origin) if not origin is None else None

    @property
    def symbol(self):
        return self.origin.Symbol

# Routine to acquire and release output streams

from io import StringIO

class RedirectWrapperIO(StringIO):

    is_error = False

    def __init__(self, is_error):
        self.is_error = is_error

    def write(self,s):
        return PythonSession.ConsoleWrite(s, self.is_error)

_stdout = sys.stdout
_stderr = sys.stderr

def acquire_output_streams():
    sys.stdout = RedirectWrapperIO(False)
    sys.stderr = RedirectWrapperIO(True)

def release_output_streams():
    sys.stdout = _stdout
    sys.stderr = _stderr
