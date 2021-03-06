﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TerexCrawler.Models.DTO;

namespace TerexCrawler.HttpHelper
{
    public interface IHttpClientHelper : IDisposable
    {
        HttpResultResponseDTO GetHttp(string url);
        HttpResultResponseDTO GetHttp(string url, bool changeAgent = false, string[] agents = null);
        HttpResultResponseDTO GetHttpAsync(string url);
        Task<HttpResultResponseDTO> GetHttpAsync(string url, bool changeAgent = false, string[] agents = null);
    }
}
