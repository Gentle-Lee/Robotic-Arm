// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Academy
{
    /// <summary>
    /// CursorFeedback class takes GameObjects to give cursor feedback
    /// to users based on different states.
    /// </summary>
    public class CursorFeedback : MonoBehaviour
    {
        [Tooltip("Drag the GameObject to display when a scroll enabled Interactible is detected.")]
        [SerializeField]
        private GameObject scrollDetectedGameObject;

        [Tooltip("Drag the GameObject to display when a pathing enabled Interactible is detected.")]
        [SerializeField]
        private GameObject pathingDetectedGameObject;

        [Tooltip("Drag the GameObject to display when a zoom enabled Interactible is detected.")]
        [SerializeField]
        private GameObject zoomDetectedGameObject;

        private HoloToolkit.Unity.InputModule.Cursor cursor;

        private bool IsNavigationFocused
        {
            get
            {
                GameObject targeted = cursor.GetTargetedObject();
                if (targeted != null)
                {
                    GestureAction gestureAction = targeted.GetComponent<GestureAction>();
                    if (gestureAction != null)
                    {
                        return gestureAction.IsNavigationEnabled;
                    }
                    else
                    {
                        gestureAction = targeted.transform.root.GetComponent<GestureAction>();
                        if (gestureAction != null)
                        {
                            return gestureAction.IsNavigationEnabled;
                        }
                    }
                }

                return false;
            }
        }


        private bool IsZoomEnabled
        {
            get
            {
                GameObject targeted = cursor.GetTargetedObject();
                if (targeted != null)
                {
                    GestureAction gestureAction = targeted.GetComponent<GestureAction>();
                    if (gestureAction != null)
                    {
                        return gestureAction.IsZoomEnabled;
                    }
                    else
                    {
                        gestureAction = targeted.transform.root.GetComponent<GestureAction>();
                        if (gestureAction != null)
                        {
                            return gestureAction.IsZoomEnabled;
                        }
                    }
                }
                return false;
            }
        }

        private bool IsManipulationFocused
        {
            get
            {
                GameObject targeted = cursor.GetTargetedObject();
                if (targeted != null)
                {
                    GestureAction gestureAction = targeted.GetComponent<GestureAction>();
                    if (gestureAction != null)
                    {
                        return !gestureAction.IsNavigationEnabled;
                    }
                    else
                    {
                        gestureAction = targeted.transform.root.GetComponent<GestureAction>();
                        if (gestureAction != null)
                        {
                            return !gestureAction.IsNavigationEnabled;
                        }
                    }
                }

                return false;
            }
        }

        private void Awake()
        {
            cursor = GetComponent<HoloToolkit.Unity.InputModule.Cursor>();
        }

        private void Update()
        {
            UpdatePathDetectedState();

            UpdateScrollDetectedState();
        }

        private void UpdatePathDetectedState()
        {
            if (pathingDetectedGameObject == null)
            {
                return;
            }

            if (!IsManipulationFocused)
            {
                pathingDetectedGameObject.SetActive(false);
                return;
            }

            pathingDetectedGameObject.SetActive(true);
        }

        private void UpdateScrollDetectedState()
        {
            if (scrollDetectedGameObject == null)
            {
                Debug.Log("scroll object == null");
                return;
            }

            if (!IsNavigationFocused)
            {
                Debug.Log("navigation not focused");
                scrollDetectedGameObject.SetActive(false);
                zoomDetectedGameObject.SetActive(false);
                return;
            }
            if (IsZoomEnabled)
            {
                Debug.Log("zoom enabled");
                scrollDetectedGameObject.SetActive(false);
                zoomDetectedGameObject.SetActive(true);
            }
            else
            {
                Debug.Log("scroll enabled");
                scrollDetectedGameObject.SetActive(true);
                zoomDetectedGameObject.SetActive(false);
            }
        }
    }
}