# KuCoinApi.Net v2.0
.Net Standard library for accessing the [KuCoin Exchange](https://www.kucoin.com/#/?r=1ds25) version 2.0 api  
  
This library is available on NuGet for download:  
```
PM> Install-Package KuCoinApi.Net -Version 0.1.0
```


To trade, log into your KuCoin account and create an api key with trading permissions:  
Account -> API Keys -> Create (with Read Information & Trading Authority)  
** if you wish to use withdraw endpoint, you need to enable withdraws on your API key  
  
Initialization:  
Non-secured endpoints only:  
```
IKuCoinDotNet kucoin = new KuCoinDotNet();
```  
  
Secure & non-secure endpoints:  
```
IKuCoinDotNet kucoin = new KuCoinDotNet("api-key", "api-secret", "api-password");
```  
or
```
create config file config.json
{
  "apiKey": "api-key",
  "apiSecret": "api-secret",
  "apiPassword": "api-password"
}
IKuCoinDotNet kucoin = new KuCoinDotNet("/path-to/config.json");
```

#### All endpoints are asyncronous.  

Using an endpoint:  
```  
var balances = await kucoin.GetBalances();
```  

Non-secure endpoints:  
GetMarkets() - Get list of markets on exchange  
GetTradingPairDetails() - Get details on trading pairs  
GetTicker() - Get Ticker for a trading pair  
GetPartOrderBook() - Get best 100 bids and asks on order book  
GetFullOrderBook() - Get all bids and asks on order book  
GetEntireOrderBook() - Get full depth order book  
GetTradeHistory() - Get latest trades for a trading pair  
GetCandlestick() - Get candlesticks for a trading pair  
Get24HrStats() - Get 24 hour statistics for a trading pair    
GetCurrencies() - Get known currencies  
GetCurrency() - Get currency detail  
GetServerTime() - Get current KuCoin server time  

Secure endpoints:  
GetBalances() - Get current asset balances  
GetAccountHistory() - Get account history  
GetHolds() - Get funds on hold  
InnerTransfer() - Send funds between main and trade accounts  
PlaceLimitOrder() - Place a limit order  
PlaceMarketOrder() - Place a market order  
PlaceStopOrder() - Place a stop order  
PlaceOrder() - Place an order  
CancelOrder() - Cancel an active order  
CancelAllOrders() - Cancel all active orders  
GetOrders() - Get multiple orders
GetOrder() - Get an order  
GetOpenOrders() - Get active orders  
GetFills() - Get fills for orders  
CreateDepositAddress() - Create a deposit address  
GetDepositAddress() - Get a deposit address
GetWithdrawalHistory() - Get withdrawal history  
GetWithdrawalQuota() - Get withdrawal quotas  
Withdrawal() - Withdrawal funds  
CancelWithdrawal() - Cancel a withdrawal  

KCS:  
0x011bd184fc7fd1964702844ffd668318f7c3d4c4  
ETH:  
0x3c8e741c0a2Cb4b8d5cBB1ead482CFDF87FDd66F  
BTC:  
1MGLPvTzxK9argeNRTHJ9EZ3WtGZV6nxit  
XLM:  
GA6JNJRSTBV54W3EGWDAWKPEGGD3QCXIGEHMQE2TUYXUKKTNKLYWEXVV  
NEO:  
AHtB1D5UtMiTJbDTn5pfJdPit77de19oao  
