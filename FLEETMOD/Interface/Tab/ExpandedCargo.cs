using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static FLEETMOD.Interface.Tab.HomeTabMenu;
using static HarmonyLib.AccessTools;
using static PulsarModLoader.Patches.HarmonyHelpers;

namespace FLEETMOD.Interface.Tab
{
    internal class ExpandedCargo
    {
        static GameObject ShipSelector;
        /// <summary>
        /// Find existing tab features and curate the new implementations for the first time
        /// </summary>
        internal static void Initialize()
        {
            GameObject SHIP_Stats1 = PLTabMenu.Instance.SHIP_Stats1.gameObject;
            ShipSelector = new GameObject("MultiShipSelector");
            ShipSelector.transform.SetParent(SHIP_Stats1.transform, false);

            VerticalLayoutGroup layout = ShipSelector.AddComponent<VerticalLayoutGroup>();
            layout.transform.localPosition += new Vector3(0, 60, 0);
            layout.childForceExpandHeight = false;
            return;
        }

        /// <summary>
        /// First execution
        /// </summary>
        internal static void OnAwake()
        {
            return;
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
            if (__instance.TabMenuActive && __instance.ExpandedComponentView && __instance.CurrentTabIndex == 1)
            {
                if (!ShipSelector.activeSelf) ShipSelector.SetActive(true);

                // Attribute Override
                PLShipInfoBase Ship = null;
                if (UpdateLabels.ShipID == -1)
                {
                    Ship = PLNetworkManager.Instance.LocalPlayer.StartingShip;
                }
                else
                {
                    Ship = PLEncounterManager.Instance.GetShipFromID(UpdateLabels.ShipID);
                }
                if (Ship != null) 
                {
                    __instance.SHIP_Stats2.text = $"Currently Viewing\n{Ship.ShipNameValue} • {(PLNetworkManager.Instance.LocalPlayer.StartingShip == Ship ? "Your Ship" : "A Fleet Ship")}\n{Ship.GetShipTypeName()}";
                    __instance.SHIP_Stats2.enabled = true;
                }
                __instance.SHIP_Stats1.text = "Fleetmod Ships";
                __instance.SHIP_Stats3.text = "Stats";
                __instance.SHIP_Stats1.enabled = true;
                __instance.SHIP_Stats3.enabled = true;

                // Ship Change Button Addons
                // Add Talents-style ship list.
                List<ShipDisplay> list = new List<ShipDisplay>();
                if (PLTabMenu.Instance.TabMenuActive)
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
                if (ShipSelector.activeSelf) ShipSelector.SetActive(false);
            }
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
    }

    [HarmonyPatch(typeof(PLTabMenu), "UpdateSCDs")]
    class DisableCargoPageStats
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> target = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(PLEncounterManager), "Instance")),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PLLevelSync), "PlayerShip")),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PLShipInfoBase), "MyStats")),
                new CodeInstruction(OpCodes.Brfalse)
            }; //PLEncounterManager.Instance.PlayerShip.MyStats != null

            List<CodeInstruction> patch = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Call, Method(typeof(DisableCargoPageStats), "Replacement"))
            };

            int index = FindSequence(instructions, target, CheckMode.NONNULL);
            patch.Add(instructions.ToList()[index - 1]); // Used to differentiate between if statement and the for loop MyStats

            return PatchBySequence(instructions, target, new List<CodeInstruction>(), PatchMode.REPLACE, CheckMode.ALWAYS, showDebugOutput: false);
        }
        public static bool Replacement()
        {
            return !Variables.isrunningmod && PLEncounterManager.Instance.PlayerShip.MyStats != null;
        }
    }
}
