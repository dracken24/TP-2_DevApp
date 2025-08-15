using System.Windows;
using System.Windows.Controls;

namespace TP_2_Developpement_Application_Burreau.Pages
{
    public partial class ParametresPage : Page
    {
        public ParametresPage()
        {
            //InitializeComponent();
        }

        private void btnSaveNow_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Sauvegarde effectuée avec succès !", "Sauvegarde", 
                           MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Êtes-vous sûr de vouloir restaurer les paramètres par défaut ?", 
                                        "Confirmation", 
                                        MessageBoxButton.YesNo, 
                                        MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                // Réinitialiser les contrôles
                ReinitControls();
                
                MessageBox.Show("Paramètres restaurés avec succès !", "Restauration", 
                               MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Cette action va réinitialiser tous les paramètres. Êtes-vous sûr ?", 
                                        "Attention", 
                                        MessageBoxButton.YesNo, 
                                        MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                // Réinitialiser les contrôles
                ReinitControls();

                MessageBox.Show("Tous les paramètres ont été réinitialisés !", "Réinitialisation", 
                               MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ReinitControls()
        {
            cmbTheme.SelectedIndex = 0;
            sliderFontSize.Value = 14;
            chkAnimations.IsChecked = true;
            chkNotifications.IsChecked = true;
            chkSound.IsChecked = true;
            chkStartup.IsChecked = false;
            cmbAutoSave.SelectedIndex = 1;
        }
    }
}
