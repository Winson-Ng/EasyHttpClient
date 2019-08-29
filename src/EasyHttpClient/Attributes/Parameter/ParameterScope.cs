using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyHttpClient.Attributes
{
    public enum ParameterScope
    {
        Query = 0,
        Path = 2,
        Header = 3,
        Cookie = 4,
        Form = 5,
        Json = 6,
        RawContent = 7
    }
}
