using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KuCoinApi.Net.Entities
{
    public enum Interval
    {
        None,
        [Description("1m")]
        OneM,
        [Description("3m")]
        ThreeM,
        [Description("5m")]
        FiveM,
        [Description("15m")]
        FifteenM,
        [Description("30m")]
        ThirtyM,
        [Description("1h")]
        OneH,
        [Description("2h")]
        TwoH,
        [Description("4h")]
        FourH,
        [Description("6h")]
        SixH,
        [Description("8h")]
        EightH,
        [Description("12h")]
        TwelveH,
        [Description("1d")]
        OneD,
        [Description("3d")]
        ThredD,
        [Description("1w")]
        OneW,
        [Description("1M")]
        OneMo
    }

    public enum TradeType
    {
        [Description("NONE")]
        NONE,
        [Description("BUY")]
        BUY,
        [Description("VOLUMEBUY")]
        VOLUMEBUY,
        [Description("VOLUMEBUYSELLOFF")]
        VOLUMEBUYSELLOFF,
        [Description("SELL")]
        SELL,
        [Description("VOLUMESELL")]
        VOLUMESELL,
        [Description("VOLUMESELLBUYOFF")]
        VOLUMESELLBUYOFF,
        [Description("STOPLOSS")]
        STOPLOSS,
        [Description("ORDERBOOKBUY")]
        ORDERBOOKBUY,
        [Description("ORDERBOOKSELL")]
        ORDERBOOKSELL,
        [Description("CANCELTRADE")]
        CANCELTRADE
    }

    public enum DWType
    {
        DEPOSIT,
        WITHDRAW
    }

    public enum DWStatus
    {
        //FINISHED,
        CANCEL,
        PENDING,
        SUCCESS,
        NONE
    }

    public enum Side
    {
        BUY,
        SELL
    }

    public enum AccountType
    {
        Main,
        Trade
    }

    public enum TimeInForce
    {
        GTC,
        GTT,
        IOC,
        FOK
    }

    public enum OrderType
    {
        LIMIT,
        MARKET,
        LIMIT_STOP,
        MARKET_STOP
    }

    public enum StopType
    {
        ENTRY,
        LOSS
    }

    public enum SelfTradeProtect
    {
        CN,
        CO,
        CB,
        DC
    }

    public enum OrderStatus
    {
        DONE,
        ACTIVE
    }

    public enum DepositStatus
    {
        PROCESSING,
        SUCCESS,
        FAILURE
    }
}
