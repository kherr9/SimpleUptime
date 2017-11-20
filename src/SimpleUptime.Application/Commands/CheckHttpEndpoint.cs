﻿using System;
using SimpleUptime.Domain.Models;

namespace SimpleUptime.Application.Commands
{
    public class CheckHttpEndpoint
    {
        public HttpMonitorId HttpMonitorId { get; set; }

        public Uri Url { get; set; }
    }
}