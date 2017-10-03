using events.parser.Engine;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using events.dal.Models;

namespace events.parser.Parsers.MicroData
{
    // https://microdata.codeplex.com/
    // parses microdata out of html
    public class MicroDataParser : IParser
    {
        private string mergeUrl(string baseUrl, string relativeUrl)
        {
            if (relativeUrl.StartsWith("http"))
                return relativeUrl;
            return new Uri(new Uri(baseUrl), relativeUrl).AbsoluteUri;
        }
        public bool Parse(HtmlDocument doc, string sourceUrl, IList<Event> eventsList)
        {
            var count = eventsList.Count;
            var items = Parse(doc, true);
            // TODO add items to eventlist
            var basePath = new Uri(new Uri(sourceUrl).GetLeftPart(UriPartial.Authority));
            //var basePath = new Uri(sourceUrl);
            foreach (var item in items.Where(t => t.ItemType.Any(u => u.EndsWith("Event"))))
            {

                var ev = new Event { SourceUri = sourceUrl, EventType = item.ItemType[0] };
                foreach (var attr in item.Properties.OfType<MicrodataKeyValuePair>())
                {
                    switch (attr.Name)
                    {
                        case "startDate":
                            ev.StartDate = attr.Value;
                            break;
                        case "endDate":
                            ev.EndDate = attr.Value;
                            break;
                        case "name":
                            ev.Name = attr.Value;
                            break;
                        case "description":
                            ev.Description = attr.Value;
                            break;
                        case "image":
                        case "photo":
                            ev.ImageUri = attr.Value;
                            break;
                        case "performer":
                            ev.Performer = attr.Value;
                            break;
                        case "url":
                            ev.Uri = mergeUrl(sourceUrl, attr.Value);
                            break;
                    }
                }
                // sub props (Place)
                var places = item.Properties.OfType<MicrodataItem>().Where(t => t.ItemType[0].EndsWith("Place"));
                foreach (var attr in places)
                {
                    var mp = attr.Properties.OfType<MicrodataKeyValuePair>();

                    foreach (var p in mp)
                    {
                        switch (p.Name)
                        {
                            case "name":
                                ev.LocationName = p.Value;
                                break;
                            case "url":
                                ev.LocationUri = mergeUrl(sourceUrl, p.Value);
                                break;
                        }
                    }
                    mp = attr.Properties.OfType<MicrodataItem>().Where(t => t.ItemType.Any(u => u.EndsWith("PostalAddress"))).SelectMany(t => t.Properties.OfType<MicrodataKeyValuePair>());
                    foreach (var p in mp)
                    {
                        switch (p.Name)
                        {
                            case "name":
                                ev.LocationName = p.Value;
                                break;
                            case "streetAddress":
                                ev.Street = p.Value;
                                break;
                            case "addressLocality":
                                ev.Locality = p.Value;
                                break;
                            case "addressRegion":
                                ev.Region = p.Value;
                                break;
                            case "addressCountry":
                                ev.Country = p.Value;
                                break;
                            case "postalCode":
                                ev.Zip = p.Value;
                                break;
                            case "url":
                                ev.LocationUri = mergeUrl(sourceUrl, p.Value);
                                break;
                        }
                    }
                    mp = attr.Properties.OfType<MicrodataItem>().Where(t => t.ItemType.Any(u => u.EndsWith("GeoCoordinates"))).SelectMany(t => t.Properties.OfType<MicrodataKeyValuePair>());
                    foreach (var p in mp)
                    {
                        switch (p.Name)
                        {
                            case "longitude":
                                ev.Longitude = decimal.Parse(p.Value, System.Globalization.CultureInfo.InvariantCulture);
                                break;
                            case "latitude":
                                ev.Latitude = decimal.Parse(p.Value, System.Globalization.CultureInfo.InvariantCulture);
                                break;

                        }
                    }
                }
                eventsList.Add(ev);
            }
            return eventsList.Count > count;
        }
        protected virtual List<MicrodataItem> Parse(HtmlDocument doc, bool removeScriptandStyles)
        {
            if (removeScriptandStyles)
                doc.DocumentNode.Descendants()
                    .Where(n => n.Name == "script" || n.Name == "style")
                    .ToList()
                    .ForEach(n => n.Remove());

            var topLevelItems = doc.DocumentNode.Descendants().Where(n => n.Attributes.Contains("itemscope") && !n.Attributes.Contains("itemprop"));

            List<MicrodataItem> mdis = new List<MicrodataItem>();

            foreach (var items in topLevelItems)
                mdis.Add(GetMicroDataItem(doc, items));

            return mdis;
        }

        #region private
        private static MicrodataItem GetMicroDataItem(HtmlDocument doc, HtmlNode htmlItem)
        {
            MicrodataItem item = new MicrodataItem();

            if (htmlItem.Attributes.Contains("itemtype"))
                htmlItem.Attributes["itemtype"].Value.Split(' ').ToList().ForEach(it => item.ItemType.Add(it));

            if (htmlItem.Attributes.Contains("itemid"))
                item.ItemId = htmlItem.Attributes["itemid"].Value;

            item.Properties = GetPropertiesValues(doc, GetProperties(doc, htmlItem));

            return item;
        }

        private static List<MicrodataBase> GetPropertiesValues(HtmlDocument doc, List<HtmlNode> properties)
        {
            List<MicrodataBase> propertiesValues = new List<MicrodataBase>();
            foreach (var property in properties)
            {
                string propertyName = property.Name.ToLower();
                string name = property.Attributes["itemprop"].Value;
                MicrodataItem item = null;
                string value = string.Empty;

                if (property.Attributes.Contains("itemscope"))
                    item = GetMicroDataItem(doc, property);
                else if (propertyName == "meta")
                    value = property.Attributes["content"].Value;
                else if ("audio,embed,iframe,img,source,track,video,".Contains(propertyName + ","))
                    value = property.Attributes["src"].Value;
                else if ("a,area,link,".Contains(propertyName + ","))
                    value = property.Attributes["href"].Value;
                else if (propertyName == "object")
                    value = property.Attributes["data"].Value;
                else if (propertyName == "data")
                    value = property.Attributes["value"].Value;
                else if (propertyName == "time")
                    value = property.InnerText;

                // This is not from the Specifications, but some sites are using it.
                else if (property.Attributes.Contains("content"))
                    value = HtmlEntity.DeEntitize(property.Attributes["content"].Value).Trim();
                else
                    value = HtmlEntity.DeEntitize(property.InnerText).Trim();

                if (item != null)
                    propertiesValues.Add(item);

                if (!string.IsNullOrWhiteSpace(value))
                    propertiesValues.Add(new MicrodataKeyValuePair { Name = name, Value = value });
            }
            return propertiesValues;
        }

        private static List<HtmlNode> GetProperties(HtmlDocument doc, HtmlNode htmlItem)
        {
            Queue<HtmlNode> memory = new Queue<HtmlNode>();
            List<HtmlNode> results = new List<HtmlNode>();
            Queue<HtmlNode> pending = new Queue<HtmlNode>();

            memory.Enqueue(htmlItem);
            foreach (var children in htmlItem.ChildNodes)
                pending.Enqueue(children);

            if (htmlItem.Attributes.Contains("itemref"))
                foreach (string itemref in htmlItem.Attributes["itemref"].Value.Split(' '))
                    pending.Enqueue(doc.DocumentNode.SelectSingleNode("//" + itemref));

            while (pending.Count > 0)
            {
                var currentItem = pending.Dequeue();

                // If the node is already added, skip it
                if (memory.Contains(currentItem))
                    continue;
                memory.Enqueue(currentItem);

                // If the node is not an ItemScope, enqueue it
                if (!currentItem.Attributes.Contains("itemscope"))
                    foreach (var children in currentItem.ChildNodes)
                        pending.Enqueue(children);

                // If the node is an itemprop, it's... an itemprop
                if (currentItem.Attributes.Contains("itemprop"))
                    results.Add(currentItem);
            }

            return results;
        }
        #endregion
    }
}
