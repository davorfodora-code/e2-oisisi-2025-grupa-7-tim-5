using Core.DTO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    public partial class SelectBookDialog : Window
    {
        public BookDTO? SelectedBook { get; private set; }

        public SelectBookDialog(IEnumerable<BookDTO> availableBooks)
        {
            InitializeComponent();
            LstBooks.ItemsSource = availableBooks;
        }

        private void LstBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnConfirm.IsEnabled = LstBooks.SelectedItem != null;
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            SelectedBook = LstBooks.SelectedItem as BookDTO;
            if (SelectedBook == null) return;
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
