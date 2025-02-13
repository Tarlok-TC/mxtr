using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Api.Bullseye
{
    public class RestSearchResponseLogDataModel
    {
        public int LocationID { get; set; }
        public string LocationName { get; set; }
        public bool Viewed { get; set; }
        public bool URLClicked { get; set; }
        public bool EmailClicked { get; set; }
        public bool LocationResultsClicked { get; set; }
        public bool LocationMapClicked { get; set; }
        public bool DirectionsClicked { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
