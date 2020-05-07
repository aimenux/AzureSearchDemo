using System.Collections.Generic;
using System.Threading.Tasks;
using Lib.Indexes;
using Lib.Models;

namespace Lib.Contracts
{
    public interface ISearchClient<TSearchIndex> where TSearchIndex : ISearchIndex
    {
        Task SaveAsync<TSearchModel>(ICollection<TSearchModel> models) where TSearchModel : ISearchModel;
        Task<ICollection<TSearchModel>> GetAsync<TSearchModel>(string query) where TSearchModel : ISearchModel;
    }
}