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

        private string L(string key) => (string)Application.Current.Resources[key];

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
                DpDateOfBirth.SelectedDate = AuthorDTO.DateOfBirth.ToDateTime(new TimeOnly(0, 0));

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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{L("Msg_LoadBooksError")} {ex.Message}", L("Dlg_ErrorTitle"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAuthorBooks()
        {
            _authorBooks.Clear();

            if (_originalAuthor.BookList == null || !_originalAuthor.BookList.Any()) return;

            foreach (var book in _originalAuthor.BookList)
            {
                var fullBook = _allBooksOriginal.FirstOrDefault(b => b.Isbn == book.Isbn);
                if (fullBook != null)
                {
                    _authorBooks.Add(fullBook);
                    var av = _availableBooks.FirstOrDefault(b => b.Isbn == fullBook.Isbn);
                    if (av != null) _availableBooks.Remove(av);
                }
                else
                {
                    _authorBooks.Add(new BookDTO { Isbn = book.Isbn, Name = book.Name });
                }
            }
        }

        // ── BooksGrid selekcija — aktivira Ukloni dugme ─────────────

        private void BooksGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnRemoveBookToolbar.IsEnabled = BooksGrid.SelectedItem != null;
        }

        // ── Dodaj knjigu — otvori SelectBookDialog ────────────────────

        private void BtnAddBook_Click(object sender, RoutedEventArgs e)
        {
            var available = _availableBooks
                .Where(b => !_authorBooks.Any(ab => ab.Isbn == b.Isbn))
                .ToList();

            if (!available.Any())
            {
                MessageBox.Show(L("Msg_NoAvailableBooks"), L("Dlg_InfoTitle"),
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dlg = new SelectBookDialog(available)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (dlg.ShowDialog() != true || dlg.SelectedBook == null) return;

            var selectedBook = dlg.SelectedBook;

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
            var inAvailable = _availableBooks.FirstOrDefault(b => b.Isbn == selectedBook.Isbn);
            if (inAvailable != null) _availableBooks.Remove(inAvailable);
        }

        // ── Ukloni selektovanu knjigu (toolbar dugme) ─────────────────

        private void BtnRemoveBookToolbar_Click(object sender, RoutedEventArgs e)
        {
            var bookToRemove = BooksGrid.SelectedItem as BookDTO;
            if (bookToRemove == null) return;

            var result = MessageBox.Show(
                $"{L("Msg_ConfirmRemoveBook")} '{bookToRemove.Name}'?",
                L("Dlg_ConfirmTitle"),
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            bookToRemove.Authors?.RemoveAll(a => a.IdCard == AuthorDTO.IdCard);

            _authorBooks.Remove(bookToRemove);
            _availableBooks.Add(bookToRemove);
            BtnRemoveBookToolbar.IsEnabled = false;
        }

        // ── Sačuvaj ───────────────────────────────────────────────────

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TxtName.Text))
                { MessageBox.Show(L("Msg_FirstNameRequired"), L("Dlg_ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error); return; }

                if (string.IsNullOrWhiteSpace(TxtLastName.Text))
                { MessageBox.Show(L("Msg_LastNameRequired"), L("Dlg_ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error); return; }

                if (!DpDateOfBirth.SelectedDate.HasValue)
                { MessageBox.Show(L("Msg_DateOfBirthRequired"), L("Dlg_ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error); return; }

                if (!double.TryParse(TxtExperienceLength.Text, out double experience))
                { MessageBox.Show(L("Msg_ExperienceMustBeNumber"), L("Dlg_ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error); return; }

                AuthorDTO.Name = TxtName.Text.Trim();
                AuthorDTO.Lastname = TxtLastName.Text.Trim();
                AuthorDTO.DateOfBirth = DateOnly.FromDateTime(DpDateOfBirth.SelectedDate.Value);
                AuthorDTO.PhoneNumber = TxtPhoneNumber.Text.Trim();
                AuthorDTO.Email = TxtEmail.Text.Trim();
                AuthorDTO.ExperienceLength = experience;

                if (AuthorDTO.Address == null) AuthorDTO.Address = new AddressDTO();
                AuthorDTO.Address.Street = TxtStreet.Text.Trim();
                AuthorDTO.Address.Number = TxtNumber.Text.Trim();
                AuthorDTO.Address.City = TxtCity.Text.Trim();
                AuthorDTO.Address.State = TxtState.Text.Trim();
                AuthorDTO.Address.Country = TxtCountry.Text.Trim();

                foreach (var book in _allBooksOriginal)
                    _bookController.EditBook(book);

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
                MessageBox.Show($"{L("Msg_SaveError")} {ex.Message}", L("Dlg_ErrorTitle"),
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