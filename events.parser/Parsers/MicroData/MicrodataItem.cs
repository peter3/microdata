using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace events.parser.Parsers.MicroData
{
    public class MicrodataItem : MicrodataBase
    {
        public MicrodataItem()
        {
            ItemType = new List<string>();
            Properties = new List<MicrodataBase>();
        }

        public string ItemId { get; set; }
        public List<string> ItemType { get; set; }
        public List<MicrodataBase> Properties { get; set; }

        public override string ToJSON()
        {
            List<string> itemList = new List<string>();
            if (!string.IsNullOrWhiteSpace(ItemId))
                itemList.Add("\"itemId\": \"" + ItemId + "\"");

            foreach (string itemType in ItemType)
                itemList.Add("\"itemType\": \"" + itemType + "\"");

            foreach (var prop in Properties)
                itemList.Add(prop.ToJSON());

            return "[" + Environment.NewLine + string.Join(",", itemList.ToArray()) + "]" + Environment.NewLine;
        }

    }
}
