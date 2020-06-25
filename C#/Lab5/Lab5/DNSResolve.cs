using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lab5
{
    class DNSResolve
    {
        public static readonly string[] HostNames =  {
            "www.microsoft.com",
            "www.apple.com",
            "www.google.com",
            "www.ibm.com",
            "cisco.netacad.net",
            "www.oracle.com",
            "www.nokia.com",
            "www.hp.com",
            "www.dell.com",
            "www.samsung.com",
            "www.toshiba.com",
            "www.siemens.com",
            "www.amazon.com",
            "www.sony.com",
            "www.canon.com",
            "www.alcatellucent.com",
            "www.acer.com",
            "www.motorola.com"
        };

        public static Dictionary<string, string> ResolveDomains()
        {
            var result = new Dictionary<string, string>();
            HostNames.AsParallel().ForAll(hostName =>
            {
                string ip = "error";
                try
                {
                    ip = Dns.GetHostAddresses(hostName).First().ToString();
                }
                catch (Exception e)
                {

                }
                lock (result)
                {
                    result.Add(hostName, ip);
                }
            });
            return result;
        }
    }
}
