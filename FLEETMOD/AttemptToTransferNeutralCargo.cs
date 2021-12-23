using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD
{
	[HarmonyPatch(typeof(PLPlayer), "AttemptToTransferNeutralCargo")]
	internal class AttemptToTransferNeutralCargo
	{
		public static bool Prefix(int inCurrentShipID, int inNetID, PLPlayer __instance)
		{
			if (!MyVariables.isrunningmod)
			{
				return true;
			}
			else
			{
				if (PLEncounterManager.Instance != null)
				{
					PLShipInfo plshipInfo = PLEncounterManager.Instance.GetShipFromID(inCurrentShipID) as PLShipInfo; // ? Gets current ship player is on
					if (plshipInfo != null)
					{
                        if (!MyVariables.CargoMenu)
                        {
                            MyVariables.CargoMenu = true;
                            CargoMenu.inCurrentShipID = inCurrentShipID;
                            CargoMenu.inNetID = inNetID;
                            var TransferMenu = new UnityEngine.GameObject("CargoMenu_GO"); // TODO: Maybe create one BIG GameObject for all Dialogs?
                            TransferMenu.AddComponent<CargoMenu>(); // Also TODO: Rename local vars...
                            UnityEngine.GameObject.DontDestroyOnLoad(TransferMenu);
                        }
					}
				}
				return false;
			}
		}
	}
}
