﻿using Goldmint.DAL.CustodyBotModels;
using Microsoft.EntityFrameworkCore;


namespace Goldmint.DAL
{
    public class CustodyBotDbContext : DbContext
    {
        public virtual DbSet<Clients> Clients { get; set; }
        public virtual DbSet<Custodies> Custodies { get; set; }
        public virtual DbSet<Files> Files { get; set; }

        public CustodyBotDbContext(DbContextOptions<CustodyBotDbContext> options) : base(options)
        {
        }
    }
}