using UnityEngine;
using UnityEngine.UI;
using RacingGameKit;
using System.Collections;
using RacingGameKit.Interfaces;

namespace RacingGameKit.UI
{
    [AddComponentMenu("Racing Game Kit/UI/Race UI for Split Screen")]
    public class RGKUI_SplitScreen : MonoBehaviour, IRGKUI
    {

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
        private GameObject m_pnlWrongWayP1;
        private GameObject m_pnlWrongWayP2;
        private bool m_IsWrongWayP1Active = false;
        private bool m_IsWrongWayP2Active = false;

        //Lap - Position Object - P1
        private GameObject m_pnlLapPosP1;
        private Text m_lblHudLapValueP1;
        private Text m_lblHudLapTextP1;
        private Text m_lblHudPosValueP1;
        
        private Text m_lblHudCurrentTimeValueP1;
        private Text m_lblHudBestTimeValueP1;
        private Text m_lblHudBestTimeTextP1;

        //Lap - Position Object - P2
        private GameObject m_pnlLapPosP2;
        private Text m_lblHudLapValueP2;
        
        private Text m_lblHudPosValueP2;
        
        private Text m_lblHudCurrentTimeValueP2;
        private Text m_lblHudBestTimeValueP2;

        //PlayerGrid In Game
        private GameObject m_pnlGridPlayers;
        private GameObject m_GridPlayersRowTemplate;
        private GameObject m_pnlHud;
        //results
        private GameObject m_pnlResults;


        private Race_Manager m_RaceManager;
        private Racer_Detail m_PlayerDetailP1;
        private Racer_Detail m_PlayerDetailP2;

        private bool m_IsUiInitilized = false;

        //TouchDrive Panel 
        private GameObject m_pnlTouchDrive;

        

        //PlayerGrid Stuff
        public float m_PlayerGridUpdateInterval;
        private float m_TimeStart;
        private float m_GridUpdateNext;
        public bool m_EnablePlayerGrid = false;

        //timer friction stuff
        public bool m_EnableTimerFriction=false;
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
        void InitializeUIStuff()
        {
            if (m_RaceManager != null)
            {
                m_pnlHud = m_UiCanvas.FindInChildren("pnlHud").gameObject;

                m_pnlGridPlayers = m_pnlHud.FindInChildren("pnlGridRacers").gameObject;

                m_pnlLapPosP1 = m_UiCanvas.FindInChildren("pnlLapPosP1").gameObject;
                m_pnlLapPosP2 = m_UiCanvas.FindInChildren("pnlLapPosP2").gameObject;

                m_pnlWrongWayP1 = m_UiCanvas.FindInChildren("pnlWrongWayP1").gameObject;
                m_pnlWrongWayP1.GetComponent<CanvasGroup>().alpha = 0;

                m_pnlWrongWayP2 = m_UiCanvas.FindInChildren("pnlWrongWayP2").gameObject;
                m_pnlWrongWayP2.GetComponent<CanvasGroup>().alpha = 0;


                m_IsWrongWayP1Active = false;
                m_IsWrongWayP2Active = false;

                m_pnlMessage = m_UiCanvas.FindInChildren("pnlMessage").gameObject;
                m_pnlMessage.GetComponent<CanvasGroup>().alpha = 0;



                m_pnlCountDown = m_UiCanvas.FindInChildren("pnlCountDown").gameObject;
                m_imgCountDown3 = m_pnlCountDown.FindInChildren("imgCountDown3").gameObject;
                m_imgCountDown2 = m_pnlCountDown.FindInChildren("imgCountDown2").gameObject;
                m_imgCountDown1 = m_pnlCountDown.FindInChildren("imgCountDown1").gameObject;
                m_pnlCountDown.GetComponent<CanvasGroup>().alpha = 1;

                //Lap - Position - P1
                m_lblHudLapValueP1 = m_pnlLapPosP1.FindInChildren("lblHudLapValue").GetComponent<Text>();
                m_lblHudLapTextP1 = m_pnlLapPosP1.FindInChildren("lblHudLapText").GetComponent<Text>();

                m_lblHudPosValueP1 = m_pnlLapPosP1.FindInChildren("lblHudPosValue").GetComponent<Text>();

                m_lblHudCurrentTimeValueP1 = m_pnlLapPosP1.FindInChildren("lblHudCurrentTimeVal").GetComponent<Text>();
                m_lblHudBestTimeValueP1 = m_pnlLapPosP1.FindInChildren("lblHudBestTimeVal").GetComponent<Text>();
                m_lblHudBestTimeTextP1 = m_pnlLapPosP1.FindInChildren("lblHudBestTimeText").GetComponent<Text>();

                //Lap - Position - P2
                m_lblHudLapValueP2 = m_pnlLapPosP2.FindInChildren("lblHudLapValue").GetComponent<Text>();

                m_lblHudPosValueP2 = m_pnlLapPosP2.FindInChildren("lblHudPosValue").GetComponent<Text>();

                m_lblHudCurrentTimeValueP2 = m_pnlLapPosP2.FindInChildren("lblHudCurrentTimeVal").GetComponent<Text>();
                m_lblHudBestTimeValueP2 = m_pnlLapPosP2.FindInChildren("lblHudBestTimeVal").GetComponent<Text>();

                //pnlResults
                m_pnlResults = m_UiCanvas.FindInChildren("pnlResults").gameObject;
                m_pnlResults.GetComponent<CanvasGroup>().alpha = 0;

                m_pnlTimer = m_UiCanvas.FindInChildren("pnlTimer").gameObject;
                m_lblTimerMessage = m_pnlTimer.FindInChildren("lblTimerMessage").GetComponent<Text>();
                m_lblTimerValue = m_pnlTimer.FindInChildren("lblTimerValue").GetComponent<Text>();

                switch (m_RaceManager.RaceType)
                {
                    case RaceTypeEnum.TimeAttack:
                        m_pnlTimer.GetComponent<CanvasGroup>().alpha = 1;
                        m_lblTimerMessage.text = "To Next Checkpoint";
                        break;
                    case RaceTypeEnum.Sprint:
                        m_lblHudLapTextP1.enabled = false;
                        m_lblHudLapValueP1.enabled = false;
                        m_lblHudBestTimeTextP1.text = "Progress";
                        break;
                }

                m_IsUiInitilized = true;


            }
        }

        void RaceManager_OnRaceInitiated()
        {
            BuildPlayerUIGrid();
        }


        void RaceManager_OnRacerKicked(Racer_Detail RacerData)
        {
            ShowMessage(RacerData.RacerName + " is Kicked!");
        }

        void RaceManager_OnCountDownStarted()
        {
            m_pnlCountDown.GetComponent<CanvasGroup>().alpha = 1;
        }

        void RaceManager_OnCountDownFinished()
        {
            //normally you should use the line below
            // pnlCountDown.GetComponent<CanvasGroup>().alpha = 0;
            //but since we're going to delay the hide, we need another way..;
            countFadeDelay = true;
            m_TimeStart = Time.time;
        }

        void RaceManager_OnRaceFinished(RaceTypeEnum RaceType)
        {
          
            m_pnlHud.GetComponent<CanvasGroup>().alpha = 0;
            m_pnlResults.FindInChildren("btnResultsRestart").GetComponent<Button>().Select() ;

            if (m_PlayerDetailP1 != null)
            {
                m_pnlResults.FindInChildren("lblPlayerNameP1").GetComponent<Text>().text = m_PlayerDetailP1.RacerName;
                m_pnlResults.FindInChildren("lblBestLapValueP1").GetComponent<Text>().text = RGKUI_Utils.FormatTime(m_PlayerDetailP1.RacerBestTime, true, 2);
                m_pnlResults.FindInChildren("lblTotalTimeValueP1").GetComponent<Text>().text = RGKUI_Utils.FormatTime(m_PlayerDetailP1.RacerTotalTime, true, 2);
            }

            if (m_PlayerDetailP2 != null)
            {
                m_pnlResults.FindInChildren("lblPlayerNameP2").GetComponent<Text>().text = m_PlayerDetailP2.RacerName;
                m_pnlResults.FindInChildren("lblBestLapValueP2").GetComponent<Text>().text = RGKUI_Utils.FormatTime(m_PlayerDetailP2.RacerBestTime, true, 2);
                m_pnlResults.FindInChildren("lblTotalTimeValueP2").GetComponent<Text>().text = RGKUI_Utils.FormatTime(m_PlayerDetailP2.RacerTotalTime, true, 2);
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
                  foreach (Racer_Detail RD in m_RaceManager.RegisteredRacers)
                {


                    GameObject racerRow = (GameObject)Instantiate(rowTemplate);
                    racerRow.name = "resultRow" + iRacer.ToString();
                    racerRow.transform.position = rowTemplate.transform.position;
                    racerRow.transform.rotation = rowTemplate.transform.rotation;
                    racerRow.transform.SetParent(resultsGrid.transform);
                    racerRow.transform.localScale = new Vector3(1, 1, 1);

                    if (RaceType != RaceTypeEnum.LapKnockout)
                    {
                        racerRow.FindInChildren("gridRowText").GetComponent<Text>().text = RD.RacerName;
                        racerRow.FindInChildren("gridRowValue").GetComponent<Text>().text = RGKUI_Utils.FormatTime(RD.RacerTotalTime, true, 2);
                    }
                    else
                    {
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
                    }

                  
                    iRacer++;
                }
                DestroyImmediate(rowTemplate);
            }
            else { Debug.Log("Results Grid Not Found!?"); }

        }

        GameObject gridRowParentForPlayerGrid;

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

        void LateUpdate()
        {
            DoUiUpdate();

            if (m_pnlGridPlayers != null && m_EnablePlayerGrid)
            {
                float mStart = Time.time - m_TimeStart;
                if (mStart >= m_GridUpdateNext)
                {
                    UpdatePlayerUIGrid();
                    m_GridUpdateNext = mStart + m_PlayerGridUpdateInterval;
                }
            }

            //CaptureFPS();
        }


        //void CaptureFPS()
        //{

        //    if (m_txtFPS == null)
        //        return;

        //    m_FPSTimeLeft -= Time.deltaTime;
        //    m_FPSAccum += Time.timeScale / Time.deltaTime;
        //    ++m_FPSFrames;

        //    if (m_FPSTimeLeft <= 0.0)
        //    {
        //        float fps = m_FPSAccum / m_FPSFrames;
        //        string format = System.String.Format("{0:F0} FPS", fps);

        //        m_txtFPS.text = format;

        //        m_FPSTimeLeft = m_FpsUpdateInterval;
        //        m_FPSAccum = 0.0F;
        //        m_FPSFrames = 0;


        //    }
        //}

        private void DoUiUpdate()
        {
            if (m_IsUiInitilized)
            {
                //Grab the RacerDetail of Player 
                if (m_PlayerDetailP1 == null && m_RaceManager.Player1 != null)
                {
                    m_PlayerDetailP1 = m_RaceManager.Player1.GetComponent<Racer_Register>().RacerDetail;
                }

                if (m_PlayerDetailP2 == null && m_RaceManager.Player2 != null)
                {
                    m_PlayerDetailP2 = m_RaceManager.Player2.GetComponent<Racer_Register>().RacerDetail;
                }

                //**************************************** PLAYER 1 ************************************
                if (m_PlayerDetailP1 != null)
                {
                    //Set lap and positio names
                    m_lblHudCurrentTimeValueP1.text = RGKUI_Utils.FormatTime(m_PlayerDetailP1.RacerTotalTime, m_EnableTimerFriction, 1);

                    m_lblHudLapValueP1.text = m_PlayerDetailP1.RacerLap + "/" + m_RaceManager.RaceLaps.ToString();
                    m_lblHudPosValueP1.text = m_PlayerDetailP1.RacerStanding + "/" + m_RaceManager.RegisteredRacers.Count;

                    switch (m_RaceManager.RaceType)
                    { 
                        case RaceTypeEnum.Sprint:
                            float mDistance = (m_RaceManager.RaceLength - m_PlayerDetailP1.RacerDistance);
                            if (mDistance<0) mDistance=0;

                            m_lblHudBestTimeValueP1.text = string.Format("{0:0} %", ((mDistance * 100) / m_RaceManager.RaceLength));
                            break;
                        default:
                            m_lblHudBestTimeValueP1.text = RGKUI_Utils.FormatTime(m_PlayerDetailP1.RacerBestTime, true, 1);
                            break;

                    }

                    

                    //set wrong way indicator
                    if (m_PlayerDetailP1.RacerWrongWay)
                    {
                        if (!m_IsWrongWayP1Active)
                        {
                            m_pnlWrongWayP1.GetComponent<CanvasGroup>().alpha = 1;
                            m_IsWrongWayP1Active = true;//This is for performance optimization..
                        }
                    }
                    else
                    {
                        if (m_IsWrongWayP1Active)
                        {
                            m_pnlWrongWayP1.GetComponent<CanvasGroup>().alpha = 0;
                            m_IsWrongWayP1Active = false;
                        }
                    }


                    if (m_RaceManager.RaceLaps>1 && m_PlayerDetailP1.RacerLap == m_RaceManager.RaceLaps && !finalLapDisplayed)
                    {
                        ShowMessage("Final Lap!");
                        finalLapDisplayed = true;
                    }

                }

                //**************************************** PLAYER 2 ************************************
                if (m_PlayerDetailP2 != null)
                {
                    //Set lap and positio names
                    m_lblHudCurrentTimeValueP2.text = RGKUI_Utils.FormatTime(m_PlayerDetailP2.RacerTotalTime, m_EnableTimerFriction, 1);

                    m_lblHudLapValueP2.text = m_PlayerDetailP2.RacerLap + "/" + m_RaceManager.RaceLaps.ToString();
                    m_lblHudPosValueP2.text = m_PlayerDetailP2.RacerStanding + "/" + m_RaceManager.RegisteredRacers.Count;

                    switch (m_RaceManager.RaceType)
                    {
                        case RaceTypeEnum.Sprint:
                            float mDistance = (m_RaceManager.RaceLength - m_PlayerDetailP2.RacerDistance);
                            if (mDistance < 0) mDistance = 0;

                            m_lblHudBestTimeValueP2.text = string.Format("{0:0} %", ((mDistance * 100) / m_RaceManager.RaceLength));
                            break;
                        default:
                            m_lblHudBestTimeValueP2.text = RGKUI_Utils.FormatTime(m_PlayerDetailP2.RacerBestTime, true, 1);
                            break;

                    }



                    //set wrong way indicator
                    if (m_PlayerDetailP2.RacerWrongWay)
                    {
                        if (!m_IsWrongWayP2Active)
                        {
                            m_pnlWrongWayP2.GetComponent<CanvasGroup>().alpha = 1;
                            m_IsWrongWayP2Active = true;//This is for performance optimization..
                        }
                    }
                    else
                    {
                        if (m_IsWrongWayP2Active)
                        {
                            m_pnlWrongWayP2.GetComponent<CanvasGroup>().alpha = 0;
                            m_IsWrongWayP2Active = false;
                        }
                    }


                    if (m_RaceManager.RaceLaps > 1 && m_PlayerDetailP2.RacerLap == m_RaceManager.RaceLaps && !finalLapDisplayed)
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

        private float countFadeDelayTimer = 1;
        private bool countFadeDelay = false;

        private float countFadeTimer = 1;
        private bool countStartFade = false;

        private float messageFadeDelayTimer = 1;
        private bool messageFadeDelay = false;

        private float messageFadeTimer = 1;
        private bool messageStartfade = false;

        private bool finalLapDisplayed = false;

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

        private void ShowMessage(string Message)
        {
            messageFadeDelayTimer = 1;
            messageFadeTimer = 1;
            messageFadeDelay = true;
            m_pnlMessage.GetComponent<CanvasGroup>().alpha = 1;
            m_pnlMessage.FindInChildren("lblMessage").GetComponent<Text>().text = Message;
        }

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