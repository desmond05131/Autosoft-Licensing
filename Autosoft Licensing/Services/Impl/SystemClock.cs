using System;
using Autosoft_Licensing.Services;

namespace Autosoft_Licensing.Services
{
    public class SystemClock : IClock
    {
        public System.DateTime UtcNow => System.DateTime.UtcNow;
    }
}