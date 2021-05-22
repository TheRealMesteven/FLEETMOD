using PulsarPluginLoader;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace FLEETMOD.Interface.Dialogs
{
    class ShipSpawnRequest : PLHailTarget
    {
        // TODO: Add sync?
        public override string GetCurrentDialogueLeft()
        {
            return this.DialogTextLeft;
        }

        public override string GetCurrentDialogueRight()
        {
            return this.DialogTextRight;
        }

        public override string GetName() // Screen Name
        {
            return "Ship Spawn Request";
        }

        public override void Start() // MonoBehaviour::Start()
        {
            base.Start(); // base init
            FirstChoice();
        }

        private void FirstChoice()
        {
            this.m_AllChoices.Clear();
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Request", new PLHailChoiceDelegate(RequestShip)));
        }

        private void NewCaptainChoice()
        {
            this.DialogTextLeft += "\nAdmiral, who will be the captain of the new ship?";
            this.m_AllChoices.Clear();
            foreach(var possibleCaptain in PLServer.Instance.AllPlayers)
            {
                if (!possibleCaptain.IsBot && possibleCaptain.GetClassID() != 0) 
                    this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData(possibleCaptain.GetPlayerName(), new PLHailChoiceDelegateData(NewCaptain), possibleCaptain.GetPlayerID()));
            }
        }

        private void NewShipChoice()
        {
            this.DialogTextLeft += "\nAdmiral, select a new ship type!";
            this.m_AllChoices.Clear();
            int _Count = 0;
            foreach (string i in PLGlobal.Instance.PlayerShipNetworkPrefabNames)
            {
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData(PLGlobal.Instance.PlayerShipNetworkPrefabNames[_Count], new PLHailChoiceDelegateData(SpawnShip), _Count));
                _Count++;
            }
        }

        private void RequestShip(bool authority, bool local)
        {
            if (PhotonNetwork.isMasterClient && !IsRequested) // admiral check
            {
                IsRequested = true;
                this.DialogTextRight += "\nRequest";
                NewCaptainChoice();
            }
        }
        private void NewCaptain(int indata, bool authority, bool local)
        {
            if (PhotonNetwork.isMasterClient && !IsSelectedCaptain) // admiral check
            {
                IsSelectedCaptain = true;
                this.DialogTextRight += $"\n{PLServer.Instance.GetPlayerFromPlayerID(indata).GetPlayerName()}";
                NewCaptainID = 0;
                NewShipChoice();
            }
        }

        private void SpawnShip(int indata, bool authority, bool local)
        {
            if (PhotonNetwork.isMasterClient && !IsSpawned) // admiral check
            {
                IsSpawned = true;
                ModMessage.SendRPC("Dragon+Mest.Fleetmod", "FLEETMOD.ModMessages.ServerCreateShip", PhotonTargets.MasterClient, new object[]
                    {
                        indata,
                        NewCaptainID,
                        PLServer.Instance.CUShipNameGenerator.GetName(UnityEngine.Random.Range(0, 7000))
                    });
                GameObject.Destroy(this.gameObject);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExitButton() =>
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Cancel", new PLHailChoiceDelegate((authority, local) => { if (PhotonNetwork.isMasterClient) GameObject.Destroy(this.gameObject); })));

        private string DialogTextLeft = "Admiral, you can request a ship to your fleet!";
        private string DialogTextRight = string.Empty;
        private bool IsSpawned = false; // anti-double-spawn check
        private bool IsRequested = false; // anti-double-click check
        private bool IsSelectedCaptain = false; // anti-double-click check
        private int NewCaptainID = 0;
    }
}
