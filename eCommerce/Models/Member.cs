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
    public required string Username { get; set; }


    /// <summary>
    /// Email of the Member 
    /// </summary>
    public required string Email { get; set; }


    /// <summary>
    /// The Member's password
    /// </summary>
    public required string Password { get; set; }


    /// <summary>
    /// The date of birth of the Member
    /// </summary>
    public DateOnly DateOfBirth { get; set; }
}
