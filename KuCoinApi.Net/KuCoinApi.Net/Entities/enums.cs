using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KuCoinApi.Net.Entities
{
    public enum Interval
    {
        None,
        [Description("1min")]
        OneM,
        [Description("3min")]
        ThreeM,
        [Description("5min")]
        FiveM,
        [Description("15min")]
        FifteenM,
        [Description("30min")]
        ThirtyM,
        [Description("1hour")]
        OneH,
        [Description("2hour")]
        TwoH,
        [Description("4hour")]
        FourH,
        [Description("6hour")]
        SixH,
        [Description("8hour")]
        EightH,
        [Description("12hour")]
        TwelveH,
        [Description("1day")]
        OneD,
        [Description("1week")]
        OneW
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

    public enum WithdrawalStatus
    {
        PROCESSING,
        WALLET_PROCESSING,
        SUCCESS,
        FAILURE
    }
}
