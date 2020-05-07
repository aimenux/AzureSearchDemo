using System;
using System.Collections.Generic;
using System.Linq;
using Lib.Models;

namespace App.Builders
{
    public class SearchModelBuilder : ISearchModelBuilder
    {
        private static readonly Random Random = new Random(Guid.NewGuid().GetHashCode());

        public ICollection<SearchModel> BuildSearchModels(int number)
        {
            if (number <= 0)
            {
                throw new ArgumentException(nameof(number));
            }

            return Enumerable.Range(0, number)
                .Select(x => BuildRandomSearchModel())
                .ToList();
        }

        private static SearchModel BuildRandomSearchModel()
        {
            return new SearchModel
            {
                PersonId = Guid.NewGuid().ToString(),
                FullName = BuildRandomString(20),
                BirthDate = BuildRandomDate()
            };
        }

        private static string BuildRandomString(int length)
        {
            const string chars = @"ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)])
                .ToArray());
        }

        private static DateTime BuildRandomDate()
        {
            var years = -Random.Next(10, 50);
            return DateTime.Now.AddYears(years);
        }
    }
}