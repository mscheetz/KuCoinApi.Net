using KuCoinApi.NetCore.Data;
using KuCoinApi.NetCore.Data.Interface;
using System;

namespace KuCoinApi.NetCore
{
    public class KuCoinApiClient
    {
        public IKuCoinRepository KuCoinRepository;

        /// <summary>
        /// Constructor, no authorization
        /// </summary>
        public KuCoinApiClient()
        {
            KuCoinRepository = new KuCoinRepository();
        }

        /// <summary>
        /// Constructor, with authorization
        /// </summary>
        /// <param name="apiKey">Api key</param>
        /// <param name="apiSecret">Api secret</param>
        public KuCoinApiClient(string apiKey, string apiSecret)
        {
            KuCoinRepository = new KuCoinRepository(apiKey, apiSecret);
        }

        /// <summary>
        /// Constructor - with authentication
        /// </summary>
        /// <param name="configPath">Path to config file</param>
        public KuCoinApiClient(string configPath)
        {
            KuCoinRepository = new KuCoinRepository(configPath);
        }
    }
}
