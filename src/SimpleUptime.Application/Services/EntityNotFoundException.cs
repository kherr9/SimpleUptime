using System;

namespace SimpleUptime.Application.Services
{
    public class EntityNotFoundException : ApplicationException
    {
        public EntityNotFoundException(string message) : base(message)
        {
        }
    }
}