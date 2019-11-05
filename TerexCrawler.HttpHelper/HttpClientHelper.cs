using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TerexCrawler.Models.DTO;

namespace TerexCrawler.HttpHelper
{
    public class HttpClientHelper : IHttpClientHelper
    {
        public HttpResultResponseDTO GetHttp(string url)
        {
            HttpResultResponseDTO result = new HttpResultResponseDTO();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = client.GetAsync(url).Result)
                    {
                        using (HttpContent content = response.Content)
                        {
                            result.Content = content.ReadAsStringAsync().Result;
                            result.HttpStatusCode = (int)response.StatusCode;
                            result.ErrorCode = 0;
                            result.Success = response.StatusCode == System.Net.HttpStatusCode.OK;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.ExeptionErrorMessage = ex.Message;
                result.Content = string.Empty;
                result.ErrorCode = -10;
                result.Success = false;
            }
            return result;
        }

        public HttpResultResponseDTO GetHttp(string url, bool changeAgent = false, string[] agents = null)
        {
            HttpResultResponseDTO result = new HttpResultResponseDTO();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (changeAgent)
                    {
                        Random rnd = new Random();
                        int agentNum = rnd.Next(0, agents.Length - 1);
                        client.DefaultRequestHeaders.Add("User-Agent", agents[agentNum]);
                    }
                    using (HttpResponseMessage response = client.GetAsync(url).Result)
                    {
                        using (HttpContent content = response.Content)
                        {
                            result.Content = content.ReadAsStringAsync().Result;
                            result.HttpStatusCode = (int)response.StatusCode;
                            result.ErrorCode = 0;
                            result.Success = response.StatusCode == System.Net.HttpStatusCode.OK;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.ExeptionErrorMessage = ex.Message;
                result.Content = string.Empty;
                result.ErrorCode = -10;
                result.Success = false;
            }
            return result;
        }

        public HttpResultResponseDTO GetHttpAsync(string url)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResultResponseDTO> GetHttpAsync(string url, bool changeAgent = false, string[] agents = null)
        {
            throw new NotImplementedException();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~HttpClientHelper()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
