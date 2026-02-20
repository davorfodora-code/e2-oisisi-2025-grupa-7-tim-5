using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.SearchUtils
{
    public static class VisitorSearchParse
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
        public static SearchType GetSearchType(List<string> words)
        {
            if (words == null || words.Count == 0)
                return SearchType.None;

            switch (words.Count)
            {
                case 1: return SearchType.ByLastName;
                case 2: return SearchType.ByLastAndFirstName;
                case 3: return SearchType.ByMembershipAndNameAndLastName;
                default: return SearchType.None;
            }
        }
    }

    public enum SearchType
    {
        None,                      
        ByLastName,                  
        ByLastAndFirstName,        
        ByMembershipAndNameAndLastName 
    }
}