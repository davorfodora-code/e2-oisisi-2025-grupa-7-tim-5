using Core.Controllers;
using Core.DAO;
using Core.DTO;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using WpfClient.Localization;

namespace WpfClient
{
    public partial class MainWindow : Window
    {
        private List<VisitorDTO> _allVisitors = new();
        private List<AuthorDTO> _allAuthors = new();
        private List<BookDTO> _allBooks = new();
        private VisitorController _visitorController;
        private ObservableCollection<VisitorDTO> _visitors;
        private DispatcherTimer _timer = new DispatcherTimer();
        private AuthorController _authorController;
        private ObservableCollection<AuthorDTO> _authors;
        private BookController _bookController;
        private ObservableCollection<BookDTO> _books;

        private PublisherController _publisherController;
        private ObservableCollection<PublisherDTO> _publishers;
        private List<PublisherDTO> _allPublishers = new();
        private VisitorDAO _visitorDAO;
        public MainWindow()
        {
            InitializeComponent();
            InitializeStatusBar();

            var purchaseDAO = new PurchaseDAO();
            var visitorDAO = new VisitorDAO(purchaseDAO);
            var bookDAO = new BookDAO();
            var publisherDAO = new PublisherDAO();
            var authorDAO = new AuthorDAO(publisherDAO);
            _visitorDAO = new VisitorDAO(purchaseDAO);

            publisherDAO.LinkAuthorsWithFullData(authorDAO);

            _authorController = new AuthorController(authorDAO);
            _bookController = new BookController(bookDAO, publisherDAO);
            _visitorController = new VisitorController(visitorDAO, purchaseDAO);

            _publisherController = new PublisherController(publisherDAO);

            _visitors = new ObservableCollection<VisitorDTO>();
            _authors = new ObservableCollection<AuthorDTO>();
            _books = new ObservableCollection<BookDTO>();
            _publishers = new ObservableCollection<PublisherDTO>();

            PublishersGrid.ItemsSource = _publishers;
            VisitorsGrid.ItemsSource = _visitors;
            AuthorsGrid.ItemsSource = _authors;
            BooksGrid.ItemsSource = _books;

            LoadVisitors();
            LoadAuthors();
            LoadBooks();
            LoadPublishers();


        }

        private void InitializeStatusBar()
        {
            StatusBarAppName.Text = "Sajam knjiga";
            UpdateTabName();
            UpdateDateTime();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdateDateTime();
        }

        private void UpdateDateTime()
        {
            var now = DateTime.Now;
            StatusBarDate.Text = $"Datum: {now:dd.MM.yyyy}";
            StatusBarTime.Text = $"Vreme: {now:HH:mm:ss}";
        }

        private void UpdateTabName()
        {
            if (MainTabs.SelectedItem is TabItem selectedTab)
            {
                StatusBarTabName.Text = $"Tab: {selectedTab.Header}";
            }
            else
            {
                StatusBarTabName.Text = "Tab: N/A";
            }
        }

        private void LoadVisitors()
        {
            var visitorModels = _visitorController.GetAllVisitors();
            _visitors = new ObservableCollection<VisitorDTO>();

            foreach (var visitor in visitorModels)
            {
                var dto = VisitorDTO.FromModel(visitor);
                _visitors.Add(dto);
            }

            VisitorsGrid.ItemsSource = _visitors;
            _allVisitors = _visitors.ToList();
        }
        private void BtnCompareBooks_Click(object sender, RoutedEventArgs e)
        {
            var allVisitors = _visitorDAO
                .GetAllVisitors()
                .Select(VisitorDTO.FromModel)
                .ToList();

            if (_allBooks == null || _allBooks.Count == 0)
            {
                MessageBox.Show("Nema učitanih knjiga u sistemu.", "Upozorenje",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var win = new BookCompareWindow(_allBooks, allVisitors);
            win.Owner = this;
            win.ShowDialog();
        }
        private void LoadAuthors()
        {
            var authorModels = _authorController.GetAllAuthors();
            _authors = new ObservableCollection<AuthorDTO>();

            foreach (var author in authorModels)
            {
                var dto = AuthorDTO.FromModel(author);
                _authors.Add(dto);
            }

            AuthorsGrid.ItemsSource = _authors;
            _allAuthors = _authors.ToList();
        }

        private void LoadBooks()
        {
            var bookModels = _bookController.GetAllBooks();
            _books = new ObservableCollection<BookDTO>();

            foreach (var book in bookModels)
            {
                var dto = BookDTO.FromModel(book);
                _books.Add(dto);
            }

            BooksGrid.ItemsSource = _books;
            _allBooks = _books.ToList();
        }
        private void LoadPublishers()
        {
            if (_publisherController == null)
                return;

            var publisherModels = _publisherController.GetAllPublishers();
            _publishers.Clear();

            foreach (var publisher in publisherModels)
            {
                var dto = PublisherDTO.FromModel(publisher);
                _publishers.Add(dto);
            }

            PublishersGrid.Items.Refresh();
        }

       



        private void Window_Loaded(object? sender, RoutedEventArgs e)
        {
            LoadVisitors();
            UpdateTabName();
        }

        private void BtnAdd_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                var selectedTab = MainTabs?.SelectedItem as TabItem;
                var tabName = selectedTab?.Header?.ToString();

                if (tabName == "Posetioci")
                {
                    var dialog = new AddVisitorDialog();
                    if (dialog.ShowDialog() == true)
                    {
                        var result = _visitorController.AddVisitor(dialog.Visitor);
                        if (result.Success)
                        {
                            ShowToast("✓ Posetilac uspešno dodat!"); 
                            LoadVisitors();
                        }
                        else
                        {
                            MessageBox.Show(result.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else if (tabName == "Izdavači")
                {
                    var dialog = new PublisherEditDialog(_authors);
                    dialog.Owner = this;

                    if (dialog.ShowDialog() == true)
                    {
                        var result = _publisherController.AddPublisher(dialog.Publisher.ToModel());
                        if (result != null)
                        {
                            ShowToast("✓ Izdavač uspešno dodat!");
                            LoadPublishers();
                        }
                        else
                        {
                            MessageBox.Show("Došlo je do greške prilikom dodavanja izdavača.",
                                "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else if (tabName == "Autori")
                {
                    var dialog = new AddAuthorDialog();
                    dialog.Owner = this;

                    if (dialog.ShowDialog() == true)
                    {
                        var result = _authorController.AddAuthor(dialog.Author);
                        if (result.Success)
                        {
                            ShowToast("✓ Autor uspešno dodat!"); 
                            LoadAuthors();
                        }
                        else
                        {
                            MessageBox.Show(result.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else if (tabName == "Knjige")
                {
                    if (_publishers == null || _publishers.Count == 0)
                    {
                        MessageBox.Show("Nema učitanih izdavača! Prvo dodajte izdavače.", "Upozorenje",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var dialog = new AddBookDialog(_authors, _publishers);
                    if (dialog.ShowDialog() == true)
                    {
                        var result = _bookController.CreateBook(dialog.Book);
                        if (result.Success)
                        {
                            ShowToast("✓ Knjiga uspešno dodata!"); 
                            LoadBooks();
                        }
                        else
                        {
                            MessageBox.Show(result.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Izaberite jedan od tabova: Posetioci, Autori ili Knjige",
                        "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška: {ex.Message}\n\nDetalji: {ex.StackTrace}",
                    "Kritična greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       
        private async void ShowToast(string message, int ms = 2500)
        {
            ToastText.Text = message;

            Toast.Visibility = Visibility.Visible;

    
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(150));
            Toast.BeginAnimation(OpacityProperty, fadeIn);

            await Task.Delay(ms);

   
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            fadeOut.Completed += (_, __) => Toast.Visibility = Visibility.Collapsed;
            Toast.BeginAnimation(OpacityProperty, fadeOut);
        }
        private void ShowVisitorsForAuthor_Click(object sender, RoutedEventArgs e)
        {
            var selectedAuthor = AuthorsGrid.SelectedItem as AuthorDTO;
            if (selectedAuthor != null)
            {
                var allVisitors = _visitorDAO.GetAllVisitors().Select(VisitorDTO.FromModel).ToList();

                var win = new VisitorsByAuthorWindow(selectedAuthor, allVisitors);
                win.Owner = this; 
                win.ShowDialog();
            }
            else
            {
                MessageBox.Show("Molimo izaberite autora iz tabele.");
            }
        }
        private void BtnEdit_Click(object? sender, RoutedEventArgs e)
        {
            var selectedTab = MainTabs?.SelectedItem as TabItem;
            var tabName = selectedTab?.Header?.ToString();

            if (tabName == "Posetioci")
            {
                var selectedVisitorDto = VisitorsGrid.SelectedItem as VisitorDTO;
                if (selectedVisitorDto == null)
                {
                    MessageBox.Show("Izaberite posetioca za izmenu", "Upozorenje",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var originalVisitor = _visitorController.GetAllVisitors()
                    .FirstOrDefault(v => v.MembershipNumber == selectedVisitorDto.MembershipNumber);

                if (originalVisitor == null)
                {
                    MessageBox.Show("Posetilac nije pronađen!", "Greška",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var dialog = new VisitorEditDialog(selectedVisitorDto, originalVisitor, _bookController);

                if (dialog.ShowDialog() == true)
                {
                    var result = _visitorController.UpdateVisitor(dialog.VisitorDTO);
                    if (result.Success)
                    {
                        ShowToast("Posetilac je uspesno izmenjen!");
                        LoadVisitors();
                    }
                    else
                    {
                        MessageBox.Show(result.Message, "Došlo je do greške prilikom izmene Posetioca.", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else if (tabName == "Izdavači")
            {
                var selectedPublisher = PublishersGrid.SelectedItem as PublisherDTO;
                if (selectedPublisher == null)
                {
                    MessageBox.Show("Izaberite izdavača za izmenu", "Upozorenje",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var dialog = new PublisherEditDialog(selectedPublisher, _authors);
                dialog.Owner = this;

                if (dialog.ShowDialog() == true)
                {
                    var publisherModel = dialog.Publisher.ToModel();
                    var result = _publisherController.UpdatePublisher(publisherModel);

                    if (result != null)
                    {
                        ShowToast("Izdavač je uspešno izmenjen!");
                        LoadPublishers();
                        LoadBooks();
                    }
                    else
                    {
                        MessageBox.Show("Došlo je do greške prilikom izmene izdavača.",
                            "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else if (tabName == "Autori")
            {
                var selectedAuthorDto = AuthorsGrid.SelectedItem as AuthorDTO;
                if (selectedAuthorDto == null)
                {
                    MessageBox.Show("Izaberite autora za izmenu", "Upozorenje",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var originalAuthor = _authorController.GetAllAuthors()
                    .FirstOrDefault(a => a.IdCard == selectedAuthorDto.IdCard);

                if (originalAuthor == null)
                {
                    MessageBox.Show("Autor nije pronađen!", "Greška",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var dialog = new AuthorEditDialog(selectedAuthorDto, originalAuthor, _bookController);
                dialog.Owner = this;

                if (dialog.ShowDialog() == true)
                {
                    var result = _authorController.UpdateAuthor(dialog.AuthorDTO);

                    LoadBooks();

                    if (result.Success)
                    {
                        MessageBox.Show(result.Message, "Uspešno",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadAuthors(); 
                        LoadPublishers(); 
                    }
                    else
                    {
                        MessageBox.Show(result.Message, "Greška",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else if (tabName == "Knjige")
            {
                var selectedBook = BooksGrid.SelectedItem as BookDTO;
                if (selectedBook == null)
                {
                    MessageBox.Show("Izaberite knjigu za izmenu", "Upozorenje",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_publishers == null || _publishers.Count == 0)
                {
                    MessageBox.Show("Nema učitanih izdavača!", "Upozorenje",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var dialog = new AddBookDialog(selectedBook, _authors, _publishers);

                if (dialog.ShowDialog() == true)
                {
                    var result = _bookController.EditBook(dialog.Book);
                    if (result.Success)
                    {
                        ShowToast("Knjiga je uspešno izmenjena.");
                           
                        LoadBooks();
                    }
                    else
                    {
                        MessageBox.Show(result.Message, "Greška",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Izaberite jedan od tabova: Posetioci, Autori ili Knjige",
                    "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object? sender, RoutedEventArgs e)
        {
            var selectedTab = MainTabs?.SelectedItem as TabItem;
            var tabName = selectedTab?.Header?.ToString();

            if (tabName == "Posetioci")
            {
                var selectedVisitor = VisitorsGrid.SelectedItem as VisitorDTO;
                if (selectedVisitor == null)
                {
                    MessageBox.Show("Izaberite posetioca za brisanje", "Upozorenje",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    $"Da li ste sigurni da želite da obrišete posetioca:\n" +
                    $"{selectedVisitor.Name} {selectedVisitor.LastName}\n" +
                    $"(Broj članske karte: {selectedVisitor.MembershipNumber})?",
                    "Potvrda brisanja",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var deleteResult = _visitorController.DeleteVisitor(selectedVisitor.MembershipNumber);
                    if (deleteResult.Success)
                    {
                        MessageBox.Show(deleteResult.Message, "Uspešno",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadVisitors();
                    }
                    else
                    {
                        MessageBox.Show(deleteResult.Message, "Greška",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else if (tabName == "Izdavači")
            {
                var selectedPublisher = PublishersGrid.SelectedItem as PublisherDTO;
                if (selectedPublisher == null)
                {
                    MessageBox.Show("Izaberite izdavača za brisanje", "Upozorenje",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    $"Da li ste sigurni da želite da obrišete izdavača:\n{selectedPublisher.Name}?",
                    "Potvrda brisanja",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    var deleteResult = _publisherController.RemovePublisher(selectedPublisher.ToModel());
                    if (deleteResult != null)
                    {
                        MessageBox.Show("Izdavač je uspešno obrisan!", "Uspešno",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadPublishers();
                        LoadBooks();
                    }
                    else
                    {
                        MessageBox.Show("Došlo je do greške prilikom brisanja izdavača.",
                            "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else if (tabName == "Autori")
            {
                var selectedAuthor = AuthorsGrid.SelectedItem as AuthorDTO;
                if (selectedAuthor == null)
                {
                    MessageBox.Show("Izaberite autora za brisanje", "Upozorenje",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    $"Da li ste sigurni da želite da obrišete autora:\n" +
                    $"{selectedAuthor.Name} {selectedAuthor.Lastname}\n" +
                    $"(ID kartica: {selectedAuthor.IdCard})?",
                    "Potvrda brisanja",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var deleteResult = _authorController.DeleteAuthor(selectedAuthor.IdCard);
                    if (deleteResult.Success)
                    {
                        MessageBox.Show(deleteResult.Message, "Uspešno",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadAuthors();
                        LoadPublishers();
                    }
                    else
                    {
                        MessageBox.Show(deleteResult.Message, "Greška",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else if (tabName == "Knjige")
            {
                var selectedBook = BooksGrid.SelectedItem as BookDTO;
                if (selectedBook == null)
                {
                    MessageBox.Show("Izaberite knjigu za brisanje", "Upozorenje",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    $"Da li ste sigurni da želite da obrišete knjigu:\n" +
                    $"{selectedBook.Name}\n" +
                    $"(ISBN: {selectedBook.Isbn})?",
                    "Potvrda brisanja",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var deleteResult = _bookController.RemoveBook(selectedBook.Isbn);
                    if (deleteResult.Success)
                    {
                        MessageBox.Show(deleteResult.Message, "Uspešno",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadBooks();
                    }
                    else
                    {
                        MessageBox.Show(deleteResult.Message, "Greška",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Izaberite jedan od tabova: Posetioci, Autori ili Knjige",
                    "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string q = TxtSearch.Text ?? "";

            var tab = MainTabs.SelectedItem as TabItem;
            string header = tab?.Header?.ToString() ?? "";

            
            if (string.IsNullOrWhiteSpace(q))
            {
                VisitorsGrid.ItemsSource = _visitors;
                AuthorsGrid.ItemsSource = _authors;
                BooksGrid.ItemsSource = _books;
                return;
            }

            if (header.Equals("Posetioci", StringComparison.OrdinalIgnoreCase))
            {
                var filtered = VisitorSearch.Filter(_allVisitors, q).ToList();
                VisitorsGrid.ItemsSource = new System.Collections.ObjectModel.ObservableCollection<VisitorDTO>(filtered);
                return;
            }

            if (header.Equals("Autori", StringComparison.OrdinalIgnoreCase))
            {
                var filtered = AuthorSearch.Filter(_allAuthors, q).ToList();
                AuthorsGrid.ItemsSource = new System.Collections.ObjectModel.ObservableCollection<AuthorDTO>(filtered);
                return;
            }

            if (header.Equals("Knjige", StringComparison.OrdinalIgnoreCase))
            {
                var filtered = BookSearch.Filter(_allBooks, q).ToList();
                BooksGrid.ItemsSource = new System.Collections.ObjectModel.ObservableCollection<BookDTO>(filtered);
                return;
            }
        }

        private void BtnSetPublisherManager_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SetPublisherManager(_publishers, _authors);
            dialog.Owner = this;

            if (dialog.ShowDialog() == true)
            {
                var updatedPublisher = dialog.GetUpdatedPublisher();

                var publisherModel = _publisherController.GetAllPublishers()
                    .FirstOrDefault(p => p.Id == updatedPublisher.Id);

                if (publisherModel != null)
                {
                    publisherModel.Manager = updatedPublisher.Manager?.ToModel();

                    var result = _publisherController.UpdatePublisher(publisherModel);

                    if (result != null)
                    {
                        MessageBox.Show("Šef izdavača je uspešno postavljen!", "Uspešno",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadPublishers();
                    }
                    else
                    {
                        MessageBox.Show("Došlo je do greške prilikom postavljanja šefa.",
                            "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }



        private void TxtSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                BtnSearch_Click(sender, e);
        }

       

        private void MainTabs_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            TxtSearch.Clear();
            UpdateTabName();
        }

        private void MenuItem_Click(object? sender, RoutedEventArgs e)
        {

        }

        protected override void OnKeyDown(KeyEventArgs e)
        {

        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _timer?.Stop();
        }

        private void TxtSearch_TextChanged(object? sender, TextChangedEventArgs e)
        {

        }

        private void AuthorsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void FileNew_Click(object sender, RoutedEventArgs e)
        {
            BtnAdd_Click(sender, e);
        }

        private void FileSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Funkcija 'Save' je pozvana.\nTrenutno stanje aplikacije je sačuvano.",
                "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OpenPosetioci_Click(object sender, RoutedEventArgs e)
        {
            MainTabs.SelectedIndex = 1;
        }

        private void OpenKnjige_Click(object sender, RoutedEventArgs e)
        {
            MainTabs.SelectedIndex = 0;
        }

        private void OpenAutori_Click(object sender, RoutedEventArgs e)
        {
            MainTabs.SelectedIndex = 2;
        }

        private void EditEdit_Click(object sender, RoutedEventArgs e)
        {
            BtnEdit_Click(sender, e);
        }

        private void EditDelete_Click(object sender, RoutedEventArgs e)
        {
            BtnDelete_Click(sender, e);
        }

        private void OpenIzdavaci_Click(object sender, RoutedEventArgs e)
        {
            MainTabs.SelectedIndex = 3;
        }
        private void HelpAbout_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }

        private void BtnShowVisitorsByAuthor_Click(object sender, RoutedEventArgs e)
        {
            var selectedAuthor = AuthorsGrid.SelectedItem as AuthorDTO;
            if (selectedAuthor != null)
            {
                var allVisitors = _visitorDAO.GetAllVisitors().Select(VisitorDTO.FromModel).ToList();

                var win = new VisitorsByAuthorWindow(selectedAuthor, allVisitors);
                win.Owner = this;
                win.ShowDialog();
            }
            else
            {
                MessageBox.Show("Molimo izaberite autora iz tabele.","Upozorenje",MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private List<BookDTO> GetBooksForPublisher(PublisherDTO publisher)
        {
            if (publisher == null) return new List<BookDTO>();

            return _allBooks
                .Where(b => b.Publisher != null &&
                            string.Equals(b.Publisher.Name, publisher.Name, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        private void PublishersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnShowPublisherBooks.IsEnabled = PublishersGrid.SelectedItem != null;
        }

        private void BtnShowPublisherBooks_Click(object sender, RoutedEventArgs e)
        {
            var publisher = PublishersGrid.SelectedItem as PublisherDTO;
            if (publisher != null)
            {
                var books = GetBooksForPublisher(publisher);

                var dlg = new PublisherBooksDialog(publisher, books)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                dlg.ShowDialog();
            }
            else
            {
                MessageBox.Show("Izaberite izdavača iz tabele.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void LangSr_Click(object sender, RoutedEventArgs e)
        {
            LanguageService.SetLanguage("sr");
        }

        private void LangEn_Click(object sender, RoutedEventArgs e)
        {
            LanguageService.SetLanguage("en");
        }
    }
}