using System.Windows;
using System.Windows.Controls;
using Core.DTO;

namespace WpfClient
{
    public partial class AddAuthorDialog : Window
    {
        public AuthorDTO Author { get; private set; }

        private bool _isEditMode;
        private AuthorDTO _originalAuthor;

        public AddAuthorDialog()
        {
            InitializeComponent();
            _isEditMode = false;
        }

        public AddAuthorDialog(AuthorDTO authorToEdit) : this()
        {
            _isEditMode = true;
            _originalAuthor = authorToEdit;
            Title = "Izmeni autora";
            BtnOk.Content = "Sačuvaj izmene";

            TxtIdCard.Text = authorToEdit.IdCard;
            TxtIdCard.IsEnabled = false; 

            TxtName.Text = authorToEdit.Name;
            TxtLastName.Text = authorToEdit.Lastname;

            DpDateOfBirth.SelectedDate = authorToEdit.DateOfBirth.ToDateTime(new TimeOnly(0, 0));


            TxtPhoneNumber.Text = authorToEdit.PhoneNumber;
            TxtEmail.Text = authorToEdit.Email;
            TxtExperienceLength.Text = authorToEdit.ExperienceLength.ToString();

            if (authorToEdit.Address != null)
            {
                TxtStreet.Text = authorToEdit.Address.Street;
                TxtNumber.Text = authorToEdit.Address.Number;
                TxtCity.Text = authorToEdit.Address.City;
                TxtState.Text = authorToEdit.Address.State;
                TxtCountry.Text = authorToEdit.Address.Country;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            // Validacija osnovnih polja
            if (string.IsNullOrWhiteSpace(TxtIdCard.Text))
            {
                MessageBox.Show("ID kartica je obavezna!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Ime je obavezno!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtLastName.Text))
            {
                MessageBox.Show("Prezime je obavezno!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!DpDateOfBirth.SelectedDate.HasValue)
            {
                MessageBox.Show("Datum rođenja je obavezan!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!double.TryParse(TxtExperienceLength.Text, out double experience))
            {
                MessageBox.Show("Dužina iskustva mora biti broj!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Kreiranje AuthorDTO
            Author = new AuthorDTO
            {
                IdCard = TxtIdCard.Text.Trim(),
                Name = TxtName.Text.Trim(),
                Lastname = TxtLastName.Text.Trim(),
                DateOfBirth = DateOnly.FromDateTime(DpDateOfBirth.SelectedDate.Value),
                PhoneNumber = TxtPhoneNumber.Text.Trim(),
                Email = TxtEmail.Text.Trim(),
                ExperienceLength = experience,
                Address = new AddressDTO
                {
                    Street = TxtStreet.Text.Trim(),
                    Number = TxtNumber.Text.Trim(),
                    City = TxtCity.Text.Trim(),
                    State = TxtState.Text.Trim(),
                    Country = TxtCountry.Text.Trim()
                }
            };

            // Validacija DTO
            var (isValid, errorMessage) = Author.Validate();
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
