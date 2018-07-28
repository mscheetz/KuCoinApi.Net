# KuCoinApi.NetCore
.Net Core library for accessing the [KuCoin Exchange](https://www.kucoin.com) api  
  
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

Using an endpoint:  
```
var kucoin = new KuCoinClient("my-key", "my-secret");  
  
var balance = kucoin.GetBalance();
```  

Non-secure endpoints:  
GetCandlesticks() - Get charting candlesticks  
GetOrderBook() - Get current order book for a trading pair  
GetTick() - Get tick for a trading pair  
GetTicks() - Get ticker for all trading pairs  
GetKuCoinTime() - Get current KuCoin server time  

Secure endpoints:  
GetBalance() - Get current asset balances  
GetOrder() - Get information for an order  
GetOrders() - Get all current user order information  
GetOpenOrders() - Get all current user open orders   
PostTrade() - Post a new trade  
DeleteTrade() - Delete a current open trade  

KCS:  
0x011bd184fc7fd1964702844ffd668318f7c3d4c4  
ETH:  
0x3c8e741c0a2Cb4b8d5cBB1ead482CFDF87FDd66F  
BTC:  
1MGLPvTzxK9argeNRTHJ9EZ3WtGZV6nxit  
