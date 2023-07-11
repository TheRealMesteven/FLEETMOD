using HarmonyLib;
using PulsarModLoader;
using UnityEngine;

namespace FLEETMOD.Bot
{
    [HarmonyPatch(typeof(PLServer), "ServerAddCrewBotPlayer")]
    internal class ServerAddCrewBotPlayer
    {
        public static bool Prefix(PLServer __instance, int inClass)
        {
            if (!Variables.isrunningmod) return true;
            if (PLNetworkManager.Instance != null && PLNetworkManager.Instance.CurrentGame != null && PLNetworkManager.Instance.LocalPlayer != null)
            {
                if (!PhotonNetwork.isMasterClient) ModMessage.SendRPC(Mod.harmonyIden, "FLEETMOD.ModMessages.RequestToSpawnBot", PhotonNetwork.masterClient, new object[] { inClass });
                else if (AbleToAddBotPlayer(PhotonNetwork.masterClient, inClass, out int PlayerID)) AddCrewBotPlayer(PLNetworkManager.Instance.LocalPlayer, PlayerID, inClass);
            }
            return false;
        }

        internal static bool AbleToAddBotPlayer(PhotonPlayer sender, int ClassId, out int PlayerID)
        {
            PlayerID = PLServer.Instance.GetLowestAvailablePlayerID();
            return true;
        }

        internal static void AddCrewBotPlayer(PLPlayer pLPlayer, int PlayerID, int inClass)
        {
            if (pLPlayer.StartingShip == null) return;
            PulsarModLoader.Utilities.Messaging.Echo(PhotonTargets.All, $"Spawning Bot With PlayerID {PlayerID}");
            PLPlayer component = PhotonNetwork.Instantiate("NetworkPrefabs/PLPlayer", Vector3.zero, Quaternion.identity, 0, null).GetComponent<PLPlayer>();
            PLServer.Instance.AddPlayer(component);
            component.SetPlayerID(PlayerID);
            component.SetClassID(inClass);
            component.IsBot = true;
            component.TeamID = 0;
            component.StartingShip = pLPlayer.StartingShip;
            component.SetSubHubAndTTIID(pLPlayer.StartingShip.MyTLI.SubHubID, 0);
            component.OnPlanet = false;
            switch (inClass)
            {
                case 0:
                    component.SetPlayerName("CapBot");
                    break;
                case 1:
                    component.SetPlayerName("PiBot");
                    break;
                case 2:
                    component.SetPlayerName("SciBot");
                    break;
                case 3:
                    component.SetPlayerName("WeapBot");
                    break;
                case 4:
                    component.SetPlayerName("EngBot");
                    break;
                default:
                    component.SetPlayerName("CrewBot");
                    break;
            }
            component.gameObject.name = component.GetPlayerName(false);
            PLServer.Instance.photonView.RPC("LoginMessageForBot", PhotonTargets.All, new object[]
            {
                    component.GetPlayerName(false),
                    component.GetClassID()
            });
            PLBot plbot = component.gameObject.AddComponent<PLBot>();
            plbot.PlayerOwner = component;
            component.MyBot = plbot;
            AIDataIndividual aidataIndividual = null;
            if (PLGlobal.Instance.LoadedAIData != null && PLGlobal.Instance.LoadedAIData.ClassData.Length > inClass && inClass > 0)
            {
                aidataIndividual = PLGlobal.Instance.LoadedAIData.ClassData[inClass - 1];
            }
            if (aidataIndividual != null)
            {
                component.RaceID = aidataIndividual.RaceID;
                component.Gender_IsMale = aidataIndividual.Gender_IsMale;
                component.RaceAndGenderHaveBeenSet = true;
                string str = "ind: ";
                AIDataIndividual aidataIndividual2 = aidataIndividual;
                Debug.Log(str + ((aidataIndividual2 != null) ? aidataIndividual2.ToString() : null));
                Debug.Log("RaceID: " + aidataIndividual.RaceID.ToString());
                Debug.Log("Gender_IsMale: " + aidataIndividual.Gender_IsMale.ToString());
            }
            component.SpawnLocalPawnForBotPlayer(PLNetworkManager.Instance.CurrentGame.GetSpawnTransformForClass(inClass, pLPlayer.StartingShip));
            if (component.GetPawn() != null)
            {
                component.GetPawn().CurrentShip = pLPlayer.StartingShip;
                ((PLBotController)component.GetPawn().MyController).MyBot = plbot;
            }
        }
    }
}
