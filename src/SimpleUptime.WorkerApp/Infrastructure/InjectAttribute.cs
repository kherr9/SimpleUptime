using System;
using Microsoft.Azure.WebJobs.Description;

namespace SimpleUptime.WorkerApp.Infrastructure
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class InjectAttribute : Attribute
    {
    }
}