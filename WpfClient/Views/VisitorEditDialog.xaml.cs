using Core.Controllers;
using Core.DTO;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    public partial class VisitorEditDialog : Window
    {
        public VisitorDTO VisitorDTO { get; private set; }
        private Visitor _originalVisitor;
        private BookController _bookController;
        private ObservableCollection<BookDTO> _wishlist;
        private ObservableCollection<BookDTO> _availableBooks;
        private List<Purchase> _purchases;

        public VisitorEditDialog(VisitorDTO visitorDto, Visitor originalVisitor, BookController bookController)
        {
            InitializeComponent();

            VisitorDTO = visitorDto;
            _originalVisitor = originalVisitor;
            _bookController = bookController;
            _purchases = originalVisitor.Purchases ?? new List<Purchase>();

            _wishlist = new ObservableCollection<BookDTO>();
            _availableBooks = new ObservableCollection<BookDTO>();

            LoadVisitorData();
            LoadAllBooks();
            LoadWishlist();
            LoadPurchasedBooks();
            CalculateTotalSpent();

            WishlistBooksGrid.ItemsSource = _wishlist;
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

            CmbStatus.SelectedIndex = VisitorDTO.Status == "R" ? 0 : 1;
        }

        private void LoadAllBooks()
        {
            var allBooks = _bookController.GetAllBooks()
                .Select(BookDTO.FromModel)
                .ToList();

            _availableBooks.Clear();
            foreach (var book in allBooks)
            {
                _availableBooks.Add(book);
            }

            CmbAvailableBooks.ItemsSource = _availableBooks;
            CmbAvailableBooks.DisplayMemberPath = "Name";
        }

        private void LoadWishlist()
        {
            _wishlist.Clear();

            // Prvo iz originalnog modela (Visitor)
            if (_originalVisitor.Wishlist != null)
            {
                foreach (var book in _originalVisitor.Wishlist)
                {
                    // Pronađi kompletnu knjigu iz _availableBooks ili kreiraj novu
                    var fullBook = _availableBooks.FirstOrDefault(b => b.Isbn == book.Isbn);

                    if (fullBook != null)
                    {
                        _wishlist.Add(fullBook);
                        _availableBooks.Remove(fullBook);
                    }
                    else
                    {
                        // Ako nema u available, kreiraj sa osnovnim podacima
                        var bookDto = new BookDTO
                        {
                            Isbn = book.Isbn,
                            Name = book.Name,
                            Authors = book.Authors?.Select(a => new AuthorDTO
                            {
                                IdCard = a.IdCard,
                                Name = a.Name,
                                Lastname = a.Lastname
                            }).ToList() ?? new System.Collections.Generic.List<AuthorDTO>(),
                            Price = book.Price
                        };
                        _wishlist.Add(bookDto);
                    }
                }
            }

            // Zatim iz DTO (VisitorDTO.Wishlist)
            if (VisitorDTO.Wishlist != null)
            {
                foreach (var book in VisitorDTO.Wishlist)
                {
                    if (!_wishlist.Any(b => b.Isbn == book.Isbn))
                    {
                        // Pronađi kompletnu knjigu
                        var fullBook = _availableBooks.FirstOrDefault(b => b.Isbn == book.Isbn);

                        if (fullBook != null)
                        {
                            _wishlist.Add(fullBook);
                            _availableBooks.Remove(fullBook);
                        }
                        else
                        {
                            _wishlist.Add(book);
                        }
                    }
                }
            }

            // Osveži ComboBox
            CmbAvailableBooks.ItemsSource = null;
            CmbAvailableBooks.ItemsSource = _availableBooks;
        }

        private void LoadPurchasedBooks()
        {
            PurchasedBooksGrid.ItemsSource = _purchases;
        }

        private void CalculateTotalSpent()
        {
            if (_purchases != null && _purchases.Any())
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
            var selectedBook = CmbAvailableBooks.SelectedItem as BookDTO;

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

            // Dodaj u wishlist (kompletna knjiga sa svim podacima)
            _wishlist.Add(selectedBook);

            // Ukloni iz dostupnih
            _availableBooks.Remove(selectedBook);

            // Osveži prikaz
            CmbAvailableBooks.ItemsSource = null;
            CmbAvailableBooks.ItemsSource = _availableBooks;
            WishlistBooksGrid.ItemsSource = null;
            WishlistBooksGrid.ItemsSource = _wishlist;

            MessageBox.Show($"Knjiga '{selectedBook.Name}' je dodata u listu želja!", "Uspešno",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnRemoveFromWishlist_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var bookToRemove = button?.DataContext as BookDTO;

            if (bookToRemove == null) return;

            var result = MessageBox.Show(
                $"Da li ste sigurni da želite da uklonite knjigu '{bookToRemove.Name}' iz liste želja?",
                "Potvrda",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Ukloni iz wishlist
                _wishlist.Remove(bookToRemove);

                // Vrati u dostupne (sa svim podacima)
                _availableBooks.Add(bookToRemove);

                // Osveži prikaz
                CmbAvailableBooks.ItemsSource = null;
                CmbAvailableBooks.ItemsSource = _availableBooks;
                WishlistBooksGrid.ItemsSource = null;
                WishlistBooksGrid.ItemsSource = _wishlist;
            }
        }

        private void CmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbStatus.SelectedItem is ComboBoxItem item)
            {
                VisitorDTO.Status = item.Tag.ToString() == "R" ? "R" : "V";
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validacija
                if (string.IsNullOrWhiteSpace(TxtName.Text) || string.IsNullOrWhiteSpace(TxtLastName.Text))
                {
                    MessageBox.Show("Ime i prezime su obavezni!", "Greška",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Ažuriraj DTO
                VisitorDTO.Name = TxtName.Text.Trim();
                VisitorDTO.LastName = TxtLastName.Text.Trim();

                if (VisitorDTO.Address == null)
                    VisitorDTO.Address = new AddressDTO();

                VisitorDTO.Address.Street = TxtStreet.Text.Trim();
                VisitorDTO.Address.Number = TxtStreetNumber.Text.Trim();
                VisitorDTO.Address.City = TxtCity.Text.Trim();
                VisitorDTO.Address.Country = TxtCountry.Text.Trim();

                // Ažuriraj Wishlist u DTO (sa kompletnim podacima)
                VisitorDTO.Wishlist = _wishlist.ToList();

                // Ažuriraj originalni model
                _originalVisitor.Name = VisitorDTO.Name;
                _originalVisitor.LastName = VisitorDTO.LastName;
                _originalVisitor.Address = VisitorDTO.Address.ToModel();
                _originalVisitor.Wishlist = VisitorDTO.Wishlist.Select(b => new Book
                {
                    Isbn = b.Isbn,
                    Name = b.Name,
                    Price = b.Price,
                    Authors = b.Authors?.Select(a => new Author
                    {
                        IdCard = a.IdCard,
                        Name = a.Name,
                        Lastname = a.Lastname
                    }).ToList()
                }).ToList();
                _originalVisitor.Status = VisitorDTO.Status;

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