using HarmonyLib;

namespace FLEETMOD.Fixes
{
    [HarmonyPatch(typeof(PLServerClassInfo), "OnPhotonSerializeView")]
    internal class OnPhotonSerializeView
    {
        public static bool Prefix(PLServerClassInfo __instance, ref int ___m_ClassID)
        {
            if (!Variables.isrunningmod) return true;
            if (PhotonNetwork.isMasterClient && ___m_ClassID != -1 && PLServer.Instance != null && PLEncounterManager.Instance.PlayerShip != null && __instance.ClassLockerInventory.AllItems.Count <= 1)
            {
                int num = (PLEncounterManager.Instance.PlayerShip.FactionID == 1) ? 1 : 0;
                int pawnInvItemIDCounter;
                if (___m_ClassID == 3)
                {
                    PLPawnInventoryBase classLockerInventory = __instance.ClassLockerInventory;
                    PLServer instance = PLServer.Instance;
                    pawnInvItemIDCounter = instance.PawnInvItemIDCounter;
                    instance.PawnInvItemIDCounter = pawnInvItemIDCounter + 1;
                    classLockerInventory.UpdateItem(pawnInvItemIDCounter, 2, 0, 1 + num, -1);
                }
                else
                {
                    if (___m_ClassID == 2)
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
                if (PLEncounterManager.Instance.PlayerShip.FactionID == 3)
                {
                    if (___m_ClassID == 4)
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
                    if (___m_ClassID == 4)
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
                if (___m_ClassID == 2)
                {
                    PLPawnInventoryBase classLockerInventory9 = __instance.ClassLockerInventory;
                    PLServer instance9 = PLServer.Instance;
                    pawnInvItemIDCounter = instance9.PawnInvItemIDCounter;
                    instance9.PawnInvItemIDCounter = pawnInvItemIDCounter + 1;
                    classLockerInventory9.UpdateItem(pawnInvItemIDCounter, 16, 0, 0, -1);
                }
                PLTabMenu.Instance.ShouldRecreateLocalInventory = true;
            }
            return true;
        }
    }
}
