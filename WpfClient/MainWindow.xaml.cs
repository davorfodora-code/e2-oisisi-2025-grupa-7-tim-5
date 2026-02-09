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
            // Kreiraj i otvori dijalog za dodavanje posetioca
            var dialog = new AddVisitorDialog();

            // Prikaži dijalog i čekaj da korisnik unese podatke
            if (dialog.ShowDialog() == true)
            {
                // Dobavi VisitorDTO iz dijaloga
                var newVisitor = dialog.Visitor;

                // Pozovi kontroler da doda posetioca
                var result = _visitorController.AddVisitor(newVisitor);

                // Proveri rezultat
                if (result.Success)
                {
                    // Prikaži poruku o uspehu
                    MessageBox.Show(result.Message, "Uspešno", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Osveži listu posetilaca
                    LoadVisitors();
                }
                else
                {
                    // Prikaži poruku o grešci
                    MessageBox.Show(result.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void BtnEdit_Click(object sender, RoutedEventArgs e) { }
        private void BtnDelete_Click(object sender, RoutedEventArgs e) { }
        private void BtnSearch_Click(object sender, RoutedEventArgs e) { }
        private void TxtSearch_KeyDown(object sender, KeyEventArgs e) { }
        private void MainTabs_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) { }
        private void MenuItem_Click(object sender, RoutedEventArgs e) { }
    }
}