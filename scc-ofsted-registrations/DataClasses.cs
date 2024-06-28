using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scc_ofsted_registrations
{
    public class OfstedQueryData
    {
        public string OfstedRef { get; set; }
    }
    [Serializable]
    public class OfstedResponseData
    {

        public string OfstedURL { get; set; }
        public string InspectionResult { get; set; }
        public string InspectionType { get; set; }
        public DateOnly InspectionDate { get; set; }
        public string ProviderID { get; set; }
    }
}
