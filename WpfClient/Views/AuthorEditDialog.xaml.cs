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
        private ObservableCollection<BookDTO> _allBooks;

        public AuthorEditDialog(AuthorDTO authorDto, Author originalAuthor, BookController bookController)
        {
            InitializeComponent();

            AuthorDTO = authorDto;
            _originalAuthor = originalAuthor;
            _bookController = bookController;

            _authorBooks = new ObservableCollection<BookDTO>();
            _allBooks = new ObservableCollection<BookDTO>();

            LoadAuthorData();
            LoadBooks();
        }

        private void LoadAuthorData()
        {
            TxtIdCard.Text = AuthorDTO.IdCard;
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

        private void LoadBooks()
        {
            try
            {
                var allBooks = _bookController.GetAllBooks()
                    .Select(BookDTO.FromModel)
                    .ToList();

                _allBooks.Clear();
                foreach (var book in allBooks)
                {
                    _allBooks.Add(book);
                }

                var authorBooks = _allBooks
                    .Where(book => book.Authors != null &&
                                  book.Authors.Any(a => a.IdCard == AuthorDTO.IdCard))
                    .ToList();

                _authorBooks.Clear();
                foreach (var book in authorBooks)
                {
                    _authorBooks.Add(book);
                }

                BooksGrid.ItemsSource = _authorBooks;

                RefreshAvailableBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri učitavanju knjiga: {ex.Message}", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshAvailableBooks()
        {
            var availableBooks = _allBooks
                .Where(book => !_authorBooks.Any(b => b.Isbn == book.Isbn))
                .ToList();

            CmbAvailableBooks.ItemsSource = availableBooks;

            if (availableBooks.Any())
                CmbAvailableBooks.SelectedIndex = 0;
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
                RefreshAvailableBooks();
                return;
            }

            _authorBooks.Add(selectedBook);

            RefreshAvailableBooks();

            MessageBox.Show($"Knjiga '{selectedBook.Name}' je dodata autoru.", "Uspešno",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnRemoveBook_Click(object sender, RoutedEventArgs e)
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
                _authorBooks.Remove(bookToRemove);

                RefreshAvailableBooks();
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

                _originalAuthor.BookList = _authorBooks.Select(b => b.ToModel()).ToList();

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