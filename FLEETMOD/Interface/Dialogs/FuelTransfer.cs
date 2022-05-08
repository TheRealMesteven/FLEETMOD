using PulsarModLoader;
using System.Runtime.CompilerServices;
using UnityEngine;

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
            text = "Select the ship you want to transfer fuel from";
            FromChoices();
        }
        private void FromChoices()
        {
            this.m_AllChoices.Clear();

            string PlayerShipName = PLNetworkManager.Instance.LocalPlayer.StartingShip.ShipName;

            foreach (PLShipInfo targetShip in PLEncounterManager.Instance.AllShips.Values)
            {
                if (targetShip != null && targetShip.TagID == -23)
                {
                    var TargetShipName = PLEncounterManager.Instance.GetShipFromID(targetShip.ShipID).ShipName;
                    this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData(TargetShipName + " (" + targetShip.NumberOfFuelCapsules.ToString() + ")", new PLHailChoiceDelegateData(OnFrom), targetShip.ShipID));
                }
            }
            ExitButton();
        }
        private void OnFrom(int selectedShip, bool IsCaptain, bool local)
        {
            FromShip = selectedShip;
            TargetChoices();
        }

        private void TargetChoices()
        {
            this.m_AllChoices.Clear();

            string PlayerShipName = PLEncounterManager.Instance.GetShipFromID(FromShip).ShipName;
            text = "Select the ship you want " + PlayerShipName + " to transfer fuel to (" + PLEncounterManager.Instance.GetShipFromID(FromShip).NumberOfFuelCapsules.ToString() + ")";

            foreach (PLShipInfo targetShip in PLEncounterManager.Instance.AllShips.Values)
            {
                if (targetShip != null && targetShip.TagID == -23)
                {
                    var TargetShipName = PLEncounterManager.Instance.GetShipFromID(targetShip.ShipID).ShipName;
                    if (TargetShipName != PlayerShipName)
                    {
                        this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData(TargetShipName + " (" + targetShip.NumberOfFuelCapsules.ToString() + ")", new PLHailChoiceDelegateData(OnTarget), targetShip.ShipID));
                    }
                }
            }
            ExitButton();
        }

        private void OnTarget(int selectedShip, bool IsCaptain, bool local)
        {
            SelectedShip = selectedShip;
            AmountChoices();
        }

        private void AmountChoices()
        {
            text = "Select the amount of fuel to transfer (" + PLEncounterManager.Instance.GetShipFromID(FromShip).NumberOfFuelCapsules.ToString() + ") -> (" + PLEncounterManager.Instance.GetShipFromID(SelectedShip).NumberOfFuelCapsules.ToString() + ")";
            this.m_AllChoices.Clear();

            for (int count = 1; count <= PLNetworkManager.Instance.LocalPlayer.StartingShip.NumberOfFuelCapsules; count++)
            {
                this.m_AllChoices.Add(new PLHailChoice_SimpleCustomData(count.ToString(), new PLHailChoiceDelegateData(OnFuel), count));
                if (count == 10) break;
            }

            ExitButton();
        }

        private void OnFuel(int FuelAmount, bool IsCaptain, bool local)
        {
            if (!YetAnotherAntiDoubleClick)
            {
                YetAnotherAntiDoubleClick = true;
                PulsarModLoader.ModMessage.SendRPC("Dragon+Mest.Fleetmod", "FLEETMOD.TransferFuel", PhotonTargets.All, new object[]
                {
                    FromShip,
                    SelectedShip,
                    FuelAmount
                });
                MyVariables.FuelDialog = false;
                GameObject.Destroy(this.gameObject);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExitButton() =>
           this.m_AllChoices.Add(new PLHailChoice_SimpleCustom("Cancel", new PLHailChoiceDelegate((authority, local) => { GameObject.Destroy(this.gameObject); MyVariables.FuelDialog = false; })));

        private bool YetAnotherAntiDoubleClick = false;
        private string text;
        private int SelectedShip = 0;
        private int FromShip = 0;
    }
}