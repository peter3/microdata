using events.dal.Models;
using events.parser.Parsers;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace events.parser.Engine
{
    public class ParserEngine
    {
        // call each parser for each url, collect the results 
        // as a list of events and return it
        public IEnumerable<Event> Parse(IEnumerable<string> urls, IEnumerable<IParser> parsers, List<string> exceptionLogger = null)
        {
            var events = new List<Event>();
            foreach (var url in urls)
            {
                if (!string.IsNullOrWhiteSpace(url))
                {
                    try
                    {
                        var doc = LoadHtml(url);
                        if (doc.DocumentNode != null)
                        {
                            foreach (var parser in parsers)
                                if (parser.Parse(doc, url, events))
                                    break;
                        }
                    }
                    catch (Exception e)
                    {
                        // TODO: better logging
                        if (exceptionLogger != null)
                            exceptionLogger.Add(e.ToString());
                    }
                }
            }
            return events;
        }

        #region
        private HtmlDocument LoadHtml(string url)
        {
            HtmlWeb web = new HtmlWeb();
            return web.Load(url);
        }

        #endregion
    }
}
