using System;
using System.Collections.Generic;
using System.Text;
using TerexCrawler.DataLayer.Repository;
using TerexCrawler.HttpHelper;
using TerexCrawler.Models;
using TerexCrawler.Models.DTO.Page;
using TerexCrawler.Models.DTO.XmlSitemap;
using TerexCrawler.Models.Enums;
using TerexCrawler.Models.Interfaces;

namespace TerexCrawler.Services.Digikala
{
    public class DigikalaHelper : IWebsiteCrawler
    {
        private const string sitename = "Digikala";
        public string WebsiteName => sitename;
        public string WebsiteUrl => "https://Digikala.com";
        IHttpClientHelper client = new RestSharpHelper();
        ILoger Logger = new LoggerHelper();

        public string GetComment(string url)
        {
            throw new NotImplementedException();
        }

        public string GetPage(string url)
        {
            var res = client.GetHttp(url);
            if (res.Success)
            {
                return res.Content;
            }
            else
            {
                LogDTO log = new LogDTO()
                {
                    DateTime = DateTime.Now,
                    Description = res.ExeptionErrorMessage,
                    ProjectId = (int)ProjectNames.HttpHelper,
                    Url = url,
                    MethodName = "Digikala - GetPage",
                    Title = "Get HTML Error"
                };
                Logger.AddLog(log);
                return string.Empty;
            }
        }

        public bool AddPages()
        {
            throw new NotImplementedException();
        }

        public string[] GetComments(string url)
        {
            throw new NotImplementedException();
        }

        public string[] GetSitemapFromAddress(string url)
        {
            throw new NotImplementedException();
        }

        public string[] GetSitemapFromFile(string path)
        {
            throw new NotImplementedException();
        }

        public string[] GetSitemapFromFiles(string[] path)
        {
            throw new NotImplementedException();
        }

        public void AddBasePage(B5_Url dto)
        {
            try
            {
                using (var digi = new DigikalaRepository())
                {
                    digi.AddDigikalaBasePage(new DigikalaPageBaseDTO(dto));
                }
            }
            catch (Exception ex)
            {
                LogDTO log = new LogDTO()
                {
                    DateTime = DateTime.Now,
                    Description = "Error Convert Model, Error= " + ex.ToString(),
                    Title = "Convert To Standard DTO, Digikala",
                    MethodName = "AddBasePage",
                    ProjectId = 1,
                    Url = dto.loc
                };
                Logger.AddLog(log);
            }
        }

        public void AddBasePages(List<B5_Url> dtos)
        {
            var pageBases = new List<DigikalaPageBaseDTO>();
            foreach (var dto in dtos)
            {
                try
                {
                    pageBases.Add(new DigikalaPageBaseDTO(dto));
                }
                catch (Exception ex)
                {
                    LogDTO log = new LogDTO()
                    {
                        DateTime = DateTime.Now,
                        Description = "Error Convert Model, Error= " + ex.ToString(),
                        Title = "Convert To Standard DTO, Digikala",
                        MethodName = "AddBasePages",
                        ProjectId = 1,
                        Url = dto.loc
                    };
                    Logger.AddLog(log);
                }
            }
            dtos.Clear();
            using (var digi = new DigikalaRepository())
            {
                digi.AddDigikalaBasePages(pageBases);
            }
            pageBases.Clear();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            client.Dispose();
            Dispose(true);
        }
        #endregion
    }
}
