using System;
using System.ComponentModel.DataAnnotations;

namespace Nest.Models
{
    public class User
    {
        //Primary Key
        public int Id { get; set; }

        //Username of the User
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Username can only contain letters and numbers.")]
        public string? UserName { get; set; }

        //Email of the User
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        //Password of the User
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long and cannot exceed 100 characters.")]
        public string? Password { get; set; }  //Store only hashed password
    }
}
