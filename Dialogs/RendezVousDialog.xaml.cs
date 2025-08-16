using System;
using System.Windows;
using System.Windows.Controls;
using TP_2.Models;

namespace TP_2.Dialogs
{
    public partial class RendezVousDialog : Window
    {
        public RendezVous ?RendezVous { get; private set; }
        private bool _isEditMode;

        public RendezVousDialog(DateTime selectedDate, RendezVous? existingRendezVous = null)
        {
            InitializeComponent();
            InitializeComboBoxes();
            
            if (existingRendezVous != null)
            {
                _isEditMode = true;
                Title = "Modifier le rendez-vous";
                LoadRendezVous(existingRendezVous);
            }
            else
            {
                _isEditMode = false;
                Title = "Nouveau rendez-vous";
                InitializeDefaultValues(selectedDate);
            }
        }

        private void InitializeComboBoxes()
        {
            // Heures (0-23)
            for (int i = 0; i < 24; i++)
            {
                cmbHeureDebut.Items.Add(i.ToString("00"));
                cmbHeureFin.Items.Add(i.ToString("00"));
            }

            // Minutes (0-59)
            for (int i = 0; i < 60; i += 15)
            {
                cmbMinuteDebut.Items.Add(i.ToString("00"));
                cmbMinuteFin.Items.Add(i.ToString("00"));
            }

            // Valeurs par défaut
            cmbHeureDebut.SelectedIndex = 9; // 09h
            cmbMinuteDebut.SelectedIndex = 0; // 00
            cmbHeureFin.SelectedIndex = 10; // 10h
            cmbMinuteFin.SelectedIndex = 0; // 00
        }

        private void InitializeDefaultValues(DateTime selectedDate)
        {
            dpDateDebut.SelectedDate = selectedDate;
            dpDateFin.SelectedDate = selectedDate;
        }

        private void LoadRendezVous(RendezVous rdv)
        {
            txtTitre.Text = rdv.Titre;
            txtDescription.Text = rdv.Description ?? "";
            dpDateDebut.SelectedDate = rdv.DateDebut.Date;
            dpDateFin.SelectedDate = rdv.DateFin.Date;
            txtLieu.Text = rdv.Lieu ?? "";
            txtClient.Text = rdv.Client ?? "";

            // Heures de début
            cmbHeureDebut.SelectedItem = rdv.DateDebut.Hour.ToString("00");
            cmbMinuteDebut.SelectedItem = rdv.DateDebut.Minute.ToString("00");

            // Heures de fin
            cmbHeureFin.SelectedItem = rdv.DateFin.Hour.ToString("00");
            cmbMinuteFin.SelectedItem = rdv.DateFin.Minute.ToString("00");

            // Statut
            foreach (ComboBoxItem item in cmbStatut.Items)
            {
                if (item.Content.ToString() == rdv.Statut)
                {
                    cmbStatut.SelectedItem = item;
                    break;
                }
            }
        }

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                var dateDebut = GetDateTimeFromPickers(dpDateDebut, cmbHeureDebut, cmbMinuteDebut);
                var dateFin = GetDateTimeFromPickers(dpDateFin, cmbHeureFin, cmbMinuteFin);

                if (_isEditMode)
                {
                    // Mode édition - on met à jour l'objet existant
                    RendezVous = new RendezVous
                    {
                        Id = 0, // Sera ignoré lors de la mise à jour
                        Titre = txtTitre.Text.Trim(),
                        Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text.Trim(),
                        DateDebut = dateDebut,
                        DateFin = dateFin,
                        Lieu = string.IsNullOrWhiteSpace(txtLieu.Text) ? null : txtLieu.Text.Trim(),
                        Client = string.IsNullOrWhiteSpace(txtClient.Text) ? null : txtClient.Text.Trim(),
                        Statut = (cmbStatut.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Confirmé",
                        DateCreation = DateTime.Now,
                        DateModification = DateTime.Now
                    };
                }
                else
                {
                    // Mode création - on crée un nouvel objet
                    RendezVous = new RendezVous
                    {
                        Titre = txtTitre.Text.Trim(),
                        Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text.Trim(),
                        DateDebut = dateDebut,
                        DateFin = dateFin,
                        Lieu = string.IsNullOrWhiteSpace(txtLieu.Text) ? null : txtLieu.Text.Trim(),
                        Client = string.IsNullOrWhiteSpace(txtClient.Text) ? null : txtClient.Text.Trim(),
                        Statut = (cmbStatut.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Confirmé",
                        DateCreation = DateTime.Now
                    };
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la sauvegarde : {ex.Message}", 
                              "Erreur", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
            }
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateForm()
        {
            // Validation du titre
            if (string.IsNullOrWhiteSpace(txtTitre.Text))
            {
                MessageBox.Show("Le titre est obligatoire.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTitre.Focus();
                return false;
            }

            // Validation des dates
            if (!dpDateDebut.SelectedDate.HasValue)
            {
                MessageBox.Show("La date de début est obligatoire.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpDateDebut.Focus();
                return false;
            }

            if (!dpDateFin.SelectedDate.HasValue)
            {
                MessageBox.Show("La date de fin est obligatoire.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpDateFin.Focus();
                return false;
            }

            // Validation des heures
            if (cmbHeureDebut.SelectedItem == null || cmbMinuteDebut.SelectedItem == null)
            {
                MessageBox.Show("L'heure de début est obligatoire.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbHeureDebut.Focus();
                return false;
            }

            if (cmbHeureFin.SelectedItem == null || cmbMinuteFin.SelectedItem == null)
            {
                MessageBox.Show("L'heure de fin est obligatoire.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbHeureFin.Focus();
                return false;
            }

            // Validation de la cohérence des dates/heures
            var dateDebut = GetDateTimeFromPickers(dpDateDebut, cmbHeureDebut, cmbMinuteDebut);
            var dateFin = GetDateTimeFromPickers(dpDateFin, cmbHeureFin, cmbMinuteFin);

            if (dateFin <= dateDebut)
            {
                MessageBox.Show("La date/heure de fin doit être postérieure à la date/heure de début.", 
                              "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private DateTime GetDateTimeFromPickers(DatePicker datePicker, ComboBox hourComboBox, ComboBox minuteComboBox)
        {
            var date = datePicker.SelectedDate ?? DateTime.Today;
            var hour = int.Parse(hourComboBox.SelectedItem.ToString() ?? "0");
            var minute = int.Parse(minuteComboBox.SelectedItem.ToString() ?? "0");

            return new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);
        }
    }
}
