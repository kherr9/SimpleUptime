using System;

namespace SimpleUptime.Domain.Models
{
    public class SlackAlertContact : IAlertContact
    {
        public SlackAlertContact(AlertContactId id, Uri webHookUrl)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            WebHookUrl = webHookUrl ?? throw new ArgumentNullException(nameof(webHookUrl));
        }

        public AlertContactId Id { get; }

        public Uri WebHookUrl { get; }
    }
}