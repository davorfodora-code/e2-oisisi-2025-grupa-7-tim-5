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
        private BookDAO _bookDAO = new BookDAO();
        private AuthorDAO _authorDAO = new AuthorDAO(new PublisherDAO());
        private const int TAB_BOOKS = 0;
        private const int TAB_VISITORS = 1;
        private const int TAB_AUTHORS = 2;
        private const int TAB_PUBLISHERS = 3;

        public MainWindow()
        {
            InitializeComponent();
            InitializeStatusBar();
            RegisterKeyboardShortcuts();

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

        private string L(string key) => (string)Application.Current.Resources[key];
        private string F(string key, params object[] args) => string.Format(L(key), args);

        private void MsgError(string msgKey)
            => MessageBox.Show(L(msgKey), L("Dlg_ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
        private void MsgWarn(string msgKey)
            => MessageBox.Show(L(msgKey), L("Dlg_WarnTitle"), MessageBoxButton.OK, MessageBoxImage.Warning);
        private void MsgInfo(string msgKey)
            => MessageBox.Show(L(msgKey), L("Dlg_InfoTitle"), MessageBoxButton.OK, MessageBoxImage.Information);
        private MessageBoxResult MsgConfirm(string msgKey)
            => MessageBox.Show(L(msgKey), L("Dlg_ConfirmTitle"), MessageBoxButton.YesNo, MessageBoxImage.Question);

        private void InitializeStatusBar()
        {
            StatusBarAppName.Text = L("App_Title");
            UpdateTabName();
            UpdateDateTime();
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void RegisterKeyboardShortcuts()
        {
            var cmdNew = new RoutedCommand();
            CommandBindings.Add(new CommandBinding(cmdNew, (s, e) => BtnAdd_Click(s, null)));
            InputBindings.Add(new KeyBinding(cmdNew, Key.N, ModifierKeys.Control));

            var cmdClose = new RoutedCommand();
            CommandBindings.Add(new CommandBinding(cmdClose, (s, e) => Close()));
            InputBindings.Add(new KeyBinding(cmdClose, Key.C, ModifierKeys.Control));

  
            var cmdEdit = new RoutedCommand();
            CommandBindings.Add(new CommandBinding(cmdEdit, (s, e) =>
            {
       
                BtnEdit_Click(s, null);
            }));
            InputBindings.Add(new KeyBinding(cmdEdit, Key.E, ModifierKeys.Control));

      
            var cmdDelete = new RoutedCommand();
            CommandBindings.Add(new CommandBinding(cmdDelete, (s, e) =>
            {
                BtnDelete_Click(s, null);
            }));
            InputBindings.Add(new KeyBinding(cmdDelete, Key.D, ModifierKeys.Control));
        }

        protected override void OnKeyDown(KeyEventArgs e) { base.OnKeyDown(e); }

        private void Timer_Tick(object? sender, EventArgs e) => UpdateDateTime();

        private void UpdateDateTime()
        {
            var now = DateTime.Now;
            StatusBarDate.Text = $"{L("Status_Date")} {now:dd.MM.yyyy}";
            StatusBarTime.Text = $"{L("Status_Time")} {now:HH:mm:ss}";
        }

        private void UpdateTabName()
        {
            StatusBarTabName.Text = MainTabs.SelectedItem is TabItem tab
                ? $"{L("Status_Tab")} {tab.Header}"
                : $"{L("Status_Tab")} N/A";
        }

        private void LoadVisitors()
        {
            var visitorModels = _visitorController.GetAllVisitors();
            _visitors = new ObservableCollection<VisitorDTO>();
            foreach (var v in visitorModels)
                _visitors.Add(VisitorDTO.FromModel(v));
            VisitorsGrid.ItemsSource = _visitors;
            _allVisitors = _visitors.ToList();
        }

        private void LoadAuthors()
        {
            var authorModels = _authorController.GetAllAuthors();
            _authors = new ObservableCollection<AuthorDTO>();
            foreach (var a in authorModels)
                _authors.Add(AuthorDTO.FromModel(a));
            AuthorsGrid.ItemsSource = _authors;
            _allAuthors = _authors.ToList();
        }

        private void LoadBooks()
        {
            var bookModels = _bookController.GetAllBooks();
            _books = new ObservableCollection<BookDTO>();
            foreach (var b in bookModels)
                _books.Add(BookDTO.FromModel(b));
            BooksGrid.ItemsSource = _books;
            _allBooks = _books.ToList();
        }

        private void LoadPublishers()
        {
            if (_publisherController == null) return;
            var publisherModels = _publisherController.GetAllPublishers();
            _publishers.Clear();
            foreach (var p in publisherModels)
                _publishers.Add(PublisherDTO.FromModel(p));
            PublishersGrid.Items.Refresh();
        }

        private void Window_Loaded(object? sender, RoutedEventArgs e)
        {
            LoadVisitors();
            UpdateTabName();
        }

        private void BtnCompareBooks_Click(object sender, RoutedEventArgs e)
        {
            var allVisitors = _visitorDAO
                .GetAllVisitors()
                .Select(VisitorDTO.FromModel)
                .ToList();

            if (_allBooks == null || _allBooks.Count == 0)
            {
                MsgWarn("Msg_OperationFailed");
                return;
            }

            var win = new BookCompareWindow(_allBooks, allVisitors);
            win.Owner = this;
            win.ShowDialog();
        }

        private void BtnAdd_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                int idx = MainTabs?.SelectedIndex ?? -1;

                if (idx == TAB_VISITORS)
                {
                    var dialog = new AddVisitorDialog();
                    if (dialog.ShowDialog() == true)
                    {
                        var result = _visitorController.AddVisitor(dialog.Visitor);
                        if (result.Success) { ShowToast(L("Toast_Added")); LoadVisitors(); }
                        else MessageBox.Show(result.Message, L("Dlg_ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if (idx == TAB_PUBLISHERS)
                {
                    var dialog = new PublisherEditDialog(_authors);
                    dialog.Owner = this;
                    if (dialog.ShowDialog() == true)
                    {
                        var result = _publisherController.AddPublisher(dialog.Publisher.ToModel());
                        if (result != null) { ShowToast(L("Toast_Added")); LoadPublishers(); }
                        else MessageBox.Show(L("Msg_OperationFailed"), L("Dlg_ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if (idx == TAB_AUTHORS)
                {
                    var dialog = new AddAuthorDialog();
                    dialog.Owner = this;
                    if (dialog.ShowDialog() == true)
                    {
                        var result = _authorController.AddAuthor(dialog.Author);
                        if (result.Success) { ShowToast(L("Toast_Added")); LoadAuthors(); }
                        else MessageBox.Show(result.Message, L("Dlg_ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if (idx == TAB_BOOKS)
                {
                    if (_publishers == null || _publishers.Count == 0) { MsgWarn("Msg_OperationFailed"); return; }
                    var dialog = new AddBookDialog(_authors, _publishers);
                    if (dialog.ShowDialog() == true)
                    {
                        var result = _bookController.CreateBook(dialog.Book);
                        if (result.Success) { ShowToast(L("Toast_Added")); LoadBooks(); }
                        else MessageBox.Show(result.Message, L("Dlg_ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else { MsgInfo("Msg_SelectTab"); }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{L("Msg_CriticalError")} {ex.Message}\n\n{ex.StackTrace}",
                    L("Dlg_ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEdit_Click(object? sender, RoutedEventArgs e)
        {
            int idx = MainTabs?.SelectedIndex ?? -1;

            if (idx == TAB_VISITORS)
            {
                var selectedVisitorDto = VisitorsGrid.SelectedItem as VisitorDTO;
                if (selectedVisitorDto == null) { MsgWarn("Msg_SelectEntityFromTable"); return; }

                var originalVisitor = _visitorController.GetAllVisitors()
                    .FirstOrDefault(v => v.MembershipNumber == selectedVisitorDto.MembershipNumber);
                if (originalVisitor == null) { MsgError("Msg_OperationFailed"); return; }

                var dialog = new VisitorEditDialog(selectedVisitorDto, originalVisitor, _bookController);

                if (dialog.ShowDialog() == true)
                {
                    var result = _visitorController.UpdateVisitor(originalVisitor);
                    if (result.Success) { ShowToast(L("Toast_Updated")); LoadVisitors(); }
                    else MsgError("Msg_OperationFailed");
                }
            }
            else if (idx == TAB_PUBLISHERS)
            {
                var selectedPublisher = PublishersGrid.SelectedItem as PublisherDTO;
                if (selectedPublisher == null) { MsgWarn("Msg_SelectEntityToEdit"); return; }

                var dialog = new PublisherEditDialog(selectedPublisher, _authors);
                dialog.Owner = this;
                if (dialog.ShowDialog() == true)
                {
                    var publisherModel = dialog.Publisher.ToModel();
                    var result = _publisherController.UpdatePublisher(publisherModel);
                    if (result != null) { ShowToast(L("Toast_Updated")); LoadPublishers(); LoadBooks(); }
                    else MsgError("Msg_OperationFailed");
                }
            }
            else if (idx == TAB_AUTHORS)
            {
                var selectedAuthorDto = AuthorsGrid.SelectedItem as AuthorDTO;
                if (selectedAuthorDto == null) { MsgWarn("Msg_SelectEntityToEdit"); return; }

                var originalAuthor = _authorController.GetAllAuthors()
                    .FirstOrDefault(a => a.IdCard == selectedAuthorDto.IdCard);
                if (originalAuthor == null) { MsgError("Msg_OperationFailed"); return; }

                var dialog = new AuthorEditDialog(selectedAuthorDto, originalAuthor, _bookController);
                dialog.Owner = this;
                if (dialog.ShowDialog() == true)
                {
                    var result = _authorController.UpdateAuthor(dialog.AuthorDTO);
                    LoadBooks();
                    if (result.Success)
                    {
                        MessageBox.Show(result.Message, L("Dlg_InfoTitle"), MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadAuthors();
                        LoadPublishers();
                    }
                    else
                        MessageBox.Show(result.Message, L("Dlg_ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (idx == TAB_BOOKS)
            {
                var selectedBook = BooksGrid.SelectedItem as BookDTO;
                if (selectedBook == null) { MsgWarn("Msg_SelectEntityToEdit"); return; }
                if (_publishers == null || _publishers.Count == 0) { MsgError("Msg_OperationFailed"); return; }

                var dialog = new AddBookDialog(selectedBook, _authors, _publishers);
                if (dialog.ShowDialog() == true)
                {
                    var result = _bookController.EditBook(dialog.Book);
                    if (result.Success) { ShowToast(L("Toast_Updated")); LoadBooks(); }
                    else MessageBox.Show(result.Message, L("Dlg_ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else { MsgInfo("Msg_SelectTab"); }
        }

        private void BtnDelete_Click(object? sender, RoutedEventArgs e)
        {
            int idx = MainTabs?.SelectedIndex ?? -1;

            if (idx == TAB_VISITORS)
            {
                var selectedVisitor = VisitorsGrid.SelectedItem as VisitorDTO;
                if (selectedVisitor == null) { MsgWarn("Msg_SelectEntityToDelete"); return; }
                var result = MessageBox.Show(
                    $"{L("Msg_ConfirmDeleteVisitor")}\n{selectedVisitor.Name} {selectedVisitor.LastName}\n({selectedVisitor.MembershipNumber})?",
                    L("Msg_DeleteConfirmTitle"), MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var deleteResult = _visitorController.DeleteVisitor(selectedVisitor.MembershipNumber);
                    if (deleteResult.Success) { ShowToast(L("Msg_DeleteSuccess")); LoadVisitors(); }
                    else MsgError("Msg_OperationFailed");
                }
            }
            else if (idx == TAB_PUBLISHERS)
            {
                var selectedPublisher = PublishersGrid.SelectedItem as PublisherDTO;
                if (selectedPublisher == null) { MsgWarn("Msg_SelectEntityToDelete"); return; }
                var result = MessageBox.Show(
                    $"{L("Msg_ConfirmDeletePublisher")}\n{selectedPublisher.Name}?",
                    L("Msg_DeleteConfirmTitle"), MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    var deleteResult = _publisherController.RemovePublisher(selectedPublisher.ToModel());
                    if (deleteResult != null) { ShowToast(L("Msg_DeleteSuccess")); LoadPublishers(); LoadBooks(); }
                    else MsgError("Msg_OperationFailed");
                }
            }
            else if (idx == TAB_AUTHORS)
            {
                var selectedAuthor = AuthorsGrid.SelectedItem as AuthorDTO;
                if (selectedAuthor == null) { MsgWarn("Msg_SelectEntityToDelete"); return; }
                var result = MessageBox.Show(
                    $"{L("Msg_ConfirmDeleteAuthor")}\n{selectedAuthor.Name} {selectedAuthor.Lastname}\n({selectedAuthor.IdCard})?",
                    L("Msg_DeleteConfirmTitle"), MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var deleteResult = _authorController.DeleteAuthor(selectedAuthor.IdCard);
                    if (deleteResult.Success) { ShowToast(L("Msg_DeleteSuccess")); LoadAuthors(); LoadPublishers(); }
                    else MsgError("Msg_OperationFailed");
                }
            }
            else if (idx == TAB_BOOKS)
            {
                var selectedBook = BooksGrid.SelectedItem as BookDTO;
                if (selectedBook == null) { MsgWarn("Msg_SelectEntityToDelete"); return; }
                var result = MessageBox.Show(
                    $"{L("Msg_ConfirmDeleteBook")}\n{selectedBook.Name}\n({selectedBook.Isbn})?",
                    L("Msg_DeleteConfirmTitle"), MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var deleteResult = _bookController.RemoveBook(selectedBook.Isbn);
                    if (deleteResult.Success) { ShowToast(L("Msg_DeleteSuccess")); LoadBooks(); }
                    else MsgError("Msg_OperationFailed");
                }
            }
            else { MsgInfo("Msg_SelectTab"); }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string q = TxtSearch.Text ?? "";
            int idx = MainTabs?.SelectedIndex ?? -1;
            if (string.IsNullOrWhiteSpace(q))
            {
                VisitorsGrid.ItemsSource = _visitors;
                AuthorsGrid.ItemsSource = _authors;
                BooksGrid.ItemsSource = _books;
                return;
            }
            if (idx == TAB_VISITORS)
            { VisitorsGrid.ItemsSource = new ObservableCollection<VisitorDTO>(VisitorSearch.Filter(_allVisitors, q).ToList()); return; }
            if (idx == TAB_AUTHORS)
            { AuthorsGrid.ItemsSource = new ObservableCollection<AuthorDTO>(AuthorSearch.Filter(_allAuthors, q).ToList()); return; }
            if (idx == TAB_BOOKS)
            { BooksGrid.ItemsSource = new ObservableCollection<BookDTO>(BookSearch.Filter(_allBooks, q).ToList()); return; }
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
                    if (result != null) { MsgInfo("Toast_Updated"); LoadPublishers(); }
                    else MsgError("Msg_OperationFailed");
                }
            }
        }
        private List<AuthorDTO> GetWishlistAuthorsForVisitor(VisitorDTO visitor)
        {
            if (visitor?.Wishlist == null || visitor.Wishlist.Count == 0)
                return new List<AuthorDTO>();

            var wishlistIsbns = visitor.Wishlist
                .Select(b => b?.Isbn)
                .Where(isbn => !string.IsNullOrWhiteSpace(isbn))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var fullBooks = _bookDAO.GetAllBooks()
                .Select(BookDTO.FromModel)
                .Where(b => b?.Isbn != null && wishlistIsbns.Contains(b.Isbn))
                .ToList();

            var authorIdCards = fullBooks
                .SelectMany(b => b.Authors ?? new List<AuthorDTO>())
                .Select(a => a?.IdCard)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            return _authorDAO.GetAllAuthors()
                .Select(AuthorDTO.FromModel)
                .Where(a => a != null && !string.IsNullOrWhiteSpace(a.IdCard)
                            && authorIdCards.Contains(a.IdCard))
                .ToList();
        }

        private void BtnShowWishlistAuthors_Click(object sender, RoutedEventArgs e)
        {
            var visitor = VisitorsGrid.SelectedItem as VisitorDTO;
            if (visitor == null)
            {
                MsgWarn("Msg_SelectEntityFromTable");
                return;
            }
         

            var authors = GetWishlistAuthorsForVisitor(visitor);

            new WishlistAuthorsDialog(visitor, authors)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            }.ShowDialog();
        }
        private void VisitorsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnShowWishlistAuthors.IsEnabled = VisitorsGrid.SelectedItem != null;
        }
        private void BtnShowAuthorsByPublisher_Click(object sender, RoutedEventArgs e)
        {
            var selectedPublisher = PublishersGrid.SelectedItem as PublisherDTO;
            if (selectedPublisher == null)
            {
                MsgWarn("Msg_SelectEntityFromTable");
                return;
            }


            var allBooks = _bookDAO.GetAllBooks()
                                   .Select(BookDTO.FromModel)
                                   .Where(b => b != null)
                                   .ToList();


            var publisherBooks = allBooks
                .Where(b => b.Publisher != null && b.Publisher.Id == selectedPublisher.Id)
                .ToList();


            var authorIdCards = publisherBooks
                .SelectMany(b => b.Authors ?? new List<AuthorDTO>())
                .Select(a => a?.IdCard)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct()
                .ToHashSet();


            var allAuthors = _authorDAO.GetAllAuthors()
                                       .Select(AuthorDTO.FromModel)
                                       .Where(a => a != null)
                                       .ToList();

            var authorsForPublisher = allAuthors
                .Where(a => authorIdCards.Contains(a.IdCard))
                .ToList();

            var win = new PublisherAuthorsDialog(selectedPublisher, authorsForPublisher) { Owner = this };
            win.ShowDialog();
        }
        private void TxtSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter) BtnSearch_Click(sender, e);
        }

        private void MainTabs_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            TxtSearch.Clear();
            UpdateTabName();
        }

        private void MenuItem_Click(object? sender, RoutedEventArgs e) { }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _timer?.Stop();
        }

        private void TxtSearch_TextChanged(object? sender, TextChangedEventArgs e) { }
        private void AuthorsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
        private void ShowVisitorsForAuthor_Click(object sender, RoutedEventArgs e) { }

        private void FileNew_Click(object sender, RoutedEventArgs e) => BtnAdd_Click(sender, e);

        private void FileSave_Click(object sender, RoutedEventArgs e)
            => MessageBox.Show(L("Msg_SaveFunctionCalled"), L("Dlg_InfoTitle"), MessageBoxButton.OK, MessageBoxImage.Information);

        private void OpenPosetioci_Click(object sender, RoutedEventArgs e) => MainTabs.SelectedIndex = 1;
        private void OpenKnjige_Click(object sender, RoutedEventArgs e) => MainTabs.SelectedIndex = 0;
        private void OpenAutori_Click(object sender, RoutedEventArgs e) => MainTabs.SelectedIndex = 2;
        private void OpenIzdavaci_Click(object sender, RoutedEventArgs e) => MainTabs.SelectedIndex = 3;
        private void EditEdit_Click(object sender, RoutedEventArgs e) => BtnEdit_Click(sender, e);
        private void EditDelete_Click(object sender, RoutedEventArgs e) => BtnDelete_Click(sender, e);

        private void HelpAbout_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow().ShowDialog();
        }

        private void BtnShowVisitorsByAuthor_Click(object sender, RoutedEventArgs e)
        {
            var selectedAuthor = AuthorsGrid.SelectedItem as AuthorDTO;
            if (selectedAuthor != null)
            {
                var allVisitors = _visitorDAO.GetAllVisitors().Select(VisitorDTO.FromModel).ToList();
                var win = new VisitorsByAuthorWindow(selectedAuthor, allVisitors) { Owner = this };
                win.ShowDialog();
            }
            else MsgWarn("Msg_SelectEntityFromTable");
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
                new PublisherBooksDialog(publisher, books)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                }.ShowDialog();
            }
            else MsgWarn("Msg_SelectEntityFromTable");
        }
        private List<BookDTO> GetWishlistBooksForVisitor(VisitorDTO visitor)
        {
            if (visitor == null) return new List<BookDTO>();

            return visitor.Wishlist?.ToList() ?? new List<BookDTO>();
        }
        private List<AuthorDTO> GetAuthorsFromBooksDistinct(List<BookDTO> books)
        {
            if (books == null) return new List<AuthorDTO>();

            return books
                .SelectMany(b => b.Authors ?? new List<AuthorDTO>())
                .Where(a => a != null && !string.IsNullOrWhiteSpace(a.IdCard))
                .GroupBy(a => a.IdCard)
                .Select(g => g.First())
                .ToList();
        }
        private void LangSr_Click(object sender, RoutedEventArgs e)
        {
            LanguageService.SetLanguage("sr");
            StatusBarAppName.Text = L("App_Title");
            UpdateDateTime(); UpdateTabName();
        }

        private void LangEn_Click(object sender, RoutedEventArgs e)
        {
            LanguageService.SetLanguage("en");
            StatusBarAppName.Text = L("App_Title");
            UpdateDateTime(); UpdateTabName();
        }
    }
}