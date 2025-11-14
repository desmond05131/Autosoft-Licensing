using System;

namespace Autosoft_Licensing.Services
{
    public interface IClock
    {
        System.DateTime UtcNow { get; }
    }
}
