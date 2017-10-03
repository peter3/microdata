using events.parser.Engine;
using events.parser.Parsers;
using events.parser.Parsers.MicroData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace events.web.Controllers
{
    [EnableCors(AllowedOrigin = @"*")]
    public class ParseController : ApiController
    {
        
        public object Get()
        {
            return new
            {
                urls = new string[] {
                    "https://www.evenemang.se/malmö/",
                    "https://www.evenemang.se/lund/",
                    "https://evenemang.vellinge.se",
                    "https://evenemang2.malmo.se/",
                    "http://www2.visitlund.se/sv/evenemang/start" },
                events = new object[0]
            };
        }

        public object Post([FromBody]string[] urls)
        {
            var events = new ParserEngine().Parse(urls, new List<IParser>() { new MicroDataParser() });
            return new
            {
                urls = urls,
                events = events
            };
        }
    }
}
