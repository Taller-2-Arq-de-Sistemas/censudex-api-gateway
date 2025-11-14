
using System.ComponentModel.DataAnnotations;

namespace translator.src.Dtos.Clients
{
    public class CreateUserRequestDto
    {
        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        /// <value>
        /// The first name of the user. Required field with maximum length of 100 characters.
        /// </value>
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        /// <value>
        /// The last name of the user. Required field with maximum length of 100 characters.
        /// </value>
        [Required, MaxLength(100)]
        public string LastName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        /// <value>
        /// The email address of the user. Must be a valid email format and belong to the censudex.cl domain.
        /// </value>
        /// <remarks>
        /// The email address must follow the pattern: username@censudex.cl
        /// </remarks>
        [Required, EmailAddress]
        [RegularExpression(@"^[^@]+@censudex\.cl$", ErrorMessage = "Email debe pertenecer a @censudex.cl")]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user's username.
        /// </summary>
        /// <value>
        /// The username for the user account. Must be between 4 and 50 characters long.
        /// </value>
        [Required, MinLength(4), MaxLength(50)]
        public string Username { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user's date of birth.
        /// </summary>
        /// <value>
        /// The birth date of the user. The user must be at least 18 years old.
        /// </value>
        /// <remarks>
        /// Validated using the MinimumAge attribute to ensure the user is 18 years or older.
        /// </remarks>
        [Required]
        public DateOnly BirthDate { get; set; }

        /// <summary>
        /// Gets or sets the user's physical address.
        /// </summary>
        /// <value>
        /// The complete physical address of the user. This is a required field.
        /// </value>
        [Required]
        public string Address { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user's phone number.
        /// </summary>
        /// <value>
        /// The phone number of the user. Must be a valid Chilean phone number format starting with +56.
        /// </value>
        /// <remarks>
        /// Expected format: +56 followed by 9 digits where the first digit after country code is 2-9.
        /// Example: +56911223344
        /// </remarks>
        [Required]
        [RegularExpression(@"^\+56[2-9]\d{8}$", ErrorMessage = "Número telefónico no válido. Formato esperado: +56911223344")]
        public string PhoneNumber { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user's phone number.
        /// </summary>
        /// <value>
        /// The phone number of the user. Must be a valid Chilean phone number format starting with +56.
        /// </value>
        /// <remarks>
        /// Expected format: +56 followed by 9 digits where the first digit after country code is 2-9.
        /// Example: +56911223344
        /// </remarks>
        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).+$",
            ErrorMessage = "La contraseña debe incluir al menos una letra mayúscula, una letra minúscula, un número y un carácter especial.")]
        public string Password { get; set; } = null!;


    }
}