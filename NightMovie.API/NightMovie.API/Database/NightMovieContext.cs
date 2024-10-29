using Microsoft.EntityFrameworkCore;
using NightMovie.API.Model;
using System.Diagnostics.Metrics;

namespace NightMovie.API.Database
{
    public class NightMovieContext :  DbContext
    {
        public NightMovieContext(DbContextOptions<NightMovieContext> options) : base(options)
        {
        }

        public DbSet<Film> Films { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Seance> Seances { get; set; }
        public DbSet<User> Users { get; set; }
        //public virtual DbSet<Vote> Votes { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

    }
}
