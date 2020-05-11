using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using App.Builders;
using Lib;
using Lib.Configuration;
using Lib.Indexes;
using Lib.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace App
{
    public static class Program
    {
        private const int SearchModelsNumber = 1000;

        public static async Task Main()
        {
            var environment = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "DEV";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection();
            services.Configure<Settings>(configuration.GetSection(nameof(Settings)));
            services.AddSingleton<ISettings>(provider =>
            {
                var options = provider.GetService<IOptions<Settings>>();
                return options.Value;
            });
            services.AddSingleton<ISearchModelBuilder, SearchModelBuilder>();
            services.AddSingleton(typeof(ISearchClient<>), typeof(SearchClient<>));

            var serviceProvider = services.BuildServiceProvider();
            var searchModelBuilder = serviceProvider.GetService<ISearchModelBuilder>();
            var searchClient = serviceProvider.GetService<ISearchClient<SearchIndex>>();

            var count = await searchClient.CountAsync();
            Console.WriteLine($"Found documents in azure search: {count}");

            var searchModels = searchModelBuilder.BuildSearchModels(SearchModelsNumber);
            await searchClient.SaveAsync(searchModels);

            await Task.Delay(1000);

            var query = searchModels.Last().FullName;
            var results = await searchClient.GetAsync<SearchModel>(query);

            Console.WriteLine($"Found results matching with query '{query}': {results.Count}");
            foreach (var result in results)
            {
                Console.WriteLine(result);
            }

            Console.WriteLine("Press any key to exit !");
            Console.ReadKey();
        }
    }
}
