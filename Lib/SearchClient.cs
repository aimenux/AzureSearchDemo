using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lib.Configuration;
using Lib.Indexes;
using Lib.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Lib
{
    public class SearchClient<TSearchIndex> : ISearchClient<TSearchIndex> where TSearchIndex : ISearchIndex
    {
        private readonly ISettings _settings;
        private readonly ISearchIndexClient _searchIndexClient;
        private readonly SearchServiceClient _searchServiceClient;

        public SearchClient(ISettings settings)
        {
            _settings = settings;
            var credentials = new SearchCredentials(settings.ApiKey);
            _searchServiceClient = new SearchServiceClient(settings.Name, credentials);
            _searchIndexClient = GetOrCreateSearchIndexClient();
        }

        public Task<long> CountAsync()
        {
            return _searchIndexClient.Documents.CountAsync();
        }

        public Task DeleteIndexAndDocumentsAsync()
        {
            return _searchServiceClient.Indexes.DeleteAsync(_settings.IndexName);
        }

        public Task DeleteDocumentsAsync(string keyName, ICollection<string> keysValues)
        {
            var batch = IndexBatch.Delete(keyName, keysValues);
            return RunBatchAsync(batch);
        }

        public Task SaveAsync<TSearchModel>(ICollection<TSearchModel> models) where TSearchModel : ISearchModel
        {
            var actions = models.Select(IndexAction.Upload);
            var batch = IndexBatch.New(actions);
            return RunBatchAsync(batch);
        }

        public async Task<ICollection<TSearchModel>> GetAsync<TSearchModel>(string query, ISearchClientParameters parameters = null) where TSearchModel : ISearchModel
        {
            var searchParameters = parameters != null ? new SearchClientParameters(parameters) : null;
            var searchResults = await _searchIndexClient.Documents.SearchAsync<TSearchModel>(query, searchParameters);
            return searchResults.Results.Select(x => x.Document).ToList();
        }

        private ISearchIndexClient GetOrCreateSearchIndexClient()
        {
            if (_searchServiceClient.Indexes.Exists(_settings.IndexName))
            {
                return _searchServiceClient.Indexes.GetClient(_settings.IndexName);
            }

            var indexDefinition = new Index
            {
                Name = _settings.IndexName,
                Fields = FieldBuilder.BuildForType<TSearchIndex>()
            };

            var index = _searchServiceClient.Indexes.Create(indexDefinition);
            return _searchServiceClient.Indexes.GetClient(index.Name);
        }

        private async Task RunBatchAsync<T>(IndexBatch<T> batch)
        {
            try
            {
                await _searchIndexClient.Documents.IndexAsync(batch);
            }
            catch (IndexBatchException ex)
            {
                var failedDocuments = ex.IndexingResults
                    .Where(r => !r.Succeeded)
                    .Select(r => r.Key);
                Console.WriteLine($"Failed to index some documents: {string.Join(", ", failedDocuments)}\n{ex}");
            }
        }
    }
}