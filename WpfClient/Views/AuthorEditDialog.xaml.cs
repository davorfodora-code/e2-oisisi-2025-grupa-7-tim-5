using Core.Controllers;
using Core.DTO;
using System;
using System.Linq;
using System.Windows;

namespace WpfClient
{
    public partial class AuthorEditDialog : Window
    {
        public AuthorDTO AuthorDTO { get; private set; }
        private Author _originalAuthor;
        private BookController _bookController;

        public AuthorEditDialog(AuthorDTO authorDto, Author originalAuthor, BookController bookController)
        {
            InitializeComponent();

            AuthorDTO = authorDto;
            _originalAuthor = originalAuthor;
            _bookController = bookController;

            LoadAuthorData();
            LoadAuthorBooks();
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

        private void LoadAuthorBooks()
        {
            try
            {
                var allBooks = _bookController.GetAllBooks();

                var booksByAuthor = allBooks
                    .Where(book => book.Authors != null &&
                                  book.Authors.Any(a => a.IdCard == AuthorDTO.IdCard))
                    .Select(BookDTO.FromModel)
                    .ToList();

                BooksGrid.ItemsSource = booksByAuthor;
            }
            catch (Exception ex)
            {
                BooksGrid.ItemsSource = null;
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