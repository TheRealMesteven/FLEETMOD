﻿using PulsarPluginLoader;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace FLEETMOD.Interface.Dialogs
{
    class FleetManager : PLHailTarget
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
            return "Faction Fleet Manager";
        }

        public override void Start() // MonoBehaviour::Start()
        {
            base.Start(); // base init
            currentdialog = 0;
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Request Captain for Ship", new PLHailChoiceDelegate(RCSShipChoice)));
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Store Ship in storage", new PLHailChoiceDelegate(SSShipChoice)));
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Withdraw Ship from storage", new PLHailChoiceDelegate(SSShipChoice)));
            ExitButton();
        }
        /// <summary>
        /// Below Lines are for Assigning Captain to Unmanned Ship
        /// </summary>
        private void RCSShipChoice(bool authority, bool local)
        {
            this.m_AllChoices.Clear();
            if (PhotonNetwork.isMasterClient && currentdialog < 1) // admiral check
            {
                currentdialog = 1;
                this.DialogTextLeft += "\nAdmiral, what ship requires a Captain?";
                foreach (PLShipInfo possibleShip in PLEncounterManager.Instance.AllShips.Values)
                {
                    if (possibleShip != null && possibleShip.TagID == -23 && PLServer.Instance.GetCachedFriendlyPlayerOfClass(0,possibleShip) == null)
                    {
                        this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData(possibleShip.ShipNameValue, new PLHailChoiceDelegateData(RCSShipDefine), possibleShip.ShipID));
                    }
                }
            }
            ExitButton();
        }
        private void RCSShipDefine(int indata, bool authority, bool local)
        {
            if (PhotonNetwork.isMasterClient && currentdialog < 2) // admiral check
            {
                currentdialog = 2;
                this.DialogTextRight += $"\n{PLEncounterManager.Instance.GetShipFromID(indata).ShipNameValue}";
                newShipID = indata;
                RCSCaptainChoice();
            }
        }
        private void RCSCaptainChoice()
        {
            this.m_AllChoices.Clear();
            if (PhotonNetwork.isMasterClient && currentdialog < 3) // admiral check
            {
                currentdialog = 3;
                this.DialogTextLeft += "\nAdmiral, who will be the captain of the new ship?";
                foreach (var possibleCaptain in PLServer.Instance.AllPlayers)
                {
                    if (!possibleCaptain.IsBot && possibleCaptain.GetClassID() != 0)
                        this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData(possibleCaptain.GetPlayerName(), new PLHailChoiceDelegateData(RCSCaptainDefine), possibleCaptain.GetPlayerID()));
                }
            }
            ExitButton();
        }
        private void RCSCaptainDefine(int indata, bool authority, bool local)
        {
            if (PhotonNetwork.isMasterClient && currentdialog < 4) // admiral check
            {
                currentdialog = 4;
                PLServer.Instance.GetPlayerFromPlayerID(indata).SetClassID(0);
                PLServer.Instance.GetPlayerFromPlayerID(indata).GetPhotonPlayer().SetScore(indata);
                GameObject.Destroy(this.gameObject);
            }
        }
        ///
        /// <summary>
        /// Below lines are for storing ships
        /// </summary>
        private void SSShipChoice(bool authority, bool local)
        {
            this.m_AllChoices.Clear();
            if (PhotonNetwork.isMasterClient && currentdialog < 1) // admiral check
            {
                currentdialog = 1;
                this.DialogTextLeft += "\nAdmiral, what ship do you want to store?";
                foreach (PLShipInfo possibleShip in PLEncounterManager.Instance.AllShips.Values)
                {
                    if (possibleShip != null && possibleShip.TagID == -23 && PLServer.Instance.GetCachedFriendlyPlayerOfClass(0, possibleShip) == PLServer.Instance.GetPlayerFromPlayerID(0))
                    {
                        this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData(possibleShip.ShipNameValue, new PLHailChoiceDelegateData(SSCrewReassignment), possibleShip.ShipID));
                    }
                }
            }
            ExitButton();
        }
        private void SSCrewReassignment(int indata, bool authority, bool local)
        {
            this.m_AllChoices.Clear();
            this.DialogTextLeft += "\nAlright Admiral, we are currently storing the ship for your future use.";
            if (PhotonNetwork.isMasterClient && currentdialog < 2)
            {
                currentdialog = 2;
                foreach (PLPlayer pLPlayer in PLServer.Instance.AllPlayers)
                {
                    if (pLPlayer != null && pLPlayer.GetPhotonPlayer().GetScore() == indata)
                    {
                        pLPlayer.SetClassID(1);
                        pLPlayer.GetPhotonPlayer().SetScore(PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().GetScore());
                        pLPlayer.photonView.RPC("NetworkTeleportToSubHub", PhotonTargets.All, new object[]
                        {
                            (PLEncounterManager.Instance.GetShipFromID(pLPlayer.GetPhotonPlayer().GetScore()) as PLShipInfo).MyTLI.SubHubID,
                            0
                        });
                    }
                }
                ShipStorage.StoreShip(indata);
                GameObject.Destroy(this.gameObject);
            }
        }
        /// 
        /// <summary>
        /// Below lines are for spawning ships from storage
        /// </summary>
        private void LSShipChoice(bool authority, bool local)
        {
            this.m_AllChoices.Clear();
            if (PhotonNetwork.isMasterClient && currentdialog < 1) // admiral check
            {
                currentdialog = 1;
                this.DialogTextLeft += "\nAdmiral, what ship do you want to withdraw from storage?";
                int Count = 0;
                foreach (string possibleShip in ShipLoading.GetShipList(true))
                {
                    this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData(Count + " " + possibleShip, new PLHailChoiceDelegateData(LSSpawnShip), Count));
                    Count++;
                }
            }
            ExitButton();
        }
        private void LSSpawnShip(int indata, bool authority, bool local)
        {
            this.m_AllChoices.Clear();
            this.DialogTextLeft += "\nAlright Admiral, we are currently withdrawing the ship from your personal hanger.";
            if (PhotonNetwork.isMasterClient && currentdialog < 2)
            {
                currentdialog = 2;
                ShipLoading.LoadShip(indata, true);
                GameObject.Destroy(this.gameObject);
            }
        }
        /// 

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExitButton()
        {
            this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Cancel", new PLHailChoiceDelegate((authority, local) => { if (PhotonNetwork.isMasterClient) GameObject.Destroy(this.gameObject); MyVariables.DialogGenerated = false; })));
        }

        private string DialogTextLeft = "Good evening Admiral, what can we do for you today?";
        private string DialogTextRight = string.Empty;
        private int currentdialog = 0;
        private int newShipID = 0;
    }
}
/// Notes:
/// Remove Double Clicking, Badruiners method is by adding boolean checks to prevent repeats (NEEDS TESTING)
/// Remove destroy gameobject, destroy if leaving the sector. (NEEDS TESTING)
