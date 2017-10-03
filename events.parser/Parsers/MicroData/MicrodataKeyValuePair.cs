using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace events.parser.Parsers.MicroData
{
    public class MicrodataKeyValuePair : MicrodataBase
    {
        public string Value { get; set; }

        public override string ToJSON()
        {
            return "\"" + Name + "\": \"" + Value + "\"";
        }
    }
}
