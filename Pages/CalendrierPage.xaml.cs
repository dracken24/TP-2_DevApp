using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using TP_2_Developpement_Application_Burreau.Data;
using TP_2_Developpement_Application_Burreau.Models;
using TP_2_Developpement_Application_Burreau.Dialogs;


namespace TP_2_Developpement_Application_Burreau.Pages
{
    public partial class CalendrierPage : Page
    {
        private ApplicationDbContext _context;
        private DateTime _currentDate = DateTime.Today;
        private User _currentUser;

        public CalendrierPage(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            InitializeDatabase();
            UpdateCalendarDisplay();
            LoadRendezVousForDate(_currentDate);
        }

        private void InitializeDatabase()
        {
            _context = new ApplicationDbContext();
            _context.Database.EnsureCreated();
            
            // S'assurer que l'utilisateur existe dans la base
            if (!_context.Users.Any(u => u.Id == _currentUser.Id))
            {
                // Si l'utilisateur n'existe pas, le recréer
                _context.Users.Add(_currentUser);
                _context.SaveChanges();
            }
        }

        private void UpdateCalendarDisplay()
        {
            txtCurrentMonth.Text = _currentDate.ToString("MMMM yyyy");
            MainCalendar.DisplayDate = _currentDate;
            MainCalendar.SelectedDate = _currentDate;
        }

        private void btnPrevMonth_Click(object sender, RoutedEventArgs e)
        {
            _currentDate = _currentDate.AddMonths(-1);
            UpdateCalendarDisplay();
        }

        private void btnNextMonth_Click(object sender, RoutedEventArgs e)
        {
            _currentDate = _currentDate.AddMonths(1);
            UpdateCalendarDisplay();
        }

        private void MainCalendar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainCalendar.SelectedDate.HasValue)
            {
                LoadRendezVousForDate(MainCalendar.SelectedDate.Value);
            }
        }

        private void btnAddRendezVous_Click(object sender, RoutedEventArgs e)
        {
            var selectedDate = MainCalendar.SelectedDate ?? DateTime.Today;
            ShowRendezVousDialog(selectedDate);
        }

        private void LoadRendezVousForDate(DateTime date)
        {
            RendezVousList.Children.Clear();

            var rendezVous = _context.RendezVous
                .Where(r => r.DateDebut.Date == date.Date && r.UserId == _currentUser.Id)
                .OrderBy(r => r.DateDebut)
                .ToList();

            if (!rendezVous.Any())
            {
                var noRendezVousText = new TextBlock
                {
                    Text = "Aucun rendez-vous pour cette date",
                    FontSize = 14,
                    Foreground = new SolidColorBrush(Colors.Gray),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 20, 0, 0)
                };
                RendezVousList.Children.Add(noRendezVousText);
            }
            else
            {
                foreach (var rdv in rendezVous)
                {
                    var rdvCard = CreateRendezVousCard(rdv);
                    RendezVousList.Children.Add(rdvCard);
                }
            }

            UpdateStats(date, rendezVous.Count);
        }

        private Border CreateRendezVousCard(RendezVous rdv)
        {
            var card = new Border
            {
                Background = new SolidColorBrush(Colors.White),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Margin = new Thickness(0, 0, 0, 10),
                Padding = new Thickness(15)
            };

            var stackPanel = new StackPanel();

            // Titre
            var titreText = new TextBlock
            {
                Text = rdv.Titre,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.Black),
                Margin = new Thickness(0, 0, 0, 5)
            };
            stackPanel.Children.Add(titreText);

            // Heures
            var heuresText = new TextBlock
            {
                Text = $"{rdv.DateDebut:HH:mm} - {rdv.DateFin:HH:mm}",
                FontSize = 14,
                Foreground = new SolidColorBrush(Colors.DarkBlue),
                Margin = new Thickness(0, 0, 0, 5)
            };
            stackPanel.Children.Add(heuresText);

            // Description
            if (!string.IsNullOrEmpty(rdv.Description))
            {
                var descText = new TextBlock
                {
                    Text = rdv.Description,
                    FontSize = 12,
                    Foreground = new SolidColorBrush(Colors.Gray),
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 5)
                };
                stackPanel.Children.Add(descText);
            }

            // Lieu
            if (!string.IsNullOrEmpty(rdv.Lieu))
            {
                var lieuText = new TextBlock
                {
                    Text = $"📍 {rdv.Lieu}",
                    FontSize = 12,
                    Foreground = new SolidColorBrush(Colors.DarkGreen),
                    Margin = new Thickness(0, 0, 0, 5)
                };
                stackPanel.Children.Add(lieuText);
            }

            // Client
            if (!string.IsNullOrEmpty(rdv.Client))
            {
                var clientText = new TextBlock
                {
                    Text = $"👤 {rdv.Client}",
                    FontSize = 12,
                    Foreground = new SolidColorBrush(Colors.DarkOrange),
                    Margin = new Thickness(0, 0, 0, 5)
                };
                stackPanel.Children.Add(clientText);
            }

            // Statut
            var statutText = new TextBlock
            {
                Text = $"📋 {rdv.Statut}",
                FontSize = 12,
                Foreground = GetStatutColor(rdv.Statut),
                Margin = new Thickness(0, 0, 0, 10)
            };
            stackPanel.Children.Add(statutText);

            // Boutons d'action
            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal };
            
                         var editButton = new Button
             {
                 Content = "✏️ Modifier",
                 Background = new SolidColorBrush(Colors.Orange),
                 Foreground = new SolidColorBrush(Colors.White),
                 Padding = new Thickness(10, 5, 10, 5),
                 Margin = new Thickness(0, 0, 5, 0),
                 BorderThickness = new Thickness(0)
             };
            editButton.Click += (s, e) => EditRendezVous(rdv);
            buttonPanel.Children.Add(editButton);

                         var deleteButton = new Button
             {
                 Content = "🗑️ Supprimer",
                 Background = new SolidColorBrush(Colors.Red),
                 Foreground = new SolidColorBrush(Colors.White),
                 Padding = new Thickness(10, 5, 10, 5),
                 BorderThickness = new Thickness(0)
             };
            deleteButton.Click += (s, e) => DeleteRendezVous(rdv);
            buttonPanel.Children.Add(deleteButton);

            stackPanel.Children.Add(buttonPanel);
            card.Child = stackPanel;

            return card;
        }

        private SolidColorBrush GetStatutColor(string? statut)
        {
            return statut switch
            {
                "Confirmé" => new SolidColorBrush(Colors.Green),
                "Annulé" => new SolidColorBrush(Colors.Red),
                "En attente" => new SolidColorBrush(Colors.Orange),
                _ => new SolidColorBrush(Colors.Gray)
            };
        }

        private void UpdateStats(DateTime date, int count)
        {
            var totalRendezVous = _context.RendezVous.Count(r => r.UserId == _currentUser.Id);
            var todayRendezVous = _context.RendezVous.Count(r => r.DateDebut.Date == DateTime.Today && r.UserId == _currentUser.Id);
            var thisWeekRendezVous = _context.RendezVous.Count(r => 
                r.DateDebut >= DateTime.Today && r.DateDebut <= DateTime.Today.AddDays(7) && r.UserId == _currentUser.Id);

            txtStats.Text = $"📅 {date:dd/MM/yyyy}: {count} rendez-vous\n" +
                           $"📊 Total: {totalRendezVous} rendez-vous\n" +
                           $"📋 Aujourd'hui: {todayRendezVous} rendez-vous\n" +
                           $"📅 Cette semaine: {thisWeekRendezVous} rendez-vous";
        }

        private void ShowRendezVousDialog(DateTime selectedDate)
        {
            var dialog = new RendezVousDialog(selectedDate);
            if (dialog.ShowDialog() == true)
            {
                var newRendezVous = dialog.RendezVous;
                newRendezVous.UserId = _currentUser.Id;
                _context.RendezVous.Add(newRendezVous);
                _context.SaveChanges();
                LoadRendezVousForDate(selectedDate);
            }
        }

        private void EditRendezVous(RendezVous rdv)
        {
            var dialog = new RendezVousDialog(rdv.DateDebut, rdv);
            if (dialog.ShowDialog() == true)
            {
                var updatedRendezVous = dialog.RendezVous;
                rdv.Titre = updatedRendezVous.Titre;
                rdv.Description = updatedRendezVous.Description;
                rdv.DateDebut = updatedRendezVous.DateDebut;
                rdv.DateFin = updatedRendezVous.DateFin;
                rdv.Lieu = updatedRendezVous.Lieu;
                rdv.Client = updatedRendezVous.Client;
                rdv.Statut = updatedRendezVous.Statut;
                rdv.DateModification = DateTime.Now;

                _context.SaveChanges();
                LoadRendezVousForDate(rdv.DateDebut);
            }
        }

        private void DeleteRendezVous(RendezVous rdv)
        {
            var result = MessageBox.Show(
                $"Êtes-vous sûr de vouloir supprimer le rendez-vous '{rdv.Titre}' ?",
                "Confirmation de suppression",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _context.RendezVous.Remove(rdv);
                _context.SaveChanges();
                LoadRendezVousForDate(rdv.DateDebut);
            }
        }
    }
}
