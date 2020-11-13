using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HattingtonGame
{
    public class Hattington : DbContext
    {
        public DbSet<Hat> Hats { get; set; }
        public DbSet<HatCharacter> HatCharacters { get; set; }
        public DbSet<HatTier> HatTiers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename=Hattington.db");
        }

    }
}
