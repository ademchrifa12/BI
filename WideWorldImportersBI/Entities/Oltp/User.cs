using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WideWorldImportersBI.Entities.Oltp;

/// <summary>
/// Represents a user in the BI Platform authentication system
/// This is a custom table for JWT authentication, not from WideWorldImporters
/// </summary>
[Table("BiUsers", Schema = "Application")]
public class User
{
    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("Username")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    [Column("Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    [Column("PasswordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("FirstName")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("LastName")]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(128)]
    [Column("FirebaseUid")]
    public string? FirebaseUid { get; set; }

    [Column("IsActive")]
    public bool IsActive { get; set; } = true;

    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("LastLoginAt")]
    public DateTime? LastLoginAt { get; set; }

    [Column("RoleID")]
    public int RoleId { get; set; }

    // Navigation properties
    [ForeignKey("RoleId")]
    public virtual Role? Role { get; set; }
}
