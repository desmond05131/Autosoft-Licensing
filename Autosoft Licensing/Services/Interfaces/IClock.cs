using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autosoft_Licensing.Services.Interfaces
{
    public interface IClock
    {
        DateTime UtcNow { get; }
    }
}
