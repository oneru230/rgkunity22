using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace RacingGameKit.UI
{
    [AddComponentMenu("")] //Menu removal
    public class RGKUI_Database : MonoBehaviour
    {
        public RGKUI_CarData[] CarData;
        public RGKUI_RaceData[] RaceData;
        public RGKUI_AiData[] AiData;
    }

    [Serializable]
    public class RGKUI_CarData
    {
        public string CarName;
        public string CarSpeed;
        public string CarPower;
        public string CarAcc;
        public string CarDrive;
        public GameObject ShowcasePrefab;
        public GameObject PlayerPrefab;
    }

    [Serializable]
    public class RGKUI_RaceData
    {
        public string RaceTitle;
        public string TrackName;
        public int TrackIndex;
        public Sprite TrackSprite;
        [TextArea(maxLines:5,minLines:2)]
        public string RaceInfo;
        public string RaceType;
        public int RaceLaps;
        public int Opponents;
        public RaceTypeEnum TrackRaceTypeEnum;
        public eSpeedTrapMode SpeedTrapEnum;
        public string TrackAiIndexes;
    }
    [Serializable]
    public class RGKUI_AiData
    {
        public GameObject AiPrefab;
    }
}