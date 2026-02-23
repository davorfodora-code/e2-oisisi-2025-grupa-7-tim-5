using Core.DTO;

public static class VisitorSearch
{
    public static IEnumerable<VisitorDTO> Filter(IEnumerable<VisitorDTO> source, string query)
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
            return source.Where(v => ContainsCI(v.LastName, parts[0]));

        if (parts.Count == 2)
            return source.Where(v =>
                ContainsCI(v.LastName, parts[0]) &&
                ContainsCI(v.Name, parts[1]));

        if (parts.Count >= 3)
            return source.Where(v =>
                ContainsCI(v.MembershipNumber, parts[0]) &&
                ContainsCI(v.Name, parts[1]) &&
                ContainsCI(v.LastName, parts[2]));

        return source;
    }
}
