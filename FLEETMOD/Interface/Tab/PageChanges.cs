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
        public static Text TabDescription;
        public static void Postfix()
        {
            Transform CREW_Tab = FindDeepChild(PLTabMenu.Instance.gameObject.transform, "CREW_Tab", 5);
            Transform Home = CREW_Tab.Find("Home"); // Main Page
            Transform CrewSettings = CREW_Tab.Find("CrewSettings"); // Captain Page

            // Disable Targetting UI
            Home.Find("TargetShipBG").gameObject.SetActive(!Variables.isrunningmod);

            PulsarModLoader.Utilities.Logger.Info($"[Fleetmod] 5 - {Variables.isrunningmod}");

            // Add Descriptive Text
            Transform descriptionLabel = CrewSettings.Find("CrewPermissionsLabel");
            Transform newLabel = GameObject.Instantiate(descriptionLabel, Home);
            newLabel.position = descriptionLabel.position;
            newLabel.localPosition = newLabel.localPosition - new Vector3(0,30f);
            newLabel.rotation = descriptionLabel.rotation;
            newLabel.localScale = descriptionLabel.localScale;
            newLabel.name = "FleetPageDescriptionLabel";
            newLabel.parent = Home;
            Text description = newLabel.GetComponent<Text>();
            description.supportRichText = true;
            TabDescription = description;
            newLabel.gameObject.SetActive(Variables.isrunningmod);
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
            if (ChangeTabMenuDisplay.TabDescription != null)
            {
                PLPlayer player = PLServer.Instance.GetCachedFriendlyPlayerOfClass(0);
                ChangeTabMenuDisplay.TabDescription.text = $"Your ship is the {player.StartingShip.ShipNameValue}\nCaptained by {player.GetPlayerName(false)}\n\nChange class or ship with the Fleet tab in the top right.";
            }
        }
    }
}
