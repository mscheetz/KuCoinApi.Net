using KuCoinApi.NetCore.Data.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KuCoinApi.NetCore.Data
{
    public class RESTRepository : IRESTRepository
    {
        public RESTRepository()
        {
        }

        /// <summary>
        /// Get call to api
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="url">Url to access</param>
        /// <param name="headers">Http Request headers (optional)</param>
        /// <returns>Type requested</returns>
        public async Task<T> GetApi<T>(string url, Dictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                try
                { 
                    var response = await client.GetAsync(url);

                    string responseMessage = await response.Content.ReadAsStringAsync();
                
                    try
                    {
                        return JsonConvert.DeserializeObject<T>(responseMessage);
                    }
                    catch
                    {
                        return default(T);
                    }
                }
                catch (HttpRequestException ex)
                {
                    return default(T);
                }
                catch (WebException e)
                {
                    return default(T);
                }
            }
        }

        /// <summary>
        /// Get call to api stream 
        /// For large json responses
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="url">Url to access</param>
        /// <param name="headers">Http Request headers (optional)</param>
        /// <returns>Type requested</returns>
        public async Task<T> GetApiStream<T>(string url, Dictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                var responseMessage = String.Empty;
                try
                { 
                    var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                    var sb = new StringBuilder();

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        using (var sr = new StreamReader(stream, Encoding.UTF8))
                        {
                            sb.Append(sr.ReadToEnd());
                        }

                        responseMessage = sb.ToString();
                        responseMessage = responseMessage.Replace("null", "0");
                    }

                    try
                    {
                        return JsonConvert.DeserializeObject<T>(responseMessage);
                    }
                    catch
                    {
                        return default(T);
                    }
                }
                catch (HttpRequestException ex)
                {
                    return default(T);
                }
                catch(WebException e)
                {
                    return default(T);
                }
            }
        }

        /// <summary>
        /// Post call to api with data
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <typeparam name="U">Type to post</typeparam>
        /// <param name="url">Url to access</param>
        /// <param name="data">Data object being sent</param>
        /// <param name="headers">Http Request headers (optional)</param>
        /// <returns>Type requested</returns>
        public async Task<T> PostApi<T, U>(string url, U data, Dictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

                try
                { 
                    var response = await client.PostAsync(url, content);

                    string responseMessage = await response.Content.ReadAsStringAsync();

                    try
                    {
                        return JsonConvert.DeserializeObject<T>(responseMessage);
                    }
                    catch
                    {
                        return default(T);
                    }
                }
                catch (HttpRequestException ex)
                {
                    return default(T);
                }
                catch (WebException e)
                {
                    return default(T);
                }
            }
        }

        /// <summary>
        /// Post call to api without data
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="url">Url to access</param>
        /// <param name="headers">Http Request headers (optional)</param>
        /// <returns>Type requested</returns>
        public async Task<T> PostApi<T>(string url, Dictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                try
                {
                    var response = await client.PostAsync(url, null);

                    string responseMessage = await response.Content.ReadAsStringAsync();

                    try
                    {
                        return JsonConvert.DeserializeObject<T>(responseMessage);
                    }
                    catch
                    {
                        return default(T);
                    }
                }
                catch (HttpRequestException ex)
                {
                    return default(T);
                }
                catch (WebException e)
                {
                    return default(T);
                }
            }
        }

        /// <summary>
        /// Delete call to api
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="url">Url to access</param>
        /// <param name="headers">Http Request headers (optional)</param>
        /// <returns>Type requested</returns>
        public async Task<T> DeleteApi<T>(string url, Dictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                try
                {
                    var response = await client.DeleteAsync(url);

                    string responseMessage = await response.Content.ReadAsStringAsync();

                    try
                    {
                        return JsonConvert.DeserializeObject<T>(responseMessage);
                    }
                    catch
                    {
                        return default(T);
                    }
                }
                catch(HttpRequestException ex)
                {
                    return default(T);
                }
                catch (WebException e)
                {
                    return default(T);
                }
            }
        }
    }
}
