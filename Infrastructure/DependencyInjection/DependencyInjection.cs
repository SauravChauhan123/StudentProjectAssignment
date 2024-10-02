using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DependencyInjection
{
        /// <summary>
        /// Provides extension methods for registering application infrastructure services.
        /// </summary>
        public static class DependencyInjection
        {
            /// <summary>
            /// Adds infrastructure services to the specified IServiceCollection.
            /// </summary>
            /// <param name="services">The service collection to add services to.</param>
            /// <param name="configuration">The application configuration for accessing connection strings.</param>
            /// <returns>The updated service collection.</returns>
            public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
            {
                // Add DbContext with the connection string
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("DBConnection")));

                // Register repositories with dependency injection
                services.AddScoped<IProjectRepository, ProjectRepository>();
                services.AddScoped<IStudentRepository, StudentRepository>();

                return services;
            }
        }
}
