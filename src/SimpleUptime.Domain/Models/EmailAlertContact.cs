namespace SimpleUptime.Domain.Models
{
    public class EmailAlertContact : IAlertContact
    {
        public AlertContactId Id { get; set; }

        public string Email { get; set; }
    }
}