using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace FLEETMOD.Interface.Tab
{
    [HarmonyPatch(typeof(PLServer), "Start")]
    internal class ChangeTabMenuDisplay
    {
        internal static bool Executed = false;
        internal static Text TabDescription;
        internal static GameObject TargetShipBG;
        internal static GameObject HomePageDescription;
        public static void Postfix()
        {
            if (Executed) return;
            Transform CREW_Tab = FindDeepChild(PLTabMenu.Instance.gameObject.transform, "CREW_Tab", 5);
            Transform Home = CREW_Tab.Find("Home"); // Main Page
            Transform CrewSettings = CREW_Tab.Find("CrewSettings"); // Captain Page

            // Disable Targetting UI
            TargetShipBG = Home.Find("TargetShipBG").gameObject;

            // Add Descriptive Text
            Transform descriptionLabel = CrewSettings.Find("CrewPermissionsLabel");
            Transform newDescriptionLabel = GameObject.Instantiate(descriptionLabel, Home);
            newDescriptionLabel.position = descriptionLabel.position;
            newDescriptionLabel.localPosition = newDescriptionLabel.localPosition - new Vector3(0,30f);
            newDescriptionLabel.rotation = descriptionLabel.rotation;
            newDescriptionLabel.localScale = descriptionLabel.localScale;
            newDescriptionLabel.name = "FleetPageDescriptionLabel";
            newDescriptionLabel.parent = Home;
            Text description = newDescriptionLabel.GetComponent<Text>();
            description.supportRichText = true;
            TabDescription = description;
            HomePageDescription = newDescriptionLabel.gameObject;

            Executed = true;
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
        public static void Postfix()
        {
            if (PLServer.Instance != null && ChangeTabMenuDisplay.Executed)
            {
                ChangeTabMenuDisplay.TargetShipBG.SetActive(!Variables.isrunningmod);
                ChangeTabMenuDisplay.HomePageDescription.SetActive(Variables.isrunningmod);

                PLPlayer player = PLServer.Instance.GetCachedFriendlyPlayerOfClass(0);
                PLPlayer localplayer = PLNetworkManager.Instance.LocalPlayer;
                ChangeTabMenuDisplay.TabDescription.text = $"Your ship is {(localplayer.StartingShip == null ? "missing!" : $"the {localplayer.StartingShip.ShipNameValue}")}\nYour Captain is {(player == null ? "No-one" : $"{player.GetPlayerName(false)}")}\n\nChange class or ship with the Fleet tab in the top right.";
            }
        }
    }
}
