using Core.DTO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    public partial class WishlistAddDialog : Window
    {
        public BookDTO? SelectedBook { get; private set; }

        public WishlistAddDialog(List<BookDTO> candidates)
        {
            InitializeComponent();

            BooksList.ItemsSource = new ObservableCollection<BookDTO>(candidates);
        }

        private void BooksList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnAdd.IsEnabled = BooksList.SelectedItem != null;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            SelectedBook = BooksList.SelectedItem as BookDTO;
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
