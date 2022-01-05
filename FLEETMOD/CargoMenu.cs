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
        private static float Width = 0.2f;
        private static float Height = 0.4f;
        public static List<int> inCurrentShipID = new List<int>();
        public static List<int> inNetID = new List<int>();
        private static GUISkin _cachedSkin;
        private static readonly Color32 _classicMenuBackground = new Color32(32, 32, 32, byte.MaxValue);
        private static readonly Color32 _classicButtonBackground = new Color32(40, 40, 40, byte.MaxValue);
        private static readonly Color32 _hoverButtonFromMenu = new Color32(18, 79, 179, byte.MaxValue);

        internal CargoMenu()
        {
            CargoMenu.Instance = this;
        }
        public void OnGUI()
        {
            this.Window = GUI.Window(999930, this.Window, new GUI.WindowFunction(this.WindowFunction), "Cargo Transfer");
        }
        private void WindowFunction(int WindowID)
        {
            GUILayout.BeginVertical();
            if (PLServer.Instance != null && CargoMenu.inCurrentShipID.Count() == 0 && PLEncounterManager.Instance != null)
            {
                var TransferMenu = UnityEngine.GameObject.FindObjectOfType<CargoMenu>();
                UnityEngine.GameObject.Destroy(TransferMenu);
                MyVariables.CargoMenu = false;
                List<int> inCurrentShipID = new List<int>();
                List<int> inNetID = new List<int>();
            }
            else
            {
                GUI.skin = this.ChangeSkin();
                PLShipInfo plshipInfo = PLEncounterManager.Instance.GetShipFromID(inCurrentShipID[0]) as PLShipInfo;
                if (plshipInfo != null)
                {
                    PLShipComponent componentFromNetID = plshipInfo.MyStats.GetComponentFromNetID(inNetID[0]);
                    if (componentFromNetID != null)
                    {
                        GUILayout.Label($"\n[FleetManager] Select which ship to transfer \n{componentFromNetID.Name} to . . .");
                        foreach (PLShipInfo pLShipInfo in PLEncounterManager.Instance.AllShips.Values)
                        {
                            if (pLShipInfo != null && pLShipInfo.TagID == -23)
                            {
                                if (GUILayout.Button($"Send to {pLShipInfo.ShipNameValue}"))
                                {
                                    if (pLShipInfo.MyStats.GetSlot(ESlotType.E_COMP_CARGO).Count != pLShipInfo.MyStats.GetSlot(ESlotType.E_COMP_CARGO).MaxItems)
                                    {
                                                    pLShipInfo.MyStats.AddShipComponent(PLWare.CreateFromHash(1, (int)PLShipComponent.createHashFromInfo((int)componentFromNetID.ActualSlotType, componentFromNetID.SubType, componentFromNetID.Level, 0, 12)) as PLShipComponent, -1, ESlotType.E_COMP_CARGO);
                                                    plshipInfo.MyStats.RemoveShipComponentByNetID(inNetID[0]);
                                                    inCurrentShipID.Remove(inCurrentShipID[0]);
                                                    inNetID.Remove(inNetID[0]);
                                    }
                                    else
                                    {
                                        PulsarModLoader.Utilities.Messaging.Notification($"[FM] The {pLShipInfo.ShipNameValue} is full on cargo!", PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer(), 0, 4000, false);
                                    }
                                }
                            }
                        }
                    }
                }
                if (GUILayout.Button("Cancel"))
                {
                    var TransferMenu = UnityEngine.GameObject.FindObjectOfType<CargoMenu>();
                    UnityEngine.GameObject.Destroy(TransferMenu);
                    MyVariables.CargoMenu = false;
                    List<int> inCurrentShipID = new List<int>();
                    List<int> inNetID = new List<int>();
                }
            }
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        private GUISkin ChangeSkin()
        {
            if (_cachedSkin == null || _cachedSkin.window.active.background == null)
            {
                _cachedSkin = GUI.skin;
                Texture2D texture2D = this.BuildTexFrom1Color(_classicMenuBackground);
                _cachedSkin.window.active.background = texture2D;
                _cachedSkin.window.onActive.background = texture2D;
                _cachedSkin.window.focused.background = texture2D;
                _cachedSkin.window.onFocused.background = texture2D;
                _cachedSkin.window.hover.background = texture2D;
                _cachedSkin.window.onHover.background = texture2D;
                _cachedSkin.window.normal.background = texture2D;
                _cachedSkin.window.onNormal.background = texture2D;
                _cachedSkin.window.hover.textColor = Color.white;
                _cachedSkin.window.onHover.textColor = Color.white;
                Texture2D texture2D2 = this.BuildTexFrom1Color(_classicButtonBackground);
                Texture2D texture2D3 = this.BuildTexFrom1Color(_hoverButtonFromMenu);
                _cachedSkin.button.active.background = texture2D2;
                _cachedSkin.button.onActive.background = texture2D2;
                _cachedSkin.button.focused.background = texture2D2;
                _cachedSkin.button.onFocused.background = texture2D2;
                _cachedSkin.button.hover.background = texture2D3;
                _cachedSkin.button.onHover.background = texture2D3;
                _cachedSkin.button.normal.background = texture2D2;
                _cachedSkin.button.onNormal.background = texture2D2;
                _cachedSkin.horizontalSlider.active.background = PLGlobal.Instance.SliderBG;
                _cachedSkin.horizontalSlider.onActive.background = PLGlobal.Instance.SliderBG;
                _cachedSkin.horizontalSlider.focused.background = PLGlobal.Instance.SliderBG;
                _cachedSkin.horizontalSlider.onFocused.background = PLGlobal.Instance.SliderBG;
                _cachedSkin.horizontalSlider.hover.background = PLGlobal.Instance.SliderBG;
                _cachedSkin.horizontalSlider.onHover.background = PLGlobal.Instance.SliderBG;
                _cachedSkin.horizontalSlider.normal.background = PLGlobal.Instance.SliderBG;
                _cachedSkin.horizontalSlider.onNormal.background = PLGlobal.Instance.SliderBG;
                _cachedSkin.horizontalSliderThumb.active.background = PLGlobal.Instance.SliderHandle;
                _cachedSkin.horizontalSliderThumb.onActive.background = PLGlobal.Instance.SliderHandle;
                _cachedSkin.horizontalSliderThumb.focused.background = PLGlobal.Instance.SliderHandle;
                _cachedSkin.horizontalSliderThumb.onFocused.background = PLGlobal.Instance.SliderHandle;
                _cachedSkin.horizontalSliderThumb.hover.background = PLGlobal.Instance.SliderHandle;
                _cachedSkin.horizontalSliderThumb.onHover.background = PLGlobal.Instance.SliderHandle;
                _cachedSkin.horizontalSliderThumb.normal.background = PLGlobal.Instance.SliderHandle;
                _cachedSkin.horizontalSliderThumb.onNormal.background = PLGlobal.Instance.SliderHandle;
                Texture2D texture2D4 = this.BuildTexFromColorArray(new Color[]
                {
                    _classicButtonBackground,
                    _classicButtonBackground,
                    _classicMenuBackground,
                    _classicMenuBackground,
                    _classicMenuBackground,
                    _classicMenuBackground,
                    _classicMenuBackground
                }, 1, 7);
                _cachedSkin.textField.active.background = texture2D4;
                _cachedSkin.textField.onActive.background = texture2D4;
                _cachedSkin.textField.focused.background = texture2D4;
                _cachedSkin.textField.onFocused.background = texture2D4;
                _cachedSkin.textField.hover.background = texture2D4;
                _cachedSkin.textField.onHover.background = texture2D4;
                _cachedSkin.textField.normal.background = texture2D4;
                _cachedSkin.textField.onNormal.background = texture2D4;
                _cachedSkin.textField.active.textColor = _hoverButtonFromMenu;
                _cachedSkin.textField.onActive.textColor = _hoverButtonFromMenu;
                _cachedSkin.textField.hover.textColor = _hoverButtonFromMenu;
                _cachedSkin.textField.onHover.textColor = _hoverButtonFromMenu;
                UnityEngine.Object.DontDestroyOnLoad(texture2D);
                UnityEngine.Object.DontDestroyOnLoad(texture2D2);
                UnityEngine.Object.DontDestroyOnLoad(texture2D3);
                UnityEngine.Object.DontDestroyOnLoad(texture2D4);
                UnityEngine.Object.DontDestroyOnLoad(_cachedSkin);
            }
            return _cachedSkin;
        }
        private Texture2D BuildTexFrom1Color(Color color)
        {
            Texture2D texture2D = new Texture2D(1, 1);
            texture2D.SetPixel(0, 0, color);
            texture2D.Apply();
            return texture2D;
        }
        private Texture2D BuildTexFromColorArray(Color[] color, int width, int height)
        {
            Texture2D texture2D = new Texture2D(width, height);
            texture2D.SetPixels(color);
            texture2D.Apply();
            return texture2D;
        }
    }
}
