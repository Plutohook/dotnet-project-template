using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlutoNetCoreTemplate.Job.Hosting
{
    using System.IO;
    using Extensions.Logger;
    using Serilog;

    public class Program
    {
        public static readonly string AppName = typeof(Program).Namespace;
        public static void Main(string[] args)
        {
            var baseConfig = GetLogConfig();
            Log.Logger = SerilogConfiguration.CreateSerilogLogger(baseConfig, AppName);
            try
            {
                Log.Information("����{ApplicationContext}...", AppName);
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "{ApplicationContext} ���ִ���:{messsage} !", AppName, ex.Message);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseIISIntegration()
                        .CaptureStartupErrors(false);
                })
                .ConfigureAppConfiguration((context, builder) =>
                {
                    var env = context.HostingEnvironment;
                    var baseConfig = GetConfiguration(env);
                    builder.AddConfiguration(baseConfig);
                })
                .UseSerilog(dispose: true);



        /// <summary>
        /// ��־����
        /// </summary>
        /// <returns></returns>
        private static IConfiguration GetLogConfig()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("serilogsetting.json", optional: false, reloadOnChange: true);
            return builder.Build();

        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        private static IConfiguration GetConfiguration(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            return builder.Build();

        }

    }


}
