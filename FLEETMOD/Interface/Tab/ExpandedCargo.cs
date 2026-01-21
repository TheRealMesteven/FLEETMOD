using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLEETMOD.Interface.Tab
{
    internal class ExpandedCargo
    {
        /// <summary>
        /// Find existing tab features and curate the new implementations for the first time
        /// </summary>
        internal static void Initialize()
        {

        }

        /// <summary>
        /// First execution
        /// </summary>
        internal static void OnAwake()
        {

        }

        /// <summary>
        /// Update the tab features
        /// </summary>
        internal static void Update()
        {
            if (!Variables.isrunningmod) return;

        }
    }
}
