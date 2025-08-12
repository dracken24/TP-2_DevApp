using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TP_2_Developpement_Application_Burreau.Pages;

namespace TP_2_Developpement_Application_Burreau
{
    /// <summary> 
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // Navigation vers la page d'accueil au démarrage
            NavigateToPage(new AccueilPage());
        }

        private void btnAccueil_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new AccueilPage());
        }

        private void btnFonctionnalites_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new FonctionnalitesPage());
        }

        private void btnParametres_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new ParametresPage());
        }

        private void btnAPropos_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new AProposPage());
        }

        private void NavigateToPage(Page page)
        {
            MainFrame.Navigate(page);
        }
    }
}