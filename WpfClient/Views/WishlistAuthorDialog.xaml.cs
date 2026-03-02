using Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    public partial class WishlistAuthorsDialog : Window
    {
        private readonly List<AuthorDTO> _allAuthors;

        private string L(string key)
        {
            var val = Application.Current.TryFindResource(key);
            return val?.ToString() ?? $"[{key}]";
        }

        public WishlistAuthorsDialog(VisitorDTO visitor, List<AuthorDTO> authors)
        {
            InitializeComponent();

            TxtHeader.Text = $"{L("WishlistAuthors_HeaderPrefix")} {visitor?.FullName}";
            _allAuthors = authors ?? new List<AuthorDTO>();
            AuthorsGrid.ItemsSource = _allAuthors;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var q = (TxtSearch.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(q))
            {
                AuthorsGrid.ItemsSource = _allAuthors;
                return;
            }

            AuthorsGrid.ItemsSource = _allAuthors
                .Where(a =>
                    Contains(a?.Name, q) ||
                    Contains(a?.Lastname, q) ||
                    Contains(a?.Email, q))
                .ToList();
        }

        private static bool Contains(string value, string q) =>
            !string.IsNullOrEmpty(value) &&
            value.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0;

        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
    }
}
