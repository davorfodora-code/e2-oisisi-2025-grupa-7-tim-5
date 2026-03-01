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
            LoadPurchasedBooks();
            LoadWishlist();
            LoadAllBooks();
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

                // prosek ocene (uzmi samo validne 1..5)
                var reviews = _purchases
                    .Select(p => p.Review)
                    .Where(r => r >= 1 && r <= 5)
                    .ToList();

                if (reviews.Any())
                {
                    double avg = reviews.Average();
                    TxtAverageReview.Text = avg.ToString("0.00");
                }
                else
                {
                    TxtAverageReview.Text = "-";
                }
            }
            else
            {
                TxtTotalSpent.Text = "0 RSD";
                TxtAverageReview.Text = "-";
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
        private void BtnDeleteFromWishlist_Click(object sender, RoutedEventArgs e)
        {
            var selectedBook = WishlistBooksGrid.SelectedItem as BookDTO;
            if (selectedBook == null) return;

            var result = MessageBox.Show(
                $"Da li ste sigurni da želite da uklonite knjigu '{selectedBook.Name}' iz liste želja?",
                "Potvrda",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            // ukloni iz wishlist
            _wishlist.Remove(selectedBook);

            // vrati u available ako već nije kupljena
            if (!_availableBooks.Any(b => b.Isbn == selectedBook.Isbn))
                _availableBooks.Add(selectedBook);

            WishlistBooksGrid.ItemsSource = null;
            WishlistBooksGrid.ItemsSource = _wishlist;

            BtnDeleteFromWishlist.IsEnabled = false;
            BtnPurchase.IsEnabled = false;
        }

        private void WishlistBooksGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool hasSelection = WishlistBooksGrid.SelectedItem != null;
            BtnPurchase.IsEnabled = hasSelection;
            BtnDeleteFromWishlist.IsEnabled = hasSelection;
        }
        private void PurchasedBooksGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnCancelPurchase.IsEnabled = PurchasedBooksGrid.SelectedItem != null;
        }
        private void BtnCancelPurchase_Click(object sender, RoutedEventArgs e)
        {
            var selectedPurchase = PurchasedBooksGrid.SelectedItem as Purchase;
            if (selectedPurchase == null) return;

            var result = MessageBox.Show(
                "Da li ste sigurni da želite da poništite kupovinu?",
                "Poništi kupovinu",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            _purchases.Remove(selectedPurchase);

            if (selectedPurchase.Book != null)
            {
                var bookDto = BookDTO.FromModel(selectedPurchase.Book);

                if (!_wishlist.Any(b => b.Isbn == bookDto.Isbn))
                    _wishlist.Add(bookDto);
            }

            PurchasedBooksGrid.ItemsSource = null;
            PurchasedBooksGrid.ItemsSource = _purchases;

            WishlistBooksGrid.ItemsSource = null;
            WishlistBooksGrid.ItemsSource = _wishlist;
            CalculateTotalSpent();

            BtnCancelPurchase.IsEnabled = false;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                if (string.IsNullOrWhiteSpace(TxtName.Text) || string.IsNullOrWhiteSpace(TxtLastName.Text))
                {
                    MessageBox.Show("Ime i prezime su obavezni!", "Greška",
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

                VisitorDTO.Wishlist = _wishlist.Select(book => new BookDTO
                {
                    Isbn = book.Isbn,
                    Name = book.Name,
                    Genre = book.Genre,
                    PublicationYear = book.PublicationYear,
                    Price = book.Price,
                    NumberOfPages = book.NumberOfPages,
                    Publisher = book.Publisher != null ? new PublisherDTO
                    {
                        Id = book.Publisher.Id,
                        Name = book.Publisher.Name
                    } : new PublisherDTO(),
                    Authors = book.Authors?.Select(a => new AuthorDTO
                    {
                        IdCard = a.IdCard,
                        Name = a.Name,
                        Lastname = a.Lastname
                    }).ToList() ?? new System.Collections.Generic.List<AuthorDTO>()
                }).ToList();

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
                _originalVisitor.Purchases = _purchases;
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

            _wishlist.Remove(selectedBook);
            _purchases.Add(purchase);

            var inAvailable = _availableBooks.FirstOrDefault(b => b.Isbn == selectedBook.Isbn);
            if (inAvailable != null)
                _availableBooks.Remove(inAvailable);
            WishlistBooksGrid.ItemsSource = null;
            WishlistBooksGrid.ItemsSource = _wishlist;
            CalculateTotalSpent();

            BtnPurchase.IsEnabled = false;
        }
    }
}