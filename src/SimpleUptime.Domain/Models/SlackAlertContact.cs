using System;

namespace SimpleUptime.Domain.Models
{
    public class SlackAlertContact : IAlertContact
    {
        public AlertContactId Id { get; set; }

        public Uri WebHookUrl { get; set; }
    }
}