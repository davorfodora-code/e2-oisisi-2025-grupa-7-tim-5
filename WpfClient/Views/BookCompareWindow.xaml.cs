using Core.DAO;
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
    public partial class BookCompareWindow : Window
    {
        private class BookComboItem
        {
            public string Isbn { get; set; }
            public string DisplayName { get; set; }
        }

        private readonly List<VisitorDTO> _allVisitors;

        private List<VisitorDTO> _bothWishlistResult = new();
        private List<VisitorDTO> _boughtFirstResult = new();

        private string L(string key) => (string)Application.Current.Resources[key];

        public BookCompareWindow(List<BookDTO> allBooks, List<VisitorDTO> allVisitors)
        {
            InitializeComponent();

            _allVisitors = allVisitors;

            var comboItems = allBooks
                .Select(b => new BookComboItem
                {
                    Isbn = b.Isbn,
                    DisplayName = $"{b.Name} (ISBN: {b.Isbn})"
                })
                .OrderBy(b => b.DisplayName)
                .ToList();

            CmbBook1.ItemsSource = comboItems;
            CmbBook2.ItemsSource = comboItems;

            UpdateGrids(new List<VisitorDTO>(), new List<VisitorDTO>());
        }

        private void BtnCompare_Click(object sender, RoutedEventArgs e)
        {
            var book1 = CmbBook1.SelectedItem as BookComboItem;
            var book2 = CmbBook2.SelectedItem as BookComboItem;

            if (book1 == null || book2 == null)
            {
                MessageBox.Show(L("Msg_SelectTwoBooks"), L("Dlg_WarnTitle"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (book1.Isbn == book2.Isbn)
            {
                MessageBox.Show(L("Msg_SelectDifferentBooks"), L("Dlg_WarnTitle"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            TxtSearch.Text = "";

            _bothWishlistResult = _allVisitors
                .Where(v =>
                    v.Wishlist != null &&
                    v.Wishlist.Any(b => b.Isbn == book1.Isbn) &&
                    v.Wishlist.Any(b => b.Isbn == book2.Isbn))
                .ToList();

            var visitorDAO = new VisitorDAO(new PurchaseDAO());
            var allVisitorModels = visitorDAO.GetAllVisitors();

            var membershipsBoughtFirst = allVisitorModels
                .Where(v =>
                    v.Purchases != null &&
                    v.Purchases.Any(p => p.Book.Isbn == book1.Isbn) &&
                    !v.Purchases.Any(p => p.Book.Isbn == book2.Isbn))
                .Select(v => v.MembershipNumber)
                .ToHashSet();

            _boughtFirstResult = _allVisitors
                .Where(v => membershipsBoughtFirst.Contains(v.MembershipNumber))
                .ToList();

            UpdateGrids(_bothWishlistResult, _boughtFirstResult);
        }

        private void UpdateGrids(List<VisitorDTO> bothWishlist, List<VisitorDTO> boughtFirst)
        {
            GridBothWishlist.ItemsSource = new ObservableCollection<VisitorDTO>(bothWishlist);
            GridBoughtFirst.ItemsSource = new ObservableCollection<VisitorDTO>(boughtFirst);

            TxtCount1.Text = $" ({bothWishlist.Count})";
            TxtCount2.Text = $" ({boughtFirst.Count})";
        }

        private void ApplySearch(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                UpdateGrids(_bothWishlistResult, _boughtFirstResult);
                return;
            }

            var q = query.ToLower();

            var filtered1 = _bothWishlistResult.Where(v => MatchesSearch(v, q)).ToList();
            var filtered2 = _boughtFirstResult.Where(v => MatchesSearch(v, q)).ToList();

            GridBothWishlist.ItemsSource = new ObservableCollection<VisitorDTO>(filtered1);
            GridBoughtFirst.ItemsSource = new ObservableCollection<VisitorDTO>(filtered2);

            TxtCount1.Text = $" ({filtered1.Count})";
            TxtCount2.Text = $" ({filtered2.Count})";
        }

        private static bool MatchesSearch(VisitorDTO v, string q)
        {
            return
                (v.MembershipNumber?.ToLower().Contains(q) ?? false) ||
                (v.Name?.ToLower().Contains(q) ?? false) ||
                (v.LastName?.ToLower().Contains(q) ?? false) ||
                (v.DisplayStatus?.ToLower().Contains(q) ?? false);
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            ApplySearch(TxtSearch.Text.Trim());
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ApplySearch(TxtSearch.Text.Trim());
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplySearch(TxtSearch.Text.Trim());
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}