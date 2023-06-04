using System.Net;
using Application.Common.Interfaces;
using Infrastracture.Persistence.Contexts;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MariaDB")!;
            // DbContext
            services.AddDbContext<ApplicationDbContext>(opt =>
                opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));



            // Scoped Services
            services.AddSingleton<IEmailService, EmailManager>();


            return services;
        }
    }
}