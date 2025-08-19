using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TP_2.Data;
using TP_2.Models;
using TP_2.Dialogs;

namespace TP_2.Pages
{
	public partial class CalendrierPage : Page
	{
		private ApplicationDbContext _context;
		private DateTime _currentDate = DateTime.Today;
		private User _currentUser;
		private DateTime _selectedDate = DateTime.Today;

		public CalendrierPage(User currentUser)
		{
			InitializeComponent();
			_currentUser = currentUser;
			InitializeDatabase();
			BuildCalendar();
			UpdateCalendarDisplay();
			LoadRendezVousForDate(_currentDate);
		}

		// Initialisation de la base de données
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

		// Construction du calendrier
		private void BuildCalendar()
		{
			CalendarGrid.Children.Clear();
			
			// Créer 42 boutons (6 semaines x 7 jours)
			for (int i = 0; i < 42; i++)
			{
				Button dayButton = new Button
				{
					Margin = new Thickness(1),
					Background = new SolidColorBrush(Colors.Transparent),
					BorderBrush = new SolidColorBrush(Colors.LightGray),
					BorderThickness = new Thickness(1),
					FontSize = 12,
					Tag = i, // Index pour identifier le bouton
					HorizontalAlignment = HorizontalAlignment.Stretch,
					VerticalAlignment = VerticalAlignment.Stretch
				};
				
				dayButton.Click += DayButton_Click;
				CalendarGrid.Children.Add(dayButton);
			}
		}

		// Mise à jour de l'affichage du calendrier
		private void UpdateCalendarDisplay()
		{
			txtCurrentMonth.Text = _currentDate.ToString("MMMM yyyy");
			UpdateCalendarDays();
		}

		// Mise à jour des jours du calendrier
		private void UpdateCalendarDays()
		{
			// Obtenir le premier jour du mois
			DateTime firstDayOfMonth = new DateTime(_currentDate.Year, _currentDate.Month, 1);
			
			// Obtenir le jour de la semaine du premier jour (0 = Dimanche, 1 = Lundi, etc.)
			int firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
			
			// Obtenir le nombre de jours dans le mois
			int daysInMonth = DateTime.DaysInMonth(_currentDate.Year, _currentDate.Month);
			
			// Obtenir le dernier jour du mois précédent
			DateTime lastDayOfPreviousMonth = firstDayOfMonth.AddDays(-1);
			int daysInPreviousMonth = DateTime.DaysInMonth(lastDayOfPreviousMonth.Year, lastDayOfPreviousMonth.Month);

			// Obtenir les dates avec des rendez-vous pour le mois actuel
			var datesWithRendezVous = GetDatesWithRendezVous();

			// Mettre à jour chaque bouton
			for (int i = 0; i < 42; i++)
			{
				Button dayButton = (Button)CalendarGrid.Children[i];
				
				// Calculer la date correspondante
				DateTime buttonDate;
				if (i < firstDayOfWeek)
				{
					// Jours du mois précédent
					int day = daysInPreviousMonth - firstDayOfWeek + i + 1;
					buttonDate = new DateTime(lastDayOfPreviousMonth.Year, lastDayOfPreviousMonth.Month, day);
					dayButton.Content = day.ToString();
					dayButton.Foreground = new SolidColorBrush(Colors.LightGray);
					dayButton.Background = new SolidColorBrush(Colors.Transparent);
				}
				else if (i >= firstDayOfWeek && i < firstDayOfWeek + daysInMonth)
				{
					// Jours du mois actuel
					int day = i - firstDayOfWeek + 1;
					buttonDate = new DateTime(_currentDate.Year, _currentDate.Month, day);
					dayButton.Content = day.ToString();
					
					// Vérifier si la date a des rendez-vous
					bool hasRendezVous = datesWithRendezVous.Contains(buttonDate.Date);
					
					// Mettre en surbrillance selon les priorités
					if (buttonDate.Date == _selectedDate.Date)
					{
						// Date sélectionnée - priorité la plus haute
						dayButton.Background = new SolidColorBrush(Colors.LightBlue);
						dayButton.BorderBrush = new SolidColorBrush(Colors.Blue);
						dayButton.Foreground = new SolidColorBrush(Colors.Black);
					}
					else if (buttonDate.Date == DateTime.Today.Date)
					{
						// Date d'aujourd'hui
						if (hasRendezVous)
						{
							dayButton.Background = new SolidColorBrush(Colors.LightGreen);
							dayButton.BorderBrush = new SolidColorBrush(Colors.Green);
						}
						else
						{
							dayButton.Background = new SolidColorBrush(Colors.LightYellow);
							dayButton.BorderBrush = new SolidColorBrush(Colors.Orange);
						}
						dayButton.Foreground = new SolidColorBrush(Colors.Black);
					}
					else if (hasRendezVous)
					{
						// Date avec rendez-vous
						dayButton.Background = new SolidColorBrush(Colors.LightPink);
						dayButton.BorderBrush = new SolidColorBrush(Colors.HotPink);
						dayButton.Foreground = new SolidColorBrush(Colors.Black);
					}
					else
					{
						// Date normale
						dayButton.Background = new SolidColorBrush(Colors.White);
						dayButton.BorderBrush = new SolidColorBrush(Colors.LightGray);
						dayButton.Foreground = new SolidColorBrush(Colors.Black);
					}
				}
				else
				{
					// Jours du mois suivant
					int day = i - firstDayOfWeek - daysInMonth + 1;
					buttonDate = new DateTime(_currentDate.AddMonths(1).Year, _currentDate.AddMonths(1).Month, day);
					dayButton.Content = day.ToString();
					dayButton.Foreground = new SolidColorBrush(Colors.LightGray);
					dayButton.Background = new SolidColorBrush(Colors.Transparent);
				}
				
				dayButton.Tag = buttonDate; // Stocker la date dans le Tag
			}
		}

		// Récupération des dates avec rendez-vous
		private HashSet<DateTime> GetDatesWithRendezVous()
		{
			// Obtenir le premier et dernier jour du mois affiché
			DateTime firstDayOfMonth = new DateTime(_currentDate.Year, _currentDate.Month, 1);
			DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
			
			// Récupérer toutes les dates avec des rendez-vous pour ce mois
			var datesWithRendezVous = _context.RendezVous
				.Where(r => r.DateDebut.Date >= firstDayOfMonth.Date && 
							r.DateDebut.Date <= lastDayOfMonth.Date && 
							r.UserId == _currentUser.Id)
				.Select(r => r.DateDebut.Date)
				.Distinct()
				.ToHashSet();
				
			return datesWithRendezVous;
		}

		// Gestion du clic sur un jour du calendrier
		private void DayButton_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button button && button.Tag is DateTime date)
			{
				_selectedDate = date;
				UpdateCalendarDays();
				LoadRendezVousForDate(date);
			}
		}

		// Gestion des boutons de navigation du calendrier
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

		private void btnAddRendezVous_Click(object sender, RoutedEventArgs e)
		{
			ShowRendezVousDialog(_selectedDate);
		}

		// Chargement des rendez-vous pour une date donnée
		private void LoadRendezVousForDate(DateTime date)
		{
			RendezVousList.Children.Clear();

			var rendezVous = _context.RendezVous
				.Where(r => r.DateDebut.Date == date.Date && r.UserId == _currentUser.Id)
				.OrderBy(r => r.DateDebut)
				.ToList();

			if (!rendezVous.Any())
			{
				TextBlock noRendezVousText = new TextBlock
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

		// Création de la carte de rendez-vous
		private Border CreateRendezVousCard(RendezVous rdv)
		{
			Border card = new Border
			{
				Background = new SolidColorBrush(Colors.White),
				BorderBrush = new SolidColorBrush(Colors.LightGray),
				BorderThickness = new Thickness(1),
				CornerRadius = new CornerRadius(8),
				Margin = new Thickness(0, 0, 0, 10),
				Padding = new Thickness(15)
			};

			StackPanel stackPanel = new StackPanel();

			// Titre
			TextBlock titreText = new TextBlock
			{
				Text = rdv.Titre,
				FontSize = 16,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Colors.Black),
				Margin = new Thickness(0, 0, 0, 5)
			};
			stackPanel.Children.Add(titreText);

			// Heures
			TextBlock heuresText = new TextBlock
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
				TextBlock descText = new TextBlock
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
				TextBlock lieuText = new TextBlock
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
				TextBlock clientText = new TextBlock
				{
					Text = $"👤 {rdv.Client}",
					FontSize = 12,
					Foreground = new SolidColorBrush(Colors.DarkOrange),
					Margin = new Thickness(0, 0, 0, 5)
				};
				stackPanel.Children.Add(clientText);
			}

			// Statut
			TextBlock statutText = new TextBlock
			{
				Text = $"📋 {rdv.Statut}",
				FontSize = 12,
				Foreground = GetStatutColor(rdv.Statut),
				Margin = new Thickness(0, 0, 0, 10)
			};
			stackPanel.Children.Add(statutText);

			// Boutons d'action
			StackPanel buttonPanel = new StackPanel { Orientation = Orientation.Horizontal };

			Button editButton = new Button
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

			Button deleteButton = new Button
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

		// Récupération de la couleur du statut
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

		// Mise à jour des statistiques
		private void UpdateStats(DateTime date, int count)
		{
			int totalRendezVous = _context.RendezVous.Count(r => r.UserId == _currentUser.Id);
			int todayRendezVous = _context.RendezVous.Count(r => r.DateDebut.Date == DateTime.Today && r.UserId == _currentUser.Id);
			int thisWeekRendezVous = _context.RendezVous.Count(r => 
				r.DateDebut >= DateTime.Today && r.DateDebut <= DateTime.Today.AddDays(7) && r.UserId == _currentUser.Id);

			txtStats.Text = $"📅 {date:dd/MM/yyyy}: {count} rendez-vous\n" +
							$"📊 Total: {totalRendezVous} rendez-vous\n" +
							$"📋 Aujourd'hui: {todayRendezVous} rendez-vous\n" +
							$"📅 Cette semaine: {thisWeekRendezVous} rendez-vous";
		}

		// Affichage du dialogue de rendez-vous
		private void ShowRendezVousDialog(DateTime selectedDate)
		{
			RendezVousDialog dialog = new RendezVousDialog(selectedDate);
			if (dialog.ShowDialog() == true)
			{
				RendezVous newRendezVous = dialog.RendezVous;
				newRendezVous.UserId = _currentUser.Id;
				_context.RendezVous.Add(newRendezVous);
				_context.SaveChanges();
				LoadRendezVousForDate(selectedDate);
				UpdateCalendarDays(); // Rafraîchir l'affichage du calendrier
			}
		}

		// Modification d'un rendez-vous
		private void EditRendezVous(RendezVous rdv)
		{
			RendezVousDialog dialog = new RendezVousDialog(rdv.DateDebut, rdv);
			if (dialog.ShowDialog() == true)
			{
				RendezVous updatedRendezVous = dialog.RendezVous;
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
				UpdateCalendarDays(); // Rafraîchir l'affichage du calendrier
			}
		}
	
		// Suppression d'un rendez-vous
		private void DeleteRendezVous(RendezVous rdv)
		{
			MessageBoxResult result = MessageBox.Show(
				$"Êtes-vous sûr de vouloir supprimer le rendez-vous '{rdv.Titre}' ?",
				"Confirmation de suppression",
				MessageBoxButton.YesNo,
				MessageBoxImage.Question);

			if (result == MessageBoxResult.Yes)
			{
				_context.RendezVous.Remove(rdv);
				_context.SaveChanges();
				LoadRendezVousForDate(rdv.DateDebut);
				UpdateCalendarDays(); // Rafraîchir l'affichage du calendrier
			}
		}
	}
}
