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
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();

            var purchaseDAO = new PurchaseDAO();
            var visitorDAO = new VisitorDAO(purchaseDAO);
            _visitorController = new VisitorController(visitorDAO, purchaseDAO);

            LoadVisitors();

            InitializeStatusBar();
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

        private void Timer_Tick(object sender, EventArgs e)
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadVisitors();
            UpdateTabName();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
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

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (VisitorsGrid.SelectedItem is VisitorDTO selectedVisitor)
            {
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
            else
            {
                MessageBox.Show("Izaberite posetioca za izmenu", "Upozorenje",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (VisitorsGrid.SelectedItem is VisitorDTO selectedVisitor)
            {
                var result = MessageBox.Show(
                    $"Da li ste sigurni da želite da obrišete posetioca:\n" +
                    $"{selectedVisitor.Name} {selectedVisitor.LastName}\n" +
                    $"({selectedVisitor.MembershipNumber})?",
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
            else
            {
                MessageBox.Show("Izaberite posetioca za brisanje", "Upozorenje",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void MainTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TxtSearch.Clear();

            if (MainTabs.SelectedIndex == 1) 
            {
                LoadVisitors();
            }

            UpdateTabName();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
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
    }
}