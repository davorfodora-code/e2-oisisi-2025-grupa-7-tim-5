using Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

public static class BookSearch
{
    public static IEnumerable<BookDTO> Filter(IEnumerable<BookDTO> source, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return source;

        string q = query.Trim();

        bool ContainsCI(string text) =>
            (text ?? "").IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0;

        return source.Where(b => ContainsCI(b.Name) || ContainsCI(b.Isbn));
    }
}
