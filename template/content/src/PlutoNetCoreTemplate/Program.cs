using Autofac.Extensions.DependencyInjection;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Events;

using System;
using System.IO;
using System.Net;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlutoNetCoreTemplate.Extensions;
using PlutoNetCoreTemplate.Infrastructure;

namespace PlutoNetCoreTemplate
{
    public class Program
    {
        public static readonly string AppName = typeof(Program).Namespace;
        public static void Main(string[] args)
        {
            var configuration = GetConfiguration();
            try
            {
                Log.Information("׼������{ApplicationContext}...", AppName);
                var host = BuildWebHost(configuration, args);
                Log.Information("{ApplicationContext} ������", AppName);
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "{ApplicationContext} ���ִ���:{messsage} !", AppName,ex.Message);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IWebHost BuildWebHost(IConfiguration configuration, string[] args)
        {
            var webHost = WebHost.CreateDefaultBuilder(args)
                .CaptureStartupErrors(false)
                .UseIISIntegration()
                .ConfigureLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddCustomerSerilog(configuration);
                })
                .ConfigureServices(services => services.AddAutofac())
                .UseStartup<Startup>()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(configuration)
                .UseSerilog()
                .Build();

            webHost.MigrateDbContext<EfCoreDbContext>((context, services) =>
            {
                var logger = services.GetService<ILogger<EfCoreDbContext>>();
            });

            return webHost;
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            return builder.Build();

        }
    }



    public static class WebHostExtension
    {
        public static void MigrateDbContext<TContext>(this IWebHost webHost,
            Action<TContext, IServiceProvider> seeder)
            where TContext : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation("��ʼǨ�����ݿ� {DbContextName}", typeof(TContext).Name);
                    // ����δ�ύ�����ݿ��Ǩ��
                    if (context.Database.GetPendingMigrations().Any())
                    {
                        // ����Ǩ��
                        context.Database.Migrate();
                        logger.LogInformation("��Ǩ�����ݿ� {DbContextName}", typeof(TContext).Name);
                    }
                    seeder?.Invoke(context, webHost.Services); // �������ݳ�ʼ��
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Ǩ�����ݿ�ʱ���� {DbContextName}", typeof(TContext).Name);
                }

            }
        }
    }

}
