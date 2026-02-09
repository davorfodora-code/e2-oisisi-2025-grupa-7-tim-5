using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls;
using Core.Controllers;
using Core.DAO;
using Core.DTO;
using System.Collections.ObjectModel;
using System.Linq;

namespace WpfClient
{
    public partial class MainWindow : Window
    {
        private VisitorController _visitorController;
        private ObservableCollection<VisitorDTO> _visitors;
        private DispatcherTimer _timer = new DispatcherTimer();
        private AuthorController _authorController;
        private ObservableCollection<AuthorDTO> _authors;
        private BookController _bookController;
        private ObservableCollection<BookDTO> _books;

        public MainWindow()
        {
          
            InitializeComponent(); 

            InitializeStatusBar(); 

      
            var purchaseDAO = new PurchaseDAO();
            var visitorDAO = new VisitorDAO(purchaseDAO);
            var publisherDAO = new PublisherDAO();
            var bookDAO = new BookDAO();
            var authorDAO = new AuthorDAO();

            _authorController = new AuthorController(authorDAO);
            _bookController = new BookController(bookDAO, publisherDAO);
            _visitorController = new VisitorController(visitorDAO, purchaseDAO);


            _visitors = new ObservableCollection<VisitorDTO>();
            _authors = new ObservableCollection<AuthorDTO>();
            _books = new ObservableCollection<BookDTO>();

            VisitorsGrid.ItemsSource = _visitors;
            AuthorsGrid.ItemsSource = _authors;
            BooksGrid.ItemsSource = _books;

 
            LoadVisitors();
            LoadAuthors();
            LoadBooks();
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

            AuthorsGrid.ItemsSource = _authors; // poveži sa DataGrid-om
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
        }


        private void Window_Loaded(object? sender, RoutedEventArgs e)
        {
            LoadVisitors();
            UpdateTabName();
        }

        private void BtnAdd_Click(object? sender, RoutedEventArgs e)
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
                        MessageBox.Show(result.Message, "Uspešno", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadVisitors();
                    }
                    else
                    {
                        MessageBox.Show(result.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else if (tabName == "Autori")
            {
                var dialog = new AddAuthorDialog();
                if (dialog.ShowDialog() == true)
                {
                    var result = _authorController.AddAuthor(dialog.Author);
                    if (result.Success)
                    {
                        MessageBox.Show(result.Message, "Uspešno", MessageBoxButton.OK, MessageBoxImage.Information);
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
                var dialog = new AddBookDialog();
                if (dialog.ShowDialog() == true)
                {
                    var result = _bookController.CreateBook(dialog.Book);
                    if (result.Success)
                    {
                        MessageBox.Show(result.Message, "Uspešno", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void BtnEdit_Click(object? sender, RoutedEventArgs e)
        {
            var selectedTab = MainTabs?.SelectedItem as TabItem;
            var tabName = selectedTab?.Header?.ToString();

            if (tabName == "Posetioci")
            {
                var selectedVisitor = VisitorsGrid.SelectedItem as VisitorDTO;
                if (selectedVisitor == null)
                {
                    MessageBox.Show("Izaberite posetioca za izmenu", "Upozorenje",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var dialog = new AddVisitorDialog(selectedVisitor);
                if (dialog.ShowDialog() == true)
                {
                    var result = _visitorController.UpdateVisitor(dialog.Visitor);
                    if (result.Success)
                    {
                        MessageBox.Show(result.Message, "Uspešno", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadVisitors();
                    }
                    else
                    {
                        MessageBox.Show(result.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else if (tabName == "Autori")
            {
                var selectedAuthor = AuthorsGrid.SelectedItem as AuthorDTO;
                if (selectedAuthor == null)
                {
                    MessageBox.Show("Izaberite autora za izmenu", "Upozorenje",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var dialog = new AddAuthorDialog(selectedAuthor);
                if (dialog.ShowDialog() == true)
                {
                    var result = _authorController.UpdateAuthor(dialog.Author);
                    if (result.Success)
                    {
                        MessageBox.Show(result.Message, "Uspešno", MessageBoxButton.OK, MessageBoxImage.Information);
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
                var selectedBook = BooksGrid.SelectedItem as BookDTO;
                if (selectedBook == null)
                {
                    MessageBox.Show("Izaberite knjigu za izmenu", "Upozorenje",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var dialog = new AddBookDialog(selectedBook);
                if (dialog.ShowDialog() == true)
                {
                    var result = _bookController.EditBook(dialog.Book);
                    if (result.Success)
                    {
                        MessageBox.Show(result.Message, "Uspešno", MessageBoxButton.OK, MessageBoxImage.Information);
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
        //private void BtnAddVisitor_Click(object? sender, RoutedEventArgs e)
        //{
        //    var dialog = new AddVisitorDialog();  
        //    if (dialog.ShowDialog() == true)     
        //    {
        //        var result = _visitorController.AddVisitor(dialog.Visitor);

        //        if (result.Success)
        //        {
        //            MessageBox.Show(result.Message, "Uspešno", MessageBoxButton.OK, MessageBoxImage.Information);
        //            LoadVisitors(); 
        //        }
        //        else
        //        {
        //            MessageBox.Show(result.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }
        //}


        //private void BtnDeleteVisitor_Click(object? sender, RoutedEventArgs e)
        //{
        //    if (VisitorsGrid.SelectedItem is VisitorDTO selectedVisitor)
        //    {
        //        var result = MessageBox.Show(
        //            $"Da li ste sigurni da želite da obrišete posetioca:\n" +
        //            $"{selectedVisitor.Name} {selectedVisitor.LastName}\n" +
        //            $"({selectedVisitor.MembershipNumber})?",
        //            "Potvrda brisanja",
        //            MessageBoxButton.YesNo,
        //            MessageBoxImage.Question);

        //        if (result == MessageBoxResult.Yes)
        //        {
        //            var deleteResult = _visitorController.DeleteVisitor(selectedVisitor.MembershipNumber);

        //            if (deleteResult.Success)
        //            {
        //                MessageBox.Show(deleteResult.Message, "Uspešno",
        //                    MessageBoxButton.OK, MessageBoxImage.Information);
        //                LoadVisitors();
        //            }
        //            else
        //            {
        //                MessageBox.Show(deleteResult.Message, "Greška",
        //                    MessageBoxButton.OK, MessageBoxImage.Error);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Izaberite posetioca za brisanje", "Upozorenje",
        //            MessageBoxButton.OK, MessageBoxImage.Warning);
        //    }
        //}

        //private void BtnEditVisitor_Click(object? sender, RoutedEventArgs e)
        //{
        //    if (VisitorsGrid.SelectedItem is VisitorDTO selectedVisitor)
        //    {
        //        var dialog = new AddVisitorDialog(selectedVisitor);

        //        if (dialog.ShowDialog() == true)
        //        {
        //            var result = _visitorController.UpdateVisitor(dialog.Visitor);

        //            if (result.Success)
        //            {
        //                MessageBox.Show(result.Message, "Uspešno", MessageBoxButton.OK, MessageBoxImage.Information);
        //                LoadVisitors();
        //            }
        //            else
        //            {
        //                MessageBox.Show(result.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Izaberite posetioca za izmenu", "Upozorenje",
        //            MessageBoxButton.OK, MessageBoxImage.Warning);
        //    }
        //}
        //private void BtnAddAuthor_Click(object? sender, RoutedEventArgs e)
        //{
        //    var dialog = new AddAuthorDialog();
        //    if (dialog.ShowDialog() == true)
        //    {
        //        var result = _authorController.AddAuthor(dialog.Author);

        //        if (result.Success)
        //        {
        //            MessageBox.Show(result.Message, "Uspešno", MessageBoxButton.OK, MessageBoxImage.Information);
        //            LoadAuthors();
        //        }
        //        else
        //        {
        //            MessageBox.Show(result.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }
        //}
        //private void BtnEditAuthor_Click(object? sender, RoutedEventArgs e)
        //{
        //    if (AuthorsGrid.SelectedItem is AuthorDTO selectedAuthor)
        //    {
        //        var dialog = new AddAuthorDialog(selectedAuthor);

        //        if (dialog.ShowDialog() == true)
        //        {
        //            var result = _authorController.UpdateAuthor(dialog.Author);

        //            if (result.Success)
        //            {
        //                MessageBox.Show(result.Message, "Uspešno", MessageBoxButton.OK, MessageBoxImage.Information);
        //                LoadAuthors();
        //            }
        //            else
        //            {
        //                MessageBox.Show(result.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Izaberite autora za izmenu", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    }
        //}


       
        //private void BtnDeleteAuthor_Click(object? sender, RoutedEventArgs e)
        //{
        //    if (AuthorsGrid.SelectedItem is AuthorDTO selectedAuthor)
        //    {
        //        var result = MessageBox.Show(
        //            $"Da li ste sigurni da želite da obrišete autora:\n" +
        //            $"{selectedAuthor.Name} {selectedAuthor.Lastname}\n" +
        //            $"({selectedAuthor.IdCard})?",
        //            "Potvrda brisanja",
        //            MessageBoxButton.YesNo,
        //            MessageBoxImage.Question);

        //        if (result == MessageBoxResult.Yes)
        //        {
        //            var deleteResult = _authorController.DeleteAuthor(selectedAuthor.IdCard);

        //            if (deleteResult.Success)
        //            {
        //                MessageBox.Show(deleteResult.Message, "Uspešno",
        //                    MessageBoxButton.OK, MessageBoxImage.Information);
        //                LoadAuthors(); 
        //            }
        //            else
        //            {
        //                MessageBox.Show(deleteResult.Message, "Greška",
        //                    MessageBoxButton.OK, MessageBoxImage.Error);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Izaberite autora za brisanje", "Upozorenje",
        //            MessageBoxButton.OK, MessageBoxImage.Warning);
        //    }
        //}
        //private void BtnAddBook_Click(object? sender, RoutedEventArgs e)
        //{
        //    var dialog = new AddBookDialog();
        //    if (dialog.ShowDialog() == true)
        //    {
        //        var result = _bookController.CreateBook(dialog.Book);

        //        if (result.Success)
        //        {
        //            MessageBox.Show(result.Message, "Uspešno", MessageBoxButton.OK, MessageBoxImage.Information);
        //            LoadBooks();
        //        }
        //        else
        //        {
        //            MessageBox.Show(result.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }
        //}
        //private void BtnEditBook_Click(object? sender, RoutedEventArgs e)
        //{
        //    if (BooksGrid.SelectedItem is BookDTO selectedBook)
        //    {
        //        var dialog = new AddBookDialog(selectedBook);

        //        if (dialog.ShowDialog() == true)
        //        {
        //            var result = _bookController.EditBook(dialog.Book);

        //            if (result.Success)
        //            {
        //                MessageBox.Show(result.Message, "Uspešno", MessageBoxButton.OK, MessageBoxImage.Information);
        //                LoadBooks();
        //            }
        //            else
        //            {
        //                MessageBox.Show(result.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Izaberite knjigu za izmenu", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    }
        //}

        //private void BtnDeleteBook_Click(object? sender, RoutedEventArgs e)
        //{
        //    if (BooksGrid.SelectedItem is BookDTO selectedBook)
        //    {
        //        var result = MessageBox.Show(
        //            $"Da li ste sigurni da želite da obrišete knjigu:\n" +
        //            $"{selectedBook.Name}\n" +
        //            $"(ISBN: {selectedBook.Isbn})",
        //            "Potvrda brisanja",
        //            MessageBoxButton.YesNo,
        //            MessageBoxImage.Question);

        //        if (result == MessageBoxResult.Yes)
        //        {
        //            var deleteResult = _bookController.RemoveBook(selectedBook.Isbn);

        //            if (deleteResult.Success)
        //            {
        //                MessageBox.Show(deleteResult.Message, "Uspešno",
        //                    MessageBoxButton.OK, MessageBoxImage.Information);
        //                LoadBooks(); // Funkcija koja osvežava BooksGrid
        //            }
        //            else
        //            {
        //                MessageBox.Show(deleteResult.Message, "Greška",
        //                    MessageBoxButton.OK, MessageBoxImage.Error);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Izaberite knjigu za brisanje", "Upozorenje",
        //            MessageBoxButton.OK, MessageBoxImage.Warning);
        //    }
        //}


        private void BtnSearch_Click(object? sender, RoutedEventArgs e)
        {

        }

        private void TxtSearch_KeyDown(object? sender, KeyEventArgs e)
        {

        }

        private void MainTabs_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            TxtSearch.Clear();

            if (MainTabs.SelectedIndex == 1) 
            {
                LoadVisitors();
            }

            UpdateTabName();
        }

        private void MenuItem_Click(object? sender, RoutedEventArgs e)
        {
            // Prazno
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
    }
}