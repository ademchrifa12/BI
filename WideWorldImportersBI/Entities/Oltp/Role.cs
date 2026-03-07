using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WideWorldImportersBI.Entities.Oltp;

/// <summary>
/// Represents a role in the BI Platform authentication system
/// This is a custom table for JWT authentication with role-based access control
/// </summary>
[Table("BiRoles", Schema = "Application")]
public class Role
{
    [Key]
    [Column("RoleID")]
    public int RoleId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("RoleName")]
    public string RoleName { get; set; } = string.Empty;

    [MaxLength(500)]
    [Column("Description")]
    public string? Description { get; set; }

    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
