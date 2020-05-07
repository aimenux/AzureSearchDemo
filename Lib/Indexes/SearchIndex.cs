using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Search;

namespace Lib.Indexes
{
    public class SearchIndex : ISearchIndex
    {
        [Key]
        [IsSearchable, IsFilterable]
        public string PersonId { get; set; }

        [IsSearchable, IsFilterable]
        public string FullName { get; set; }

        [IsFilterable, IsSortable]
        public DateTime BirthDate { get; set; }
    }
}