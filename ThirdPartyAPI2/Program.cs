using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ThirdPartyAPI2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /*var requestObjects = new List<RequestObject>();

            void InitializeObjects() => requestObjects = Enumerable.Range(1, 5)
                .Select(index => new RequestObject(index, $"Object #{index}", "Started", "started"))
                .ToList();

            InitializeObjects();*/

            CreateHostBuilder(args).Build().Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

    }

   // public record RequestObject(UInt64 Id, string Body, string Status, string Detail);
}