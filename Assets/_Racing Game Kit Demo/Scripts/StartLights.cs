/// This script help you manage a startlights for track

using UnityEngine;
using System.Collections;
using RacingGameKit;
public class StartLights : MonoBehaviour
{

    private Race_Manager m_RaceManager;
    public Light Light3;
    public Light Light2;
    public Light Light1;
    public Light Light0;

    void Start()
    {

        GameObject RaceManagerObject = GameObject.Find("_RaceManager");
        if (RaceManagerObject != null)
        {
            m_RaceManager = RaceManagerObject.GetComponent<Race_Manager>();

            Light0.enabled = false;
            Light1.enabled = false;
            Light2.enabled = false;
            Light3.enabled = false;
        }
        else
        {
            this.enabled = false;
        }
    }


    void Update()
    {
        if (this.enabled && m_RaceManager!=null)
        {
            if (m_RaceManager.CurrentCount >= 0 && m_RaceManager.CurrentCount < 4)
            {
                switch (m_RaceManager.CurrentCount)
                {
                    case 3:
                        Light3.enabled = true;
                        break;
                    case 2:
                        Light2.enabled = true;
                        break;
                    case 1:
                        Light1.enabled = true;
                        break;
                    case 0:
                        Light1.enabled = false;
                        Light2.enabled = false;
                        Light3.enabled = false;
                        Light0.enabled = true;
                        break;
                }
            }
        }
    }
}