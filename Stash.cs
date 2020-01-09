using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POEScouringRecipe
{
    public class Colour
    {
        public int r { get; set; }
        public int g { get; set; }
        public int b { get; set; }
    }

    public class Tab
    {
        public string n { get; set; }
        public int i { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public bool hidden { get; set; }
        public bool selected { get; set; }
        public Colour colour { get; set; }
        public string srcL { get; set; }
        public string srcC { get; set; }
        public string srcR { get; set; }
    }

    public class Socket
    {
        public int group { get; set; }
        public string attr { get; set; }
        public string sColour { get; set; }
    }

    public class Property
    {
        public string name { get; set; }
        public List<object> values { get; set; }
        public int displayMode { get; set; }
        public int type { get; set; }
    }

    public class Requirement
    {
        public string name { get; set; }
        public List<List<object>> values { get; set; }
        public int displayMode { get; set; }
    }

    public class Item
    {
        public bool verified { get; set; }
        public int w { get; set; }
        public int h { get; set; }
        public int ilvl { get; set; }
        public string icon { get; set; }
        public string league { get; set; }
        public string id { get; set; }
        public List<Socket> sockets { get; set; }
        public string name { get; set; }
        public string typeLine { get; set; }
        public bool identified { get; set; }
        public List<Property> properties { get; set; }
        public List<Requirement> requirements { get; set; }
        public int frameType { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string inventoryId { get; set; }
        public List<object> socketedItems { get; set; }
        public List<string> implicitMods { get; set; }
    }

    public class Stash
    {
        public int numTabs { get; set; }
        public List<Tab> tabs { get; set; }
        public List<Item> items { get; set; }
    }
}
