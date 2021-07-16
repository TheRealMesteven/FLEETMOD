using System;
using HarmonyLib;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLShipInfoBase), "OnEndWarp")]
	internal class OnEndWarp
	{
		public static bool Prefix(PLShipInfoBase __instance)
		{
			if (!MyVariables.isrunningmod)
			{
				return true;
			}
			else
			{
				if (PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip == __instance as PLShipInfo)
				{
					foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
					{
						if (plshipInfoBase != null && plshipInfoBase.InWarp && plshipInfoBase.GetIsPlayerShip() && plshipInfoBase != __instance)
						{
							plshipInfoBase.SkipWarp();
							plshipInfoBase.InWarp = false;
                            plshipInfoBase.WarpChargeStage = EWarpChargeStage.E_WCS_COLD_START;
                        }
					}
                    if (PLServer.GetCurrentSector().Name.Contains("W.D. HUB") || PLServer.GetCurrentSector().Name.Contains("Outpost 448") || PLServer.GetCurrentSector().Name.Contains("The Estate"))
                    {
                        if (MyVariables.DialogGenerated != true)
                        {
                            var go = new UnityEngine.GameObject("ShipCaptainRequest_GO"); // TODO: Maybe create one BIG GameObject for all Dialogs?
                            go.AddComponent<Interface.Dialogs.ShipCaptainRequest>(); // Also TODO: Rename local vars...
                            UnityEngine.GameObject.DontDestroyOnLoad(go);
                            PulsarPluginLoader.Utilities.Messaging.Notification("Check your dialogue screen!");
                            MyVariables.DialogGenerated = true;
                        }
                    }
                }
				return false;
			}
		}
	}
}
