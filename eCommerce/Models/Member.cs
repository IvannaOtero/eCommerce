using System.ComponentModel.DataAnnotations;

namespace eCommerce.Models;

/// <summary>
/// Represents an individual website user
/// </summary>
public class Member
{
    /// <summary>
    /// Unique identifier for the member
    /// </summary>
    [Key]
    public int MemberId { get; set; }


    /// <summary>
    /// Public facing username for the member.
    /// Alphanumeric characters only
    /// </summary>
    [RegularExpression("^[a-zA-Z0-9]+$", 
        ErrorMessage ="Username must be alphanumeric only")]
    [StringLength(25)]
    public required string Username { get; set; }


    /// <summary>
    /// Email of the Member 
    /// </summary>
    public required string Email { get; set; }


    /// <summary>
    /// The Member's password
    /// </summary>
    [StringLength(50, MinimumLength = 6,
        ErrorMessage = "Your password must be between 6 and 50 characters")]
    public required string Password { get; set; }


    /// <summary>
    /// The date of birth of the Member
    /// </summary>
    public DateOnly DateOfBirth { get; set; }
}
