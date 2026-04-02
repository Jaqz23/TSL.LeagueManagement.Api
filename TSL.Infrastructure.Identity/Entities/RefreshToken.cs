using System.ComponentModel.DataAnnotations;

namespace TSL.Infrastructure.Identity.Entities
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime Expires { get; set; }

        [Required]
        public bool IsExpired => DateTime.UtcNow >= Expires;

        [Required]
        public DateTime Created { get; set; }

        public string? CreatedByIp { get; set; }

        public DateTime? Revoked { get; set; }

        public string? RevokedByIp { get; set; }

        public string? ReplacedByToken { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;

        // Relacion con ApplicationUser
        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;

        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}
