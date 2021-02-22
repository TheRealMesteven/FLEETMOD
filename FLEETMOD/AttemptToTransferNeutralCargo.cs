using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	// Token: 0x02000024 RID: 36
	[HarmonyPatch(typeof(PLPlayer), "AttemptToTransferNeutralCargo")]
	internal class AttemptToTransferNeutralCargo
	{
		// Token: 0x06000046 RID: 70 RVA: 0x00006684 File Offset: 0x00004884
		public static bool Prefix(int inCurrentShipID, int inNetID, PLPlayer __instance)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = PLEncounterManager.Instance != null;
				if (flag2)
				{
					PLShipInfo plshipInfo = PLEncounterManager.Instance.GetShipFromID(inCurrentShipID) as PLShipInfo; // ? Gets current ship player is on
					bool flag3 = plshipInfo != null;
					if (flag3)
					{
                        int inID = 1;
                        bool Check = false;
						List<int> list = new List<int>();
						foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
						{
							if (plshipInfoBase != plshipInfo && plshipInfoBase.GetIsPlayerShip() && PLEncounterManager.Instance.GetShipFromID(__instance.GetPhotonPlayer().GetScore()).ShipID == plshipInfoBase.ShipID)
							{
                                inID = (plshipInfoBase.ShipID); // Sets the ship to send the comps to as Players Ship
                                Check = true; // Ensures identifier assigned to which ship sending to
							}
						}
						PLShipComponent componentFromNetID = plshipInfo.MyStats.GetComponentFromNetID(inNetID);
						if (componentFromNetID != null && Check) // If component we're transferring isnt null
                        {
							(PLEncounterManager.Instance.GetShipFromID(inID) as PLShipInfo).MyStats.AddShipComponent(PLWare.CreateFromHash(1, (int)PLShipComponent.createHashFromInfo((int)componentFromNetID.ActualSlotType, componentFromNetID.SubType, componentFromNetID.Level, 0, 12)) as PLShipComponent, -1, ESlotType.E_COMP_CARGO);
                            plshipInfo.MyStats.RemoveShipComponentByNetID(inNetID); // It adds the component to the ship and removes the component from the current ship player is on
                            PLPlayer cachedFriendlyPlayerOfClass = PLServer.Instance.GetCachedFriendlyPlayerOfClass(0);
                            PLServer.Instance.photonView.RPC("AddNotification", cachedFriendlyPlayerOfClass.GetPhotonPlayer(), new object[]
                            {
                                __instance.GetPlayerName(false).Substring(__instance.GetPlayerName(false).LastIndexOf("•") + 2)+" has sent "+PLShipComponent.CreateShipComponentFromHash((int)PLShipComponent.createHashFromInfo((int)componentFromNetID.ActualSlotType, componentFromNetID.SubType, componentFromNetID.Level, 0, 12), null).GetItemName()+" to "+PLEncounterManager.Instance.GetShipFromID(__instance.GetPhotonPlayer().GetScore()).ShipName,
                                __instance.GetPlayerID(),
                                PLServer.Instance.GetEstimatedServerMs() + 6000,
                                true
                            });
                        }
					}
				}
				result = false;
			}
			return result;
		}
	}
}
