using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FLEETMOD.Interface.Tab
{
    internal class FleetShipListView
    {
        internal static GameObject ShipSelector = null;
        internal static int ShipID;

        /// <summary>
        /// Find existing tab features and curate the new implementations for the first time
        /// </summary>
        internal static void Initialize()
        {
            ShipSelector = new GameObject("FleetShipSelector");
            UnityEngine.Object.DontDestroyOnLoad(ShipSelector);
            VerticalLayoutGroup layout = ShipSelector.AddComponent<VerticalLayoutGroup>();
            layout.childForceExpandHeight = false;
        }

        internal static bool ShowShipList = false;
        internal static Transform CurrentParent = null;

        internal static void ChangeVisual(Transform parent, Vector3 offset)
        {
            CurrentParent = parent;
            if (parent != null)
            {
                ShipSelector.transform.SetParent(parent, false);
                ShipSelector.transform.localPosition = offset;
            }
        }

        /// <summary>
        /// First execution
        /// </summary>
        internal static void OnAwake()
        {
            ShipID = -1;
        }

        static List<ShipDisplay> allSDs = new List<ShipDisplay>();
        static float totalWidth = 600f;
        static float nameWidth = 210f;
        static float descWidth = 210f;
        static float rankStartX = 140f;
        static float rankSpacing = 20f;

        /// <summary>
        /// Update the tab features
        /// </summary>
        internal static void Update(PLTabMenu __instance)
        {
            if (!Variables.isrunningmod) return;
            if (ShowShipList)
            {
                if (ShipSelector != null && !ShipSelector.activeSelf) ShipSelector.SetActive(true);

                // Ship Change Button Addons
                // Add Talents-style ship list.
                List<ShipDisplay> list = new List<ShipDisplay>();
                if (PLTabMenu.Instance.TabMenuActive && Variables.Fleet != null)
                {
                    List<int> list2 = Variables.Fleet.Keys.ToList();
                    for (int i = 0; i < list2.Count; i++)
                    {
                        int shipID = list2[i];
                        ShipDisplay sd = GetShipDisplay(shipID);
                        int maxPlayers = 5;
                        if (sd == null)
                        {
                            sd = new ShipDisplay();
                            sd.ShipID = shipID;
                            GameObject gameObject = new GameObject();
                            sd.BG = gameObject.AddComponent<Image>();
                            Button button = gameObject.AddComponent<Button>();
                            ColorBlock colorBlock = default(ColorBlock);
                            colorBlock.normalColor = Color.black;
                            colorBlock.highlightedColor = PLPlayer.GetClassColorFromID(-1) * 0.5f;
                            colorBlock.selectedColor = colorBlock.highlightedColor;
                            colorBlock.pressedColor = Color.black;
                            colorBlock.colorMultiplier = 1f;
                            button.colors = colorBlock;
                            button.onClick.AddListener(delegate
                            {
                                PressSD(sd);
                            });

                            gameObject.transform.SetParent(ShipSelector.transform);
                            gameObject.transform.localRotation = Quaternion.identity;
                            gameObject.transform.localScale = Vector3.one;
                            sd.BG.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalWidth);
                            sd.BG.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 45f);
                            sd.BG.sprite = PLTabMenu.Instance.TalentBGSprite;
                            sd.BG.type = Image.Type.Sliced;
                            sd.BG.color = new Color(0.1f, 0.1f, 0.1f, 1f);

                            // Name Label
                            GameObject gameObject2 = new GameObject();
                            sd.Name = gameObject2.AddComponent<Text>();
                            gameObject2.transform.SetParent(gameObject.transform);
                            gameObject2.transform.localRotation = Quaternion.identity;
                            gameObject2.transform.localScale = Vector3.one;
                            gameObject2.transform.localPosition = new Vector3(-totalWidth / 2f + nameWidth / 2f + 10f, 0f);
                            gameObject2.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, nameWidth);
                            gameObject2.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 45f);
                            sd.Name.color = Color.white;
                            sd.Name.alignment = TextAnchor.MiddleLeft;
                            sd.Name.font = PLGlobal.Instance.MainFont;
                            sd.Name.resizeTextForBestFit = true;
                            sd.Name.resizeTextMaxSize = 8;
                            sd.Name.resizeTextMaxSize = 24;

                            // Desc Label
                            GameObject gameObject3 = new GameObject();
                            sd.Desc = gameObject3.AddComponent<Text>();
                            gameObject3.transform.SetParent(gameObject.transform);
                            gameObject3.transform.localRotation = Quaternion.identity;
                            gameObject3.transform.localScale = Vector3.one;
                            gameObject3.transform.localPosition = new Vector3(0, 0f);
                            gameObject3.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, descWidth);
                            gameObject3.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 45f);
                            sd.Desc.color = Color.white * 0.8f;
                            sd.Desc.alignment = TextAnchor.MiddleLeft;
                            sd.Desc.font = PLGlobal.Instance.MainFont;
                            sd.Desc.resizeTextForBestFit = true;
                            sd.Desc.resizeTextMaxSize = 8;
                            sd.Desc.resizeTextMaxSize = 14;

                            // Rank Icons
                            sd.Ranks = new Image[maxPlayers];
                            for (int j = 0; j < maxPlayers; j++)
                            {
                                GameObject gameObject4 = new GameObject();
                                gameObject4.transform.SetParent(gameObject.transform);
                                gameObject4.transform.localRotation = Quaternion.identity;
                                gameObject4.transform.localScale = Vector3.one;
                                float xPos = rankStartX + j * rankSpacing;
                                gameObject4.transform.localPosition = new Vector3(xPos, 0f);
                                sd.Ranks[j] = gameObject4.AddComponent<Image>();
                                sd.Ranks[j].sprite = PLTabMenu.Instance.TalentRankSprite;
                                gameObject4.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 20f);
                                gameObject4.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 20f);
                            }
                            sd.BG.gameObject.layer = 5;
                            foreach (object obj in sd.BG.transform)
                            {
                                ((Transform)obj).gameObject.layer = 5;
                            }
                            allSDs.Add(sd);
                            list.Add(sd);
                        }
                        else
                        {
                            list.Add(sd);
                        }
                        PLShipInfoBase ship = PLEncounterManager.Instance.GetShipFromID(shipID);
                        sd.Name.text = PLLocalize.Localize(ship.ShipNameValue, false);
                        PLPlayer Captain = PLServer.Instance.GetCachedFriendlyPlayerOfClass(0, ship);
                        sd.Desc.text = PLLocalize.Localize($"{ship.GetShipTypeName()} which is Captained by {(Captain == null ? "No-one" : Captain.GetPlayerName(false))}", false);
                        sd.Available = true;
                    }
                    allSDs.Sort(delegate (ShipDisplay a, ShipDisplay b)
                    {
                        if (a.Available != b.Available)
                        {
                            return b.Available.CompareTo(a.Available);
                        }
                        return a.Name.text.CompareTo(b.Name.text);
                    });
                    float num = 0f;
                    for (int k = 0; k < allSDs.Count; k++)
                    {
                        ShipDisplay shipDisplay = allSDs[k];
                        if (PLTabMenu.Instance.CurrentTabIndex == 0 && PLTabMenu.Instance.GetCrewPageIndex() == 0 && (EventSystem.current.currentSelectedGameObject == null || !EventSystem.current.currentSelectedGameObject.activeInHierarchy))
                        {
                            EventSystem.current.SetSelectedGameObject(shipDisplay.BG.gameObject);
                            shipDisplay.BG.GetComponent<Button>().Select();
                        }
                        shipDisplay.BG.transform.localPosition = new Vector3(0f, num, 0f);
                        Color color = new Color(0.66f, 0.66f, 0.66f, 1f);
                        //color = PLGlobal.Instance.ClassColors[0];
                        for (int l = 0; l < shipDisplay.Ranks.Length; l++)
                        {
                            if (shipDisplay.Available)
                            {
                                if (l < Variables.Fleet[shipDisplay.ShipID].Count())
                                {
                                    shipDisplay.Ranks[l].color = color;
                                }
                                else
                                {
                                    shipDisplay.Ranks[l].color = color * 0.5f;
                                }
                            }
                            else
                            {
                                shipDisplay.Ranks[l].color = color * 0.2f;
                            }
                        }
                        if (shipDisplay.Available)
                        {
                            shipDisplay.BG.color = color;
                            shipDisplay.Name.color = color;
                            shipDisplay.Desc.color = color;
                        }
                        else
                        {
                            shipDisplay.BG.color = color * 0.5f;
                            shipDisplay.Name.color = color * 0.5f;
                            shipDisplay.Desc.color = color * 0.5f;
                        }
                        num -= 55f;
                    }
                    //ShipScrollContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -num);
                }

                List<ShipDisplay> list3 = new List<ShipDisplay>();
                for (int m = 0; m < allSDs.Count; m++)
                {
                    ShipDisplay talentDisplay2 = allSDs[m];
                    if (!list.Contains(talentDisplay2))
                    {
                        list3.Add(talentDisplay2);
                    }
                }
                for (int n = 0; n < list3.Count; n++)
                {
                    ShipDisplay talentDisplay3 = list3[n];
                    global::UnityEngine.Object.Destroy(talentDisplay3.BG.gameObject);
                    allSDs.Remove(talentDisplay3);
                }
            }
            else
            {
                if (ShipSelector != null && ShipSelector.activeSelf) ShipSelector.SetActive(false);
            }
        }

        /// <summary>
        /// Method activated when user selects a ship in the ship display list on home page.
        /// </summary>

        public static void PressSD(ShipDisplay inSD)
        {
            PLMusic.PostEvent("play_titlemenu_ui_click", PLTabMenu.Instance.gameObject);
            PLShipInfoBase Ship = PLEncounterManager.Instance.GetShipFromID(inSD.ShipID);
            if (Ship == null)
            {
                PLTabMenu.Instance.TimedErrorMsg = PLLocalize.Localize("Ship doesnt exist!", false);
                return;
            }
            ShipID = inSD.ShipID;
            return;
        }
        static ShipDisplay GetShipDisplay(int inShipId)
        {
            for (int i = 0; i < allSDs.Count; i++)
            {
                ShipDisplay talentDisplay = allSDs[i];
                if (talentDisplay.ShipID == inShipId)
                {
                    return talentDisplay;
                }
            }
            return null;
        }

        public class ShipDisplay
        {
            public int ShipID;
            public Text Name;
            public Text Desc;
            public Image BG;
            public Image[] Ranks;
            public bool Available;
        }
    }
}
