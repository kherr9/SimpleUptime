using System;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.Application.Exceptions
{
    public class EntityNotFoundException : ApplicationException
    {
        public EntityNotFoundException(string message) : base(message)
        {
        }

        public EntityNotFoundException(HttpMonitorId httpMonitorId) :
            this($"Unknown {nameof(HttpMonitor)} with id {httpMonitorId}")
        {
        }
    }
}