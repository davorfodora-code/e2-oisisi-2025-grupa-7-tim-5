using Core.Controllers;
using Core.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace WpfClient
{
    public partial class VisitorEditDialog : Window
    {
        public VisitorDTO VisitorDTO { get; private set; }
        private Visitor _originalVisitor;
        private List<Purchase> _purchases;
        private ObservableCollection<Book> _wishlist;
        private List<Book> _allBooks;

        public VisitorEditDialog(VisitorDTO visitorDto, Visitor originalVisitor, BookController bookController)
        {
            InitializeComponent();

            VisitorDTO = visitorDto;
            _originalVisitor = originalVisitor;
            _purchases = originalVisitor.Purchases ?? new List<Purchase>();
            _wishlist = new ObservableCollection<Book>(originalVisitor.Wishlist ?? new List<Book>());

            var allBooks = bookController.GetAllBooks();
            _allBooks = allBooks ?? new List<Book>();

            LoadVisitorData();
            LoadPurchasedBooks();
            LoadWishlistBooks();
            LoadAvailableBooks();
            CalculateTotalSpent();

        }

        private void LoadVisitorData()
        {
            TxtMembershipNumber.Text = VisitorDTO.MembershipNumber;
            TxtMembershipYear.Text = _originalVisitor.MembershipYear;
            TxtName.Text = VisitorDTO.Name;
            TxtLastName.Text = VisitorDTO.LastName;
            TxtDateOfBirth.Text = _originalVisitor.DateOfBirth;

            TxtStreet.Text = _originalVisitor.Address?.Street ?? "";
            TxtStreetNumber.Text = _originalVisitor.Address?.Number ?? "";
            TxtCity.Text = _originalVisitor.Address?.City ?? "";
            TxtCountry.Text = _originalVisitor.Address?.Country ?? "";

            if (VisitorDTO.Status == "R")
                CmbStatus.SelectedIndex = 0;
            else
                CmbStatus.SelectedIndex = 1;
        }

        private void LoadPurchasedBooks()
        {
            PurchasedBooksGrid.ItemsSource = _purchases;
        }

        private void LoadWishlistBooks()
        {
            WishlistBooksGrid.ItemsSource = _wishlist;
        }

        private void LoadAvailableBooks()
        {
            var availableBooks = _allBooks
                .Where(book => !_wishlist.Any(w => w.Isbn == book.Isbn))
                .ToList();

            CmbAvailableBooks.ItemsSource = availableBooks;

            if (availableBooks.Any())
                CmbAvailableBooks.SelectedIndex = 0;
        }


        private void CalculateTotalSpent()
        {
            if (_purchases != null && _purchases.Count > 0)
            {
                double total = _purchases.Sum(p => p.Book?.Price ?? 0);
                TxtTotalSpent.Text = $"{total:C}";
            }
            else
            {
                TxtTotalSpent.Text = "0 RSD";
            }
        }

        private void BtnAddToWishlist_Click(object sender, RoutedEventArgs e)
        {
            var selectedBook = CmbAvailableBooks.SelectedItem as Book;

            if (selectedBook == null)
            {
                MessageBox.Show("Izaberite knjigu za dodavanje!", "Upozorenje",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_wishlist.Any(b => b.Isbn == selectedBook.Isbn))
            {
                MessageBox.Show("Ova knjiga je već u listi želja!", "Upozorenje",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _wishlist.Add(selectedBook);

            WishlistBooksGrid.ItemsSource = null;
            WishlistBooksGrid.ItemsSource = _wishlist;

            LoadAvailableBooks();

            MessageBox.Show($"Knjiga '{selectedBook.Name}' je dodata u listu želja!", "Uspešno",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnRemoveFromWishlist_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var bookToRemove = button?.DataContext as Book;

            if (bookToRemove == null) return;

            var result = MessageBox.Show(
                $"Da li ste sigurni da želite da uklonite knjigu '{bookToRemove.Name}' iz liste želja?",
                "Potvrda",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _wishlist.Remove(bookToRemove);

                WishlistBooksGrid.ItemsSource = null;
                WishlistBooksGrid.ItemsSource = _wishlist;

                LoadAvailableBooks();
            }
        }


        private void CmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbStatus.SelectedItem is ComboBoxItem item)
            {
                if (item.Tag.ToString() == "R")
                {
                    VisitorDTO.Status = "R";
                }
                else
                {
                    VisitorDTO.Status = "V";
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TxtName.Text))
                {
                    MessageBox.Show("Ime je obavezno!", "Greška",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(TxtLastName.Text))
                {
                    MessageBox.Show("Prezime je obavezno!", "Greška",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                VisitorDTO.Name = TxtName.Text.Trim();
                VisitorDTO.LastName = TxtLastName.Text.Trim();


                if (VisitorDTO.Address == null)
                    VisitorDTO.Address = new AddressDTO();

                VisitorDTO.Address.Street = TxtStreet.Text.Trim();
                VisitorDTO.Address.Number = TxtStreetNumber.Text.Trim();
                VisitorDTO.Address.City = TxtCity.Text.Trim();
                VisitorDTO.Address.Country = TxtCountry.Text.Trim();

                _originalVisitor.Wishlist = _wishlist.ToList();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri čuvanju: {ex.Message}", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}