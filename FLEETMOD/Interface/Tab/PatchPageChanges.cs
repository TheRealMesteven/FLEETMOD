using HarmonyLib;
using UnityEngine;

namespace FLEETMOD.Interface.Tab
{
    [HarmonyPatch(typeof(PLServer), "Start")]
    internal class ChangeTabMenuDisplay
    {
        internal static bool Executed = false;

        // Main pages / navigation (BGLeft)
        internal static Transform CREW_Tab;

        // Main pages / navigation (BGRight)
        internal static Transform BGRight;
        internal static Transform SHIP;
        internal static GameObject CREW;
        public static void Postfix()
        {
            UpdateLabels.Executed = false;
            if (Executed) return;
            Executed = true;
            CREW_Tab = FindDeepChild(PLTabMenu.Instance.gameObject.transform, "CREW_Tab", 5);
            BGRight = FindDeepChild(PLTabMenu.Instance.gameObject.transform, "BGRight", 5);
            SHIP = BGRight.Find("SHIP");
            CREW = BGRight.Find("CREW").gameObject;
            HomeTabMenu.Initialize();
            ExpandedCargo.Initialize();
        }

        public static Transform FindDeepChild(Transform parent, string name, int depth = 3)
        {
            if (depth <= 0) return null;
            depth--;
            foreach (Transform child in parent)
            {
                if (child.name == name)
                    return child;

                Transform result = FindDeepChild(child, name, depth);
                if (result != null)
                    return result;
            }
            return null;
        }
    }

    [HarmonyPatch(typeof(PLTabMenu), "Update")]
    internal class UpdateLabels
    {
        internal static bool Executed = false;
        public static void Postfix()
        {
            if (PLServer.Instance == null || !ChangeTabMenuDisplay.Executed) return;
            if (!Executed)
            {
                Executed = true;
                ShipID = -1;
                HomeTabMenu.OnAwake();
                ExpandedCargo.OnAwake();
            }

            if (!Variables.isrunningmod || PLNetworkManager.Instance.LocalPlayer == null || !PLNetworkManager.Instance.LocalPlayer.GetHasStarted()) return;
            HomeTabMenu.Update();
            ExpandedCargo.Update();
        }

        internal static int ShipID = -1;
    }
}
