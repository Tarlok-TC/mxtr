using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.Helpers
{
    public class RegionHelper
    {
        public Region Region { get; set; }
        public State[] States { get; set; }
    }
    public class Region
    {
        public string[] North { get; set; }
        public string[] East { get; set; }
        public string[] South { get; set; }
        public string[] West { get; set; }
    }
    public class State
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
    }
}