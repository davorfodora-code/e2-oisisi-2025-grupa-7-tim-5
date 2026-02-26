using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

            CmbManager.ItemsSource = AllAuthors;
            LstAuthors.ItemsSource = AllAuthors;
        }

        public PublisherEditDialog(PublisherDTO publisherToEdit, ObservableCollection<AuthorDTO> allAuthors)
        {
            InitializeComponent();
            AllAuthors = allAuthors;
            _isEditMode = true;
            _originalPublisher = publisherToEdit;
            Title = "Izmeni izdavača";
            BtnOk.Content = "Sačuvaj izmene";

            CmbManager.ItemsSource = AllAuthors;
            LstAuthors.ItemsSource = AllAuthors;

            TxtName.Text = publisherToEdit.Name;

            if (publisherToEdit.Manager != null)
            {
                var manager = AllAuthors.FirstOrDefault(a =>
                    a.IdCard == publisherToEdit.Manager.IdCard);
                if (manager != null)
                    CmbManager.SelectedItem = manager;
            }

            if (publisherToEdit.Authors != null && publisherToEdit.Authors.Any())
            {
                var authorIds = publisherToEdit.Authors.Select(a => a.IdCard).ToHashSet();
                foreach (var author in AllAuthors)
                {
                    if (authorIds.Contains(author.IdCard))
                        LstAuthors.SelectedItems.Add(author);
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

            if (CmbManager.SelectedItem == null)
            {
                MessageBox.Show("Morate izabrati menadžera!", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (LstAuthors.SelectedItems.Count == 0)
            {
                MessageBox.Show("Morate izabrati bar jednog autora!", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Publisher = new PublisherDTO
            {
                Id = _isEditMode ? _originalPublisher.Id : 0,
                Name = TxtName.Text.Trim(),
                Manager = (AuthorDTO)CmbManager.SelectedItem,
                Authors = LstAuthors.SelectedItems.Cast<AuthorDTO>().ToList()
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