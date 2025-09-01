﻿using System;

namespace OtoKiralama.Domain.Entities
{
    public class ApplicationUser
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
