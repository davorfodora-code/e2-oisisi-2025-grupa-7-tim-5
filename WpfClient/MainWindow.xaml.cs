using System.Windows;
using System.Windows.Input;
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

        public MainWindow()
        {
            InitializeComponent();
            var purchaseDAO = new PurchaseDAO();
            var visitorDAO = new VisitorDAO(purchaseDAO);
            _visitorController = new VisitorController(visitorDAO, purchaseDAO);

            LoadVisitors();
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
        }


        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddVisitorDialog();

            if (dialog.ShowDialog() == true)
            {
                var newVisitor = dialog.Visitor;

                var result = _visitorController.AddVisitor(newVisitor);

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
        private void BtnSearch_Click(object sender, RoutedEventArgs e) { }
        private void TxtSearch_KeyDown(object sender, KeyEventArgs e) { }
        private void MainTabs_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) { }
        private void MenuItem_Click(object sender, RoutedEventArgs e) { }
    }
}