using Core.DTO;
using System.Collections.Generic;
using System.Windows;

namespace WpfClient
{
    public partial class PublisherBooksDialog : Window
    {
        private string L(string key) => (string)Application.Current.Resources[key];

        public PublisherBooksDialog(PublisherDTO publisher, List<BookDTO> books)
        {
            InitializeComponent();

            TxtHeader.Text = $"{L("PublisherBooks_HeaderPrefix")} {publisher?.Name}";
            BooksGrid.ItemsSource = books ?? new List<BookDTO>();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}