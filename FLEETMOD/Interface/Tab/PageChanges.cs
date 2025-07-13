using FLEETMOD.Fixes;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FLEETMOD.Interface.Tab
{
    [HarmonyPatch(typeof(PLServer), "Start")]
    internal class ChangeTabMenuDisplay
    {
        internal static bool Executed = false;
        internal static Text TabDescription;
        internal static GameObject TargetShipBG;
        internal static GameObject HomePageDescription;
        internal static GameObject PlayerList;
        internal static GameObject ShipList;
        internal static Transform ShipGrid;
        internal static RectTransform ShipScrollContent;
        internal static Transform ChangeClass;

        internal static GameObject CrewPage;
        internal static Transform FLEET_ShipDisplay;
        internal static Text FLEET_ShipName;
        internal static Text FLEET_ShipType;
        internal static Text FLEET_ShipDesc;
        internal static Text FLEET_ShipRole;
        internal static Text FLEET_ShipPlayerLeft;
        internal static Text FLEET_ShipPlayerRight;
        public static void Postfix()
        {
            UpdateLabels.Executed = false;
            if (Executed) return;
            Executed = true;
            Transform CREW_Tab = FindDeepChild(PLTabMenu.Instance.gameObject.transform, "CREW_Tab", 5);
            Transform Home = CREW_Tab.Find("Home"); // Main Page
            Transform CrewSettings = CREW_Tab.Find("CrewSettings"); // Captain Page
            Transform Talents = CREW_Tab.Find("Talents"); // Talents Page

            // Disable Targetting UI
            TargetShipBG = Home.Find("TargetShipBG").gameObject;

            // Add Descriptive Text
            Transform descriptionLabel = CrewSettings.Find("CrewPermissionsLabel");
            Transform newDescriptionLabel = GameObject.Instantiate(descriptionLabel, Home);
            newDescriptionLabel.position = descriptionLabel.position;
            newDescriptionLabel.localPosition = newDescriptionLabel.localPosition;
            newDescriptionLabel.rotation = descriptionLabel.rotation;
            newDescriptionLabel.localScale = descriptionLabel.localScale;
            newDescriptionLabel.name = "FleetPageDescriptionLabel";
            newDescriptionLabel.parent = Home;
            Text description = newDescriptionLabel.GetComponent<Text>();
            description.supportRichText = true;
            TabDescription = description;
            HomePageDescription = newDescriptionLabel.gameObject;
            newDescriptionLabel.GetComponent<RectTransform>().sizeDelta = new Vector2(520, 160);
            TabDescription.alignment = TextAnchor.MiddleCenter;

            // Hide the Player list
            PlayerList = Home.Find("Players").gameObject;

            // Add Change Class Buttons
            Transform PlayerFive = PlayerList.transform.Find("PlayerFive");
            ChangeClass = GameObject.Instantiate(PlayerFive, PlayerList.transform);
            ChangeClass.position = PlayerFive.position;
            ChangeClass.localPosition = PlayerFive.localPosition;
            ChangeClass.rotation = PlayerFive.rotation;
            ChangeClass.localScale = PlayerFive.localScale;
            ChangeClass.name = "FleetPageChangeClass";
            ChangeClass.parent = Home;
            /*PLOverviewPlayerInfoDisplay PlayerInfo = ChangeClass.GetComponent<PLOverviewPlayerInfoDisplay>();
            OverviewFleetInfoDisplay FleetInfo = ChangeClass.gameObject.AddComponent<OverviewFleetInfoDisplay>();
            FleetInfo.Buttons.Concat(PlayerInfo.Buttons);
            UnityEngine.Object.Destroy(PlayerInfo);
            for (int i = 2; i < 6; i++)
            {
                GameObject button = ChangeClass.GetChild(i).gameObject;
                UnityEngine.Object.Destroy(button.GetComponent<PLTabMenuPlayerInfoButton>());
                FleetInfoDisplayButton FleetInfoButton = ChangeClass.gameObject.AddComponent<FleetInfoDisplayButton>();
                FleetInfo.Buttons.Append(FleetInfoButton);
            }*/


            // Add Ship List
            Transform scrollView = Talents.Find("Scroll View");
            Transform shipScrollView = GameObject.Instantiate(scrollView, Home);
            shipScrollView.position = PlayerList.transform.position; //-0.078 -399.4167 17.7626
            shipScrollView.localPosition = PlayerList.transform.position + new Vector3(-7f, -76f); // 40.1707 -74.8114 -109.4851
            shipScrollView.rotation = scrollView.rotation;
            shipScrollView.localScale = scrollView.localScale;
            shipScrollView.name = "FleetShipList";
            shipScrollView.parent = Home;
            ShipList = shipScrollView.gameObject;
            ShipGrid = FindDeepChild(shipScrollView, "TalentGrid", 5);
            ShipGrid.name = "ShipGrid";
            ShipScrollContent = ShipGrid.GetComponent<RectTransform>();

            shipScrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(UpdateLabels.totalWidth, 300);
            shipScrollView.GetComponent<ScrollRect>().verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;


            // Fleet Ship Info Panel
            Transform BGRight = FindDeepChild(PLTabMenu.Instance.gameObject.transform, "BGRight", 5);
            CrewPage = BGRight.Find("CREW").gameObject;

            Transform ComponentDisplay = BGRight.Find("SHIP");
            FLEET_ShipDisplay = GameObject.Instantiate(ComponentDisplay, BGRight);
            FLEET_ShipDisplay.position = ComponentDisplay.position;
            FLEET_ShipDisplay.localPosition = ComponentDisplay.position;
            FLEET_ShipDisplay.rotation = ComponentDisplay.rotation;
            FLEET_ShipDisplay.localScale = ComponentDisplay.localScale;
            FLEET_ShipDisplay.name = "FleetCrewDisplay";
            FLEET_ShipDisplay.parent = BGRight;

            Transform CompInfo = FLEET_ShipDisplay.GetChild(1);
            CompInfo.gameObject.SetActive(true);
            FLEET_ShipName = CompInfo.GetChild(1).GetComponent<Text>();
            FLEET_ShipType = CompInfo.GetChild(2).GetComponent<Text>();
            FLEET_ShipDesc = CompInfo.GetChild(3).GetComponent<Text>();
            FLEET_ShipRole = CompInfo.GetChild(4).GetComponent<Text>();
            FLEET_ShipPlayerRight = CompInfo.GetChild(6).GetComponent<Text>();
            FLEET_ShipPlayerLeft = CompInfo.GetChild(7).GetComponent<Text>();
            GameObject.Destroy(CompInfo.GetChild(5).gameObject);
        }

        public static Transform FindDeepChild(Transform parent, string name, int depth = 3)
        {
            if (depth <= 0) return null;
            depth--;
            foreach (Transform child in parent)
            {
                if (child.name == name)
                    return child;

                Transform result = FindDeepChild(child, name, depth);
                if (result != null)
                    return result;
            }
            return null;
        }
    }

    [HarmonyPatch(typeof(PLTabMenu), "Update")]
    internal class UpdateLabels
    {
        internal static bool Executed = false;
        public static void Postfix()
        {
            if (PLServer.Instance == null || !ChangeTabMenuDisplay.Executed) return;
            if (!Executed)
            {
                Executed = true;
                ShipID = -1;
                ChangeTabMenuDisplay.TargetShipBG.SetActive(!Variables.isrunningmod);
                ChangeTabMenuDisplay.HomePageDescription.SetActive(Variables.isrunningmod);
                ChangeTabMenuDisplay.PlayerList.SetActive(!Variables.isrunningmod);
                ChangeTabMenuDisplay.ChangeClass.gameObject.SetActive(Variables.isrunningmod);
                ChangeTabMenuDisplay.CrewPage.SetActive(true);
                ChangeTabMenuDisplay.FLEET_ShipDisplay.gameObject.SetActive(false);
            }

            if (!Variables.isrunningmod || PLNetworkManager.Instance.LocalPlayer == null || !PLNetworkManager.Instance.LocalPlayer.GetHasStarted()) return;
            UpdateTDs();
        }

        private static List<ShipDisplay> allSDs = new List<ShipDisplay>();
        internal static float totalWidth = 600f;
        static float nameWidth = 210f;
        static float descWidth = 210f;
        static float rankStartX = 140f;
        static float rankSpacing = 20f;
        private static void UpdateTDs()
        {
            if (!Variables.isrunningmod) return;

            // Primary Description
            PLPlayer admiral = PLServer.Instance.GetCachedFriendlyPlayerOfClass(0);
            PLPlayer localplayer = PLNetworkManager.Instance.LocalPlayer;
            PLPlayer captain = null;
            if (localplayer.StartingShip != null) captain = PLServer.Instance.GetCachedFriendlyPlayerOfClass(0, localplayer.StartingShip);
            string description =
"You are a " + localplayer.GetClassName() +
(localplayer.GetPawn() == null ? "" : (localplayer.GetPawn().CurrentShip == null ? ", planetside" : " onboard " + (localplayer.GetPawn().CurrentShip == localplayer.StartingShip ? "your ship" : "the " + localplayer.GetPawn().CurrentShip.ShipNameValue))) + "\n" +
(localplayer.StartingShip == null ? "You do not currently have a ship" : "Your ship is the " + localplayer.StartingShip.ShipNameValue + ", an " + localplayer.StartingShip.GetShipTypeName()) +
(admiral == null || admiral == localplayer ? "" : " in " + admiral.GetPlayerName(false) + "'s Fleet") +
"\n\n";

            // Secondary Description
            if (localplayer.GetClassID() == 0)
            {
                if (localplayer == admiral)
                {
                    description += "Your Fleet ships are listed below. To allow them to be crewed, you must assign a captain to them when at a station.\n";
                }
                description += "As a Captain, you cannot abandon your ship.";
            }
            else
            {
                description += "As a Crew Member, you can change ships by selecting a ship below and change class with the buttons at the bottom.";
            }
            ChangeTabMenuDisplay.TabDescription.text = description;

            // Hide Fleet Ship Display Menu when not needed.
            if (ChangeTabMenuDisplay.FLEET_ShipDisplay.gameObject.activeSelf && (PLTabMenu.Instance.CurrentTabIndex != 0 || PLTabMenu.Instance.GetCrewPageIndex() != 0 || !PLTabMenu.Instance.TabMenuActive))
            {
                ChangeTabMenuDisplay.CrewPage.SetActive(true);
                ChangeTabMenuDisplay.FLEET_ShipDisplay.gameObject.SetActive(false);
            }

            // Update Fleet Ship Display Menu Details
            if (ChangeTabMenuDisplay.FLEET_ShipDisplay.gameObject.activeSelf && ShipID != -1)
            {
                PLShipInfoBase Ship = PLEncounterManager.Instance.GetShipFromID(ShipID);
                if (Ship == null)
                {
                    ShipID = -1;
                    ChangeTabMenuDisplay.CrewPage.SetActive(true);
                    ChangeTabMenuDisplay.FLEET_ShipDisplay.gameObject.SetActive(false);
                }
                else
                {
                    ChangeTabMenuDisplay.FLEET_ShipDisplay.gameObject.SetActive(true);
                    ChangeTabMenuDisplay.FLEET_ShipName.text = Ship.ShipNameValue;
                    ChangeTabMenuDisplay.FLEET_ShipType.text = Ship.GetShipTypeName();
                    ChangeTabMenuDisplay.FLEET_ShipDesc.text = "Fleetmod ship";
                    ChangeTabMenuDisplay.FLEET_ShipPlayerLeft.text = "Role";
                    ChangeTabMenuDisplay.FLEET_ShipPlayerRight.text = "Name";
                }
            }

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

                        gameObject.transform.SetParent(ChangeTabMenuDisplay.ShipGrid);
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
                    sd.Desc.text = PLLocalize.Localize($"{ship.GetShipTypeName()} which is Captained by {(Captain == null ? "No-one" : Captain.GetPlayerName(false) )}", false);
                    sd.Available = true;
                    if (Variables.Fleet[shipID].Count >= 5)
                    {
                        sd.Available = false;
                    }
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
                ChangeTabMenuDisplay.ShipScrollContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -num);
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

        internal static int ShipID = -1;
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
            ChangeTabMenuDisplay.CrewPage.SetActive(false);
            ChangeTabMenuDisplay.FLEET_ShipDisplay.gameObject.SetActive(true);
            return;
            /*PLPlayer playerFromPlayerID = PLServer.Instance.GetPlayerFromPlayerID(TalentsListSelectedPlayerID);
            if (playerFromPlayerID != null)
            {
                if (!inTD.Available)
                {
                    if (!PLServer.Instance.IsTalentUnlocked(inTD.MyType))
                    {
                        PLTabMenu.Instance.TimedErrorMsg = PLLocalize.Localize("Can't upgrade Talent. It needs to be researched!", false);
                        return;
                    }
                    TalentInfo talentInfoForTalentType = PLGlobal.GetTalentInfoForTalentType(inTD.MyType);
                    if (talentInfoForTalentType != null)
                    {
                        if (talentInfoForTalentType.ExtendsTalent != ETalents.MAX)
                        {
                            TalentInfo talentInfoForTalentType2 = PLGlobal.GetTalentInfoForTalentType(talentInfoForTalentType.ExtendsTalent);
                            if (playerFromPlayerID.Talents[(int)talentInfoForTalentType.ExtendsTalent] < talentInfoForTalentType2.MaxRank)
                            {
                                PLTabMenu.Instance.TimedErrorMsg = PLLocalize.Localize("Can't upgrade Talent. It requires ", false) + talentInfoForTalentType2.Name + PLLocalize.Localize(" to be fully upgraded!", false);
                                return;
                            }
                        }
                        if (talentInfoForTalentType.MinLevel > PLServer.Instance.CurrentCrewLevel)
                        {
                            PLTabMenu.Instance.TimedErrorMsg = PLLocalize.Localize("Can't upgrade Talent. Crew level is below ", false) + talentInfoForTalentType.MinLevel.ToString() + "!";
                            return;
                        }
                    }
                    return;
                }
                else if (PLNetworkManager.Instance.LocalPlayer != null && PLServer.Instance != null && inTD != null && PLServer.Instance.IsTalentUnlocked(inTD.MyType) && TalentsListSelectedPlayerID != -1 && LocalPlayerCanEditTalentsOfPlayer(playerFromPlayerID) && playerFromPlayerID.TalentPointsAvailable > 0 && inTD != null)
                {
                    TalentInfo talentInfoForTalentType3 = PLGlobal.GetTalentInfoForTalentType(inTD.MyType);
                    if (playerFromPlayerID.Talents[(int)inTD.MyType] < talentInfoForTalentType3.MaxRank)
                    {
                        //PLMusic.PostEvent("play_sx_ui_talent_added", base.gameObject);
                        playerFromPlayerID.photonView.RPC("ServerRankTalent", PhotonTargets.MasterClient, new object[] { talentInfoForTalentType3.TalentID });
                    }
                }
            }*/
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
        private static ShipDisplay GetShipDisplay(int inShipId)
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
}
