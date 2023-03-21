using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using PulsarModLoader;
using UnityEngine;

namespace FLEETMOD.Setup
{   
    [HarmonyPatch(typeof(PLServer), "LoginMessage")] //Initial Sync. If multiple messages exist consider making new Modmessage for initial sync
    class LoginMessagePatch
    {
        static void Postfix(PhotonPlayer newPhotonPlayer)
        {
            if (PhotonNetwork.isMasterClient)
            {
                ModMessage.SendRPC(Mod.harmonyIden, "FLEETMOD.ModMessages.ActivateFleetmod", newPhotonPlayer, new object[] { });
            }
        }
    }
    [HarmonyPatch(typeof(PLServer), "Start")]
    class StartPatch
    { /// Initial Patch creating the dictionaries and lists.
        static void Postfix()
        {
            Variables.survivalBonusDict = new Dictionary<int, int>();
            Variables.Fleet = new Dictionary<int, List<int>>();
            Variables.DialogGenerated = false;
            Variables.Modded = new List<int>();
            Variables.NonModded = new List<int>();
            if (PhotonNetwork.isMasterClient)
            {
                Variables.UnModdedCrews = new Dictionary<int, int>();
                foreach (PulsarMod pulsarMod in ModManager.Instance.GetAllMods())
                {
                    if (pulsarMod.HarmonyIdentifier() == "mod.id107.beammeup")
                    {
                        PulsarModLoader.Utilities.Logger.Info("Fleetmod has disabled " + pulsarMod.Name + " due to mod conflicts.");
                        pulsarMod.Unload();
                    }
                }
            }   
        }
    }
}
