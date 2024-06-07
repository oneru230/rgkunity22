using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using RacingGameKit;


namespace RacingGameKit.Editors
{
    public class WaypointUtils : Editor
    {
        [MenuItem("Racing Game Kit/Waypoint Utils/Reverse Waypoint Order", false, 71)]
        public static void Reverse()
        {
            bool showError = false;

            GameObject oWP = GameObject.Find("_WayPoints");

            if (oWP != null)
            {
                WayPointManager oWPM = oWP.GetComponent(typeof(WayPointManager)) as WayPointManager;

                if (oWPM == null)
                {
                    showError = true;
                }
            }
            else
            {
                showError = true;
            }

            if (showError)
            {
                EditorUtility.DisplayDialog("Opps..", "Waypoint Container not found!", "OK");
                return;
            }

            bool blnAnswer = EditorUtility.DisplayDialog(
                                          "Confirm Next Action",
                                          "You are about to change waypoint configuration. After this acton complete, your first waypoint will become last waypoint and your last waypoint wll become first waypoint." +
                                          "\r\n\r\nYou should check speed breakers on this waypoins. Also you need to edit spawn points too. " +
                                          "\r\n\r\nAre you sure want to continue?",
                                          "Yes",
                                          "No");

            if (!blnAnswer)
            {
                return;
            }



            List<GameObject> waypoints = new List<GameObject>();

            for (int i = 0; i < oWP.transform.childCount; ++i)
            {
                if (oWP.transform.GetChild(i).GetComponent(typeof(WayPointItem)) != null)
                {
                    waypoints.Add(oWP.transform.GetChild(i).gameObject);
                }
            }

            //foreach (GameObject go in Selection.gameObjects)
            //{
            //    if (go.GetComponent<WayPointItem>() != null)
            //        waypoints.Add(go);
            //}
            waypoints.Sort(new SortGameObjectByName());

            for (int i = 0; i < waypoints.Count; ++i)
            {
                waypoints[i].name = string.Format("{0:00}", (i + 1));
                waypoints[i].transform.localEulerAngles = new Vector3(waypoints[i].transform.localEulerAngles.x, waypoints[i].transform.localEulerAngles.y + 180, waypoints[i].transform.localEulerAngles.z);
            }

            EditorUtility.DisplayDialog("Complete", "Waypoints order reversed.", "OK");
        }
        [MenuItem("Racing Game Kit/Waypoint Utils/Flip Widers", false, 72)]
        public static void FlipWiders()
        {
            bool showError = false;

            GameObject oWP = GameObject.Find("_WayPoints");

            if (oWP != null)
            {
                WayPointManager oWPM = oWP.GetComponent(typeof(WayPointManager)) as WayPointManager;

                if (oWPM == null)
                {
                    showError = true;
                }
            }
            else
            {
                showError = true;
            }

            if (showError)
            {
                EditorUtility.DisplayDialog("Opps..", "Waypoint Container not found!", "OK");
                return;
            }

            bool blnAnswer = EditorUtility.DisplayDialog(
                                          "Confirm Next Action",
                                          "You are about to change waypoint configuration. After this action complete, left wider values will be assigned to right wider values, vice versa." +
                                          "\r\n\r\nThis will be only applied on seperated widers. Non seperated widers will not effected." +
                                          "\r\n\r\nAre you sure want to continue?",
                                          "Yes",
                                          "No");

            if (!blnAnswer)
            {
                return;
            }



            //List<GameObject> waypoints = new List<GameObject>();

            for (int i = 0; i < oWP.transform.childCount; ++i)
            {
                if (oWP.transform.GetChild(i).GetComponent(typeof(WayPointItem)) != null)
                {
                    WayPointItem oWorkingWPItem = oWP.transform.GetChild(i).GetComponent(typeof(WayPointItem)) as WayPointItem;
                    if (oWorkingWPItem.SeperatedWiders)
                    {
                        float _tmpLeft = oWorkingWPItem.LeftWide;
                        float _tmpRight = oWorkingWPItem.RightWide;
                        oWorkingWPItem.LeftWide = _tmpRight;
                        oWorkingWPItem.RightWide = _tmpLeft;
                    }
                }
            }


            EditorUtility.DisplayDialog("Complete", "Waypoints Widers Flipped.", "OK");
        }

        [MenuItem("Racing Game Kit/Waypoint Utils/Clean SpeedBrake Values", false, 73)]
        public static void CleanSpeedBrakes()
        {
            bool showError = false;

            GameObject oWP = GameObject.Find("_WayPoints");

            if (oWP != null)
            {
                WayPointManager oWPM = oWP.GetComponent(typeof(WayPointManager)) as WayPointManager;

                if (oWPM == null)
                {
                    showError = true;
                }
            }
            else
            {
                showError = true;
            }

            if (showError)
            {
                EditorUtility.DisplayDialog("Opps..", "Waypoint Container not found!", "OK");
                return;
            }

            bool blnAnswer = EditorUtility.DisplayDialog(
                                          "Confirm Next Action",
                                          "You are about to change waypoint configuration. After this action complete, Speed Braker valies will set as 0." +
                                          "\r\n\r\nThen you can reconfigure your Speed Braker configuration." +
                                          "\r\n\r\nAre you sure want to continue?",
                                          "Yes",
                                          "No");

            if (!blnAnswer)
            {
                return;
            }



            //List<GameObject> waypoints = new List<GameObject>();

            for (int i = 0; i < oWP.transform.childCount; ++i)
            {
                if (oWP.transform.GetChild(i).GetComponent(typeof(WayPointItem)) != null)
                {
                    WayPointItem oWorkingWPItem = oWP.transform.GetChild(i).GetComponent(typeof(WayPointItem)) as WayPointItem;

                    oWorkingWPItem.SoftBrakeSpeed = 0;
                    oWorkingWPItem.HardBrakeSpeed = 0;

                }
            }

            EditorUtility.DisplayDialog("Complete", "Speed Brake values cleared.", "OK");
        }

        public class SortGameObjectByName : IComparer<GameObject>
        {
            public int Compare(GameObject a, GameObject b)
            {
                int ia = int.Parse(a.name);
                int ib = int.Parse(b.name);
                if (ia > ib)
                    return -1;
                else if (ia < ib)
                    return 1;
                else return 0;
            }
        }
        public class SortGameObjectByNameFw : IComparer<GameObject>
        {
            public int Compare(GameObject a, GameObject b)
            {
                int ia = int.Parse(a.name);
                int ib = int.Parse(b.name);
                if (ia > ib)
                    return 1;
                else if (ia < ib)
                    return -1;
                else return 0;
            }
        }
        List<Transform> m_OldWaypoints;
        [MenuItem("Racing Game Kit/Waypoint Utils/Set Start Waypoint", false, 74)]
        public static void ChangeStartWaypoint()
        {
            if (Selection.activeTransform == null)
            {
                return;
            }

            WayPointItem oSelected = Selection.activeTransform.GetComponent<WayPointItem>();


            bool showError = false;

            GameObject oWP = oSelected.transform.parent.gameObject;

            if (oWP != null)
            {
                WayPointManager oWPM = oWP.GetComponent(typeof(WayPointManager)) as WayPointManager;

                if (oWPM == null)
                {
                    showError = true;
                }
            }
            else
            {
                showError = true;
            }

            if (showError)
            {
                EditorUtility.DisplayDialog("Opps..", "Waypoint Container not found!", "OK");
                return;
            }

            bool blnAnswer = EditorUtility.DisplayDialog(
                                          "Confirm Next Action",
                                          "message here...",
                                          "Yes",
                                          "No");

            if (!blnAnswer)
            {
                return;
            }




            List<GameObject> waypoints = new List<GameObject>();
            List<GameObject> cutWaypoints = new List<GameObject>();

            for (int i = 0; i < oWP.transform.childCount; ++i)
            {
                if (oWP.transform.GetChild(i).GetComponent(typeof(WayPointItem)) != null)
                {
                    waypoints.Add(oWP.transform.GetChild(i).gameObject);
                }
            }

            waypoints.Sort(new SortGameObjectByNameFw());

            int newStartIndex = waypoints.FindIndex(x => x == oSelected.gameObject);

            for (int x = newStartIndex; x < waypoints.Count; ++x)
            {
                cutWaypoints.Add(waypoints[x]);
            }
            for (int y = 0; y < newStartIndex; ++y)
            {
                cutWaypoints.Add(waypoints[y]);
            }

            for (int i = 0; i < cutWaypoints.Count; ++i)
            {
                cutWaypoints[i].name = string.Format("{0:00}", (i + 1));
            }
        }


        [MenuItem("Racing Game Kit/Waypoint Utils/Change Start Waypoint", true, 74)]
        public static bool ValidateChangeStartWaypoint()
        {
            return isWaypointSelected();
        }

        [MenuItem("Racing Game Kit/Waypoint Utils/Set Start Number", false, 75)]
        public static void SetStartNumber()
        {
            if (Selection.activeTransform == null)
            {
                return;
            }

            WayPointItem oSelected = Selection.activeTransform.GetComponent<WayPointItem>();


            bool showError = false;

            GameObject oWP = oSelected.transform.parent.gameObject;

            if (oWP != null)
            {
                WayPointManager oWPM = oWP.GetComponent(typeof(WayPointManager)) as WayPointManager;

                if (oWPM == null)
                {
                    showError = true;
                }
            }
            else
            {
                showError = true;
            }

            if (showError)
            {
                EditorUtility.DisplayDialog("Opps..", "Waypoint Container not found!", "OK");
                return;
            }

            string path = EditorUtility.SaveFilePanelInProject("Type t", oSelected.transform.name + ".prefab", "prefab", "Please select file name to save prefab to:");
            int iNum = 0;

            if (!string.IsNullOrEmpty(path))
            {
                string[] val = path.Split('/');
                string num = val[val.Length - 1].Replace(".prefab", "");
                Debug.Log(num);
                iNum = System.Convert.ToInt32(num);
            }
            else
            {
                Debug.Log("CANCELLED");
                return;
            }


            if (iNum == 0) return;

            List<GameObject> waypoints = new List<GameObject>();

            for (int i = 0; i < oWP.transform.childCount; ++i)
            {
                if (oWP.transform.GetChild(i).GetComponent(typeof(WayPointItem)) != null)
                {
                    waypoints.Add(oWP.transform.GetChild(i).gameObject);
                }
            }


            for (int y = 0; y < waypoints.Count; ++y)
            {
                waypoints[y].name = string.Format("{0:00}", (y + iNum));
            }

        }
        private static bool isWaypointSelected()
        {
            bool blnRes = false;
            if (Selection.activeTransform != null)
            {
                WayPointItem oRM = Selection.activeTransform.GetComponent<WayPointItem>();
                if (oRM != null)
                {
                    blnRes = true;
                }
            }

            return blnRes;
        }
    }
}