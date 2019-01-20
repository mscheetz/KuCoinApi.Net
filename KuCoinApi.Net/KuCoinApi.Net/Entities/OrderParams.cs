// -----------------------------------------------------------------------------
// <copyright file="OrderParams" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 11:46:37 AM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    #region Usings
    #endregion Usings

    public class OrderParams
    {
        public string ClientOid { get; set; }

        public OrderType Type { get; set; }

        public Side Side { get; set; }

        public string Pair { get; set; }

        public string Remark { get; set; }

        public SelfTradeProtect? SelfTradeProtect { get; set; }

        public decimal Size { get; set; }
    }
}
