using PulsarPluginLoader;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Logger = PulsarPluginLoader.Utilities.Logger;

namespace FLEETMOD.Interface.Dialogs
{
    class FuelTransfer : PLHailTarget
    {
        public override string GetCurrentDialogueText()
        {
            return text;
        }
        public override string GetName() // Screen Name
        {
            return "Fuel Transfer";
        }

        public override void Start() // MonoBehaviour::Start()
        {
            base.Start(); // base init
            text = "Select the ship you want to transfer fuel to";
            TargetChoices();
        }

        private void TargetChoices()
        {
            this.m_AllChoices.Clear();

            string PlayerShipName = PLNetworkManager.Instance.LocalPlayer.GetShip().ShipName;
            
            foreach(var targetShip in Global.Fleet.Values)
            {
                var TargetShipName = PLEncounterManager.Instance.GetShipFromID(targetShip).ShipName;
                if(TargetShipName != PlayerShipName)
                    this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData(TargetShipName, new PLHailChoiceDelegateData(OnTarget), targetShip));
            }
            ExitButton();
        }
        
        private void AmountChoices()
        {
            text = "Select the amount of fuel to transfer";
            this.m_AllChoices.Clear();

            for (int count = 1; count <= PLNetworkManager.Instance.LocalPlayer.GetShip().NumberOfFuelCapsules; count++)
            {
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData(count.ToString(), new PLHailChoiceDelegateData(OnFuel), count));
                if (count == 10) break;
            }
            
            ExitButton();
        }

        private void OnTarget(int selectedShip, bool IsCaptain, bool local)
        {
            SelectedShip = selectedShip;
            AmountChoices();
        }

        private void OnFuel(int FuelAmount, bool IsCaptain, bool local)
        {
            if (!YetAnotherAntiDoubleClick)
            {
                YetAnotherAntiDoubleClick = true;
                foreach (var dict in Global.PlayerCrewList)
                {
                    Logger.Info(dict.Key + "=" + dict.Value);
                }
                
                ModMessage.SendRPC("Dragon+Mest.Fleetmod", "FLEETMOD.ModMessages.FuelTransfer", PhotonTargets.All, new object[]
                {
                    Global.Fleet[Global.PlayerCrewList[PLNetworkManager.Instance.LocalPlayerID]],
                    SelectedShip,
                    FuelAmount
                });
                
                GameObject.Destroy(this.gameObject);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExitButton() =>
           this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Cancel", new PLHailChoiceDelegate((authority, local) => { if (PhotonNetwork.isMasterClient) GameObject.Destroy(this.gameObject); })));

        private bool YetAnotherAntiDoubleClick = false;
        private string text;
        private int SelectedShip = 0;
    }
}
