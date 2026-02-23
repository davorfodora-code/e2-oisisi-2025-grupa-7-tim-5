using Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

public static class AuthorSearch
{
    public static IEnumerable<AuthorDTO> Filter(IEnumerable<AuthorDTO> source, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return source;

        var parts = query.Split(',')
                         .Select(p => p.Trim())
                         .Where(p => p.Length > 0)
                         .ToList();

        bool ContainsCI(string text, string term) =>
            (text ?? "").IndexOf(term ?? "", StringComparison.OrdinalIgnoreCase) >= 0;

        if (parts.Count == 1)
            return source.Where(a => ContainsCI(a.Lastname, parts[0]));

        if (parts.Count >= 2)
            return source.Where(a =>
                ContainsCI(a.Lastname, parts[0]) &&
                ContainsCI(a.Name, parts[1]));

        return source;
    }
}
