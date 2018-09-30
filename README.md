# KuCoinApi.NetCore
.Net Core library for accessing the [KuCoin Exchange](https://www.kucoin.com/#/?r=1ds25) api  
  
This library is available on NuGet for download: https://www.nuget.org/packages/KuCoinApi.NetCore  
```
PM> Install-Package KuCoinApi.NetCore
```

  
To trade, log into your KuCoin account and create an api key with trading permissions:  
Account -> API Keys -> Create (with Read Information & Trading Authority)  
  
Initialization:  
Non-secured endpoints only:  
```
var kucoin = new KuCoinClient();
```  
  
Secure & non-secure endpoints:  
```
var kucoin = new KuCoinClient("api-key", "api-secret");
```  
or
```
create config file config.json
{
  "apiKey": "api-key",
  "apiSecret": "api-secret"
}
var kucoin = new KuCoinClient("/path-to/config.json");
```

Using an endpoint:  
```  
var balance = await kucoin.GetBalanceAsync();
```  
or  
```
var balance = kucoin.GetBalance();
```

Non-secure endpoints:  
GetCandlesticks() | GetCandlesticksAsync() - Get charting candlesticks  
GetOrderBook() | GetOrderBookAsync() - Get current order book for a trading pair  
GetTick() | GetTickAsync()- Get tick for a trading pair  
GetTicks() | GetTicksAsync() - Get ticker for all trading pairs  
GetMarkets() | GetMarketsAsync() - Get markets trading on exchange  
GetTradingSymbolTick() GetTradingSymbolTickAsync() - Get details for all coins  
GetTradingPairs() | GetTradingPairsAsync() - Get all trading pairs on exchange  
GetCoin() | GetCoinAsync() - Get information about a coin
GetCoins() | GetCoinsAsync() - Get information about all coins
GetTrendings() | GetTrendingsAsync() - Get open sells for all pairs or a market  
GetKuCoinTime() - Get current KuCoin server time  

Secure endpoints:  
GetBalance() | GetBalanceAsync() - Get current asset balances  
GetOrder() | GetOrderAsync() - Get information for an order  
GetOrders() | GetOrdersAsync() - Get all current user order information  
GetOpenOrders() | GetOpenOrdersAsync() - Get all current user open orders   
PostTrade() | PostTradeAsync() - Post a new trade  
DeleteTrade() | DeleteTradeAsync() - Delete a current open trade  
GetDepositAddress() | GetDepositAddressAsync() - Get deposit address  

KCS:  
0x011bd184fc7fd1964702844ffd668318f7c3d4c4  
ETH:  
0x3c8e741c0a2Cb4b8d5cBB1ead482CFDF87FDd66F  
BTC:  
1MGLPvTzxK9argeNRTHJ9EZ3WtGZV6nxit  
XLM:  
GA6JNJRSTBV54W3EGWDAWKPEGGD3QCXIGEHMQE2TUYXUKKTNKLYWEXVV  
