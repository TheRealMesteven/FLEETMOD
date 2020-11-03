using System;
using HarmonyLib;

namespace FLEETMOD
{
	// Token: 0x0200000A RID: 10
	[HarmonyPatch(typeof(PLServerClassInfo), "OnPhotonSerializeView")]
	internal class OnPhotonSerializeView
	{
		// Token: 0x0600000F RID: 15 RVA: 0x000025F0 File Offset: 0x000007F0
		public static bool Prefix(PLServerClassInfo __instance, ref int ___m_ClassID)
		{
			bool flag = !MyVariables.isrunningmod;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = PhotonNetwork.isMasterClient && ___m_ClassID != -1 && PLServer.Instance != null && PLEncounterManager.Instance.PlayerShip != null && __instance.ClassLockerInventory.AllItems.Count <= 1;
				if (flag2)
				{
					int num = (PLEncounterManager.Instance.PlayerShip.FactionID == 1) ? 1 : 0;
					bool flag3 = ___m_ClassID == 3;
					int pawnInvItemIDCounter;
					if (flag3)
					{
						PLPawnInventoryBase classLockerInventory = __instance.ClassLockerInventory;
						PLServer instance = PLServer.Instance;
						pawnInvItemIDCounter = instance.PawnInvItemIDCounter;
						instance.PawnInvItemIDCounter = pawnInvItemIDCounter + 1;
						classLockerInventory.UpdateItem(pawnInvItemIDCounter, 2, 0, 1 + num, -1);
					}
					else
					{
						bool flag4 = ___m_ClassID == 2;
						if (flag4)
						{
							PLPawnInventoryBase classLockerInventory2 = __instance.ClassLockerInventory;
							PLServer instance2 = PLServer.Instance;
							pawnInvItemIDCounter = instance2.PawnInvItemIDCounter;
							instance2.PawnInvItemIDCounter = pawnInvItemIDCounter + 1;
							classLockerInventory2.UpdateItem(pawnInvItemIDCounter, 26, 0, num, -1);
						}
						else
						{
							PLPawnInventoryBase classLockerInventory3 = __instance.ClassLockerInventory;
							PLServer instance3 = PLServer.Instance;
							pawnInvItemIDCounter = instance3.PawnInvItemIDCounter;
							instance3.PawnInvItemIDCounter = pawnInvItemIDCounter + 1;
							classLockerInventory3.UpdateItem(pawnInvItemIDCounter, 2, 0, num, -1);
						}
					}
					bool flag5 = PLEncounterManager.Instance.PlayerShip.FactionID == 3;
					if (flag5)
					{
						bool flag6 = ___m_ClassID == 4;
						if (flag6)
						{
							PLPawnInventoryBase classLockerInventory4 = __instance.ClassLockerInventory;
							PLServer instance4 = PLServer.Instance;
							pawnInvItemIDCounter = instance4.PawnInvItemIDCounter;
							instance4.PawnInvItemIDCounter = pawnInvItemIDCounter + 1;
							classLockerInventory4.UpdateItem(pawnInvItemIDCounter, 24, 0, 1, -1);
						}
						else
						{
							PLPawnInventoryBase classLockerInventory5 = __instance.ClassLockerInventory;
							PLServer instance5 = PLServer.Instance;
							pawnInvItemIDCounter = instance5.PawnInvItemIDCounter;
							instance5.PawnInvItemIDCounter = pawnInvItemIDCounter + 1;
							classLockerInventory5.UpdateItem(pawnInvItemIDCounter, 24, 0, 0, -1);
						}
					}
					else
					{
						bool flag7 = ___m_ClassID == 4;
						if (flag7)
						{
							PLPawnInventoryBase classLockerInventory6 = __instance.ClassLockerInventory;
							PLServer instance6 = PLServer.Instance;
							pawnInvItemIDCounter = instance6.PawnInvItemIDCounter;
							instance6.PawnInvItemIDCounter = pawnInvItemIDCounter + 1;
							classLockerInventory6.UpdateItem(pawnInvItemIDCounter, 3, 0, 1, -1);
						}
						else
						{
							PLPawnInventoryBase classLockerInventory7 = __instance.ClassLockerInventory;
							PLServer instance7 = PLServer.Instance;
							pawnInvItemIDCounter = instance7.PawnInvItemIDCounter;
							instance7.PawnInvItemIDCounter = pawnInvItemIDCounter + 1;
							classLockerInventory7.UpdateItem(pawnInvItemIDCounter, 3, 0, 0, -1);
						}
					}
					PLPawnInventoryBase classLockerInventory8 = __instance.ClassLockerInventory;
					PLServer instance8 = PLServer.Instance;
					pawnInvItemIDCounter = instance8.PawnInvItemIDCounter;
					instance8.PawnInvItemIDCounter = pawnInvItemIDCounter + 1;
					classLockerInventory8.UpdateItem(pawnInvItemIDCounter, 4, 0, 0, -1);
					bool flag8 = ___m_ClassID == 2;
					if (flag8)
					{
						PLPawnInventoryBase classLockerInventory9 = __instance.ClassLockerInventory;
						PLServer instance9 = PLServer.Instance;
						pawnInvItemIDCounter = instance9.PawnInvItemIDCounter;
						instance9.PawnInvItemIDCounter = pawnInvItemIDCounter + 1;
						classLockerInventory9.UpdateItem(pawnInvItemIDCounter, 16, 0, 0, -1);
					}
					PLTabMenu.Instance.ShouldRecreateLocalInventory = true;
				}
				result = true;
			}
			return result;
		}
	}
}
