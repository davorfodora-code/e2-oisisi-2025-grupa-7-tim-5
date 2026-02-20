using Core.DTO;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace WpfClient
{
    public partial class AddBookDialog : Window
    {
        public BookDTO? Book { get; private set; }
        private bool _isEditMode;
        private BookDTO? _originalBook;
        public ObservableCollection<AuthorDTO> AllAuthors { get; set; }

        public AddBookDialog(ObservableCollection<AuthorDTO> authors)
        {
            InitializeComponent();

            _isEditMode = false;
            AllAuthors = authors ?? new ObservableCollection<AuthorDTO>();

            LstAuthors.ItemsSource = AllAuthors;
            CmbGenre.ItemsSource = Enum.GetValues(typeof(Genre)).Cast<Genre>();
        }

        public AddBookDialog(BookDTO bookToEdit, ObservableCollection<AuthorDTO> authors)
        {
            InitializeComponent();

            _isEditMode = true;
            _originalBook = bookToEdit;
            AllAuthors = authors ?? new ObservableCollection<AuthorDTO>();

            Title = "Izmeni knjigu";
            BtnSave.Content = "Sačuvaj izmene";

            LstAuthors.ItemsSource = AllAuthors;
            CmbGenre.ItemsSource = Enum.GetValues(typeof(Genre)).Cast<Genre>();

            TxtIsbn.Text = bookToEdit.Isbn;
            TxtIsbn.IsEnabled = false;
            TxtBookName.Text = bookToEdit.Name;
            CmbGenre.SelectedItem = bookToEdit.Genre;
            TxtPublicationYear.Text = bookToEdit.PublicationYear;
            TxtPrice.Text = bookToEdit.Price.ToString();
            TxtPages.Text = bookToEdit.NumberOfPages.ToString();
            TxtPublisher.Text = bookToEdit.Publisher?.Name ?? "";

            // Preselect autore
            if (bookToEdit.Authors != null && bookToEdit.Authors.Any())
            {
                foreach (var author in bookToEdit.Authors)
                {
                    var authorToSelect = AllAuthors.FirstOrDefault(a => a.IdCard == author.IdCard);
                    if (authorToSelect != null)
                    {
                        var index = AllAuthors.IndexOf(authorToSelect);
                        if (index >= 0)
                        {
                            LstAuthors.SelectedItems.Add(authorToSelect);
                        }
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TxtIsbn.Text))
                {
                    MessageBox.Show("ISBN je obavezan!", "Greška",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(TxtBookName.Text))
                {
                    MessageBox.Show("Naziv knjige je obavezan!", "Greška",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (CmbGenre.SelectedItem == null)
                {
                    MessageBox.Show("Izaberite žanr knjige!", "Greška",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(TxtPages.Text, out int pages) || pages <= 0)
                {
                    MessageBox.Show("Broj stranica mora biti pozitivan ceo broj!", "Greška",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!double.TryParse(TxtPrice.Text, out double price) || price <= 0)
                {
                    MessageBox.Show("Cena mora biti pozitivan broj!", "Greška",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var selectedAuthors = LstAuthors.SelectedItems.Cast<AuthorDTO>().ToList();
                if (selectedAuthors.Count == 0)
                {
                    MessageBox.Show("Morate izabrati bar jednog autora!", "Greška",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Book = new BookDTO
                {
                    Isbn = TxtIsbn.Text.Trim(),
                    Name = TxtBookName.Text.Trim(),
                    Genre = (Genre)CmbGenre.SelectedItem,
                    PublicationYear = TxtPublicationYear.Text.Trim(),
                    Price = price,
                    NumberOfPages = pages,
                    Publisher = new PublisherDTO { Name = TxtPublisher.Text.Trim() },
                    Authors = selectedAuthors
                };

                var (isValid, errorMessage) = Book.Validate();
                if (!isValid)
                {
                    MessageBox.Show(errorMessage, "Greška",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri čuvanju knjige: {ex.Message}", "Greška",
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