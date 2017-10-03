using events.dal.Models;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace events.parser.Parsers
{
    public interface IParser
    {
        // parse html into events
        // return true if the parser has handled the document
        //bool Parse(string html, IList<Event> eventsList);
        bool Parse(HtmlDocument doc, string sourceUri, IList<Event> eventsList);
    }
}
