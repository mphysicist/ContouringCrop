using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace june19
{
    public class Logic
    {
        public string abcd { get; set; }
        
        public Logic(PlanSetup planSetup)
        {
            abcd = planSetup.Id;
        }

    }
}
