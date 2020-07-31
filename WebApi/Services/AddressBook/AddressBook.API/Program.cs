using System;
using System.IO;
using AddressBook.API.Domains;
using Dapper;
using Dapper.ColumnMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace AddressBook.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();

                InitDapperColumnsMapping();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        // to allow reead data from multiple tables with join and custom columns names
        public static void InitDapperColumnsMapping()
        {
            SqlMapper.SetTypeMap(typeof(Contact), new ColumnTypeMapper(typeof(Contact)));
            SqlMapper.SetTypeMap(typeof(Country), new ColumnTypeMapper(typeof(Country)));
        }
    }
}
