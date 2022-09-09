using Microsoft.EntityFrameworkCore;
using Projeto02.Services.Api.Entities;

namespace Projeto02.Services.Api.Contexts
{
    public class SqlServerContext : DbContext
    {
        public SqlServerContext(DbContextOptions<SqlServerContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .HasIndex(x=>x.Email)
                .IsUnique();
        }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
