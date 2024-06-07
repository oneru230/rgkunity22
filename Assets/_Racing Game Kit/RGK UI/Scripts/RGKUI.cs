//This is Race UI that displays time, standing and results information on HUD. 
//This should attached to Race Manager object.

using UnityEngine;
using UnityEngine.UI;
using RacingGameKit;
using System.Collections;
using RacingGameKit.Interfaces;

namespace RacingGameKit.UI
{
    [AddComponentMenu("Racing Game Kit/UI/Race UI")]
    [RequireComponent(typeof(Race_Manager))]
    public class RGKUI : MonoBehaviour, IRGKUI
    {

        private float m_FPSTimeLeft;
        private float m_FPSAccum;
        private int m_FPSFrames;
        private Text m_txtFPS;
        public GameObject m_UiCanvas;
        public Sprite m_SpriteStartRed;
        public Sprite m_SpriteStartYellow;
        public Sprite m_SpriteStartGreen;

        //CountDown
        private GameObject m_pnlCountDown;
        private GameObject m_imgCountDown3;
        private GameObject m_imgCountDown2;
        private GameObject m_imgCountDown1;

        //Message panel
        private GameObject m_pnlMessage;

        //Timer panel
        private GameObject m_pnlTimer;
        private Text m_lblTimerMessage;
        private Text m_lblTimerValue;

        //wrong way indicator
        private GameObject m_pnlWrongWay;
        private bool m_IsWrongWayActive = false;

        //Lap - Position Object
        private GameObject m_pnlLapPos;
        private Text m_lblHudLapValue;
        private Text m_lblHudLapText;
        private Text m_lblHudPosText;
        private Text m_lblHudPosValue;

        private Text m_lblHudCurrentTimeValue;
        private Text m_lblHudBestTimeValue;
        private Text m_lblHudBestTimeText;

        //PlayerGrid In Game
        private GameObject m_pnlGridPlayers;
        private GameObject m_GridPlayersRowTemplate;
        private GameObject m_pnlHud;
        //results
        private GameObject m_pnlResults;


        private Race_Manager m_RaceManager;
        private Racer_Detail m_PlayerDetail;

        private bool m_IsUiInitilized = false;

        //TouchDrive Panel 
        private GameObject m_pnlTouchDrive;



        //PlayerGrid Stuff
        [Space]
        public bool m_ShowPlayerGrid = false;
        [Tooltip("Caution! Lower values like 0 causes overhead and may effect FPS on mobile devices! On Mobile use minimum 0.5")]
        public float m_GridUpdateInterval = 0.5f;
        private float m_GridUpdateLeft;

        //timer friction stuff
        [Space]
        [Tooltip("Show hide time friction of Current Time on hud.")]
        public bool m_EnableTimerFriction = false;
        //Fps
        [Space]
        public bool m_ShowFPS = false;
        public float m_FpsUpdateInterval = 1;

        private float countFadeDelayTimer = 1;
        private bool countFadeDelay = false;

        private float countFadeTimer = 1;
        private bool countStartFade = false;

        private float messageFadeDelayTimer = 1;
        private bool messageFadeDelay = false;

        private float messageFadeTimer = 1;
        private bool messageStartfade = false;

        private bool finalLapDisplayed = false;


        //Find the RaceManager to make event hooks then intialize UI stuff..
        void Awake()
        {
            m_RaceManager = GetComponent<Race_Manager>();

            if (m_RaceManager != null)
            {
                m_RaceManager.OnRaceInitiated += RaceManager_OnRaceInitiated;
                m_RaceManager.OnRaceFinished += RaceManager_OnRaceFinished;
                m_RaceManager.OnCountDownFinished += RaceManager_OnCountDownFinished;
                m_RaceManager.OnCountDownStarted += RaceManager_OnCountDownStarted;
                m_RaceManager.OnRacerKicked += RaceManager_OnRacerKicked;

                InitializeUIStuff();
            }

            if (m_UiCanvas != null)
            {
                m_pnlTouchDrive = m_UiCanvas.GetComponent<RGKUI_PauseMenu>().m_TouchDriveManager;
            }
        }

        //This function finds some specific objects to make ui intact. This way much easier to manually assign object declerations in unity. But 
        //bad part is you shouldn't change any object name after decleraton below. 
        void InitializeUIStuff()
        {
            if (m_RaceManager != null)
            {
                m_pnlHud = m_UiCanvas.FindInChildren("pnlHud").gameObject;


                m_pnlGridPlayers = m_pnlHud.FindInChildren("pnlGridRacers").gameObject;
                if (m_pnlGridPlayers != null && !m_ShowPlayerGrid)
                {
                    m_pnlGridPlayers.SetActive(false);
                }

                GameObject otxtFPS = m_pnlHud.FindInChildren("lblFPS").gameObject;
                if (otxtFPS != null) m_txtFPS = otxtFPS.GetComponent<Text>();

                m_pnlLapPos = m_UiCanvas.FindInChildren("pnlLapPos").gameObject;
                m_pnlWrongWay = m_UiCanvas.FindInChildren("pnlWrongWay").gameObject;
                m_pnlWrongWay.GetComponent<CanvasGroup>().alpha = 0;
                m_IsWrongWayActive = false;

                m_pnlMessage = m_UiCanvas.FindInChildren("pnlMessage").gameObject;
                m_pnlMessage.GetComponent<CanvasGroup>().alpha = 0;



                m_pnlCountDown = m_UiCanvas.FindInChildren("pnlCountDown").gameObject;
                m_imgCountDown3 = m_pnlCountDown.FindInChildren("imgCountDown3").gameObject;
                m_imgCountDown2 = m_pnlCountDown.FindInChildren("imgCountDown2").gameObject;
                m_imgCountDown1 = m_pnlCountDown.FindInChildren("imgCountDown1").gameObject;
                m_pnlCountDown.GetComponent<CanvasGroup>().alpha = 1;

                //Lap - Position
                m_lblHudLapValue = m_pnlLapPos.FindInChildren("lblHudLapValue").GetComponent<Text>();
                m_lblHudLapText = m_pnlLapPos.FindInChildren("lblHudLapText").GetComponent<Text>();

                m_lblHudPosText = m_pnlLapPos.FindInChildren("lblHudPosText").GetComponent<Text>();
                m_lblHudPosValue = m_pnlLapPos.FindInChildren("lblHudPosValue").GetComponent<Text>();

                m_lblHudCurrentTimeValue = m_pnlLapPos.FindInChildren("lblHudCurrentTimeVal").GetComponent<Text>();
                m_lblHudBestTimeValue = m_pnlLapPos.FindInChildren("lblHudBestTimeVal").GetComponent<Text>();
                m_lblHudBestTimeText = m_pnlLapPos.FindInChildren("lblHudBestTimeText").GetComponent<Text>();


                //pnlResults
                m_pnlResults = m_UiCanvas.FindInChildren("pnlResults").gameObject;
                m_pnlResults.GetComponent<CanvasGroup>().alpha = 0;

                m_pnlTimer = m_UiCanvas.FindInChildren("pnlTimer").gameObject;
                m_lblTimerMessage = m_pnlTimer.FindInChildren("lblTimerMessage").GetComponent<Text>();
                m_lblTimerValue = m_pnlTimer.FindInChildren("lblTimerValue").GetComponent<Text>();

                m_IsUiInitilized = true;


            }
        }

        //After race initiated (means racers spawned and registered to race manager) we create a small grid based user standings and grid positions
        void RaceManager_OnRaceInitiated()
        {
            switch (m_RaceManager.RaceType)
            {
                case RaceTypeEnum.TimeAttack:
                    m_pnlTimer.GetComponent<CanvasGroup>().alpha = 1;
                    m_lblTimerMessage.text = "To Next Checkpoint";
                    m_lblHudLapText.enabled = false; // becasue w're going to display this value in "POS" text
                    m_lblHudLapValue.enabled = false;
                    m_lblHudPosText.text = "LAP";
                    break;
                case RaceTypeEnum.Sprint:
                    m_lblHudLapText.enabled = false;
                    m_lblHudLapValue.enabled = false;
                    m_lblHudBestTimeText.text = "Progress";
                    break;
                case RaceTypeEnum.Speedtrap:

                    if (m_RaceManager.SpeedTrapMode == eSpeedTrapMode.HighestSpeed)
                    { m_lblHudBestTimeText.text = "Highest Speed"; }
                    else if (m_RaceManager.SpeedTrapMode == eSpeedTrapMode.HighestTotalSpeed)
                    { m_lblHudBestTimeText.text = "Total Speed"; }
                    else if (m_RaceManager.SpeedTrapMode == eSpeedTrapMode.HighestSpeedInRace)
                    { m_lblHudBestTimeText.text = "Top Speed"; }
                    break;
            }


            if (m_ShowPlayerGrid)
            {
                BuildPlayerUIGrid();
            }
        }

        //Display kicked message whenever a racer kicked out
        void RaceManager_OnRacerKicked(Racer_Detail RacerData)
        {
            ShowMessage(RacerData.RacerName + " is Kicked!");
        }
        //display countdown panel when countdown started
        void RaceManager_OnCountDownStarted()
        {
            m_pnlCountDown.GetComponent<CanvasGroup>().alpha = 1;
        }
        //hide countdown panel when countdown complete.
        void RaceManager_OnCountDownFinished()
        {
            //normally you should use the line below
            // pnlCountDown.GetComponent<CanvasGroup>().alpha = 0;
            //but since we're going to delay the hide, we need another way..;
            countFadeDelay = true;
        }
        //when race finished hide hud panel and show results panel.
        void RaceManager_OnRaceFinished(RaceTypeEnum RaceType)
        {

            m_pnlHud.GetComponent<CanvasGroup>().alpha = 0;
            m_pnlResults.FindInChildren("btnResultsRestart").GetComponent<Button>().Select();

            if (m_PlayerDetail != null)
            {


                switch (RaceType)
                {
                    case RaceTypeEnum.Speedtrap:
                        m_pnlResults.FindInChildren("lblStandings").GetComponent<Text>().text = RGKUI_Utils.Ordinal(System.Convert.ToInt16(m_PlayerDetail.RacerStanding));
                        m_pnlResults.FindInChildren("lblTotalTimeValue").GetComponent<Text>().text = RGKUI_Utils.FormatTime(m_PlayerDetail.RacerTotalTime, true, 2);

                        if (m_RaceManager.SpeedTrapMode == eSpeedTrapMode.HighestSpeed)
                        {
                            m_pnlResults.FindInChildren("lblBestLap").GetComponent<Text>().text = "Highest Speed :";
                            m_pnlResults.FindInChildren("lblBestLapValue").GetComponent<Text>().text = RGKUI_Utils.FormatSpeed(m_PlayerDetail.RacerHighestSpeed, false);
                        }
                        else if (m_RaceManager.SpeedTrapMode == eSpeedTrapMode.HighestTotalSpeed)
                        {
                            m_pnlResults.FindInChildren("lblBestLap").GetComponent<Text>().text = "Total Speed :";
                            m_pnlResults.FindInChildren("lblBestLapValue").GetComponent<Text>().text = RGKUI_Utils.FormatSpeed(m_PlayerDetail.RacerSumOfSpeeds, false);
                        }
                        else if (m_RaceManager.SpeedTrapMode == eSpeedTrapMode.HighestSpeedInRace)
                        {
                            m_pnlResults.FindInChildren("lblBestLap").GetComponent<Text>().text = "Top Speed :";
                            m_pnlResults.FindInChildren("lblBestLapValue").GetComponent<Text>().text = RGKUI_Utils.FormatSpeed(m_PlayerDetail.RacerSumOfSpeeds, false);
                        }
                        break;

                    case RaceTypeEnum.TimeAttack:
                        if (!m_PlayerDetail.RacerDestroyed)
                        {
                            m_pnlResults.FindInChildren("lblStandings").GetComponent<Text>().text = "WIN!";
                        }
                        else
                        {
                            m_pnlResults.FindInChildren("lblStandings").GetComponent<Text>().text = "FAIL!";
                        }

                        m_pnlResults.FindInChildren("lblTotalTimeValue").GetComponent<Text>().text = RGKUI_Utils.FormatTime(m_PlayerDetail.RacerTotalTime, true, 2);
                        m_pnlResults.FindInChildren("lblBestLapValue").GetComponent<Text>().text = RGKUI_Utils.FormatTime(m_PlayerDetail.RacerBestTime, true, 2);
                        break;
                    default:
                        m_pnlResults.FindInChildren("lblStandings").GetComponent<Text>().text = RGKUI_Utils.Ordinal(System.Convert.ToInt16(m_PlayerDetail.RacerStanding));
                        m_pnlResults.FindInChildren("lblTotalTimeValue").GetComponent<Text>().text = RGKUI_Utils.FormatTime(m_PlayerDetail.RacerTotalTime, true, 2);
                        m_pnlResults.FindInChildren("lblBestLapValue").GetComponent<Text>().text = RGKUI_Utils.FormatTime(m_PlayerDetail.RacerBestTime, true, 2);
                        break;
                }



            }

            m_pnlResults.GetComponent<CanvasGroup>().alpha = 1;
            m_pnlResults.GetComponent<CanvasGroup>().blocksRaycasts = true;
            m_pnlResults.GetComponent<CanvasGroup>().interactable = true;

            if (m_pnlTouchDrive != null)
            {
                m_pnlTouchDrive.GetComponent<CanvasGroup>().alpha = 0;
                m_pnlTouchDrive.GetComponent<CanvasGroup>().blocksRaycasts = false;
                m_pnlTouchDrive.GetComponent<CanvasGroup>().interactable = false;
            }

            GameObject resultsGrid = m_pnlResults.FindInChildren("ResultsGrid");
            if (resultsGrid != null)
            {
                GameObject rowTemplate = resultsGrid.FindInChildren("gridRowTemplate");

                int iRacer = 1;
                float lastTime = 0;
                float lastSpeed = 0;
                float lastSpeedSum = 0;
                bool playerDisplayed = false;
                foreach (Racer_Detail RD in m_RaceManager.RegisteredRacers)
                {


                    GameObject racerRow = (GameObject)Instantiate(rowTemplate);
                    racerRow.name = "resultRow" + iRacer.ToString();
                    racerRow.transform.position = rowTemplate.transform.position;
                    racerRow.transform.rotation = rowTemplate.transform.rotation;
                    racerRow.transform.SetParent(resultsGrid.transform);
                    racerRow.transform.localScale = new Vector3(1, 1, 1);

                    switch (RaceType)
                    {
                        case RaceTypeEnum.LapKnockout:

                            if (!RD.RacerDestroyed)
                            {
                                racerRow.FindInChildren("gridRowText").GetComponent<Text>().text = RD.RacerName;
                                racerRow.FindInChildren("gridRowValue").GetComponent<Text>().text = RGKUI_Utils.FormatTime(RD.RacerTotalTime, true, 2);
                            }
                            else
                            {
                                racerRow.FindInChildren("gridRowText").GetComponent<Text>().text = RD.RacerName;
                                racerRow.FindInChildren("gridRowValue").GetComponent<Text>().text = "DNF";
                            }

                            break;
                        case RaceTypeEnum.Speedtrap:

                            if (m_RaceManager.SpeedTrapMode == eSpeedTrapMode.HighestSpeed)
                            {
                                if (!playerDisplayed)
                                {
                                    racerRow.FindInChildren("gridRowText").GetComponent<Text>().text = RD.RacerName;
                                    racerRow.FindInChildren("gridRowValue").GetComponent<Text>().text = RGKUI_Utils.FormatSpeed(RD.RacerHighestSpeed, false);
                                }
                                else // this guys not finished on race time so instead of waiting their finish to display, we'll display some mockup timings.
                                {

                                    lastSpeed = lastSpeed - UnityEngine.Random.Range(5f, 30f);
                                    racerRow.FindInChildren("gridRowText").GetComponent<Text>().text = RD.RacerName;
                                    racerRow.FindInChildren("gridRowValue").GetComponent<Text>().text = RGKUI_Utils.FormatSpeed(lastSpeed, false);

                                }
                            }
                            else if (m_RaceManager.SpeedTrapMode == eSpeedTrapMode.HighestTotalSpeed)
                            {
                                if (!playerDisplayed)
                                {
                                    racerRow.FindInChildren("gridRowText").GetComponent<Text>().text = RD.RacerName;
                                    racerRow.FindInChildren("gridRowValue").GetComponent<Text>().text = RGKUI_Utils.FormatSpeed(RD.RacerSumOfSpeeds, false);
                                }
                                else // this guys not finished on race time so instead of waiting their finish to display, we'll display some mockup timings.
                                {

                                    lastSpeedSum = lastSpeedSum - UnityEngine.Random.Range(10f, 150f);
                                    racerRow.FindInChildren("gridRowText").GetComponent<Text>().text = RD.RacerName;
                                    racerRow.FindInChildren("gridRowValue").GetComponent<Text>().text = RGKUI_Utils.FormatSpeed(lastSpeedSum, false);

                                }
                            }
                            else if (m_RaceManager.SpeedTrapMode == eSpeedTrapMode.HighestSpeedInRace)
                            {
                                if (!playerDisplayed)
                                {
                                    racerRow.FindInChildren("gridRowText").GetComponent<Text>().text = RD.RacerName;
                                    racerRow.FindInChildren("gridRowValue").GetComponent<Text>().text = RGKUI_Utils.FormatSpeed(RD.RacerHighestSpeedInRaceAsKm, false);
                                }
                                else // this guys not finished on race time so instead of waiting their finish to display, we'll display some mockup timings.
                                {
                                    lastSpeed = lastSpeed - UnityEngine.Random.Range(2f, 15f);
                                    racerRow.FindInChildren("gridRowText").GetComponent<Text>().text = RD.RacerName;
                                    racerRow.FindInChildren("gridRowValue").GetComponent<Text>().text = RGKUI_Utils.FormatSpeed(lastSpeed, false);

                                }
                            }
                            break;
                        default:
                            if (!playerDisplayed)
                            {
                                racerRow.FindInChildren("gridRowText").GetComponent<Text>().text = RD.RacerName;
                                racerRow.FindInChildren("gridRowValue").GetComponent<Text>().text = RGKUI_Utils.FormatTime(RD.RacerTotalTime, true, 2);
                            }
                            else // this guys not finished on race time so instead of waiting their finish to display, we'll display some mockup timings.
                            {

                                lastTime = lastTime + UnityEngine.Random.Range(5f, 10f);
                                racerRow.FindInChildren("gridRowText").GetComponent<Text>().text = RD.RacerName;
                                racerRow.FindInChildren("gridRowValue").GetComponent<Text>().text = RGKUI_Utils.FormatTime(lastTime, true, 2);

                            }

                            break;

                    }

                    if (RD.IsPlayer)
                    {
                        lastTime = RD.RacerTotalTime;
                        lastSpeed = RD.RacerHighestSpeed;
                        lastSpeedSum = RD.RacerSumOfSpeeds;
                        playerDisplayed = true;
                    }

                    iRacer++;
                }
                DestroyImmediate(rowTemplate);
            }
            else { Debug.Log("Results Grid Not Found!?"); }

        }

        GameObject gridRowParentForPlayerGrid;

        //this builds up the player grid in race
        void BuildPlayerUIGrid()
        {
            if (m_pnlGridPlayers != null)
            {

                gridRowParentForPlayerGrid = m_pnlGridPlayers.gameObject.FindInChildren("gridRows");

                if (m_GridPlayersRowTemplate == null)
                {
                    m_GridPlayersRowTemplate = m_pnlGridPlayers.gameObject.FindInChildren("gridRowTemplate");
                    m_GridPlayersRowTemplate.transform.SetParent(null);
                    m_GridPlayersRowTemplate.transform.position = new Vector3(-5000, -5000);
                }
                FlushGoChildren(gridRowParentForPlayerGrid);

                foreach (Racer_Detail RD in m_RaceManager.RegisteredRacers)
                {
                    GameObject playerRow = (GameObject)Instantiate(m_GridPlayersRowTemplate);

                    playerRow.name = RD.ID.ToString();//.customProperties["UID"].ToString();
                    playerRow.transform.position = gridRowParentForPlayerGrid.transform.position;
                    playerRow.transform.rotation = gridRowParentForPlayerGrid.transform.rotation;
                    playerRow.transform.SetParent(gridRowParentForPlayerGrid.transform);
                    playerRow.transform.localScale = new Vector3(1, 1, 1);

                    Text txtPlayerName = playerRow.gameObject.FindInChildren("txtGridPlayerName").GetComponent<Text>();
                    txtPlayerName.text = RD.RacerName;// +"\r\n" + Player.customProperties["UID"].ToString();
                    Image imgMe = playerRow.gameObject.FindInChildren("imgGridPlayerIcon").GetComponent<Image>();

                    if (!RD.IsPlayer)
                    {
                        imgMe.enabled = false;
                    }
                }
            }
        }

        //this updates previously created player grid based standings
        void UpdatePlayerUIGrid()
        {
            if (m_pnlGridPlayers != null && m_RaceManager != null)
            {
                if (gridRowParentForPlayerGrid == null)
                {
                    gridRowParentForPlayerGrid = m_pnlGridPlayers.gameObject.FindInChildren("gridRows");
                }

                foreach (Racer_Detail RD in m_RaceManager.RegisteredRacers)
                {
                    Transform RDRow = gridRowParentForPlayerGrid.transform.Find(RD.ID);


                    if (RDRow != null)
                    {
                        RDRow.SetSiblingIndex(System.Convert.ToInt16(RD.RacerStanding));
                        RDRow.gameObject.FindInChildren("txtGridPlayerPos").GetComponent<Text>().text = System.Convert.ToInt16(RD.RacerStanding).ToString();
                        Text txtPlayerDis = RDRow.gameObject.FindInChildren("txtGridPlayerDistance").GetComponent<Text>();
                        //txtPlayerPos.text = System.Convert.ToInt16(RD.RacerStanding).ToString();
                        if (RD.RacerDistance > 0 && !RD.RacerDestroyed)
                        {
                            txtPlayerDis.text = string.Format("{0:#}m", RD.RacerDistance);
                        }
                        else if (RD.RacerDestroyed)
                        {
                            txtPlayerDis.text = "DNF";
                        }
                        else
                        {
                            txtPlayerDis.text = "Finished";
                        }
                    }
                }
            }
        }

        //update stuff
        void LateUpdate()
        {
            DoUiUpdate();

            if (m_ShowPlayerGrid)
            {
                UpdateGridInterval();
            }

            if (m_ShowFPS)
            {
                CaptureFPS();
                if (!m_txtFPS.gameObject.activeInHierarchy) m_txtFPS.gameObject.SetActive(true);
            }
            else
            {
                if (m_txtFPS.gameObject.activeInHierarchy) m_txtFPS.gameObject.SetActive(false);
            }
        }

        private void UpdateGridInterval()
        {
            if (m_pnlGridPlayers != null)
            {
                m_GridUpdateLeft -= Time.deltaTime;

                if (m_GridUpdateLeft <= 0)
                {
                    UpdatePlayerUIGrid();
                    m_GridUpdateLeft = m_GridUpdateInterval;
                }
            }
        }

        //fps calculation
        void CaptureFPS()
        {

            if (m_txtFPS == null)
                return;

            m_FPSTimeLeft -= Time.deltaTime;
            m_FPSAccum += Time.timeScale / Time.deltaTime;
            ++m_FPSFrames;

            if (m_FPSTimeLeft <= 0)
            {
                float fps = m_FPSAccum / m_FPSFrames;
                string format = System.String.Format("{0:F0} FPS", fps);

                m_txtFPS.text = format;

                m_FPSTimeLeft = m_FpsUpdateInterval;
                m_FPSAccum = 0;
                m_FPSFrames = 0;
            }
        }

        //update UI controls like laps, time etc.
        private void DoUiUpdate()
        {
            if (m_IsUiInitilized)
            {
                //Grab the RacerDetail of Player 
                if (m_PlayerDetail == null && m_RaceManager.Player1 != null)
                {
                    m_PlayerDetail = m_RaceManager.Player1.GetComponent<Racer_Register>().RacerDetail;
                }

                if (m_PlayerDetail != null)
                {
                    //Set lap and positio names
                    m_lblHudCurrentTimeValue.text = RGKUI_Utils.FormatTime(m_PlayerDetail.RacerTotalTime, m_EnableTimerFriction, 1);




                    switch (m_RaceManager.RaceType)
                    {
                        case RaceTypeEnum.Sprint:
                            float mDistance = (m_RaceManager.RaceLength - m_PlayerDetail.RacerDistance);
                            if (mDistance < 0) mDistance = 0;
                            m_lblHudBestTimeValue.text = string.Format("{0:0} %", ((mDistance * 100) / m_RaceManager.RaceLength));

                            m_lblHudPosValue.text = m_PlayerDetail.RacerStanding + "/" + m_RaceManager.RegisteredRacers.Count;
                            m_lblHudLapValue.text = m_PlayerDetail.RacerLap + "/" + m_RaceManager.RaceLaps.ToString();
                            break;
                        case RaceTypeEnum.Speedtrap:
                            if (m_RaceManager.SpeedTrapMode == eSpeedTrapMode.HighestSpeed)
                            {
                                m_lblHudBestTimeValue.text = string.Format("{0:0} Km/h", m_PlayerDetail.RacerHighestSpeed);
                            }
                            else if (m_RaceManager.SpeedTrapMode == eSpeedTrapMode.HighestTotalSpeed)
                            {
                                m_lblHudBestTimeValue.text = string.Format("{0:0} Km/h", m_PlayerDetail.RacerSumOfSpeeds);
                            }
                            else if (m_RaceManager.SpeedTrapMode == eSpeedTrapMode.HighestSpeedInRace)
                            {
                                m_lblHudBestTimeValue.text = string.Format("{0:0} Km/h", m_PlayerDetail.RacerHighestSpeedInRaceAsKm);
                            }
                            m_lblHudPosValue.text = m_PlayerDetail.RacerStanding + "/" + m_RaceManager.RegisteredRacers.Count;
                            m_lblHudLapValue.text = m_PlayerDetail.RacerLap + "/" + m_RaceManager.RaceLaps.ToString();
                            break;
                        case RaceTypeEnum.TimeAttack:
                            m_lblHudBestTimeValue.text = RGKUI_Utils.FormatTime(m_PlayerDetail.RacerBestTime, true, 1);
                            m_lblHudPosValue.text = m_PlayerDetail.RacerLap + "/" + m_RaceManager.RaceLaps.ToString();
                            break;
                        default:
                            m_lblHudBestTimeValue.text = RGKUI_Utils.FormatTime(m_PlayerDetail.RacerBestTime, true, 1);
                            m_lblHudPosValue.text = m_PlayerDetail.RacerStanding + "/" + m_RaceManager.RegisteredRacers.Count;
                            m_lblHudLapValue.text = m_PlayerDetail.RacerLap + "/" + m_RaceManager.RaceLaps.ToString();
                            break;

                    }



                    //set wrong way indicator
                    if (m_PlayerDetail.RacerWrongWay)
                    {
                        if (!m_IsWrongWayActive)
                        {
                            m_pnlWrongWay.GetComponent<CanvasGroup>().alpha = 1;
                            m_IsWrongWayActive = true;//This is for performance optimization..
                        }
                    }
                    else
                    {
                        if (m_IsWrongWayActive)
                        {
                            m_pnlWrongWay.GetComponent<CanvasGroup>().alpha = 0;
                            m_IsWrongWayActive = false;
                        }
                    }


                    if (m_RaceManager.RaceLaps > 1 && m_PlayerDetail.RacerLap == m_RaceManager.RaceLaps && !finalLapDisplayed)
                    {
                        ShowMessage("Final Lap!");
                        finalLapDisplayed = true;
                    }

                }

                //set countdown
                if (!m_RaceManager.IsRaceStarted)
                {
                    switch (m_RaceManager.CurrentCount)
                    {
                        case 2:
                            m_imgCountDown1.GetComponent<Image>().sprite = m_SpriteStartYellow;
                            //m_imgCountDown2.GetComponent<Image>().sprite = m_SpriteStartRed;
                            //m_imgCountDown3.GetComponent<Image>().sprite = m_SpriteStartRed;
                            break;
                        case 1:
                            //m_imgCountDown1.GetComponent<Image>().sprite = m_SpriteStartYellow;
                            m_imgCountDown2.GetComponent<Image>().sprite = m_SpriteStartYellow;
                            //m_imgCountDown3.GetComponent<Image>().sprite = m_SpriteStartRed;
                            break;
                        case 0:
                            m_imgCountDown1.GetComponent<Image>().sprite = m_SpriteStartGreen;
                            m_imgCountDown2.GetComponent<Image>().sprite = m_SpriteStartGreen;
                            m_imgCountDown3.GetComponent<Image>().sprite = m_SpriteStartGreen;

                            break;
                    }
                }


                switch (m_RaceManager.RaceType)
                {
                    case RaceTypeEnum.TimeAttack:
                        m_lblTimerValue.text = RGKUI_Utils.FormatTime(m_RaceManager.TimeNextCheckPoint, true, 1);
                        break;
                }
            }

            ProcessTimers();
        }



        void ProcessTimers()
        {
            //Count down appears on screen 3 more seconds after count finished
            if (countFadeDelay)
            {
                countFadeDelayTimer -= Time.deltaTime;

                if (countFadeDelayTimer < 0f)
                {
                    countStartFade = true;


                    countFadeDelay = false;
                }
            }

            //Fadeout the count panel
            if (countStartFade)
            {
                countFadeTimer -= Time.deltaTime;

                if (countFadeTimer >= 0)
                {
                    m_pnlCountDown.GetComponent<CanvasGroup>().alpha = countFadeTimer;
                }
                else
                {
                    m_pnlCountDown.GetComponent<CanvasGroup>().alpha = 0;
                    countStartFade = false;
                }
            }

            if (messageFadeDelay)
            {
                messageFadeDelayTimer -= Time.deltaTime;

                if (messageFadeDelayTimer < 0f)
                {
                    messageStartfade = true;
                    messageFadeDelay = false;
                }
            }


            if (messageStartfade)
            {
                messageFadeTimer -= Time.deltaTime;

                if (messageFadeTimer >= 0)
                {
                    m_pnlMessage.GetComponent<CanvasGroup>().alpha = messageFadeTimer;
                }
                else
                {
                    messageStartfade = false;
                }
            }
        }

        //display message on screen
        private void ShowMessage(string Message)
        {
            messageFadeDelayTimer = 1;
            messageFadeTimer = 1;
            messageFadeDelay = true;
            m_pnlMessage.GetComponent<CanvasGroup>().alpha = 1;
            m_pnlMessage.FindInChildren("lblMessage").GetComponent<Text>().text = Message;
        }

        //remove all child objects in hierarchy
        void FlushGoChildren(GameObject Target)
        {
            foreach (Transform child in Target.transform)
            {
                Destroy(child.gameObject);
            }
        }

        #region IRGKUI Interface
        public float CurrentCount
        {
            get
            {
                return 0;
            }
            set
            {

            }
        }

        public void PlayerCheckPointPassed(CheckPointItem PassedCheckpoint)
        {

        }

        public void RaceFinished(string RaceType)
        {

        }

        public bool ShowCountdownWindow
        {
            get
            {
                return false;
            }
            set
            {

            }
        }

        public void ShowResultsWindow()
        {

        }

        public bool ShowWrongWayWindow
        {
            get
            {
                return false;
            }
            set
            {

            }
        }
        #endregion

    }
}