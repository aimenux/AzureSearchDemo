using System;

namespace Lib.Models
{
    public class SearchModel : ISearchModel
    {
        public string PersonId { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }

        public override string ToString()
        {
            return $"{nameof(PersonId)}='{PersonId}' {nameof(FullName)}='{FullName}'";
        }
    }
}
