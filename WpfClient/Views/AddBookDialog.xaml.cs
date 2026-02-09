using System;
using System.Windows;
using Core.DTO;

namespace WpfClient
{
    public partial class AddBookDialog : Window
    {
        public BookDTO Book { get; private set; }

        private bool _isEditMode;
        private BookDTO _originalBook;

        public AddBookDialog()
        {
            InitializeComponent();
            _isEditMode = false;
        }

        public AddBookDialog(BookDTO bookToEdit) : this()
        {
            _isEditMode = true;
            _originalBook = bookToEdit;
            Title = "Izmeni knjigu";
            BtnSave.Content = "Sačuvaj izmene";

            TxtIsbn.Text = bookToEdit.Isbn;
            TxtIsbn.IsEnabled = false; // Ne može se menjati ISBN

            TxtBookName.Text = bookToEdit.Name;
            CmbGenre.SelectedItem = CmbGenre.Items[0]; // ili postavi odgovarajući item po bookToEdit.Genre
            TxtPublicationYear.Text = bookToEdit.PublicationYear;
            TxtPrice.Text = bookToEdit.Price.ToString();
            TxtPages.Text = bookToEdit.NumberOfPages.ToString();
            TxtPublisher.Text = bookToEdit.Publisher?.Name ?? "";
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validacija osnovnih polja
            if (string.IsNullOrWhiteSpace(TxtIsbn.Text))
            {
                MessageBox.Show("ISBN je obavezan!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtBookName.Text))
            {
                MessageBox.Show("Naziv knjige je obavezan!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(TxtPages.Text, out int pages))
            {
                MessageBox.Show("Broj stranica mora biti ceo broj!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!double.TryParse(TxtPrice.Text, out double price))
            {
                MessageBox.Show("Cena mora biti broj!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Kreiranje BookDTO
            Book = new BookDTO
            {
                Isbn = TxtIsbn.Text.Trim(),
                Name = TxtBookName.Text.Trim(),
                Genre = (Genre)CmbGenre.SelectedIndex, // Pretpostavka: redosled u ComboBox odgovara enumu
                PublicationYear = TxtPublicationYear.Text.Trim(),
                Price = price,
                NumberOfPages = pages,
                Publisher = new PublisherDTO
                {
                    Name = TxtPublisher.Text.Trim()
                }
            };

            // Validacija DTO
            var (isValid, errorMessage) = Book.Validate();
            if (!isValid)
            {
                MessageBox.Show(errorMessage, "Greška u validaciji", MessageBoxButton.OK, MessageBoxImage.Error);
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
