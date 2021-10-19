using System;

namespace APIFrame.Core.DTOs
{
    public class BaseUserDTO
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public int Balance { get; set; }

        public DateTime RegistrationDate { get; set; }

        public string AuthIP { get; set; }

        public DateTime AuthDate { get; set; }

        public bool IsBanned { get; set; }
    }
}
