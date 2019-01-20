// -----------------------------------------------------------------------------
// <copyright file="RepositoryBase" company="Matt Scheetz">
//     Copyright (c) Matt Scheetz All Rights Reserved
// </copyright>
// <author name="Matt Scheetz" date="1/19/2019 8:40:42 PM" />
// -----------------------------------------------------------------------------

namespace KuCoinApi.Net.Data
{
    #region Usings

    using DateTimeHelpers;
    using KuCoinApi.Net.Core;
    using KuCoinApi.Net.Entities;
    using Newtonsoft.Json;
    using RESTApiAccess;
    using RESTApiAccess.Interface;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    #endregion Usings

    public class RepositoryBase
    {
        #region Properties

        private Security security;
        private IRESTRepository _restRepo;
        private string baseUrl;
        private ApiInformation _apiInfo;
        private DateTimeHelper _dtHelper;
        private Helper _helper;
        private bool _systemTimetamp = false;

        #endregion Properties

        public RepositoryBase()
        {
            security = new Security();
            _restRepo = new RESTRepository();
            baseUrl = "https://openapi-v2.kucoin.com";
            _dtHelper = new DateTimeHelper();
            _helper = new Helper();
        }

        public void SetTimestamp(bool useSystemTimestamp)
        {
            _systemTimetamp = useSystemTimestamp;
        }

        /// <summary>
        /// Initiate a Get request
        /// </summary>
        /// <typeparam name="T">Object to return</typeparam>
        /// <param name="endpoint">Endpoint of request</param>
        /// <returns>Object from response</returns>
        public async Task<T> GetRequest<T>(string endpoint)
        {
            var url = baseUrl + endpoint;

            try
            {
                var response = await _restRepo.GetApiStream<T>(url);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Initiate a Get request
        /// </summary>
        /// <typeparam name="T">Object to return</typeparam>
        /// <param name="endpoint">Endpoint of request</param>
        /// <param name="timestamp">Timestamp for transaction</param>
        /// <returns>Object from response</returns>
        public async Task<T> GetRequest<T>(string endpoint, long timestamp)
        {
            var headers = GetRequestHeaders(HttpMethod.Get, endpoint, timestamp);

            var url = baseUrl + endpoint;

            try
            {
                var response = await _restRepo.GetApiStream<T>(url, headers);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Initiate a Post request
        /// </summary>
        /// <typeparam name="T">Object to return</typeparam>
        /// <param name="endpoint">Endpoint of request</param>
        /// <param name="timestamp">Timestamp for transaction</param>
        /// <param name="body">Request body data</param>
        /// <returns>Object from response</returns>
        public async Task<T> PostRequest<T>(string endpoint, long timestamp, SortedDictionary<string, object> body)
        {
            var headers = GetRequestHeaders(HttpMethod.Post, endpoint, timestamp, body);

            var url = baseUrl + endpoint;

            try
            {
                var response = await _restRepo.PostApi<T, SortedDictionary<string, object>>(url, body, headers);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Initiate a Post request
        /// </summary>
        /// <typeparam name="T">Object to return</typeparam>
        /// <param name="endpoint">Endpoint of request</param>
        /// <param name="timestamp">Timestamp for transaction</param>
        /// <returns>Object from response</returns>
        public async Task<T> DeleteRequest<T>(string endpoint, long timestamp)
        {
            var headers = GetRequestHeaders(HttpMethod.Delete, endpoint, timestamp);

            var url = baseUrl + endpoint;

            try
            {
                var response = await _restRepo.DeleteApi<T>(url, headers);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get Request headers
        /// </summary>
        /// <param name="httpMethod">Http Method</param>
        /// <param name="endpoint">Endpoint to access</param>
        /// <param name="timestamp">Timestamp for transaction</param>
        /// <param name="body">Body data to be passed</param>
        /// <returns>Dictionary of request headers</returns>
        private Dictionary<string, string> GetRequestHeaders(HttpMethod httpMethod, string endpoint, long timestamp, SortedDictionary<string, object> body = null)
        {
            var nonce = timestamp.ToString();
            var headers = new Dictionary<string, string>();

            headers.Add("KC-API-KEY", _apiInfo.ApiKey);
            headers.Add("KC-API-SIGN", GetSignature(httpMethod, endpoint, nonce, body));
            headers.Add("KC-API-TIMESTAMP", nonce);
            headers.Add("KC-API-PASSPHRASE", _apiInfo.ApiPassword);
            headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)");

            return headers;
        }

        /// <summary>
        /// Create signature for message
        /// </summary>
        /// <typeparam name="T">Type of data in post request</typeparam>
        /// <param name="endpoint">Endpoint to access</param>
        /// <param name="nonce">Current nonce</param>
        /// <param name="queryString">Querystring to be passed</param>
        /// <param name="value">Data to be sent</param>
        /// <returns>String of signature</returns>
        private string GetSignature(HttpMethod method, string endpoint, string nonce, SortedDictionary<string, object> parms = null)
        {
            var callMethod = method.ToString().ToUpper();

            var jsonedParams = parms.Count > 0
                ? JsonConvert.SerializeObject(parms)
                : string.Empty;

            var sigString = $"{nonce}{callMethod}{endpoint}{jsonedParams}";

            var signature = security.GetKuCoinHMACSignature(_apiInfo.ApiSecret, sigString);

            return signature;
        }
    }
}