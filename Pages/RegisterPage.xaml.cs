using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using TP_2_Developpement_Application_Burreau.Data;
using TP_2_Developpement_Application_Burreau.Models;

namespace TP_2_Developpement_Application_Burreau.Pages
{
    public partial class RegisterPage : Page
    {
        public event EventHandler<User>? UserRegistered;

        public RegisterPage()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                using var context = new ApplicationDbContext();
                
                // Vérifier si l'utilisateur existe déjà
                if (context.Users.Any(u => u.Username == txtUsername.Text.Trim()))
                {
                    MessageBox.Show("Ce nom d'utilisateur existe déjà.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (context.Users.Any(u => u.Email == txtEmail.Text.Trim()))
                {
                    MessageBox.Show("Cette adresse email est déjà utilisée.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Créer le nouvel utilisateur
                var newUser = new User
                {
                    Username = txtUsername.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Password = txtPassword.Password, // En production, utiliser un hash
                    Prenom = string.IsNullOrWhiteSpace(txtPrenom.Text) ? null : txtPrenom.Text.Trim(),
                    Nom = string.IsNullOrWhiteSpace(txtNom.Text) ? null : txtNom.Text.Trim(),
                    Telephone = string.IsNullOrWhiteSpace(txtTelephone.Text) ? null : txtTelephone.Text.Trim(),
                    EstActif = true,
                    DateCreation = DateTime.Now
                };

                context.Users.Add(newUser);
                context.SaveChanges();

                MessageBox.Show("Compte créé avec succès ! Vous pouvez maintenant vous connecter.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Déclencher l'événement d'inscription
                UserRegistered?.Invoke(this, newUser);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la création du compte : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnGoToLogin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private bool ValidateForm()
        {
            // Validation du nom d'utilisateur
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Le nom d'utilisateur est obligatoire.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUsername.Focus();
                return false;
            }

            if (txtUsername.Text.Length < 3)
            {
                MessageBox.Show("Le nom d'utilisateur doit contenir au moins 3 caractères.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUsername.Focus();
                return false;
            }

            // Validation de l'email
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("L'adresse email est obligatoire.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return false;
            }

            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Veuillez saisir une adresse email valide.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return false;
            }

            // Validation du mot de passe
            if (string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Le mot de passe est obligatoire.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return false;
            }

            if (txtPassword.Password.Length < 6)
            {
                MessageBox.Show("Le mot de passe doit contenir au moins 6 caractères.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return false;
            }

            // Validation de la confirmation du mot de passe
            if (txtPassword.Password != txtConfirmPassword.Password)
            {
                MessageBox.Show("Les mots de passe ne correspondent pas.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtConfirmPassword.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }
    }
}
