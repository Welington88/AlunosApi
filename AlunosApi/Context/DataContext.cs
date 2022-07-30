using Microsoft.EntityFrameworkCore;
using AlunosApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace AlunosApi.Context
{
    public class DataContext : IdentityDbContext<IdentityUser>
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options)
		{
		}
		public DbSet<Usuario>? Usuario { get; set; }
		public DbSet<Alunos>? Alunos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			modelBuilder.Entity<Alunos>()
				.HasKey(a => a.Id);

			base.OnModelCreating(modelBuilder);
        }
    }
}

