using System;
using System.Collections.Generic;
using System.Text;

namespace TerexCrawler.Models.Interfaces
{
    public interface ILoger : IDisposable
    {
        void AddLog(LogDTO log);
    }
}
