using System;

namespace Core.SearchUtils
{
    public static class BookSearchParse
    {
        public static string NormalizeQuery(string query)
        {
            return (query ?? "").Trim();
        }

        public static BookSearchType GetSearchType(string query)
        {
            return string.IsNullOrWhiteSpace(query)
                ? BookSearchType.None
                : BookSearchType.ByNameOrIsbn;
        }
    }

    public enum BookSearchType
    {
        None,
        ByNameOrIsbn
    }
}
