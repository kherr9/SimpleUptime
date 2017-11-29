using System;
using Microsoft.Azure.WebJobs.Description;

namespace SimpleUptime.FuncApp.Infrastructure
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter)]
    public class InjectAttribute : Attribute
    {
    }
}