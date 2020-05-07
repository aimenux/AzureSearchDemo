using System.Collections.Generic;
using Lib.Models;

namespace App.Builders
{
    public interface ISearchModelBuilder
    {
        ICollection<SearchModel> BuildSearchModels(int number);
    }
}
