﻿using System;
using System.Collections.Generic;
using System.Text;
using TerexCrawler.Models;
using TerexCrawler.Models.Interfaces;
using TerexCrawler.DataLayer.Context;
using TerexCrawler.Entites;

namespace TerexCrawler.Services
{
    public class LoggerHelper : ILoger
    {
        //protected ApplicationDbContext db = new ApplicationDbContext();
        public void AddLog(LogDTO dto)
        {
            Log log = new Log(dto);
            //db.Logs.Add(log);
            //db.SaveChangesAsync();
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //db.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
