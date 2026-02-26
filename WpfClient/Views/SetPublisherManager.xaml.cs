using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Core.DTO;

namespace WpfClient
{
    public partial class SetPublisherManager : Window
    {
        private ObservableCollection<PublisherDTO> _publishers;
        private ObservableCollection<AuthorDTO> _allAuthors;
        private PublisherDTO _selectedPublisher;

        public SetPublisherManager(ObservableCollection<PublisherDTO> publishers, ObservableCollection<AuthorDTO> allAuthors)
        {
            InitializeComponent();
            _publishers = publishers;
            _allAuthors = allAuthors;

            CmbPublisher.ItemsSource = publishers;
        }

        private void CmbPublisher_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _selectedPublisher = CmbPublisher.SelectedItem as PublisherDTO;

            if (_selectedPublisher != null)
            {
                TxtCurrentManager.Text = _selectedPublisher.DisplayManager;
                AuthorsGrid.ItemsSource = _selectedPublisher.Authors;
                BtnSetManager.IsEnabled = false;
            }
        }

        private void AuthorsGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            BtnSetManager.IsEnabled = AuthorsGrid.SelectedItem != null;
        }

        private void BtnSetManager_Click(object sender, RoutedEventArgs e)
        {
            var selectedAuthor = AuthorsGrid.SelectedItem as AuthorDTO;

            if (selectedAuthor == null)
            {
                MessageBox.Show("Izaberite autora.", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (selectedAuthor.ExperienceLength < 5)
            {
                MessageBox.Show($"Autor {selectedAuthor.Name} {selectedAuthor.Lastname} ima {selectedAuthor.ExperienceLength} godina iskustva.\n" +
                               "Za šefa izdavača može biti postavljen samo autor sa najmanje 5 godina iskustva.",
                               "Neispunjen uslov",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _selectedPublisher.Manager = new AuthorDTO
            {
                IdCard = selectedAuthor.IdCard,
                Name = selectedAuthor.Name,
                Lastname = selectedAuthor.Lastname,
                ExperienceLength = selectedAuthor.ExperienceLength,
                Email = selectedAuthor.Email
            };

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public PublisherDTO GetUpdatedPublisher()
        {
            return _selectedPublisher;
        }
    }
}