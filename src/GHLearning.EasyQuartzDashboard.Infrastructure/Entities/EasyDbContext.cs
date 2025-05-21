using GHLearning.EasyQuartzDashboard.Infrastructure.Entities.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace GHLearning.EasyQuartzDashboard.Infrastructure.Entities;

public class EasyDbContext(DbContextOptions options) : DbContext(options)
{
	public DbSet<Customer> Customers { get; init; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.Entity<Customer>().ToCollection("customers");
	}
}