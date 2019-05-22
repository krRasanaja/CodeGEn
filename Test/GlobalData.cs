using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    public class GlobalData
    {
        public static List<string> serverList = new List<string>();
        public static Thread Session = null;
    }
}
