using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OLTPSystem.DAL;

namespace OLTPSystem;

// Registers the database context and your services for dependency injection.
//	You won't use this until Blazor. In the Blazor app's Program.cs you will add:
//
//		builder.Services.OLTPDependencies(options =>
//			options.UseSqlServer(connectionString));
//
//	Add one AddScoped line for every service you create, e.g.
//		services.AddScoped<CustomerService>();
//	(you will also need "using OLTPSystem.BLL;" at the top)
public static class OLTPExtension
{
	public static void OLTPDependencies(this IServiceCollection services,
		Action<DbContextOptionsBuilder> options)
	{
		services.AddDbContext<OLTPContext>(options);

	}
}
