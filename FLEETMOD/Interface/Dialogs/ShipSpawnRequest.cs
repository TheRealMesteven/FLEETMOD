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
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData("Intrepid", new PLHailChoiceDelegateData(SpawnShip), 1));
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData("W.D. Cruisere", new PLHailChoiceDelegateData(SpawnShip), 2));
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData("Stargazer", new PLHailChoiceDelegateData(SpawnShip), 3)); // Sector Commander ? +_+
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData("Rolands", new PLHailChoiceDelegateData(SpawnShip), 4)); // TODO: Fix ID's
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData("W.D. Destroyer", new PLHailChoiceDelegateData(SpawnShip), 5));
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData("Carrier", new PLHailChoiceDelegateData(SpawnShip), 6));
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData("Fluffy One", new PLHailChoiceDelegateData(SpawnShip), 8));
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData("W.D. Annihilator", new PLHailChoiceDelegateData(SpawnShip), 9));
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData("SunCircler Nomad 5", new PLHailChoiceDelegateData(SpawnShip), 10));
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData("Interceptor", new PLHailChoiceDelegateData(SpawnShip), 11));
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData("Sylvassi Swordship", new PLHailChoiceDelegateData(SpawnShip), 12));
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData("Paladin", new PLHailChoiceDelegateData(SpawnShip), 13));
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
