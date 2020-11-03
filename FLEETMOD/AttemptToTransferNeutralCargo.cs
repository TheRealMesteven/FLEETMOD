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
		public static bool Prefix(int inCurrentShipID, int inNetID)
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
					PLShipInfo plshipInfo = PLEncounterManager.Instance.GetShipFromID(inCurrentShipID) as PLShipInfo;
					bool flag3 = plshipInfo != null;
					if (flag3)
					{
						List<int> list = new List<int>();
						foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
						{
							bool flag4 = plshipInfoBase != plshipInfo && plshipInfoBase.GetIsPlayerShip();
							if (flag4)
							{
								list.Add(plshipInfoBase.ShipID);
							}
						}
						int inID = list[UnityEngine.Random.Range(0, list.Count)];
						PLShipComponent componentFromNetID = plshipInfo.MyStats.GetComponentFromNetID(inNetID);
						bool flag5 = componentFromNetID != null;
						if (flag5)
						{
							(PLEncounterManager.Instance.GetShipFromID(inID) as PLShipInfo).MyStats.AddShipComponent(PLWare.CreateFromHash(1, (int)PLShipComponent.createHashFromInfo((int)componentFromNetID.ActualSlotType, componentFromNetID.SubType, componentFromNetID.Level, 0, 12)) as PLShipComponent, -1, ESlotType.E_COMP_CARGO);
							plshipInfo.MyStats.RemoveShipComponentByNetID(inNetID);
						}
					}
				}
				result = false;
			}
			return result;
		}
	}
}
