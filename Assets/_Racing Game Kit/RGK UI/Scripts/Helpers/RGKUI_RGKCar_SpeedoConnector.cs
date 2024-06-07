// This script is created for displaying speed data to  UI.
// Besides RGKCar, you can use it on any vehicle components. 

using UnityEngine;
using RacingGameKit;
using RacingGameKit.RGKCar;


namespace RacingGameKit.UI
{
    [AddComponentMenu("Racing Game Kit/UI/SpeedoUI Connector")]
    [RequireComponent(typeof(RGKCar_Engine))]
    public class RGKUI_RGKCar_SpeedoConnector : MonoBehaviour
    {
        private Race_Manager m_RaceManager;
        private RGKCar_Setup m_RGKCarSetup;
        private RGKCar_Engine m_RGKCarEngine;
        private bool IsSplitScreen = false;
        SpeedoUI m_SpeedoUI;

        // IF YOU WANT TO USE DAMAGEFX IN SPEEDOUI UNCOMMENT LINES BELOW
        // LINE 25
        // LINE 31
        // LINE 86 to 89 
        // private DamageFX m_RGKCarDamage;

        void Start()
        {
            m_RGKCarSetup = GetComponent<RGKCar_Setup>();
            m_RGKCarEngine = GetComponent<RGKCar_Engine>();
            //m_RGKCarDamage = GetComponent<DamageFX>();

            GameObject oRM = GameObject.Find("_RaceManager");
            if (oRM != null)
            {
                m_RaceManager = oRM.GetComponent<Race_Manager>();
                if (m_RaceManager.SplitScreen) IsSplitScreen = true;
            }

            if (!IsSplitScreen)
            {
                GameObject oSpeedoUI = GameObject.Find("SpeedoUI");
                if (oSpeedoUI != null)
                { m_SpeedoUI = oSpeedoUI.GetComponent<SpeedoUI>(); }

            }
            else
            {
                if (m_RaceManager.Player1 == this.gameObject)
                {

                    GameObject oSpeedoUI = GameObject.Find("SpeedoUIP1");
                    if (oSpeedoUI != null)
                    { m_SpeedoUI = oSpeedoUI.GetComponent<SpeedoUI>(); }
                }
                else
                {

                    GameObject oSpeedoUI = GameObject.Find("SpeedoUIP2");
                    if (oSpeedoUI != null)
                    { m_SpeedoUI = oSpeedoUI.GetComponent<SpeedoUI>(); }
                }
            }


        }

        void FixedUpdate()
        {
            if (m_SpeedoUI != null)
            {
                m_SpeedoUI.Speed = m_RGKCarEngine.SpeedAsKM;
                string strGear = m_RGKCarEngine.Gear.ToString();
                if (m_RGKCarEngine.Gear == 0)
                {
                    strGear = "N";
                }
                else if (m_RGKCarEngine.Gear == -1)
                {
                    strGear = "R";
                }
                m_SpeedoUI.Gear = strGear;
                m_SpeedoUI.Rpm = m_RGKCarEngine.RPM;
                m_SpeedoUI.Nitro = (m_RGKCarSetup.Nitro.NitroLeft / m_RGKCarSetup.Nitro.InitialAmount) * 100;

                //if (m_RGKCarDamage != null)
                //{
                //    m_SpeedoUI.Repair = m_RGKCarDamage.Health.VehicleHealth;
                //}
            }
        }
    }
}