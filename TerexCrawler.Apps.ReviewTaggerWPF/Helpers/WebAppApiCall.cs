using erexCrawler.HttpHelper.ApiCall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TerexCrawler.Apps.ReviewTaggerWPF.Helpers
{
    public class WebAppApiCall : IDisposable
    {
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
        // ~WebAppApiCall()
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

        private const string _baseUrl1 = "http://localhost:9487/api/digikala/";
        private const string _baseUrl2 = "http://185.112.32.241:1368/api/digikala/";
        private const string _baseUrl3 = "http://185.165.118.208/api/digikala/";
        public T GetFromApi<T>(string url, object obj = null)
        {
            T result = default(T);
            try
            {
                url = _baseUrl3 + url;
                result = ApiCallGeneric.ApiCall<T>(url, obj);
            }
            catch (Exception ex)
            {
            }
            return result;
        }
    }
}