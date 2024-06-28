using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigChange
{
    public static class Credentials
    {
       
        public static string bcKey = System.Environment.GetEnvironmentVariable("bcKey");
        public static string authstring = System.Environment.GetEnvironmentVariable("authstring");
        public static string environmentUrl = System.Environment.GetEnvironmentVariable("environmentUrl");
        public static string wsPath = System.Environment.GetEnvironmentVariable("wsPath");

    };
}
