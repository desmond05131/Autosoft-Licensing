using System;
using Autosoft_Licensing.Services;

namespace Autosoft_Licensing.Services
{
    public sealed class SystemClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}