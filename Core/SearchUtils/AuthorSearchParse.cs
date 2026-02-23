using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.SearchUtils
{
    public static class AuthorSearchParse
    {
        public static List<string> ParseQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<string>();

            return query.Split(',')
                        .Select(part => part.Trim())
                        .Where(part => !string.IsNullOrEmpty(part))
                        .ToList();
        }

        public static AuthorSearchType GetSearchType(List<string> words)
        {
            if (words == null || words.Count == 0)
                return AuthorSearchType.None;

            switch (words.Count)
            {
                case 1: return AuthorSearchType.ByLastName;
                case 2: return AuthorSearchType.ByLastAndFirstName;
                default: return AuthorSearchType.None;
            }
        }
    }

    public enum AuthorSearchType
    {
        None,
        ByLastName,
        ByLastAndFirstName
    }
}
