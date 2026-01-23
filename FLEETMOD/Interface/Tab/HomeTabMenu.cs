using PulsarModLoader;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static FLEETMOD.Interface.Tab.ChangeTabMenuDisplay;
using static FLEETMOD.Interface.Tab.FleetShipListView;

namespace FLEETMOD.Interface.Tab
{
    internal class HomeTabMenu
    {
        static Text TabDescription;
        static GameObject TargetShipBG;
        static GameObject HomePageDescription;
        static GameObject PlayerList;
        static Transform ChangeClass;

        static Transform Home;

        internal static Transform FLEET_ShipDisplay;
        static Text FLEET_ShipName;
        static Text FLEET_ShipType;
        static Text FLEET_ShipDesc;
        static Text FLEET_ShipRole;
        static Text FLEET_ShipPlayerLeft;
        static Text FLEET_ShipPlayerRight;
        static GameObject FLEET_EquipButton;
        static GameObject FLEET_DiscardButton;

        /// <summary>
        /// Find existing tab features and curate the new implementations for the first time
        /// </summary>
        internal static void Initialize()
        {
            Home = CREW_Tab.Find("Home"); // Main Page
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
            newDescriptionLabel.name = "FleetDescriptionLabel";
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
            ChangeClass.name = "FleetChangeClass"; // <--- This has lots of name matching you'll need to update.
            ChangeClass.parent = Home;

            // Fleet Ship Info Panel
            FLEET_ShipDisplay = GameObject.Instantiate(SHIP, BGRight);
            FLEET_ShipDisplay.position = SHIP.position;
            FLEET_ShipDisplay.localPosition = SHIP.position;
            FLEET_ShipDisplay.rotation = SHIP.rotation;
            FLEET_ShipDisplay.localScale = SHIP.localScale;
            FLEET_ShipDisplay.name = "FleetShipInfoDisplay";
            FLEET_ShipDisplay.parent = BGRight;

            Transform CompInfo = FLEET_ShipDisplay.GetChild(1);
            CompInfo.gameObject.SetActive(true);
            FLEET_ShipName = CompInfo.GetChild(1).GetComponent<Text>();
            FLEET_ShipType = CompInfo.GetChild(2).GetComponent<Text>();
            FLEET_ShipDesc = CompInfo.GetChild(3).GetComponent<Text>();
            FLEET_ShipRole = CompInfo.GetChild(4).GetComponent<Text>();
            FLEET_ShipPlayerRight = CompInfo.GetChild(6).GetComponent<Text>();
            FLEET_ShipPlayerLeft = CompInfo.GetChild(7).GetComponent<Text>();
            FLEET_EquipButton = CompInfo.GetChild(8).gameObject;
            FLEET_EquipButton.transform.GetChild(0).GetComponent<Text>().text = "Join Crew";
            EventTrigger Equip = FLEET_EquipButton.GetComponent<EventTrigger>();
            Equip.triggers.Clear();
            EventTrigger.Entry equip_entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            equip_entry.callback.AddListener((BaseEventData data) =>
            {
                ModMessage.SendRPC(Mod.harmonyIden, "FLEETMOD.ModMessages.ChangeShip", PhotonTargets.MasterClient, new object[]
                {
                    FleetShipListView.ShipID
                });
            });
            Equip.triggers.Add(equip_entry);
            FLEET_DiscardButton = CompInfo.GetChild(9).gameObject;
            FLEET_DiscardButton.transform.GetChild(0).GetComponent<Text>().text = "Destroy Ship";
            EventTrigger Discard = FLEET_DiscardButton.GetComponent<EventTrigger>();
            Discard.triggers.Clear();
            EventTrigger.Entry discard_entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            discard_entry.callback.AddListener((BaseEventData data) =>
            {
                PLEncounterManager.Instance.GetShipFromID(FleetShipListView.ShipID).DestroySelf(PLEncounterManager.Instance.GetShipFromID(FleetShipListView.ShipID));
                UnityEngine.Object.Destroy(PLEncounterManager.Instance.GetShipFromID(FleetShipListView.ShipID).gameObject);
            });
            Discard.triggers.Add(discard_entry);
            GameObject.Destroy(CompInfo.GetChild(5).gameObject);
        }

        /// <summary>
        /// First execution
        /// </summary>
        internal static void OnAwake()
        {
            TargetShipBG.SetActive(!Variables.isrunningmod);
            HomePageDescription.SetActive(Variables.isrunningmod);
            PlayerList.SetActive(!Variables.isrunningmod);
            ChangeClass.gameObject.SetActive(Variables.isrunningmod);
            CREW.SetActive(true);
            FLEET_ShipDisplay.gameObject.SetActive(false);
        }

        /// <summary>
        /// Update the tab features
        /// </summary>
        internal static void Update()
        {
            if (!Variables.isrunningmod) return;
            if (PLTabMenu.Instance.TabMenuActive && PLTabMenu.Instance.CurrentTabIndex == 0 && PLTabMenu.Instance.GetCrewPageIndex() == 0)
            {
                // Description Variables
                PLPlayer admiral = PLServer.Instance.GetCachedFriendlyPlayerOfClass(0);
                PLPlayer localplayer = PLNetworkManager.Instance.LocalPlayer;
                PLPlayer captain = null;
                if (localplayer.StartingShip != null) captain = PLServer.Instance.GetCachedFriendlyPlayerOfClass(0, localplayer.StartingShip);

                // Primary Description
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
                TabDescription.text = description;

                if (Home != null && CurrentParent != Home)
                { // Fleet Ship List Enable
                    ChangeVisual(Home, new Vector3(20, 35, 0));
                    ShowShipList = true;
                }

                if (FLEET_ShipDisplay.gameObject.activeSelf && ShipID != -1)
                { // Fleet Ship Info Display
                    PLShipInfoBase Ship = PLEncounterManager.Instance.GetShipFromID(ShipID);
                    if (Ship == null)
                    {
                        ShipID = -1;
                        CREW.SetActive(true);
                        FLEET_ShipDisplay.gameObject.SetActive(false);
                    }
                    else
                    {
                        FLEET_ShipDisplay.position = SHIP.position;
                        FLEET_ShipDisplay.gameObject.SetActive(true);
                        FLEET_ShipName.text = Ship.ShipNameValue;
                        FLEET_ShipType.text = Ship.GetShipTypeName();
                        FLEET_ShipDesc.text = "Fleetmod ship";
                        FLEET_ShipRole.text = "Class Name";
                        FLEET_ShipPlayerLeft.text = "Player Name";
                        FLEET_ShipPlayerRight.text = "Extra";

                        if ((localplayer != admiral && FLEET_DiscardButton.gameObject.activeSelf) || (localplayer.StartingShip == Ship && FLEET_DiscardButton.gameObject.activeSelf))
                        {
                            FLEET_DiscardButton.SetActive(false);
                        }
                        else if (localplayer == admiral && !FLEET_DiscardButton.gameObject.activeSelf && localplayer.StartingShip != Ship)
                        {
                            FLEET_DiscardButton.SetActive(true);
                        }

                        if (localplayer.StartingShip == Ship && FLEET_EquipButton.gameObject.activeSelf)
                        {
                            FLEET_EquipButton.SetActive(false);
                        }
                        else if (localplayer.StartingShip != Ship && !FLEET_EquipButton.gameObject.activeSelf)
                        {
                            FLEET_EquipButton.SetActive(true);
                        }
                    }
                }
            }
            else
            {
                if (FLEET_ShipDisplay.gameObject.activeSelf)
                { // Hide Fleet Ship Display Menu when not needed.
                    FLEET_ShipDisplay.gameObject.SetActive(false);
                    CREW.SetActive(true);
                }

                if (Home != null && CurrentParent == Home)
                { // Fleet Ship List Disable
                    ChangeVisual(null, new Vector3(0, 0, 0));
                    ShowShipList = false;
                }
            }
        }
    }
}
