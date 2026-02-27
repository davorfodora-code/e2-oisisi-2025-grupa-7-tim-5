using Core.DAO;
using Core.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace WpfClient
{
    public partial class VisitorsByAuthorWindow : Window
    {
        private AuthorDTO _selectedAuthor;
        private List<VisitorDTO> _allVisitors;
        private List<VisitorDTO> _filteredVisitors;

        public VisitorsByAuthorWindow(AuthorDTO selectedAuthor, List<VisitorDTO> allVisitors)
        {
            InitializeComponent();
            _selectedAuthor = selectedAuthor;
            _allVisitors = allVisitors;

            LoadVisitors();
        }

        private void LoadVisitors()
        {
            try
            {
                BookDAO bookDAO = new BookDAO();
                var allBooks = bookDAO.GetAllBooks();

                // Pronalazimo ISBN-ove knjiga koje je pisao taj autor
                var authorBookIsbns = allBooks
                    .Where(b => b.Authors != null && b.Authors.Any(a => a.IdCard == _selectedAuthor.IdCard))
                    .Select(b => b.Isbn)
                    .ToList();

                // Filtriramo posetioce
                _filteredVisitors = _allVisitors.Where(v =>
                    v.Wishlist != null && v.Wishlist.Any(wishBook => authorBookIsbns.Contains(wishBook.Isbn))
                ).ToList();

                VisitorsGrid.ItemsSource = _filteredVisitors;
                // TxtAuthorInfo mora biti ime TextBlock-a u tvom XAML-u novog prozora
                TxtAuthorInfo.Text = $"Autor: {_selectedAuthor.Name} {_selectedAuthor.Lastname} (Pronađeno: {_filteredVisitors.Count})";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = TxtSearch.Text?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(query))
                {
                    VisitorsGrid.ItemsSource = _filteredVisitors;
                    return;
                }

                var filtered = _filteredVisitors.Where(v =>
                    (v.MembershipNumber?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (v.Name?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (v.LastName?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (v.DisplayAddress?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (v.DisplayStatus?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();

                VisitorsGrid.ItemsSource = filtered;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri pretrazi: {ex.Message}", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                BtnSearch_Click(sender, e);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}