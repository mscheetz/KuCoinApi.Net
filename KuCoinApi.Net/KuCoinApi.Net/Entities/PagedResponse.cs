// -----------------------------------------------------------------------------
// <copyright file="PagedResponse" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="11/15/2018 7:55:12 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Entities
{
    using Newtonsoft.Json;
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion Usings

    public class PagedResponse<T>
    {
        #region Properties

        [JsonProperty(PropertyName = "currentPage")]
        public int CurrentPage { get; set; }

        [JsonProperty(PropertyName = "pageSize")]
        public int PageSize { get; set; }

        [JsonProperty(PropertyName = "totalNum")]
        public int TotalNum { get; set; }

        [JsonProperty(PropertyName = "totalPage")]
        public int TotalPage { get; set; }

        [JsonProperty(PropertyName = "items")]
        public T Data { get; set; }

        #endregion Properties
    }
}