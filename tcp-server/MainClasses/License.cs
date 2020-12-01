using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_server
{
    class License
    {

        public int licenseCompanyCode { get; set; }
        public string licenseAPIKey { get; set; }
        public string licenseLicenseKey { get; set; }
        public string licenseMAC { get; set; }
        public string licenseProcessorInfo { get; set; }
        public string licenseSystemboardInfo { get; set; }
        public License()
        {

        }
        public License(
            int licenseCompanyCode,
            string licenseAPIKey,
            string licenseLicenseKey,
            string licenseMAC,
            string licenseProcessorInfo,
            string licenseSystemboardInfo
            )
        {
            this.licenseAPIKey = licenseAPIKey;
            this.licenseLicenseKey = licenseLicenseKey;
            this.licenseMAC = licenseMAC;
            this.licenseProcessorInfo = licenseProcessorInfo;
            this.licenseSystemboardInfo = licenseSystemboardInfo;
        }

    }
}
