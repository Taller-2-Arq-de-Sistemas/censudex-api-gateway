using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace translator.src.Dtos.Clients
{
    public class VerifyCredentialsDto
    {
        /// <summary>
        /// The client's email address (optional if username is provided).
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// The client's username (optional if email is provided).
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// The client's password (required).
        /// </summary>
        public string Password { get; set; } = string.Empty;

    }
}