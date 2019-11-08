using System;
using System.Collections.Generic;
using System.Text;
using TerexCrawler.Models;
using TerexCrawler.Models.Interfaces;
using TerexCrawler.DataLayer.Context;
using AutoMapper;
using TerexCrawler.Entites;
using MongoDB.Driver;
using MongoDB.Bson;

namespace TerexCrawler.Services
{
    public class MongoDBLoggerHelper : ILoger
    {
        private MongoClient client;
        private MongoServer server;
        private MongoDatabase db;
        private MongoCollection<Log> logs;

        public MongoDBLoggerHelper()
        {
            client = new MongoClient("mongodb://localhost");
            server = client.GetServer();
            db = server.GetDatabase("Digikala");
            logs = db.GetCollection<Log>("Logs");
        }

        IMapper mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<LogDTO, Log>()));
        public void AddLog(LogDTO dto)
        {
            try
            {
                Log log = mapper.Map<Log>(dto);
                log._id = ObjectId.GenerateNewId();
                logs.Insert(dto);
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                throw;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

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
            Dispose(true);
        }
        #endregion
    }
}
