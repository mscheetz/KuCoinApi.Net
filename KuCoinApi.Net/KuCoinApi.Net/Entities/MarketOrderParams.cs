// -----------------------------------------------------------------------------
// <copyright file="MarketOrderParams" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 11:46:37 AM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    #region Usings
    #endregion Usings

    public class MarketOrderParams : OrderParams
    {        
        public decimal Funds { get; set; }

        public MarketOrderParams()
        {
            this.Type = OrderType.MARKET;
        }
    }
}
