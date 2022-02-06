using System;
using HarmonyLib;
using PulsarModLoader;
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

				if (___CurrentSelectedShipIndex >= 3 && ___CurrentSelectedShipIndex <= 11)
					PLNetworkManager.Instance.SelectedShipTypeID = ___CurrentSelectedShipIndex + 1;
				else
					PLNetworkManager.Instance.SelectedShipTypeID = ___CurrentSelectedShipIndex;
				
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
				return true;
			}
		}
	}
}
