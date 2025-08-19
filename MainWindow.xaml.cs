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
using TP_2.Pages;
using TP_2.Models;

namespace TP_2
{

    public partial class MainWindow : Window
    {
        // Instances des pages
        private AccueilPage? accueilPage;
        private CalendrierPage? calendrierPage;
        private User? _currentUser;

        public MainWindow()
        {
            InitializeComponent();
            
            // Commencer par la page de connexion
            ShowLoginPage();
        }

		// Bouton pour acceder a la page d'accueil
        private void btnAccueil_Click(object sender, RoutedEventArgs e)
        {
            // Si l'utilisateur n'est pas connecte, afficher la page de connexion
            if (_currentUser == null)
            {
                ShowLoginPage();
            }
            else
            {
                // Si l'utilisateur est connecte, afficher la page d'accueil
                if (accueilPage == null)
                    accueilPage = new AccueilPage();
                NavigateToPage(accueilPage);
            }
        }

		// Bouton pour acceder a la page de calendrier
        private void btnCalendrier_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null)
            {
                MessageBox.Show("Veuillez vous connecter pour acceder au calendrier.", "Connexion requise", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (calendrierPage == null)
                calendrierPage = new CalendrierPage(_currentUser);
            NavigateToPage(calendrierPage);
        }

		// Affichage de la page de connexion
        private void ShowLoginPage()
        {
            LoginPage loginPage = new LoginPage();
            loginPage.UserLoggedIn += OnUserLoggedIn;
            MainFrame.Navigate(loginPage);
        }

		// Evenement pour la connexion de l'utilisateur
        private void OnUserLoggedIn(object sender, User user)
        {
            _currentUser = user;
            accueilPage = new AccueilPage();
            
            // Afficher les boutons apres connexion
            btnCalendrier.Visibility = Visibility.Visible;
            btnDeconnexion.Visibility = Visibility.Visible;
            
            // Changer le texte du bouton accueil
            btnAccueil.Content = "🏠 Accueil";
            
            NavigateToPage(accueilPage);
        }

		// Bouton pour se deconnecter
        private void btnDeconnexion_Click(object sender, RoutedEventArgs e)
        {
            // Masquer les boutons
            btnCalendrier.Visibility = Visibility.Collapsed;
            btnDeconnexion.Visibility = Visibility.Collapsed;
            
            // Remettre le texte original du bouton accueil
            btnAccueil.Content = "🔐 Connexion";
            
            // Reinitialiser les variables
            _currentUser = null;
            accueilPage = null;
            calendrierPage = null;
            
            // Retourner a la page de connexion
            ShowLoginPage();
        }

		// Navigation vers une page
        private void NavigateToPage(Page page)
        {
            MainFrame.Navigate(page);
        }
    }
}