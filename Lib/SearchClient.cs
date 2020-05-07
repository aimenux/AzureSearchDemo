using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lib.Configuration;
using Lib.Contracts;
using Lib.Indexes;
using Lib.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Lib
{
    public class SearchClient<TSearchIndex> : ISearchClient<TSearchIndex> where TSearchIndex : ISearchIndex
    {
        private readonly ISearchIndexClient _searchIndexClient;

        public SearchClient(ISettings settings)
        {
            _searchIndexClient = GetOrCreateSearchIndexClient(settings);
        }

        public async Task SaveAsync<TSearchModel>(ICollection<TSearchModel> models) where TSearchModel : ISearchModel
        {
            var actions = models.Select(IndexAction.Upload);
            var batch = IndexBatch.New(actions);
            try
            {
                await _searchIndexClient.Documents.IndexAsync(batch);
            }
            catch (IndexBatchException ex)
            {
                var documents = ex
                    .IndexingResults
                    .Where(r => !r.Succeeded)
                    .Select(r => r.Key);
                var failedDocuments = string.Join(", ", documents);
                Console.WriteLine($"Failed to index some documents: {failedDocuments}");
                Console.WriteLine(ex);
            }
        }

        public async Task<ICollection<TSearchModel>> GetAsync<TSearchModel>(string query) where TSearchModel : ISearchModel
        {
            var searchResults = await _searchIndexClient.Documents.SearchAsync<TSearchModel>(query);
            return searchResults.Results.Select(x => x.Document).ToList();
        }

        private static ISearchIndexClient GetOrCreateSearchIndexClient(ISettings settings)
        {
            var credentials = new SearchCredentials(settings.ApiKey);
            var searchServiceClient = new SearchServiceClient(settings.Name, credentials);

            if (searchServiceClient.Indexes.Exists(settings.IndexName))
            {
                return searchServiceClient.Indexes.GetClient(settings.IndexName);
            }

            var indexDefinition = new Index
            {
                Name = settings.IndexName,
                Fields = FieldBuilder.BuildForType<TSearchIndex>()
            };

            var index = searchServiceClient.Indexes.Create(indexDefinition);
            return searchServiceClient.Indexes.GetClient(index.Name);
        }
    }
}