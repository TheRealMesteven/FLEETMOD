using Photon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FLEETMOD
{
    internal class CargoMenu : UnityEngine.MonoBehaviour
    {
        public static CargoMenu Instance = null;
        private Rect Window = new Rect((float)Screen.width * 0.5f - (float)Screen.width * CargoMenu.Width / 2f, (float)Screen.height * 0.5f - (float)Screen.height * CargoMenu.Height / 2f, (float)Screen.width * CargoMenu.Width, (float)Screen.height * CargoMenu.Height);
        private static float Width = 0.4f;
        private static float Height = 0.4f;
        public static int inCurrentShipID;
        public static int inNetID;
        public static Dictionary<int, int> CargoTransfer = new Dictionary<int, int>();
        /// inCurrentShipID | inNetID
        internal CargoMenu()
        {
            CargoMenu.Instance = this;
        }
        public void OnGUI()
        {
            this.Window = GUI.Window(999920, this.Window, new GUI.WindowFunction(this.WindowFunction), "Cargo Transfer");
        }
        private void WindowFunction(int WindowID)
        {
            GUILayout.BeginVertical();
            PLShipInfo plshipInfo = PLEncounterManager.Instance.GetShipFromID(CargoTransfer.First().Key) as PLShipInfo;
            PLShipComponent componentFromNetID = plshipInfo.MyStats.GetComponentFromNetID(CargoTransfer.First().Value);
            GUILayout.Label($"\n[FleetManager] {componentFromNetID.Name}");
            foreach (PLShipInfo pLShipInfo in PLEncounterManager.Instance.AllShips.Values)
            {
                if (pLShipInfo != null && pLShipInfo.TagID == -23)
                {
                    if (GUILayout.Button($"Send to {pLShipInfo.ShipNameValue}"))
                    {
                        if (pLShipInfo.MyStats.GetSlot(ESlotType.E_COMP_CARGO).Count != pLShipInfo.MyStats.GetSlot(ESlotType.E_COMP_CARGO).MaxItems)
                        {
                            if (PLEncounterManager.Instance != null)
                            {
                                if (plshipInfo != null)
                                {
                                    if (componentFromNetID != null)
                                    {
                                        pLShipInfo.MyStats.AddShipComponent(PLWare.CreateFromHash(1, (int)PLShipComponent.createHashFromInfo((int)componentFromNetID.ActualSlotType, componentFromNetID.SubType, componentFromNetID.Level, 0, 12)) as PLShipComponent, -1, ESlotType.E_COMP_CARGO);
                                        plshipInfo.MyStats.RemoveShipComponentByNetID(inNetID);
                                        var TransferMenu = UnityEngine.GameObject.FindObjectOfType<CargoMenu>();
                                        UnityEngine.GameObject.Destroy(TransferMenu);
                                        MyVariables.CargoMenu = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            PulsarModLoader.Utilities.Messaging.Notification($"[FM] The {pLShipInfo.ShipNameValue} is full on cargo!", PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer(), 0, 4000, false);
                        }
                    }
                }
            }
            if (GUILayout.Button("Cancel"))
            {
                var TransferMenu = UnityEngine.GameObject.FindObjectOfType<CargoMenu>();
                UnityEngine.GameObject.Destroy(TransferMenu);
                MyVariables.CargoMenu = false;
            }
        }
    }
}
