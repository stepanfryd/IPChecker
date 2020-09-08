using Microsoft.EntityFrameworkCore;

using System.IO;

namespace IPChecker
{
	public class IPCheckerContext : DbContext
	{
		private const string _DbFile = "Data.db";

		public DbSet<AddressInfo> Addresses { get; set; }

		public IPCheckerContext()
		{
			if(!File.Exists(_DbFile))
			{
				Database.EnsureDeleted();
				Database.EnsureCreated();
			}
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite($"Data Source={_DbFile}");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<AddressInfo>();
		}
	}
}
