using System;

namespace SimpleUptime.Domain.Models
{
    public class EmailAlertContact : IAlertContact
    {
        public EmailAlertContact(AlertContactId id, string email)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }

        public AlertContactId Id { get; }

        public string Email { get; }
    }
}