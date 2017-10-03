using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace events.parser.Parsers.MicroData
{
    public abstract class MicrodataBase
    {
        public string Name { get; set; }

        public abstract string ToJSON();
    }
}
