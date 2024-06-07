using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using RacingGameKit.UI;
using RacingGameKit.TouchDrive;
using UnityEngine.EventSystems;

namespace RacingGameKit.UI
{
    [AddComponentMenu("Racing Game Kit/UI/Pause Menu UI")]
    public class RGKUI_PauseMenu : RGKPauseMenuBase
    {
        [Tooltip("Enable this if you're going to test mobile input on desktop. In devices it will be enabled automatically!")]
        public bool m_IsMobile = false;
        private bool m_IsTDPro = false;
        private bool m_IsGamePadActive = false;

        private Race_Manager m_RaceManager;
        private Race_Audio m_RaceAudio;
        iRGKTDM m_TouchDrive;

        public GameObject m_TouchDriveManager;
        public GameObject m_PnlHud;
        public GameObject m_PnlPause;
        public GameObject m_PnlSettings;


        private CanvasGroup m_CanvasHud;
        private CanvasGroup m_CanvasPause;
        private CanvasGroup m_CanvasSettings;
        private CanvasGroup m_CanvasTouchDrive;

        private CanvasGroup m_CanvasVideoPanel;
        private CanvasGroup m_CanvasAudioPanel;
        private CanvasGroup m_CanvasControlPanelTouchDrive;
        private CanvasGroup m_CanvasControlPanelTouchDrivePro;
        private CanvasGroup m_CanvasControlPanelDesktop;

        private Toggle m_SettingsVideoQualityLow;
        private Toggle m_SettingsVideoQualityMed;
        private Toggle m_SettingsVideoQualityHigh;
        private Toggle m_SettingsVideoParticles;
        private Slider m_SettingsAudioMusic;
        private Slider m_SettingsAudioSFX;

        private Toggle m_SettingsControllerType14;
        private Toggle m_SettingsControllerType15;


        private Slider m_SettingsControllerSensitivty;
        private Toggle m_SettingsControlsFlip;
        private int m_ConfigVideoQualityTemp;
        private bool m_ConfigParticlesTemp;
        private int m_ConfigControlsTemp;
        private bool m_ConfigControlsFlipTemp;
        private float m_ConfigControlsSensitivityTemp;

        private bool m_isPaused = false;


        //In this function we're going to access components and managers for setting their property. Check out start function that most important settings.
        void Awake()
        {
            GameObject oRacemanager = GameObject.Find("_RaceManager");
            if (oRacemanager != null)
            {
                m_RaceManager = oRacemanager.GetComponent<Race_Manager>();
            }

            if (m_RaceManager == null)
            {
                Debug.LogError("RaceManager not assigned to PauseMenu!");
            }
            else
            {
                m_RaceAudio = m_RaceManager.GetComponent<Race_Audio>();
                m_RaceManager.OnRaceInitiated += RaceManager_OnRaceInitiated;

                if (RGKUI_StaticData.m_FromMain)
                {
                    m_RaceManager.RaceInitsOnStartup = false;
                    m_RaceManager.RaceStartsOnStartup = false;
                }

                RGKTDM oTDM = (RGKTDM)FindObjectOfType(typeof(RGKTDM));
                if (oTDM != null)
                {
                    m_TouchDrive = (iRGKTDM)oTDM.gameObject.GetComponent(typeof(iRGKTDM));
                    m_IsTDPro = m_TouchDrive.IsPro;
                }
            }

            InitializeUIStuff();
            DedectPlatform();
            DedectGamePad();
            SetUIValuesToControls();

            //if (!m_IsMobile)
            //{
            //    StandaloneInputModule inputStandalone = EventSystem.current.gameObject.GetComponent<StandaloneInputModule>();
            //    if (inputStandalone != null)
            //    {
            //        inputStandalone.forceModuleActive = true;
            //    }

            //    TouchInputModule inputTouch = EventSystem.current.gameObject.GetComponent<TouchInputModule>();
            //    if (inputTouch != null)
            //    {
            //        //inputTouch.DeactivateModule();
            //        inputTouch.enabled = false;
            //    }
            //}
        }

        //In after accessing components we're going to set their values, check the race manager part particualrly.
        void Start()
        {
            ///UI Initials
            m_CanvasHud.alpha = 1;
            m_CanvasPause.alpha = 0;
            m_CanvasPause.blocksRaycasts = false;
            m_CanvasPause.interactable = false;
            ShowSettingsPanel(1);

            /* Alright, this is most important part.
             * As design you dont have to create a scene for each race. In this script I'm gonna show you how to configure RaceManager for different race types and different ais. 
             * 
             * Since we're able to dynamically configure race type, we're going to set race type with values from main screen . Check #1 mark (line 124) for this configuration
             * Also you can check RGKUI_MainMenu.cs Line 478 for how to set this values from previous screen.
             * 
             * Then we're going to set AI properties of this race. Check #2 Marked (line 135) This example uses very simple configuration for this purpose. In main menu database you'll see AiData array that holds AI prefabs
             * Only thing you have to set the indexes of this Ai's to TrackAi properties of TrackData.Ea; if you want 4 ai and you should set 0,0,0,0 of 1,1,1,1 . Also you can make combined like 0,1,1,0
             * DO NOT FORGET! They're array indexes. so they've start from 0 
             * 
             * 
             * And you need to set Player prefab. We're reading this value from a static class as RGKUI_StaticData.m_SelectedVehiclePrefab 
             * for easy access. And set back to race managers Human RacerPrefab propert. Check #3
             * 
             * 
             * And Finally we've to init Race.. Check #4 (line 160)
             * After initiation complete, we've to call StartRace.. Check #5 (line 201)
             */

            //RGKUI_StaticData.m_FromMain checks if we're landing here from main menu. If so, we've to set some specific configuratons like race audio, racer prefab etc. 
            if (RGKUI_StaticData.m_FromMain && m_RaceManager != null)
            {
                //This ai settings set as this configuration in case of we changed accidently. 
                m_RaceManager.AiSpawnMode = eAISpawnMode.OneTimeEach;
                m_RaceManager.AiNamingMode = eAINamingMode.Random;
                m_RaceManager.AiSpawnOrder = eAISpawnOrder.Order;

                //Show hide checkpoint arrow if needed.. Also this hides checkpoints from scene...
                if (RGKUI_StaticData.m_CurrentRaceTypeEnum == RaceTypeEnum.Speedtrap || RGKUI_StaticData.m_CurrentRaceTypeEnum == RaceTypeEnum.TimeAttack)
                {
                    m_RaceManager.EnableCheckpointArrow = true;
                    if (!m_RaceManager.CheckPoints.activeInHierarchy) m_RaceManager.CheckPoints.gameObject.SetActive(true);
                }
                else
                {
                    m_RaceManager.EnableCheckpointArrow = false;
                    if (m_RaceManager.CheckPoints.activeInHierarchy) m_RaceManager.CheckPoints.gameObject.SetActive(false);
                }

                // #1  Current race type settings based selected race configuration from main menu
                m_RaceManager.RaceType = RGKUI_StaticData.m_CurrentRaceTypeEnum;
                // Also if racetype is speedtrap, we should its configuration from main menu too.
                m_RaceManager.SpeedTrapMode = RGKUI_StaticData.m_CurrentRaceSpeedTrapEnum;
                //Current race lap settings based selected race configuration from main menu
                m_RaceManager.RaceLaps = RGKUI_StaticData.m_CurrentRaceLaps;

                //#2 Lets configure AI part of this game if its configured 
                if (RGKUI_StaticData.m_CurrentRaceAis != null)
                {
                    int aiCount = RGKUI_StaticData.m_CurrentRaceAis.Length;

                    //Expand the AiPrefab array size in RaceManager to given amount of AI
                    ChangeAIArraySize(aiCount);
                    // Assign Selected AI's to Racemanages airacer array.
                    for (int i = 0; i < aiCount; i++)
                    {
                        m_RaceManager.AIRacerPrefab[i] = RGKUI_StaticData.m_CurrentRaceAis[i];
                    }

                    //Set total racers as AI count+1 . That +1 for player
                    m_RaceManager.RacePlayers = aiCount + 1;
                }
                else
                {
                    //This means no AI in this game so we need a stack for player only.
                    m_RaceManager.RacePlayers = 1;
                }


                if (RGKUI_StaticData.m_SelectedVehiclePrefab != null)
                {
                    //#3
                    m_RaceManager.HumanRacerPrefab = RGKUI_StaticData.m_SelectedVehiclePrefab;
                }

                //C#
                m_RaceManager.InitRace();

                //We set the intital race audio volumes. This initials stored in RGKUI_StaticData file and it also get updated by Ui.
                if (m_RaceAudio != null)
                {
                    m_RaceAudio.BackgroundMusicVolume = RGKUI_StaticData.m_ConfigAudioMusic;
                    m_RaceAudio.EffectsSoundVolume = RGKUI_StaticData.m_ConfigAudioSFX;
                    m_RaceAudio.EngineSoundVolume = RGKUI_StaticData.m_ConfigAudioSFX;
                }
                //If we're going to use touchdrive, we've to update its initials too. Also this will be stored in RGKUI_StaticData file. 
                // This initials possbly changed in game by user, so we've to set this initials again to users choice.
                SetTouchDriveControls(RGKUI_StaticData.m_ConfigControl, RGKUI_StaticData.m_ConfigControlsFlipped);

                //If this game not mobile one, we've to set desktop control initials too. Ie gamepad or keyboard.
                if (!Application.isMobilePlatform)
                {
                    SwitchDesktopControl(RGKUI_StaticData.m_ConfigControlEnableGamepad);
                }

            }
            //This part get executed directly from scene.
            else if (m_TouchDrive != null)
            {
                SetTouchDriveControls(1, false);
                if (!m_RaceManager.RaceInitsOnStartup)
                {
                    m_RaceManager.InitRace(); ;
                }
            }


        }

        //This functon changes size ai prefab array of RaceManager based configuration.
        private void ChangeAIArraySize(int Amount)
        {
            GameObject[] temp = new GameObject[Amount];
            m_RaceManager.AIRacerPrefab = temp;

        }

        //#5 On race initiation complete we'll start race. 
        void RaceManager_OnRaceInitiated()
        {
            m_RaceManager.StartRace();
        }


        private void DedectPlatform()
        {
            if (Application.isMobilePlatform)
            {
                m_IsMobile = true;
            }


        }

        private void DedectGamePad()
        {
            string[] m_Jnames = Input.GetJoystickNames();

            if (m_Jnames.Length > 0)
            {
                for (int i = 0; i < m_Jnames.Length; i++)
                {
                    if (m_Jnames[i].Contains("Xbox 360")) m_IsGamePadActive = true;
                }
            }

            if (m_IsGamePadActive) m_SettingsControllerType15.interactable = true;
        }

        private void SwitchDesktopControl(bool EnableGamePad)
        {
            RGKCarInputSwitcher mInputSwitcher = m_RaceManager.Player1.GetComponent<RGKCarInputSwitcher>();
            if (mInputSwitcher != null)
            {
                if (EnableGamePad && m_IsGamePadActive)
                {
                    mInputSwitcher.SwitchController(EnableGamePad);
                }
                else
                { mInputSwitcher.SwitchController(false); }
            }
        }

        void SetTouchDriveControls(int Config, bool IsFlipped)
        {
            if (m_TouchDrive != null)
            {
                if (m_TouchDrive.IsPro)
                {
                    m_TouchDrive.FlipPositions = IsFlipped;
                }

                switch (Config)
                {
                    case 1:
                        m_TouchDrive.SwitchTemplate(20);
                        break;
                    case 2:
                        m_TouchDrive.SwitchTemplate(22);
                        break;
                    case 3:
                        m_TouchDrive.SwitchTemplate(23);
                        break;
                    case 4:
                        m_TouchDrive.SwitchTemplate(21);
                        break;
                    case 5:
                        m_TouchDrive.SwitchTemplate(0);
                        break;
                    case 6:
                        m_TouchDrive.SwitchTemplate(1);
                        break;
                    case 7:
                        m_TouchDrive.SwitchTemplate(2);
                        break;
                    case 8:
                        m_TouchDrive.SwitchTemplate(10);
                        break;
                    case 9:
                        m_TouchDrive.SwitchTemplate(11);
                        break;
                    case 10:
                        m_TouchDrive.SwitchTemplate(12);
                        break;
                }
                m_TouchDrive.SteeringSensitivity = RGKUI_StaticData.m_ConfigControlSensitivity;

            }
        }


        void Update()
        {
            if (m_TouchDrive != null)
            {
                if ((m_TouchDrive.TouchItems[10].IsPressed || Input.GetButtonUp("Cancel") || Input.GetKeyUp(KeyCode.Escape)) && !IsPaused)
                {
                    PauseGame();
                }
                else if (Input.GetKeyUp(KeyCode.Escape) && IsPaused)
                {
                    ResumeGame();
                }
            }
            else
            {
                if ((Input.GetButtonUp("Cancel") || Input.GetKeyUp(KeyCode.Escape)) && !IsPaused)
                {
                    PauseGame();
                }
                else if (Input.GetKeyUp(KeyCode.Escape) && IsPaused)
                {
                    ResumeGame();
                }
            }
        }


        private void InitializeUIStuff()
        {
            if (m_PnlHud != null)
            {
                m_CanvasHud = m_PnlHud.GetComponent<CanvasGroup>();
            }
            if (m_PnlPause != null)
            {
                m_CanvasPause = m_PnlPause.GetComponent<CanvasGroup>();
            }
            if (m_TouchDriveManager != null)
            {
                m_CanvasTouchDrive = m_TouchDriveManager.GetComponent<CanvasGroup>();
            }

            if (m_PnlSettings != null)
            {
                m_CanvasSettings = m_PnlSettings.GetComponent<CanvasGroup>();
                m_CanvasVideoPanel = m_PnlSettings.gameObject.FindInChildren("pnlVideoPanel").GetComponent<CanvasGroup>();
                m_CanvasAudioPanel = m_PnlSettings.gameObject.FindInChildren("pnlAudioPanel").GetComponent<CanvasGroup>();
                m_CanvasControlPanelTouchDrive = m_PnlSettings.gameObject.FindInChildren("pnlControlsPanelTouchDrive").GetComponent<CanvasGroup>();
                m_CanvasControlPanelTouchDrivePro = m_PnlSettings.gameObject.FindInChildren("pnlControlsPanelTouchDrivePro").GetComponent<CanvasGroup>();
                m_CanvasControlPanelDesktop = m_PnlSettings.gameObject.FindInChildren("pnlControlsPanelDesktop").GetComponent<CanvasGroup>();


                m_SettingsVideoQualityLow = m_PnlSettings.gameObject.FindInChildren("chkVideoLow").GetComponent<Toggle>();
                m_SettingsVideoQualityMed = m_PnlSettings.gameObject.FindInChildren("chkVideoMedium").GetComponent<Toggle>();
                m_SettingsVideoQualityHigh = m_PnlSettings.gameObject.FindInChildren("chkVideoHigh").GetComponent<Toggle>();
                m_SettingsVideoParticles = m_PnlSettings.gameObject.FindInChildren("chkVideoParticles").GetComponent<Toggle>();
                m_SettingsAudioMusic = m_PnlSettings.gameObject.FindInChildren("sliderSettingsAudioMusic").GetComponent<Slider>();
                m_SettingsAudioSFX = m_PnlSettings.gameObject.FindInChildren("sliderSettingsAudioSfx").GetComponent<Slider>();



                m_SettingsControllerType14 = m_PnlSettings.gameObject.FindInChildren("chkController14").GetComponent<Toggle>();
                m_SettingsControllerType15 = m_PnlSettings.gameObject.FindInChildren("chkController15").GetComponent<Toggle>();

                if (m_CanvasControlPanelTouchDrivePro != null)
                {
                    m_SettingsControllerSensitivty = m_CanvasControlPanelTouchDrivePro.gameObject.FindInChildren("sliderSettingsControlsSensitivity").GetComponent<Slider>();
                    m_SettingsControlsFlip = m_CanvasControlPanelTouchDrivePro.gameObject.FindInChildren("chkTDProFlip").GetComponent<Toggle>();
                }
                else if (m_CanvasControlPanelTouchDrive != null)
                { m_SettingsControllerSensitivty = m_CanvasControlPanelTouchDrive.gameObject.FindInChildren("sliderSettingsControlsSensitivity").GetComponent<Slider>(); }
            }


        }

        private void SetUIValuesToControls()
        {
            switch (RGKUI_StaticData.m_ConfigVideoQuality)
            {
                case 1:
                    m_SettingsVideoQualityLow.isOn = true;
                    break;
                case 2:
                    m_SettingsVideoQualityMed.isOn = true;
                    break;
                case 3:
                    m_SettingsVideoQualityHigh.isOn = true;
                    break;
            }


            for (int i = 1; i <= 14; i++)
            {
                bool blnIsChecked = false;
                {
                    if (RGKUI_StaticData.m_ConfigControl == i) blnIsChecked = true;
                }
                m_PnlSettings.gameObject.FindInChildren("chkController" + i + "").GetComponent<Toggle>().isOn = blnIsChecked;
            }



            if (RGKUI_StaticData.m_ConfigControlEnableGamepad && m_IsGamePadActive)
            {
                m_SettingsControllerType14.isOn = false;
                m_SettingsControllerType15.isOn = true;
            }
            else
            {
                m_SettingsControllerType14.isOn = true;
                m_SettingsControllerType15.isOn = false;
            }

            m_SettingsVideoParticles.isOn = RGKUI_StaticData.m_ConfigParticles;
            m_SettingsAudioMusic.value = RGKUI_StaticData.m_ConfigAudioMusic;
            m_SettingsAudioSFX.value = RGKUI_StaticData.m_ConfigAudioSFX;
            if (m_SettingsControllerSensitivty != null) m_SettingsControllerSensitivty.value = RGKUI_StaticData.m_ConfigControlSensitivity;
            if (m_SettingsControlsFlip != null) m_SettingsControlsFlip.isOn = RGKUI_StaticData.m_ConfigControlsFlipped;
            m_RaceAudio.BackgroundMusicVolume = RGKUI_StaticData.m_ConfigAudioMusic;
            m_RaceAudio.EffectsSoundVolume = RGKUI_StaticData.m_ConfigAudioSFX;
            m_RaceAudio.EngineSoundVolume = RGKUI_StaticData.m_ConfigAudioSFX;

        }

        public override void PauseGame()
        {
            if (m_isPaused) return;
            m_RaceAudio.Mute = true;
            m_CanvasHud.alpha = 0;
            m_CanvasPause.alpha = 1;
            m_CanvasPause.interactable = true;
            m_CanvasPause.blocksRaycasts = true;

            if (m_CanvasTouchDrive != null)
            {
                m_CanvasTouchDrive.alpha = 0;
                m_CanvasTouchDrive.blocksRaycasts = false;
                m_CanvasTouchDrive.interactable = false;
            }

            Time.timeScale = 0;
            m_isPaused = true;
        }

        public override void ResumeGame()
        {
            if (!m_isPaused) return;
            m_RaceAudio.Mute = false;
            m_CanvasHud.alpha = 1;
            m_CanvasPause.alpha = 0;
            m_CanvasPause.blocksRaycasts = false;
            m_CanvasPause.interactable = false;
            Time.timeScale = 1;

            if (m_CanvasTouchDrive != null)
            {
                m_CanvasTouchDrive.alpha = 1;
                m_CanvasTouchDrive.blocksRaycasts = true;
                m_CanvasTouchDrive.interactable = true;
            }


            m_isPaused = false;
        }

        public void UI_Restart()
        {
            ResumeGame();
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        public void UI_ExitToMain()
        {
            ResumeGame();
            RGKUI_StaticData.m_FromRace = true;
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        public void UI_ShowSettings()
        {
            if (m_CanvasSettings != null)
            {
                m_CanvasSettings.alpha = 1;
                m_CanvasSettings.blocksRaycasts = true;
                m_CanvasSettings.interactable = true;
            }
        }

        public override bool IsPaused
        {
            get { return m_isPaused; }
            set { m_isPaused = value; }
        }

        public void ShowSettingsPanel(int Panel)
        {
            switch (Panel)
            {
                case 1://Video Panel
                    m_CanvasVideoPanel.alpha = 1;
                    m_CanvasVideoPanel.blocksRaycasts = true;
                    m_CanvasVideoPanel.interactable = true;

                    m_CanvasAudioPanel.alpha = 0;
                    m_CanvasAudioPanel.blocksRaycasts = false;
                    m_CanvasAudioPanel.interactable = false;

                    m_CanvasControlPanelTouchDrive.alpha = 0;
                    m_CanvasControlPanelTouchDrive.blocksRaycasts = false;
                    m_CanvasControlPanelTouchDrive.interactable = false;

                    m_CanvasControlPanelTouchDrivePro.alpha = 0;
                    m_CanvasControlPanelTouchDrivePro.blocksRaycasts = false;
                    m_CanvasControlPanelTouchDrivePro.interactable = false;

                    m_CanvasControlPanelDesktop.alpha = 0;
                    m_CanvasControlPanelDesktop.blocksRaycasts = false;
                    m_CanvasControlPanelDesktop.interactable = false;
                    break;
                case 2: // Audio Panel
                    m_CanvasAudioPanel.alpha = 1;
                    m_CanvasAudioPanel.blocksRaycasts = true;
                    m_CanvasAudioPanel.interactable = true;

                    m_CanvasVideoPanel.alpha = 0;
                    m_CanvasVideoPanel.blocksRaycasts = false;
                    m_CanvasVideoPanel.interactable = false;

                    m_CanvasControlPanelTouchDrive.alpha = 0;
                    m_CanvasControlPanelTouchDrive.blocksRaycasts = false;
                    m_CanvasControlPanelTouchDrive.interactable = false;

                    m_CanvasControlPanelTouchDrivePro.alpha = 0;
                    m_CanvasControlPanelTouchDrivePro.blocksRaycasts = false;
                    m_CanvasControlPanelTouchDrivePro.interactable = false;

                    m_CanvasControlPanelDesktop.alpha = 0;
                    m_CanvasControlPanelDesktop.blocksRaycasts = false;
                    m_CanvasControlPanelDesktop.interactable = false;
                    break;
                case 3:
                    if (m_IsMobile && !m_IsTDPro)
                    {
                        m_CanvasControlPanelTouchDrive.alpha = 1;
                        m_CanvasControlPanelTouchDrive.blocksRaycasts = true;
                        m_CanvasControlPanelTouchDrive.interactable = true;

                        m_CanvasControlPanelDesktop.alpha = 0;
                        m_CanvasControlPanelDesktop.blocksRaycasts = false;
                        m_CanvasControlPanelDesktop.interactable = false;
                    }
                    else if (m_IsMobile && m_IsTDPro)
                    {
                        m_CanvasControlPanelTouchDrivePro.alpha = 1;
                        m_CanvasControlPanelTouchDrivePro.blocksRaycasts = true;
                        m_CanvasControlPanelTouchDrivePro.interactable = true;

                        m_CanvasControlPanelDesktop.alpha = 0;
                        m_CanvasControlPanelDesktop.blocksRaycasts = false;
                        m_CanvasControlPanelDesktop.interactable = false;
                    }
                    else
                    {
                        m_CanvasControlPanelTouchDrive.alpha = 0;
                        m_CanvasControlPanelTouchDrive.blocksRaycasts = false;
                        m_CanvasControlPanelTouchDrive.interactable = false;

                        m_CanvasControlPanelDesktop.alpha = 1;
                        m_CanvasControlPanelDesktop.blocksRaycasts = true;
                        m_CanvasControlPanelDesktop.interactable = true;
                    }

                    m_CanvasVideoPanel.alpha = 0;
                    m_CanvasVideoPanel.blocksRaycasts = false;
                    m_CanvasVideoPanel.interactable = false;
                    m_CanvasAudioPanel.alpha = 0;
                    m_CanvasAudioPanel.blocksRaycasts = false;
                    m_CanvasAudioPanel.interactable = false;
                    break;
            }
        }

        public void SaveAndCloseSettingsPanel()
        {
            if (m_CanvasSettings != null)
            {
                m_CanvasSettings.alpha = 0;
                m_CanvasSettings.blocksRaycasts = false;
                m_CanvasSettings.interactable = false;

                RGKUI_StaticData.m_ConfigVideoQuality = m_ConfigVideoQualityTemp;
                RGKUI_StaticData.m_ConfigParticles = m_ConfigParticlesTemp;
                RGKUI_StaticData.m_ConfigAudioMusic = m_SettingsAudioMusic.value;
                RGKUI_StaticData.m_ConfigAudioSFX = m_SettingsAudioSFX.value;
                RGKUI_StaticData.m_ConfigControl = m_ConfigControlsTemp;
                RGKUI_StaticData.m_ConfigControlsFlipped = m_ConfigControlsFlipTemp;

                RGKUI_StaticData.m_ConfigControlSensitivity = m_ConfigControlsSensitivityTemp;

                m_RaceAudio.BackgroundMusicVolume = RGKUI_StaticData.m_ConfigAudioMusic;
                m_RaceAudio.EffectsSoundVolume = RGKUI_StaticData.m_ConfigAudioSFX;
                m_RaceAudio.EngineSoundVolume = RGKUI_StaticData.m_ConfigAudioSFX;

                SetQuality(m_ConfigVideoQualityTemp);
                SetTouchDriveControls(RGKUI_StaticData.m_ConfigControl, RGKUI_StaticData.m_ConfigControlsFlipped);

                if (!Application.isMobilePlatform)
                {
                    if (m_ConfigControlsTemp == 15)
                    {
                        RGKUI_StaticData.m_ConfigControlEnableGamepad = true;
                        SwitchDesktopControl(true);
                    }
                    else if (m_ConfigControlsTemp == 14)
                    {
                        RGKUI_StaticData.m_ConfigControlEnableGamepad = false;
                        SwitchDesktopControl(false);
                    }
                }

            }
        }

        private void SetQuality(int QualityIndex)
        {
            QualitySettings.SetQualityLevel(QualityIndex, false);
        }

        public void SetSettingsParticles(bool Value)
        {
            m_ConfigParticlesTemp = Value;
        }

        public void SetSettingsFlipControls(bool Value)
        {
            m_ConfigControlsFlipTemp = Value;
        }
        public void SetSettingsVideoQualityTemp(int Selected)
        {
            m_ConfigVideoQualityTemp = Selected;
        }

        public void SetSettingsControllerTemp(int Selected)
        {
            m_ConfigControlsTemp = Selected;
        }

        public void SetSettingsControlsSensitivity(float Value)
        {
            m_ConfigControlsSensitivityTemp = Value;
        }

    }
}