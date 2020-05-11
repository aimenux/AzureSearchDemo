using System.Collections.Generic;
using System.Threading.Tasks;
using Lib.Indexes;
using Lib.Models;

namespace Lib
{
    public interface ISearchClient<TSearchIndex> where TSearchIndex : ISearchIndex
    {
        Task<long> CountAsync();
        Task DeleteIndexAndDocumentsAsync();
        Task DeleteDocumentsAsync(string keyName, ICollection<string> keysValues);
        Task SaveAsync<TSearchModel>(ICollection<TSearchModel> models) where TSearchModel : ISearchModel;
        Task<ICollection<TSearchModel>> GetAsync<TSearchModel>(string query, ISearchClientParameters parameters = null) where TSearchModel : ISearchModel;
    }
}