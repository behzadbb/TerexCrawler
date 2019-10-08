using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TerexCrawler.Entites;

namespace TerexCrawler.DataLayer.Context
{
    public class ApplicationDbContext : DbContext
    {
        public virtual DbSet<Log> Logs { set; get; }
        public virtual DbSet<DigikalaBasePage> DigikalaBasePages { get; set; }
    }
}
