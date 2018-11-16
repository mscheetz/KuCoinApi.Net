// -----------------------------------------------------------------------------
// <copyright file="PagedResponse" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="11/15/2018 7:55:12 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.NetCore.Entities
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion Usings

    public class PagedResponse<T>
    {

        #region Properties

        public int currPageNo { get; set; }
        public T datas { get; set; }
        public int limit { get; set; }
        public int pageNos { get; set; }
        public int total { get; set; }

        #endregion Properties
    }
}