using Core.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfClient
{
    public partial class VisitorAuthorDialog : Window
    {
        private AuthorDTO _selectedAuthor;
        private List<VisitorDTO> _allVisitors;
        private ObservableCollection<VisitorDTO> _filteredVisitors;

        private string L(string key) => (string)Application.Current.Resources[key];

        public VisitorAuthorDialog(AuthorDTO selectedAuthor, List<VisitorDTO> allVisitors)
        {
            InitializeComponent();

            _selectedAuthor = selectedAuthor;
            _allVisitors = allVisitors;

            TxtAuthorInfo.Text = $"{_selectedAuthor.Name} {_selectedAuthor.Lastname} (ID: {_selectedAuthor.IdCard})";

            FilterVisitors();

            VisitorsGrid.ItemsSource = _filteredVisitors;

            UpdateStatus();
        }

        private void FilterVisitors(string searchQuery = "")
        {
            var visitorsWithAuthorBook = _allVisitors.Where(visitor =>
                visitor.Wishlist != null &&
                visitor.Wishlist.Any(book =>
                    book.Authors != null &&
                    book.Authors.Any(a => a.IdCard == _selectedAuthor.IdCard)
                )
            ).ToList();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var searchLower = searchQuery.ToLower();
                visitorsWithAuthorBook = visitorsWithAuthorBook.Where(v =>
                    (v.MembershipNumber != null && v.MembershipNumber.ToLower().Contains(searchLower)) ||
                    (v.Name != null && v.Name.ToLower().Contains(searchLower)) ||
                    (v.LastName != null && v.LastName.ToLower().Contains(searchLower))
                ).ToList();
            }

            _filteredVisitors = new ObservableCollection<VisitorDTO>(visitorsWithAuthorBook);
            VisitorsGrid.ItemsSource = _filteredVisitors;
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            TxtStatus.Text = $"{L("VisitorsByAuthor_TotalVisitors")} {_filteredVisitors?.Count ?? 0}";
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            FilterVisitors(TxtSearch.Text.Trim());
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FilterVisitors(TxtSearch.Text.Trim());
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}