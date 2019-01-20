// -----------------------------------------------------------------------------
// <copyright file="StopLimitOrderParams" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 11:46:37 AM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    #region Usings
    #endregion Usings

    public class StopLimitOrderParams : LimitOrderParams
    {

        public StopType? Stop { get; set; }

        public decimal StopPrice { get; set; }
    }
}
