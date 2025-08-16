using System.ComponentModel.DataAnnotations;

namespace TP_2.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? Nom { get; set; }
        
        [MaxLength(100)]
        public string? Prenom { get; set; }
        
        [MaxLength(20)]
        public string? Telephone { get; set; }
        
        public DateTime DateCreation { get; set; } = DateTime.Now;
        
        public DateTime? DerniereConnexion { get; set; }
        
        public bool EstActif { get; set; } = true;
        
        // Relation avec les rendez-vous (cascade delete)
        public virtual ICollection<RendezVous> RendezVous { get; set; } = new List<RendezVous>();
    }
}
