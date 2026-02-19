using System.Windows;
using System.Windows.Controls;
using Core.DTO;

namespace WpfClient
{
    public partial class AddVisitorDialog : Window
    {
        public VisitorDTO Visitor { get; private set; } = new VisitorDTO();
        private bool _isEditMode;
        private VisitorDTO _originalVisitor= new VisitorDTO();

        public AddVisitorDialog()
        {
            InitializeComponent();
            _isEditMode = false;
        }

        public AddVisitorDialog(VisitorDTO visitorToEdit) : this()
        {
            _isEditMode = true;
            _originalVisitor = visitorToEdit;
            Title = "Izmeni posetioca";
            BtnOk.Content = "Sačuvaj izmene";


            TxtMembershipNumber.Text = visitorToEdit.MembershipNumber;
            TxtMembershipNumber.IsEnabled = false; 

            TxtName.Text = visitorToEdit.Name;
            TxtLastName.Text = visitorToEdit.LastName;


            foreach (ComboBoxItem item in CmbStatus.Items)
            {
                if (item.Tag.ToString() == visitorToEdit.Status)
                {
                    item.IsSelected = true;
                    break;
                }
            }
            if (visitorToEdit.Address != null)
            {
                TxtStreet.Text = visitorToEdit.Address.Street;
                TxtNumber.Text = visitorToEdit.Address.Number;
                TxtCity.Text = visitorToEdit.Address.City;
                TxtState.Text = visitorToEdit.Address.State;
                TxtCountry.Text = visitorToEdit.Address.Country;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(TxtMembershipNumber.Text))
            {
                MessageBox.Show("Broj članske karte je obavezan!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
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

            Visitor = new VisitorDTO
            {
                MembershipNumber = TxtMembershipNumber.Text.Trim(),
                Name = TxtName.Text.Trim(),
                LastName = TxtLastName.Text.Trim(),
                Status = ((ComboBoxItem)CmbStatus.SelectedItem)?.Tag.ToString() ?? "R",
                Address = new AddressDTO
                {
                    Street = TxtStreet.Text.Trim(),
                    Number = TxtNumber.Text.Trim(),
                    City = TxtCity.Text.Trim(),
                    State = TxtState.Text.Trim(),
                    Country = TxtCountry.Text.Trim()
                }
            };


            var (isValid, errorMessage) = Visitor.Validate();
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