using Core.DTO;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    public partial class AddBookDialog : Window
    {
        public BookDTO? Book { get; private set; }
        private bool _isEditMode;
        private BookDTO? _originalBook;

        public ObservableCollection<AuthorDTO> AllAuthors { get; set; }
        public ObservableCollection<PublisherDTO> AllPublishers { get; set; }

        private ObservableCollection<AuthorDTO> _selectedAuthors = new();

        private string L(string key) => (string)Application.Current.Resources[key];

        public AddBookDialog(ObservableCollection<AuthorDTO> authors, ObservableCollection<PublisherDTO> publishers)
        {
            InitializeComponent();
            _isEditMode = false;
            AllAuthors = authors ?? new ObservableCollection<AuthorDTO>();
            AllPublishers = publishers ?? new ObservableCollection<PublisherDTO>();
            Setup();
        }

        public AddBookDialog(BookDTO bookToEdit, ObservableCollection<AuthorDTO> authors, ObservableCollection<PublisherDTO> publishers)
        {
            InitializeComponent();
            _isEditMode = true;
            _originalBook = bookToEdit;
            AllAuthors = authors ?? new ObservableCollection<AuthorDTO>();
            AllPublishers = publishers ?? new ObservableCollection<PublisherDTO>();

            Title = L("EditBook_Title");
            BtnSave.Content = L("Btn_SaveChanges");

            Setup();

            TxtIsbn.Text = bookToEdit.Isbn;
            TxtIsbn.IsEnabled = false;
            TxtBookName.Text = bookToEdit.Name;
            CmbGenre.SelectedItem = bookToEdit.Genre;
            TxtPublicationYear.Text = bookToEdit.PublicationYear;
            TxtPrice.Text = bookToEdit.Price.ToString();
            TxtPages.Text = bookToEdit.NumberOfPages.ToString();

            if (bookToEdit.Publisher != null && !string.IsNullOrWhiteSpace(bookToEdit.Publisher.Name))
            {
                var pub = AllPublishers.FirstOrDefault(p => p.Name == bookToEdit.Publisher.Name);
                if (pub != null) CmbPublisher.SelectedItem = pub;
            }

            if (bookToEdit.Authors != null)
            {
                foreach (var a in bookToEdit.Authors)
                {
                    var match = AllAuthors.FirstOrDefault(x => x.IdCard == a.IdCard);
                    if (match != null && !_selectedAuthors.Any(x => x.IdCard == match.IdCard))
                        _selectedAuthors.Add(match);
                }
            }

            UpdateAddButtonVisibility();
        }

        public AddBookDialog(ObservableCollection<AuthorDTO> authors)
            : this(authors, new ObservableCollection<PublisherDTO>()) { }

        public AddBookDialog(BookDTO bookToEdit, ObservableCollection<AuthorDTO> authors)
            : this(bookToEdit, authors, new ObservableCollection<PublisherDTO>()) { }
        private void Setup()
        {
            CmbPublisher.ItemsSource = AllPublishers;
            CmbGenre.ItemsSource = Enum.GetValues(typeof(Genre)).Cast<Genre>();
            LstSelectedAuthors.ItemsSource = _selectedAuthors;

            _selectedAuthors.CollectionChanged += (s, e) => UpdateAddButtonVisibility();

            UpdateAddButtonVisibility();
        }

        private void UpdateAddButtonVisibility()
        {
            BtnAddAuthor.Visibility = _selectedAuthors.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
        }


        private void LstSelectedAuthors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }


        private void BtnAddAuthor_Click(object sender, RoutedEventArgs e)
        {
            var available = AllAuthors
                .Where(a => !_selectedAuthors.Any(s => s.IdCard == a.IdCard))
                .ToList();

            if (!available.Any())
            {
                MessageBox.Show(L("Msg_NoAvailableAuthors"), L("Dlg_InfoTitle"),
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dlg = new SelectAuthorDialog(available)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (dlg.ShowDialog() == true && dlg.SelectedAuthor != null)
                _selectedAuthors.Add(dlg.SelectedAuthor);
        }

        private void BtnRemoveAuthor_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var author = btn?.Tag as AuthorDTO;
            if (author == null) return;

            var result = MessageBox.Show(
                $"{L("Msg_ConfirmRemoveAuthor")} '{author.FullName}'?",
                L("Dlg_ConfirmTitle"),
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
                _selectedAuthors.Remove(author);
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TxtIsbn.Text))
                { MessageBox.Show(L("Msg_IsbnRequired"), L("Dlg_ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error); return; }

                if (string.IsNullOrWhiteSpace(TxtBookName.Text))
                { MessageBox.Show(L("Msg_BookNameRequired"), L("Dlg_ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error); return; }

                if (CmbGenre.SelectedItem == null)
                { MessageBox.Show(L("Msg_GenreRequired"), L("Dlg_ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error); return; }

                if (!int.TryParse(TxtPages.Text, out int pages) || pages <= 0)
                { MessageBox.Show(L("Msg_PagesPositiveInt"), L("Dlg_ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error); return; }

                if (!double.TryParse(TxtPrice.Text, out double price) || price <= 0)
                { MessageBox.Show(L("Msg_PricePositiveNumber"), L("Dlg_ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error); return; }

                if (_selectedAuthors.Count == 0)
                { MessageBox.Show(L("Msg_AtLeastOneAuthor"), L("Dlg_ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error); return; }

                var selectedPublisher = CmbPublisher.SelectedItem as PublisherDTO;
                if (selectedPublisher == null)
                { MessageBox.Show(L("Msg_PublisherRequired"), L("Dlg_ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error); return; }

                Book = new BookDTO
                {
                    Isbn = TxtIsbn.Text.Trim(),
                    Name = TxtBookName.Text.Trim(),
                    Genre = (Genre)CmbGenre.SelectedItem,
                    PublicationYear = TxtPublicationYear.Text.Trim(),
                    Price = price,
                    NumberOfPages = pages,
                    Publisher = selectedPublisher,
                    Authors = _selectedAuthors.ToList()
                };

                var (isValid, errorMessage) = Book.Validate();
                if (!isValid)
                { MessageBox.Show(errorMessage, L("Dlg_ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error); return; }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{L("Msg_SaveBookError")} {ex.Message}", L("Dlg_ErrorTitle"),
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