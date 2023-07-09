using System;
using HarmonyLib;
using Steamworks;
using UnityEngine;

namespace FLEETMOD.Interface.Tab
{
    [HarmonyPatch(typeof(PLOverviewPlayerInfoDisplay), "OnButtonPress")]
    internal class OnButtonPress
    {
        private static void ChangeClass(PLOverviewPlayerInfoDisplay __instance, int ClassID, string ClassName)
        {
            PLServer.Instance.photonView.RPC("SetPlayerAsClassID", PhotonTargets.All, new object[]
            {
                PLNetworkManager.Instance.LocalPlayer.GetPlayerID(),
                ClassID
            });
            PhotonNetwork.player.SetScore(__instance.MyPlayer.StartingShip.ShipID);
            PLMusic.PostEvent("play_titlemenu_ui_click", __instance.gameObject);
            PLTabMenu.Instance.TabMenuActive = false;
            PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLErrorMessageMenu(string.Concat(new string[]
            {
                $"<color=#FFFFFF>You Are Now {ClassName} Aboard The ",
                __instance.MyPlayer.StartingShip.ShipNameValue,
                "</color>\n\n<color=#c0c0c0>",
                PLGlobal.Instance.ClassDesc[ClassID],
                "</color>"
            })));
        }
        public static void Prefix(PLOverviewPlayerInfoDisplay __instance, ref PLTabMenuPlayerInfoButton inButton, ref float ___LastButtonPressProcessTime, ref float ___cached_LastUpdatedPlayerInfoTime)
        {
            if (!Variables.isrunningmod) return;
            if (Time.unscaledTime - ___LastButtonPressProcessTime < 0.1f || !(!PLNetworkManager.IsActiveMenuOpen() && PLTabMenu.Instance.TabMenuActive && inButton.m_Label.gameObject.activeSelf)) return;
            int num = -1;
            switch (inButton.m_Label.text)
            {
                case "Pilot":
                    num = 1;
                    break;
                case "Science":
                    num = 2;
                    break;
                case "Weapons":
                    num = 3;
                    break;
                case "Engineer":
                    num = 4;
                    break;
                case "Add PiBot":
                    num = 5;
                    break;
                case "Add SciBot":
                    num = 6;
                    break;
                case "Add WeapBot":
                    num = 7;
                    break;
                case "Add EngBot":
                    num = 8;
                    break;
                default:
                    break;
            }
            if (num > 0 && num < 5) ChangeClass(__instance, num, inButton.m_Label.text);
            else if (num > 4 && num < 9) PLServer.Instance.ServerAddCrewBotPlayer(num - 4);
            /*
            if (inButton.m_Label.text == "To Brig")
            {
                ___cached_LastUpdatedPlayerInfoTime = 0f;
                __instance.MyPlayer.StartingShip = PLNetworkManager.Instance.LocalPlayer.StartingShip;
                __instance.MyPlayer.GetPhotonPlayer().SetScore(PLNetworkManager.Instance.LocalPlayer.StartingShip.ShipID);
                MyVariables.BriggedCrew.Add(__instance.MyPlayer.GetPlayerID());
                PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
                {
                                    __instance.MyPlayer.GetPlayerName(false).Substring(__instance.MyPlayer.GetPlayerName(false).LastIndexOf("•") + 2) + " Has Been Sent To The Brig By The Admiral.",
                                    Color.green,
                                    0,
                                    "SHIP"
                });
                PLNetworkManager.Instance.ConsoleText.Insert(0, "You Have Sent " + __instance.MyPlayer.GetPlayerName(false).Substring(__instance.MyPlayer.GetPlayerName(false).LastIndexOf("•") + 2) + " To The Brig");
                __instance.MyPlayer.photonView.RPC("NetworkTeleportToSubHub", PhotonTargets.All, new object[]
                {
                                    PLNetworkManager.Instance.LocalPlayer.StartingShip.MyTLI.SubHubID,
                                    0
                });
                PulsarModLoader.ModMessage.SendRPC("Dragon+Mest.Fleetmod", "FLEETMOD.ServerUpdateVariables", PhotonTargets.All, new object[]{
                            MyVariables.shipfriendlyfire,
                            MyVariables.recentfriendlyfire,
                            MyVariables.survivalBonusDict,
                            MyVariables.BriggedCrew
                        });
            }
            if (inButton.m_Label.text == "Release")
            {
                if (MyVariables.BriggedCrew.Contains(__instance.MyPlayer.GetPlayerID()))
                {
                    MyVariables.BriggedCrew.Remove(__instance.MyPlayer.GetPlayerID());
                }
                ___cached_LastUpdatedPlayerInfoTime = 0f;
                PLNetworkManager.Instance.ConsoleText.Insert(0, "You Have Released " + __instance.MyPlayer.GetPlayerName(false).Substring(__instance.MyPlayer.GetPlayerName(false).LastIndexOf("•") + 2) + " From The Brig");
                PulsarModLoader.ModMessage.SendRPC("Dragon+Mest.Fleetmod", "FLEETMOD.ServerUpdateVariables", PhotonTargets.All, new object[]{
                            MyVariables.shipfriendlyfire,
                            MyVariables.recentfriendlyfire,
                            MyVariables.survivalBonusDict,
                            MyVariables.BriggedCrew
                        });
            }*/
        }
    }
}

// On the Crew selection page, make it so that non-captained ships are displayed
// Set Ship limit to 5 total
// Use Mod etc to allow up to 10 ships by using the same crew list page switch method
// Remove auto captain
// Make a way for crew to become captain via tab