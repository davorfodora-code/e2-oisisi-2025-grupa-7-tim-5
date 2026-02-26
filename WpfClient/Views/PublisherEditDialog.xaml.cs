using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Core.DTO;

namespace WpfClient
{
    public partial class PublisherEditDialog : Window
    {
        public PublisherDTO Publisher { get; private set; }
        public ObservableCollection<AuthorDTO> AllAuthors { get; set; }

        private bool _isEditMode;
        private PublisherDTO _originalPublisher;

        public PublisherEditDialog(ObservableCollection<AuthorDTO> allAuthors)
        {
            InitializeComponent();
            AllAuthors = allAuthors;
            Publisher = new PublisherDTO();
            _isEditMode = false;
            Title = "Dodaj izdavača";

            LstAuthors.ItemsSource = AllAuthors;
        }

        public PublisherEditDialog(PublisherDTO publisherToEdit, ObservableCollection<AuthorDTO> allAuthors)
        {
            InitializeComponent();
            AllAuthors = allAuthors;
            _isEditMode = true;
            _originalPublisher = publisherToEdit;
            Title = "Izmeni izdavača";

            LstAuthors.ItemsSource = AllAuthors;
            TxtName.Text = publisherToEdit.Name;
        }

        private void LstAuthors_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isEditMode && _originalPublisher.Authors != null)
            {
                var authorIds = _originalPublisher.Authors.Select(a => a.IdCard).ToHashSet();

                for (int i = 0; i < LstAuthors.Items.Count; i++)
                {
                    var author = LstAuthors.Items[i] as AuthorDTO;
                    if (author != null && authorIds.Contains(author.IdCard))
                    {
                        LstAuthors.SelectedItems.Add(author);
                    }
                }
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Naziv izdavača je obavezan!", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selectedAuthors = LstAuthors.SelectedItems.Cast<AuthorDTO>().ToList();
            if (selectedAuthors.Count == 0)
            {
                MessageBox.Show("Morate izabrati bar jednog autora!", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Publisher = new PublisherDTO
            {
                Id = _isEditMode ? _originalPublisher.Id : 0,
                Name = TxtName.Text.Trim(),
                Manager = _isEditMode ? _originalPublisher.Manager : null,
                Authors = selectedAuthors
            };

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