using System;

namespace Autosoft_Licensing.Services
{
    public interface IClock
    {
        DateTime UtcNow { get; }
    }
}
