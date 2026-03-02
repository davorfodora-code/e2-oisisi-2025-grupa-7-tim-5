using Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    public partial class PublisherAuthorsDialog : Window
    {
        private readonly List<AuthorDTO> _allAuthors;

        private string L(string key)
        {
            var val = Application.Current.TryFindResource(key);
            return val?.ToString() ?? $"[{key}]";
        }

        public PublisherAuthorsDialog(PublisherDTO publisher, List<AuthorDTO> authors)
        {
            InitializeComponent();

            TxtHeader.Text = $"{L("PublisherAuthors_HeaderPrefix")} {publisher?.Name}";

            _allAuthors = authors ?? new List<AuthorDTO>();
            AuthorsGrid.ItemsSource = _allAuthors;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var q = (TxtSearch.Text ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(q))
            {
                AuthorsGrid.ItemsSource = _allAuthors;
                return;
            }

            // case-insensitive pretraga po osnovnim poljima
            AuthorsGrid.ItemsSource = _allAuthors
                .Where(a =>
                    Contains(a?.Name, q) ||
                    Contains(a?.Lastname, q) ||
                    Contains(a?.PhoneNumber, q) ||
                    Contains(a?.Email, q)
                )
                .ToList();
        }

        private static bool Contains(string value, string query)
            => !string.IsNullOrEmpty(value) &&
               value.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;

        private static string FormatDate(DateTime? dt)
            => dt?.ToString("dd.MM.yyyy") ?? string.Empty;

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
