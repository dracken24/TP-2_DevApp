using System.ComponentModel.DataAnnotations;

namespace TP_2_Developpement_Application_Burreau.Models
{
    public class RendezVous
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Titre { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [Required]
        public DateTime DateDebut { get; set; }
        
        [Required]
        public DateTime DateFin { get; set; }
        
        [MaxLength(100)]
        public string? Lieu { get; set; }
        
        [MaxLength(100)]
        public string? Client { get; set; }
        
        [MaxLength(20)]
        public string? Statut { get; set; } = "Confirmé"; // Confirmé, Annulé, En attente
        
        public DateTime DateCreation { get; set; } = DateTime.Now;
        
        public DateTime? DateModification { get; set; }
        
        // Relation avec l'utilisateur
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
