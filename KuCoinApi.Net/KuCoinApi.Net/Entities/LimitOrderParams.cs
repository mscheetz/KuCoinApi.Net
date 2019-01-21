// -----------------------------------------------------------------------------
// <copyright file="LimitOrderParams" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 11:46:37 AM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    #region Usings
    #endregion Usings

    public class LimitOrderParams : OrderParams
    {
        public decimal Price { get; set; }

        public TimeInForce? TimeInForce { get; set; }

        public long CancelAfter { get; set; }

        public bool PostOnly { get; set; }

        public LimitOrderParams()
        {
            this.Type = OrderType.LIMIT;
        }
    }
}
