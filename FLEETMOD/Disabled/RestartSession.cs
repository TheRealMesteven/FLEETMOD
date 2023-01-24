using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLEETMOD.Disabled
{
    [HarmonyPatch(typeof(PLGameOverScreen), "ClickLoad")]
    internal class ClickLoad
    {
        public static bool Prefix()
        {
            if (MyVariables.isrunningmod)
            {
                PulsarModLoader.Utilities.Messaging.Notification("FLEETMOD - Disabled Load", PhotonTargets.All);
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(PLGameOverScreen), "ClickRetry")]
    internal class ClickRetry
    {
        public static bool Prefix()
        {
            if (MyVariables.isrunningmod)
            {
                PulsarModLoader.Utilities.Messaging.Notification("FLEETMOD - Disabled Retry", PhotonTargets.All);
                return false;
            }
            return true;
        }
    }
}
