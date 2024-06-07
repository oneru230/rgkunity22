using System;
using System.Linq;
using UnityEngine;

namespace RacingGameKit.UI
{
    public static class RGKUI_Utils
    {
        //public static GameObject FindInChildren(this GameObject go, string name)
        //{
        //    return (from x in go.GetComponentsInChildren<Transform>()
        //            where x.gameObject.name == name
        //            select x.gameObject).First();
        //}

        public static GameObject FindInChildren(this GameObject gameObject, string name)
        {
            if (gameObject == null) return null;
            Transform transform = gameObject.transform;
            Transform child = FindChildTransform(transform, name);
            return child != null ? child.gameObject : null;
        }

        public static Transform FindChildTransform(Transform transform, string name)
        {
            if (transform == null) return null;
            int count = transform.childCount;
            for (int i = 0; i < count; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.name == name) return child;
                Transform subChild = FindChildTransform(child, name);
                if (subChild != null) return subChild;
            }
            return null;
        }
        public static string FormatSpeed(float SpeedValue, bool IsMile)
        {
            String strReturn = "-- Km/h";
            strReturn=String.Format("{0:0} Km/h", SpeedValue);
            return strReturn;
        }
        public static string FormatTime(float TimeValue, bool ShowFraction, int FractionDecimals)
        {
            String strReturn = "--:--.--";
            if (!ShowFraction)
            { strReturn = "--:--"; }

            if (TimeValue > 0)
            {
                TimeSpan tTime = TimeSpan.FromSeconds(TimeValue);

                float minutes = tTime.Minutes;
                float seconds = tTime.Seconds;

                

                if (ShowFraction)
                {
                    if (FractionDecimals == 1)
                    {
                        float fractions = (TimeValue * 10) % 10;
                        if (fractions >9) fractions = 0;
                        strReturn = String.Format("{0:00}:{1:00}.{2:0}", minutes, seconds, fractions); 
                    }
                    else
                    {
                        float fractions = (TimeValue * 100) % 100;
                        if (fractions >99) fractions = 0;
                        strReturn = String.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, fractions);
                    }
                }
                else
                {
                    strReturn = String.Format("{0:00}.{1:00}", minutes, seconds);
                }
            }
            return strReturn;
        }

        public static string Ordinal(int number)
        {
            string suffix = String.Empty;

            int ones = number % 10;
            int tens = (int)Math.Floor(number / 10M) % 10;

            if (tens == 1)
            {
                suffix = "th";
            }
            else
            {
                switch (ones)
                {
                    case 1:
                        suffix = "st";
                        break;

                    case 2:
                        suffix = "nd";
                        break;

                    case 3:
                        suffix = "rd";
                        break;

                    default:
                        suffix = "th";
                        break;
                }
            }
            return String.Format("{0}{1}", number, suffix);
        }

        public static AudioSource CreateAudioSource(Transform Parent, string AudioSourceName, bool Loop)
        {
            GameObject oAudioSource = new GameObject(AudioSourceName);
            oAudioSource.transform.parent = Parent;
            oAudioSource.transform.localPosition = Vector3.zero;
            oAudioSource.transform.localRotation = Quaternion.identity;
            AudioSource AS = (AudioSource)oAudioSource.AddComponent(typeof(AudioSource));
            AS.loop = Loop;
            return AS;
        }
    }
}