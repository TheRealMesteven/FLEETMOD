using System;
using HarmonyLib;
using PulsarPluginLoader;
using UnityEngine;

namespace FLEETMOD
{
    [HarmonyPatch(typeof(PLUICreateGameMenu), "ClickEngage")]
    internal class ClickEngage
    {
        public static bool Prefix(PLUICreateGameMenu __instance, ref int ___CurrentSelectedShipIndex)
        {
            Debug.unityLogger.logEnabled = false;
            if (PLServer.Instance != null && PLServer.Instance.GameHasStarted)
            {
                PLMusic.PostEvent("play_sx_playermenu_click_major", PLGlobal.Instance.gameObject);
                switch (___CurrentSelectedShipIndex)
                {
                    case 3:
                        PLNetworkManager.Instance.SelectedShipTypeID = 4;
                        break;
                    case 4:
                        PLNetworkManager.Instance.SelectedShipTypeID = 5;
                        break;
                    case 5:
                        PLNetworkManager.Instance.SelectedShipTypeID = 6;
                        break;
                    case 6:
                        PLNetworkManager.Instance.SelectedShipTypeID = 7;
                        break;
                    case 7:
                        PLNetworkManager.Instance.SelectedShipTypeID = 8;
                        break;
                    case 8:
                        PLNetworkManager.Instance.SelectedShipTypeID = 9;
                        break;
                    case 9:
                        PLNetworkManager.Instance.SelectedShipTypeID = 10;
                        break;
                    case 10:
                        PLNetworkManager.Instance.SelectedShipTypeID = 11;
                        break;
                    case 11:
                        PLNetworkManager.Instance.SelectedShipTypeID = 12;
                        break;
                    default:
                        PLNetworkManager.Instance.SelectedShipTypeID = ___CurrentSelectedShipIndex;
                        break;
                }
                if (PLNetworkManager.Instance.SelectedShipTypeID == 8)
                {
                    PLNetworkManager.Instance.SelectedShipTypeID = 3;
                }
                if (PLNetworkManager.Instance.SelectedShipTypeID == 0)
                {
                    PLNetworkManager.Instance.SelectedShipTypeID = 62;
                }
                PLNetworkManager.Instance.MainMenu.CloseActiveMenu();
                PLNetworkManager.Instance.LocalPlayer.SetClassID(0);
                ModMessage.SendRPC("Dragon+Mest.Fleetmod", "FLEETMOD.ServerCreateShip", PhotonTargets.MasterClient, new object[]
                {
                    PLNetworkManager.Instance.SelectedShipTypeID,
                    PLNetworkManager.Instance.LocalPlayer.GetPlayerID(),
                    __instance.ShipNameField.text
                });
                PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLErrorMessageMenu(string.Concat(new string[]
                {
                    "<color=#0066FF>You Are Now The Captain Of ",
                    __instance.ShipNameField.text,
                    "</color>\n\n<color=#c0c0c0>",
                    PLGlobal.Instance.ClassDesc[0],
                    "</color>"
                })));
                return false;
            }
            else
            {
                Global.isrunningmod = true;
                return true;
            }
        }
    }
}