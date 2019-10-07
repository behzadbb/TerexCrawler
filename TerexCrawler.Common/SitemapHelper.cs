﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using TerexCrawler.Models.DTO.XmlSitemap;

namespace TerexCrawler.Common
{
    public class SitemapHelper : IDisposable
    {
        public Urlset SitemapToObject(string path)
        {
            Urlset Data = new Urlset();
            XmlSerializer serializer = new XmlSerializer(typeof(Urlset));
            StreamReader reader = new StreamReader(path);
            Data = (Urlset)serializer.Deserialize(reader);
            reader.Close();
            return Data;
        }

        public void CleanFile(FileInfo fileInfo)
        {
            if (File.Exists(fileInfo.ToString()))
            {
                string readText = File.ReadAllText(fileInfo.ToString());
                readText = readText.Replace("image:", "");
                string filename = fileInfo.Directory + "\\clean\\" + fileInfo.Name + ".xml";
                File.WriteAllText(filename, readText, Encoding.UTF8);
            }
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
        // ~SitemapHelper()
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
