using System;
using System.Collections.Generic;
using System.Text;
using TerexCrawler.DataLayer.Context;
using TerexCrawler.Entites;
using TerexCrawler.Models.DTO.Page;

namespace TerexCrawler.DataLayer.Repository
{
    public class DigikalaRepository : IDisposable
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public void AddDigikalaBasePage(DigikalaPageBaseDTO dto)
        {
            DigikalaBasePage model = new DigikalaBasePage();
            model.CreateDate = DateTime.Now;
            model.ChangeFreq = dto.ChangeFreq;
            model.ImageCaption = dto.ImageCaption;
            model.ImageLoc = dto.ImageLoc;
            model.Loc = dto.Loc;
            model.Priority = dto.Priority;

            db.DigikalaBasePages.Add(model);
            db.SaveChanges();
        }

        public void AddDigikalaBasePages(List<DigikalaPageBaseDTO> dtos)
        {
            List<DigikalaBasePage> models = new List<DigikalaBasePage>();
            DateTime CreateDate = DateTime.Now;
            foreach (var dto in dtos)
            {
                DigikalaBasePage model = new DigikalaBasePage();
                model.DKP = dto.DKP;
                model.Crawled = false;
                model.CreateDate = CreateDate;
                model.ChangeFreq = dto.ChangeFreq;
                model.ImageCaption = dto.ImageCaption;
                model.ImageLoc = dto.ImageLoc;
                model.Loc = dto.Loc;
                model.Priority = dto.Priority;
                models.Add(model);
            }

            db.DigikalaBasePages.AddRange(models);
            db.SaveChanges();
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
            db.Dispose();
        }
        #endregion
    }
}
