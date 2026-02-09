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
            = new ObservableCollection<AuthorDTO>();

        public AddBookDialog(ObservableCollection<AuthorDTO> authors)
        {
            InitializeComponent();

            _isEditMode = false;
            AllAuthors = authors;

            CmbAuthors.ItemsSource = AllAuthors;
        }

        public AddBookDialog(BookDTO bookToEdit,
                             ObservableCollection<AuthorDTO> authors)
            : this(authors)
        {
            _isEditMode = true;
            _originalBook = bookToEdit;

            Title = "Izmeni knjigu";
            BtnSave.Content = "Sačuvaj izmene";

            TxtIsbn.Text = bookToEdit.Isbn;
            TxtIsbn.IsEnabled = false;

            TxtBookName.Text = bookToEdit.Name;
            CmbGenre.SelectedIndex = (int)bookToEdit.Genre;
            TxtPublicationYear.Text = bookToEdit.PublicationYear;
            TxtPrice.Text = bookToEdit.Price.ToString();
            TxtPages.Text = bookToEdit.NumberOfPages.ToString();
            TxtPublisher.Text = bookToEdit.Publisher?.Name ?? "";

            // Selektuj autora u ComboBox
            var author = bookToEdit.Authors?.FirstOrDefault();
            if (author != null)
            {
                CmbAuthors.SelectedItem =
                    AllAuthors.FirstOrDefault(a => a.IdCard == author.IdCard);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
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

            if (!int.TryParse(TxtPages.Text, out int pages))
            {
                MessageBox.Show("Broj stranica mora biti ceo broj!", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!double.TryParse(TxtPrice.Text, out double price))
            {
                MessageBox.Show("Cena mora biti broj!", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selectedAuthor = CmbAuthors.SelectedItem as AuthorDTO;

            if (selectedAuthor == null)
            {
                MessageBox.Show("Morate izabrati autora!", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Book = new BookDTO
            {
                Isbn = TxtIsbn.Text.Trim(),
                Name = TxtBookName.Text.Trim(),
                Genre = (Genre)CmbGenre.SelectedIndex,
                PublicationYear = TxtPublicationYear.Text.Trim(),
                Price = price,
                NumberOfPages = pages,
                Publisher = new PublisherDTO
                {
                    Name = TxtPublisher.Text.Trim()
                },
                Authors = new System.Collections.Generic.List<AuthorDTO>
                {
                    selectedAuthor
                }
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

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
