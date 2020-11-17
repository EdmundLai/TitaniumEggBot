using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using HattingtonGame.DbTypes;

namespace HattingtonGame
{
    public class Hattington : DbContext
    {
        public DbSet<Hat> Hats { get; set; }
        public DbSet<HatCharacter> HatCharacters { get; set; }
        public DbSet<HatTier> HatTiers { get; set; }
        public DbSet<Enemy> Enemies { get; set; }
        public DbSet<EnemyRank> EnemyRanks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename=Hattington.db");
        }

    }
}
