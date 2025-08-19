using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using TP_2.Data;
using TP_2.Models;

namespace TP_2.Pages
{
    public partial class LoginPage : Page
    {
        public event EventHandler<User>? UserLoggedIn;

        public LoginPage()
        {
            InitializeComponent();
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                using ApplicationDbContext context = new ApplicationDbContext();
                
                // Forcer la création de la base de données
                context.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la creation de la database: LoginPage.cs", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

		// Connexion de l'utilisateur
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using ApplicationDbContext context = new ApplicationDbContext();
                User ?user = context.Users
                    .FirstOrDefault(u => u.Username == username && u.Password == password && u.EstActif);

                if (user != null)
                {
                    // Mettre à jour la dernière connexion
                    user.DerniereConnexion = DateTime.Now;
                    context.SaveChanges();

                    // Déclencher l'événement de connexion
                    UserLoggedIn?.Invoke(this, user);
                }
                else
                {
                    MessageBox.Show("Nom d'utilisateur ou mot de passe incorrect.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la connexion : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

		// Navigation vers la page d'inscription
        private void btnGoToRegister_Click(object sender, RoutedEventArgs e)
        {
            // Navigation vers la page d'inscription
            var registerPage = new RegisterPage();
            registerPage.UserRegistered += (s, user) =>
            {
                // Rediriger vers la page de connexion après inscription
                UserLoggedIn?.Invoke(this, user);
            };
            
            NavigationService?.Navigate(registerPage);
        }
    }
}
