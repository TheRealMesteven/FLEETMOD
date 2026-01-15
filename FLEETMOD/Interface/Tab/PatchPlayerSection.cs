using System;
using System.Collections.Generic;
using HarmonyLib;
using Steamworks;
using UnityEngine;

namespace FLEETMOD.Interface.Tab
{
    [HarmonyPatch(typeof(PLOverviewPlayerInfoDisplay), "Update")]
    internal class UpdatePlayerOverview
    {
        public static void Postfix(PLOverviewPlayerInfoDisplay __instance, ref float ___cached_LastUpdatedPlayerInfoTime, ref PLPlayer ___cached_DisplayedPlayer, ref int ___cached_DisplayedPlayerClass, ref float ___cached_DisplayedPlayerHealth, ref bool ___cached_DisplayedPlayerIsTalking)
        {
            if (!Variables.isrunningmod || __instance.name != "FleetPageChangeClass") return;
            PLPlayer LocalPlayer = PLNetworkManager.Instance.LocalPlayer;
            __instance.MyPlayer = LocalPlayer;
            __instance.ClassID = LocalPlayer.GetClassID();
            __instance.PlayerName.text = LocalPlayer.GetClassName();//LocalPlayer.GetPlayerName(false);
            __instance.ClassName.text = LocalPlayer.StartingShip == null ? "" : LocalPlayer.StartingShip.ShipNameValue;
        }
    }

    [HarmonyPatch(typeof(PLOverviewPlayerInfoDisplay), "UpdateButtons")]
    internal class UpdateButtons
    {
        internal static string GetNameForButton(bool Captain, int ButtonID)
        {
            PLPlayer player = PLServer.Instance.GetCachedFriendlyPlayerOfClass(ButtonID, PLNetworkManager.Instance.LocalPlayer.StartingShip);
            if (player)
            {
                if (!player.IsBot || !Captain) return player.GetPlayerName(false);
                switch (ButtonID)
                {
                    case 1: return "Remove\nPiBot";
                    case 2: return "Remove\nSciBot";
                    case 3: return "Remove\nWeapBot";
                    case 4: return "Remove\nEngBot";
                    default: return "";
                }
            }
            switch (ButtonID)
            {
                case 1: return Captain ? "Add\nPiBot" : "Be\nPilot";
                case 2: return Captain ? "Add\nSciBot" : "Be\nScientist";
                case 3: return Captain ? "Add\nWeapBot" : "Be\nWeapons";
                case 4: return Captain ? "Add\nEngBot" : "Be\nEngineer";
                default: return "";
            }
        }
        public static bool Prefix(PLOverviewPlayerInfoDisplay __instance, ref List<int> ___ButtonsActiveTypes)
        {
            if (!Variables.isrunningmod || __instance.name != "FleetPageChangeClass" || PLNetworkManager.Instance.LocalPlayer == null) return true;
            
            ___ButtonsActiveTypes.Clear();
            bool Captain = false;
            if (PLNetworkManager.Instance.LocalPlayer == null || PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0)
            {
                Captain = true;
            }
            for (int i = 0; i < 4; i++)
            {
                if (i < __instance.Buttons.Length)
                {
                    __instance.Buttons[i].MyPID = __instance;
                    if (__instance.Buttons[i].m_Label != null && !__instance.Buttons[i].m_Label.gameObject.activeSelf)
                    {
                        __instance.Buttons[i].m_Label.gameObject.SetActive(true);
                    }
                    __instance.Buttons[i].m_Label.text = GetNameForButton(Captain, 4-i);
                }
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(PLOverviewPlayerInfoDisplay), "OnButtonPress")]
    internal class OnButtonPress
    {
        private static void ChangeClass(PLOverviewPlayerInfoDisplay __instance, int ClassID)
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
                $"<color=#FFFFFF>You Are Now {PLGlobal.Instance.ClassNames[ClassID]} Aboard The ",
                __instance.MyPlayer.StartingShip.ShipNameValue,
                "</color>\n\n<color=#c0c0c0>",
                PLGlobal.Instance.ClassDesc[ClassID],
                "</color>"
            })));
        }
        public static void Prefix(PLOverviewPlayerInfoDisplay __instance, ref PLTabMenuPlayerInfoButton inButton, ref float ___LastButtonPressProcessTime, ref float ___cached_LastUpdatedPlayerInfoTime)
        {
            if (!Variables.isrunningmod || __instance.name != "FleetPageChangeClass") return;
            if (Time.unscaledTime - ___LastButtonPressProcessTime < 0.1f || !(!PLNetworkManager.IsActiveMenuOpen() && PLTabMenu.Instance.TabMenuActive && inButton.m_Label.gameObject.activeSelf)) return;
            string[] args = inButton.m_Label.text.Split(new[] { '\n' });
            if (args.Length < 2) return;
            int ClassID = -1;
            switch (args[1][0])
            {
                case 'P': ClassID = 1; break;
                case 'S': ClassID = 2; break;
                case 'W': ClassID = 3; break;
                case 'E': ClassID = 4; break;
                default: return;
            }
            switch (args[0][0])
            {
                case 'A': PLServer.Instance.ServerAddCrewBotPlayer(ClassID); break;//Bot.ServerAddCrewBotPlayer.AddCrewBotPlayer(PLNetworkManager.Instance.LocalPlayer, PLServer.Instance.GetLowestAvailablePlayerID(), ClassID); break;
                case 'R': PLServer.Instance.photonView.RPC("ServerRemoveCrewBotPlayer", PhotonTargets.MasterClient, new object[] { PLServer.Instance.GetCachedFriendlyPlayerOfClass(ClassID, PLNetworkManager.Instance.LocalPlayer.StartingShip).GetPlayerID() }); break;
                case 'B': ChangeClass(__instance, ClassID); break;
            }
        }
    }
}
