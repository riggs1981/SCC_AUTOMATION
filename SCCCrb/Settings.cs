using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCCrb
{
    public class Credentials
    {
        public string orgname = System.Environment.GetEnvironmentVariable("orgname");
        public string userforename = System.Environment.GetEnvironmentVariable("userforename");
        public string usersurname = System.Environment.GetEnvironmentVariable("usersurname");
    }
}
