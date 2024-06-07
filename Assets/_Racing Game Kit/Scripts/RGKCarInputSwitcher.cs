/* Since current rgk uses different controller for devices/platforms and most
 * developers forgetting to switching this inputs to proper controller, I created this small
 * script to avoid confusion. Just disable if you dont want to use this switch mechanism.
 * Cheers.
 */


using UnityEngine;
using System.Collections;
using RacingGameKit;
using RacingGameKit.RGKCar.CarControllers;


public class RGKCarInputSwitcher : MonoBehaviour
{
    public bool m_UseGamePad = false;
    private bool m_SetByExternalCall = false;
    Race_Manager m_RaceManager;
    RGKCar_C2_Human m_KeyboarcController;
    RGKCar_C2_MobileRace m_MobileController;
    RGKCar_C2_GamePadRace m_GamePadController;

    void Awake()
    {
        GameObject oRM = GameObject.Find("_RaceManager");
        if (oRM != null) m_RaceManager = oRM.GetComponent<Race_Manager>();

        m_KeyboarcController = GetComponent<RGKCar_C2_Human>();
        m_MobileController = GetComponent<RGKCar_C2_MobileRace>();
        m_GamePadController = GetComponent<RGKCar_C2_GamePadRace>();
        if (m_KeyboarcController != null) m_KeyboarcController.enabled = false;
        if (m_GamePadController != null) m_GamePadController.enabled = false;
        if (m_MobileController != null) m_MobileController.enabled = false;
    }

    void Start()
    {
        
        if (m_SetByExternalCall) return;
        if (m_RaceManager.SplitScreen) return;

        if (Application.platform == RuntimePlatform.tvOS)
        {
            m_KeyboarcController.enabled = false;
            m_GamePadController.enabled = false;
            m_MobileController.enabled = true;
            //this apple scpefific, if doesn't work, set it true
            m_MobileController.UseXAxis = false;
        }
        else if (Application.isMobilePlatform)
        {
            m_KeyboarcController.enabled = false;
            m_GamePadController.enabled = false;
            m_MobileController.enabled = true;
            if (Application.platform == RuntimePlatform.Android)
            {
                m_MobileController.UseXAxis = true;
                Debug.Log("Switching Player Controller To Android!");
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                m_MobileController.UseXAxis = false;
                Debug.Log("Switching Player Controller To iOS!");
            }
        }
        else
        {
            if (!m_UseGamePad)
            {
                m_KeyboarcController.enabled = true;
                m_MobileController.enabled = false;
                m_GamePadController.enabled = false;
                Debug.Log("Switching Player Controller To Keyboard!");
            }
            else
            {
                m_GamePadController.enabled = true;
                m_KeyboarcController.enabled = false;
                m_MobileController.enabled = false;
                Debug.Log("Switching Player Controller To GamePad!");
            }
        }

    }

    public void SwitchController(bool ToGamePad)
    {
        m_UseGamePad = ToGamePad;

        if (!m_UseGamePad)
        {
            m_KeyboarcController.enabled = true;
            m_MobileController.enabled = false;
            m_GamePadController.enabled = false;
            Debug.Log("Switching Player Controller To Keyboard!");
        }
        else
        {
            m_GamePadController.enabled = true;
            m_KeyboarcController.enabled = false;
            m_MobileController.enabled = false;
            Debug.Log("Switching Player Controller To GamePad!");
        }
        m_SetByExternalCall = true;
    }


}
