using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLEETMOD
{
    internal class Global
    {
        // Boolean check if the mod is running
        public static bool isrunningmod;

        //Dictionary linking Crew IDs and Ship IDs owned by the fleet.
        public static Dictionary<int, int> Fleet;
    }
}
