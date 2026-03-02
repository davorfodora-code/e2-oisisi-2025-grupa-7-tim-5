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

        private string L(string key) => (string)Application.Current.Resources[key];

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

            // ← NOVO: DatePicker
            if (_originalVisitor.DateOfBirth != default)
                DpDateOfBirth.SelectedDate = _originalVisitor.DateOfBirth.ToDateTime(TimeOnly.MinValue);

            // ← NOVO: ContactPhone, Email
            TxtContactPhone.Text = _originalVisitor.ContactPhone ?? "";
            TxtEmail.Text = _originalVisitor.Email ?? "";

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
                    var fullBook = _availableBooks.FirstOrDefault(b => b.Isbn == book.Isbn);

                    if (fullBook != null)
                    {
                        _wishlist.Add(fullBook);
                        _availableBooks.Remove(fullBook);
                    }
                    else
                    {
                        var bookDto = new BookDTO
                        {
                            Isbn = book.Isbn,
                            Name = book.Name,
                            Authors = book.Authors?.Select(a => new AuthorDTO
                            {
                                IdCard = a.IdCard,
                                Name = a.Name,
                                Lastname = a.Lastname
                            }).ToList() ?? new List<AuthorDTO>(),
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
                TxtTotalSpent.Text = $"{total:N2} RSD";

                var reviews = _purchases
                    .Select(p => p.Review)
                    .Where(r => r >= 1 && r <= 5)
                    .ToList();

                TxtAverageReview.Text = reviews.Any() ? reviews.Average().ToString("0.00") : "-";
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
                var purchasedIsbns = _purchases
                    .Where(p => p.Book != null)
                    .Select(p => p.Book.Isbn)
                    .ToHashSet();

                var wishlistIsbns = _wishlist.Select(b => b.Isbn).ToHashSet();

                var candidates = _availableBooks
                    .Where(b => !wishlistIsbns.Contains(b.Isbn) && !purchasedIsbns.Contains(b.Isbn))
                    .ToList();

                if (!candidates.Any())
                {
                    MessageBox.Show(L("Msg_NoAvailableBooks"), L("Dlg_InfoTitle"),
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
                if (_wishlist.Any(b => b.Isbn == selectedBook.Isbn)) return;

                _wishlist.Add(selectedBook);
                var inAvailable = _availableBooks.FirstOrDefault(b => b.Isbn == selectedBook.Isbn);
                if (inAvailable != null) _availableBooks.Remove(inAvailable);

                RefreshWishlistGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), L("Dlg_ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteFromWishlist_Click(object sender, RoutedEventArgs e)
        {
            var selectedBook = WishlistBooksGrid.SelectedItem as BookDTO;
            if (selectedBook == null) return;

            var result = MessageBox.Show(
                $"{L("Msg_ConfirmRemoveFromWishlist")} '{selectedBook.Name}'?",
                L("Dlg_ConfirmTitle"), MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            _wishlist.Remove(selectedBook);
            if (!_availableBooks.Any(b => b.Isbn == selectedBook.Isbn))
                _availableBooks.Add(selectedBook);

            RefreshWishlistGrid();
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

            var result = MessageBox.Show(L("Msg_ConfirmCancelPurchase"), L("Btn_CancelPurchase"),
                MessageBoxButton.YesNo, MessageBoxImage.Question);

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
            RefreshWishlistGrid();
            CalculateTotalSpent();
            BtnCancelPurchase.IsEnabled = false;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TxtName.Text) || string.IsNullOrWhiteSpace(TxtLastName.Text))
                {
                    MessageBox.Show(L("Msg_NameRequired"), L("Dlg_ErrorTitle"),
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                VisitorDTO.Name = TxtName.Text.Trim();
                VisitorDTO.LastName = TxtLastName.Text.Trim();

                // ← NOVO: DateOfBirth iz DatePickera
                if (DpDateOfBirth.SelectedDate.HasValue)
                    VisitorDTO.DateOfBirth = DateOnly.FromDateTime(DpDateOfBirth.SelectedDate.Value);

                // ← NOVO: ContactPhone, Email
                VisitorDTO.ContactPhone = TxtContactPhone.Text.Trim();
                VisitorDTO.Email = TxtEmail.Text.Trim();

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
                    Publisher = book.Publisher != null
                        ? new PublisherDTO { Id = book.Publisher.Id, Name = book.Publisher.Name }
                        : new PublisherDTO(),
                    Authors = book.Authors?.Select(a => new AuthorDTO
                    {
                        IdCard = a.IdCard,
                        Name = a.Name,
                        Lastname = a.Lastname
                    }).ToList() ?? new List<AuthorDTO>()
                }).ToList();

                // Ažuriramo originalVisitor
                _originalVisitor.Name = VisitorDTO.Name;
                _originalVisitor.LastName = VisitorDTO.LastName;
                _originalVisitor.DateOfBirth = VisitorDTO.DateOfBirth;
                _originalVisitor.ContactPhone = VisitorDTO.ContactPhone;
                _originalVisitor.Email = VisitorDTO.Email;
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
                MessageBox.Show($"{L("Msg_SaveError")} {ex.Message}", L("Dlg_ErrorTitle"),
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
            if (inAvailable != null) _availableBooks.Remove(inAvailable);

            RefreshWishlistGrid();
            CalculateTotalSpent();
            BtnPurchase.IsEnabled = false;
        }

        private void RefreshWishlistGrid()
        {
            WishlistBooksGrid.ItemsSource = null;
            WishlistBooksGrid.ItemsSource = _wishlist;
        }
    }
}