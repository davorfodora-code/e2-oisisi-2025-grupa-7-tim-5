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

           
        }

        private void LoadAllBooks()
        {
            var allBooks = _bookController.GetAllBooks()
                .Select(BookDTO.FromModel)
                .ToList();

            _availableBooks.Clear();

            var purchasedIsbns = _purchases
                .Where(p => p.Book != null)
                .Select(p => p.Book.Isbn)
                .ToHashSet();

            var wishlistIsbns = _wishlist
                .Select(b => b.Isbn)
                .ToHashSet();

            foreach (var book in allBooks)
            {
                if (!wishlistIsbns.Contains(book.Isbn) && !purchasedIsbns.Contains(book.Isbn))
                    _availableBooks.Add(book);
            }

          
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
            try
            {
                // knjige koje mogu u wishlist: nisu ni u wishlist, ni u purchased
                var purchasedIsbns = _purchases
                    .Where(p => p.Book != null)
                    .Select(p => p.Book.Isbn)
                    .ToHashSet();

                var wishlistIsbns = _wishlist
                    .Select(b => b.Isbn)
                    .ToHashSet();

                var candidates = _availableBooks
                    .Where(b => !wishlistIsbns.Contains(b.Isbn) && !purchasedIsbns.Contains(b.Isbn))
                    .ToList();

                if (!candidates.Any())
                {
                    MessageBox.Show("Nema dostupnih knjiga za dodavanje u listu želja.", "Informacija",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var dlg = new WishlistAddDialog(candidates)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                bool? ok = dlg.ShowDialog();
                if (ok != true || dlg.SelectedBook == null) return;

                var selectedBook = dlg.SelectedBook;

                if (_wishlist.Any(b => b.Isbn == selectedBook.Isbn))
                    return;

                _wishlist.Add(selectedBook);

                var inAvailable = _availableBooks.FirstOrDefault(b => b.Isbn == selectedBook.Isbn);
                if (inAvailable != null)
                    _availableBooks.Remove(inAvailable);

                WishlistBooksGrid.ItemsSource = null;
                WishlistBooksGrid.ItemsSource = _wishlist;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Greška (Dodaj)", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

                // Vrati u dostupne SAMO ako nije kupljena
                bool isPurchased = _purchases.Any(p => p.Book != null && p.Book.Isbn == bookToRemove.Isbn);
                if (!isPurchased && !_availableBooks.Any(b => b.Isbn == bookToRemove.Isbn))
                    _availableBooks.Add(bookToRemove);

                // Osveži prikaz
              
                WishlistBooksGrid.ItemsSource = null;
                WishlistBooksGrid.ItemsSource = _wishlist;
            }
        }

        private void WishlistBooksGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var btn = FindName("BtnPurchase") as Button;
            if (btn != null)
                btn.IsEnabled = WishlistBooksGrid.SelectedItem != null;
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

       

        private void BtnPurchase_Click(object sender, RoutedEventArgs e)
        {
            var selectedBook = WishlistBooksGrid.SelectedItem as BookDTO;
            if (selectedBook == null) return;

            var dlg = new PurchaseDialog(selectedBook.Isbn, selectedBook.Name)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            bool? ok = dlg.ShowDialog();
            if (ok != true) return;

            // napravi Purchase i prebaci knjigu iz wishlist u purchased
            var purchase = new Purchase
            {
                Book = new Book
                {
                    Isbn = selectedBook.Isbn,
                    Name = selectedBook.Name,
                    Price = selectedBook.Price,
                    Authors = selectedBook.Authors?.Select(a => new Author
                    {
                        IdCard = a.IdCard,
                        Name = a.Name,
                        Lastname = a.Lastname
                    }).ToList()
                },
                Review = dlg.Review,
                DateOfPurchase = DateOnly.FromDateTime(dlg.DateOfPurchase),
                Comment = ""
            };

            // 1) ukloni iz wishlist
            _wishlist.Remove(selectedBook);

            // 2) dodaj u kupljene
            _purchases.Add(purchase);

            // 3) ukloni iz dostupnih (da se ne pojavljuje više) - i NE vraćaj kupljeno u available
            var inAvailable = _availableBooks.FirstOrDefault(b => b.Isbn == selectedBook.Isbn);
            if (inAvailable != null)
                _availableBooks.Remove(inAvailable);

            // 4) osveži prikaze
           

            WishlistBooksGrid.ItemsSource = null;
            WishlistBooksGrid.ItemsSource = _wishlist;

           

            // 5) preračunaj potrošeno
            CalculateTotalSpent();

            BtnPurchase.IsEnabled = false;
        }
    }
}