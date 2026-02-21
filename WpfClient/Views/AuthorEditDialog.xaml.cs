using Core.Controllers;
using Core.DTO;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    public partial class AuthorEditDialog : Window
    {
        public AuthorDTO AuthorDTO { get; private set; }
        private Author _originalAuthor;
        private BookController _bookController;
        private ObservableCollection<BookDTO> _authorBooks;
        private ObservableCollection<BookDTO> _availableBooks;
        private ObservableCollection<BookDTO> _allBooksOriginal; 

        public AuthorEditDialog(AuthorDTO authorDto, Author originalAuthor, BookController bookController)
        {
            InitializeComponent();

            AuthorDTO = authorDto;
            _originalAuthor = originalAuthor;
            _bookController = bookController;

            _authorBooks = new ObservableCollection<BookDTO>();
            _availableBooks = new ObservableCollection<BookDTO>();
            _allBooksOriginal = new ObservableCollection<BookDTO>();

            LoadAuthorData();
            LoadAllBooks();
            LoadAuthorBooks();

            BooksGrid.ItemsSource = _authorBooks;
        }

        private void LoadAuthorData()
        {
            TxtIdCard.Text = AuthorDTO.IdCard;
            TxtIdCard.IsEnabled = false;

            TxtName.Text = AuthorDTO.Name;
            TxtLastName.Text = AuthorDTO.Lastname;

            if (AuthorDTO.DateOfBirth != default)
            {
                DpDateOfBirth.SelectedDate = AuthorDTO.DateOfBirth.ToDateTime(new TimeOnly(0, 0));
            }

            TxtPhoneNumber.Text = AuthorDTO.PhoneNumber;
            TxtEmail.Text = AuthorDTO.Email;
            TxtExperienceLength.Text = AuthorDTO.ExperienceLength.ToString();

            if (AuthorDTO.Address != null)
            {
                TxtStreet.Text = AuthorDTO.Address.Street;
                TxtNumber.Text = AuthorDTO.Address.Number;
                TxtCity.Text = AuthorDTO.Address.City;
                TxtState.Text = AuthorDTO.Address.State;
                TxtCountry.Text = AuthorDTO.Address.Country;
            }
        }

        private void LoadAllBooks()
        {
            try
            {
                var allBooks = _bookController.GetAllBooks()
                    .Select(BookDTO.FromModel)
                    .ToList();

                _allBooksOriginal.Clear();
                _availableBooks.Clear();

                foreach (var book in allBooks)
                {
                    _allBooksOriginal.Add(book);
                    _availableBooks.Add(book);
                }

                CmbAvailableBooks.ItemsSource = _availableBooks;
                CmbAvailableBooks.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju knjiga: {ex.Message}", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAuthorBooks()
        {
            _authorBooks.Clear();

            if (_originalAuthor.BookList != null && _originalAuthor.BookList.Any())
            {
                foreach (var book in _originalAuthor.BookList)
                {
                    var fullBook = _allBooksOriginal.FirstOrDefault(b => b.Isbn == book.Isbn);
                    if (fullBook != null)
                    {
                        _authorBooks.Add(fullBook);
                        var availableBook = _availableBooks.FirstOrDefault(b => b.Isbn == fullBook.Isbn);
                        if (availableBook != null)
                            _availableBooks.Remove(availableBook);
                    }
                    else
                    {

                        var bookDto = new BookDTO
                        {
                            Isbn = book.Isbn,
                            Name = book.Name
                        };
                        _authorBooks.Add(bookDto);
                    }
                }
            }
        }

        private void BtnAddBook_Click(object sender, RoutedEventArgs e)
        {
            var selectedBook = CmbAvailableBooks.SelectedItem as BookDTO;

            if (selectedBook == null)
            {
                MessageBox.Show("Izaberite knjigu za dodavanje!", "Upozorenje",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_authorBooks.Any(b => b.Isbn == selectedBook.Isbn))
            {
                MessageBox.Show("Ova knjiga je već dodeljena autoru!", "Upozorenje",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (selectedBook.Authors == null)
                selectedBook.Authors = new System.Collections.Generic.List<AuthorDTO>();

            if (!selectedBook.Authors.Any(a => a.IdCard == AuthorDTO.IdCard))
            {
                selectedBook.Authors.Add(new AuthorDTO
                {
                    IdCard = AuthorDTO.IdCard,
                    Name = AuthorDTO.Name,
                    Lastname = AuthorDTO.Lastname
                });
            }

            _authorBooks.Add(selectedBook);

            _availableBooks.Remove(selectedBook);

            CmbAvailableBooks.ItemsSource = null;
            CmbAvailableBooks.ItemsSource = _availableBooks;
            CmbAvailableBooks.SelectedItem = null;

            MessageBox.Show($"Knjiga '{selectedBook.Name}' je dodata autoru.", "Uspešno",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnRemoveBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var bookToRemove = button?.DataContext as BookDTO;

                if (bookToRemove == null) return;

                var result = MessageBox.Show(
                    $"Da li ste sigurni da želite da uklonite knjigu '{bookToRemove.Name}'?",
                    "Potvrda",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (bookToRemove.Authors != null)
                    {
                        var authorToRemove = bookToRemove.Authors
                            .FirstOrDefault(a => a.IdCard == AuthorDTO.IdCard);
                        if (authorToRemove != null)
                        {
                            bookToRemove.Authors.Remove(authorToRemove);
                        }
                    }

                    _authorBooks.Remove(bookToRemove);

                    _availableBooks.Add(bookToRemove);

                    CmbAvailableBooks.ItemsSource = null;
                    CmbAvailableBooks.ItemsSource = _availableBooks;

                    MessageBox.Show("Knjiga uklonjena sa autora!", "Uspešno",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri uklanjanju: {ex.Message}", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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

                if (!DpDateOfBirth.SelectedDate.HasValue)
                {
                    MessageBox.Show("Datum rođenja je obavezan!", "Greška",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!double.TryParse(TxtExperienceLength.Text, out double experience))
                {
                    MessageBox.Show("Dužina iskustva mora biti broj!", "Greška",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                AuthorDTO.Name = TxtName.Text.Trim();
                AuthorDTO.Lastname = TxtLastName.Text.Trim();
                AuthorDTO.DateOfBirth = DateOnly.FromDateTime(DpDateOfBirth.SelectedDate.Value);
                AuthorDTO.PhoneNumber = TxtPhoneNumber.Text.Trim();
                AuthorDTO.Email = TxtEmail.Text.Trim();
                AuthorDTO.ExperienceLength = experience;

                if (AuthorDTO.Address == null)
                    AuthorDTO.Address = new AddressDTO();

                AuthorDTO.Address.Street = TxtStreet.Text.Trim();
                AuthorDTO.Address.Number = TxtNumber.Text.Trim();
                AuthorDTO.Address.City = TxtCity.Text.Trim();
                AuthorDTO.Address.State = TxtState.Text.Trim();
                AuthorDTO.Address.Country = TxtCountry.Text.Trim();

                foreach (var book in _allBooksOriginal)
                {
                    _bookController.EditBook(book);
                }


                AuthorDTO.BookList = _authorBooks.ToList();

 
                _originalAuthor.Name = AuthorDTO.Name;
                _originalAuthor.Lastname = AuthorDTO.Lastname;
                _originalAuthor.DateOfBirth = AuthorDTO.DateOfBirth;
                _originalAuthor.PhoneNumber = AuthorDTO.PhoneNumber;
                _originalAuthor.Email = AuthorDTO.Email;
                _originalAuthor.ExperienceLength = AuthorDTO.ExperienceLength;
                _originalAuthor.Address = AuthorDTO.Address.ToModel();
                _originalAuthor.BookList = AuthorDTO.BookList.Select(b => b.ToModel()).ToList();

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