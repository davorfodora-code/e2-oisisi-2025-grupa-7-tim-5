using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfClient
{
    public partial class PurchaseDialog : Window
    {
        public int Review { get; private set; }
        public DateTime DateOfPurchase { get; private set; }

        public PurchaseDialog(string isbn, string naziv)
        {
            InitializeComponent();

            TxtIsbn.Text = isbn;
            TxtNaziv.Text = naziv;

            // zabrani budući datum (po želji)
            DpDatum.DisplayDateEnd = DateTime.Today;

            ValidateForm();
        }

        private void TxtOcena_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // dozvoli samo cifre
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private void AnyFieldChanged(object sender, TextChangedEventArgs e)
        {
            ValidateForm();
        }

        private void DpDatum_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidateForm();
        }

        private void ValidateForm()
        {
            bool isbnOk = !string.IsNullOrWhiteSpace(TxtIsbn.Text);
            bool nazivOk = !string.IsNullOrWhiteSpace(TxtNaziv.Text);

            bool ocenaParsed = int.TryParse(TxtOcena.Text, NumberStyles.Integer,
                CultureInfo.InvariantCulture, out int oc);

            bool ocenaOk = ocenaParsed && oc >= 1 && oc <= 5;

           
            TxtOcenaError.Visibility = ocenaOk || string.IsNullOrWhiteSpace(TxtOcena.Text)
                ? Visibility.Collapsed
                : Visibility.Visible;

            bool datumOk = DpDatum.SelectedDate.HasValue;

            BtnConfirm.IsEnabled = isbnOk && nazivOk && ocenaOk && datumOk;
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!BtnConfirm.IsEnabled) return;

            Review = int.Parse(TxtOcena.Text, CultureInfo.InvariantCulture);
            DateOfPurchase = DpDatum.SelectedDate!.Value.Date;

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
