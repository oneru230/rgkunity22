///This is a sample static data for passing configuration data from start screen to levels.

using UnityEngine;


namespace RacingGameKit.UI
{
    public static class RGKUI_StaticData
    {

        //This is the actual prefab of the selected racer car from car database
        public static GameObject m_SelectedVehiclePrefab;
        //Selected Car Index from Car Database
        public static int m_SelectedCarIndex;
        //Selected Track Index from Track Database
        public static int m_SelectedTrackIndex;

        public static GameObject[]  m_CurrentRaceAis;
        public static int m_CurrentRaceLaps;
        public static RaceTypeEnum m_CurrentRaceTypeEnum;
        public static eSpeedTrapMode m_CurrentRaceSpeedTrapEnum;

        //********************************
        public static int m_ConfigVideoQuality = 1;
        public static bool m_ConfigParticles = true;
        public static float m_ConfigAudioMusic = .7f;
        public static float m_ConfigAudioSFX = .8f;
        public static int m_ConfigControl = 2;
        public static bool m_ConfigControlsFlipped = false;
        public static float m_ConfigControlSensitivity = 1;
        public static bool m_ConfigControlEnableGamepad = false;

        public static bool m_FromRace = false;
        public static bool m_FromMain = false;

    }
}