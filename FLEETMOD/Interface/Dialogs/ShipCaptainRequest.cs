using PulsarPluginLoader;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace FLEETMOD.Interface.Dialogs
{
    class ShipCaptainRequest : PLHailTarget
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
            return "Assign Captain To Ship";
        }

        public override void Start() // MonoBehaviour::Start()
        {
            base.Start(); // base init
            currentdialog = 0;
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Promote a Captain for new ship", new PLHailChoiceDelegate(NewShipChoice)));
            ExitButton();
        }
        private void NewShipChoice(bool authority, bool local)
        {
            this.DialogTextLeft += "\nAdmiral, what ship requires a Captain?";
            this.m_AllChoices.Clear();
            if (PhotonNetwork.isMasterClient && currentdialog < 1) // admiral check
            {
                currentdialog = 1;
                foreach (PLShipInfo possibleShip in PLEncounterManager.Instance.AllShips.Values)
                {
                    if (possibleShip != null && possibleShip.TagID == -23 && PLServer.Instance.GetCachedFriendlyPlayerOfClass(0,possibleShip) == null)
                    {
                        this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData(possibleShip.ShipNameValue, new PLHailChoiceDelegateData(NewShip), possibleShip.ShipID));
                    }
                }
            }
            ExitButton();
        }
        private void NewShip(int indata, bool authority, bool local)
        {
            if (PhotonNetwork.isMasterClient && currentdialog < 2) // admiral check
            {
                currentdialog = 2;
                this.DialogTextRight += $"\n{PLEncounterManager.Instance.GetShipFromID(indata).ShipNameValue}";
                newShipID = indata;
                NewCaptainChoice();
            }
        }
        private void NewCaptainChoice()
        {
            this.DialogTextLeft += "\nAdmiral, who will be the captain of the new ship?";
            this.m_AllChoices.Clear();
            if (PhotonNetwork.isMasterClient && currentdialog < 3) // admiral check
            {
                currentdialog = 3;
                foreach (var possibleCaptain in PLServer.Instance.AllPlayers)
                {
                    if (!possibleCaptain.IsBot && possibleCaptain.GetClassID() != 0)
                        this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData(possibleCaptain.GetPlayerName(), new PLHailChoiceDelegateData(NewCaptain), possibleCaptain.GetPlayerID()));
                }
            }
            ExitButton();
        }
        private void NewCaptain(int indata, bool authority, bool local)
        {
            if (PhotonNetwork.isMasterClient && currentdialog < 4) // admiral check
            {
                currentdialog = 4;
                PLServer.Instance.GetPlayerFromPlayerID(indata).SetClassID(0);
                PLServer.Instance.GetPlayerFromPlayerID(indata).GetPhotonPlayer().SetScore(indata);
                GameObject.Destroy(this.gameObject);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExitButton() =>
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Cancel", new PLHailChoiceDelegate((authority, local) => { if (PhotonNetwork.isMasterClient) GameObject.Destroy(this.gameObject); })));

        private string DialogTextLeft = "Good evening Admiral, what can we do for you today?";
        private string DialogTextRight = string.Empty;
        private int currentdialog = 0;
        private int newShipID = 0;
    }
}
/// Notes:
/// Remove Double Clicking, Badruiners method is by adding boolean checks to prevent repeats (NEEDS TESTING)
/// Remove destroy gameobject, destroy if leaving the sector. (NEEDS TESTING)
