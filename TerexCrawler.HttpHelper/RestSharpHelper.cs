using System;
using System.Collections.Generic;
using System.Text;
using TerexCrawler.Models.DTO;
using RestSharp;
using System.Net;
using System.Threading.Tasks;

namespace TerexCrawler.HttpHelper
{
    public class RestSharpHelper : IHttpClientHelper
    {
        public HttpResultResponseDTO GetHttp(string url)
        {
            HttpResultResponseDTO result = new HttpResultResponseDTO();
            try
            {
                IRestClient restClient = new RestClient(url);
                var request = new RestRequest(Method.GET);
                var response = restClient.Execute(request);

                result.Content = response.StatusCode == HttpStatusCode.OK ? response.Content : string.Empty;
                result.HttpStatusCode = (int)response.StatusCode;
                result.ErrorCode = 0;
                result.Success = response.StatusCode == HttpStatusCode.OK;
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

        public HttpResultResponseDTO GetHttp(string url, string proxy, bool changeAgent = false, string[] agents = null)
        {
            HttpResultResponseDTO result = new HttpResultResponseDTO();
            try
            {
                IRestClient restClient = new RestClient(url);
                if (!string.IsNullOrEmpty(proxy))
                {
                    restClient.Proxy = new WebProxy(proxy);
                   
                }
                var request = new RestRequest(Method.GET);

                if (changeAgent)
                {
                    Random rnd = new Random();
                    int agentNum = rnd.Next(0, agents.Length - 1);
                    request.AddHeader("User-Agent", agents[agentNum]);
                }

                var response = restClient.Execute(request);

                result.Content = response.StatusCode == HttpStatusCode.OK ? response.Content : string.Empty;
                result.HttpStatusCode = (int)response.StatusCode;
                result.ErrorCode = 0;
                result.Success = response.StatusCode == HttpStatusCode.OK;
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
        // ~RestSharpHelper()
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
