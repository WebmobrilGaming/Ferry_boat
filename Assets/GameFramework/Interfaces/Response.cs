using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GF
{
    public interface Response
    {
        long status { get; }
        string message { get; }
    }
}