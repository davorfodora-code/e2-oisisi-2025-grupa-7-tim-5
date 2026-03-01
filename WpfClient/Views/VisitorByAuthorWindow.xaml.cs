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

        private string L(string key) => (string)Application.Current.Resources[key];

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

                var authorBookIsbns = allBooks
                    .Where(b => b.Authors != null && b.Authors.Any(a => a.IdCard == _selectedAuthor.IdCard))
                    .Select(b => b.Isbn)
                    .ToList();

                _filteredVisitors = _allVisitors.Where(v =>
                    v.Wishlist != null && v.Wishlist.Any(wishBook => authorBookIsbns.Contains(wishBook.Isbn))
                ).ToList();

                VisitorsGrid.ItemsSource = _filteredVisitors;

                TxtAuthorInfo.Text = $"{L("VisitorsByAuthor_AuthorLabel")} {_selectedAuthor.Name} {_selectedAuthor.Lastname} ({L("VisitorsByAuthor_Found")}: {_filteredVisitors.Count})";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
                MessageBox.Show($"{L("Msg_SearchError")} {ex.Message}", L("Dlg_ErrorTitle"),
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