using Core.DTO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    public partial class SelectAuthorDialog : Window
    {
        public AuthorDTO? SelectedAuthor { get; private set; }

        public SelectAuthorDialog(IEnumerable<AuthorDTO> availableAuthors)
        {
            InitializeComponent();
            LstAuthors.ItemsSource = availableAuthors;
        }

        private void LstAuthors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnConfirm.IsEnabled = LstAuthors.SelectedItem != null;
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            SelectedAuthor = LstAuthors.SelectedItem as AuthorDTO;
            if (SelectedAuthor == null) return;
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
