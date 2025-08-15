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
using TP_2_Developpement_Application_Burreau.Models;

namespace TP_2_Developpement_Application_Burreau
{

    public partial class MainWindow : Window
    {
        // Instances des pages
        private AccueilPage? accueilPage;
        private CalendrierPage? calendrierPage;
        private User? _currentUser;
        //AProposPage aProposPage = new AProposPage();
        //FonctionnalitesPage fonctionnalitesPage = new FonctionnalitesPage();
        //ParametresPage parametresPage = new ParametresPage();

        public MainWindow()
        {
            InitializeComponent();
            
            // Commencer par la page de connexion
            ShowLoginPage();
        }

        private void btnAccueil_Click(object sender, RoutedEventArgs e)
        {
            // Si l'utilisateur n'est pas connecté, afficher la page de connexion
            if (_currentUser == null)
            {
                ShowLoginPage();
            }
            else
            {
                // Si l'utilisateur est connecté, afficher la page d'accueil
                if (accueilPage == null)
                    accueilPage = new AccueilPage();
                NavigateToPage(accueilPage);
            }
        }

        //private void btnFonctionnalites_Click(object sender, RoutedEventArgs e)
        //{
        //    NavigateToPage(fonctionnalitesPage);
        //}

        //private void btnParametres_Click(object sender, RoutedEventArgs e)
        //{
        //    NavigateToPage(parametresPage);
        //}

        //private void btnAPropos_Click(object sender, RoutedEventArgs e)
        //{
        //    NavigateToPage(aProposPage);
        //}

        private void btnCalendrier_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null)
            {
                MessageBox.Show("Veuillez vous connecter pour accéder au calendrier.", "Connexion requise", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (calendrierPage == null)
                calendrierPage = new CalendrierPage(_currentUser);
            NavigateToPage(calendrierPage);
        }

        private void ShowLoginPage()
        {
            var loginPage = new LoginPage();
            loginPage.UserLoggedIn += OnUserLoggedIn;
            MainFrame.Navigate(loginPage);
        }

        private void OnUserLoggedIn(object sender, User user)
        {
            _currentUser = user;
            accueilPage = new AccueilPage();
            
            // Afficher les boutons après connexion
            btnCalendrier.Visibility = Visibility.Visible;
            btnDeconnexion.Visibility = Visibility.Visible;
            
            // Changer le texte du bouton accueil
            btnAccueil.Content = "🏠 Accueil";
            
            NavigateToPage(accueilPage);
        }

        private void btnDeconnexion_Click(object sender, RoutedEventArgs e)
        {
            // Masquer les boutons
            btnCalendrier.Visibility = Visibility.Collapsed;
            btnDeconnexion.Visibility = Visibility.Collapsed;
            
            // Remettre le texte original du bouton accueil
            btnAccueil.Content = "🔐 Connexion";
            
            // Réinitialiser les variables
            _currentUser = null;
            accueilPage = null;
            calendrierPage = null;
            
            // Retourner à la page de connexion
            ShowLoginPage();
        }

        private void NavigateToPage(Page page)
        {
            MainFrame.Navigate(page);
        }
    }
}