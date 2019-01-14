// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using SimpleHTTP;
using System.Collections;
using UnityEngine;


namespace Academy
{
    /// <summary>
    /// GestureAction performs custom actions based on 
    /// which gesture is being performed.
    /// </summary>
    public class GestureAction : MonoBehaviour, INavigationHandler, IManipulationHandler, ISpeechHandler
    {
        [Tooltip("Rotation max speed controls amount of rotation.")]
        [SerializeField]
        private float RotationSensitivity = 10.0f;

        private bool isNavigationEnabled = true;
        private bool isZoomEnabled = false;
        private bool isOperationEnabled = false;
        public bool IsZoomEnabled
        {
            get { return isZoomEnabled; }
            set { isZoomEnabled = value; }
        }
        public bool IsNavigationEnabled
        {
            get { return isNavigationEnabled; }
            set { isNavigationEnabled = value; }
        }

        private Vector3 manipulationOriginalPosition = Vector3.zero;

        void INavigationHandler.OnNavigationStarted(NavigationEventData eventData)
        {
            InputManager.Instance.PushModalInputHandler(gameObject);
        }

        void INavigationHandler.OnNavigationUpdated(NavigationEventData eventData)
        {
            if (isNavigationEnabled && !isOperationEnabled)
            {
                if (!isZoomEnabled)
                {
                    Debug.Log("rotate");
                    /* TODO: DEVELOPER CODING EXERCISE 2.c */

                    // 2.c: Calculate a float rotationFactor based on eventData's NormalizedOffset.x multiplied by RotationSensitivity.
                    // This will help control the amount of rotation.
                    Debug.Log(eventData.NormalizedOffset.x);
                    float rotationFactor = eventData.NormalizedOffset.x * RotationSensitivity;

                    // 2.c: transform.Rotate around the Y axis using rotationFactor.
                    transform.Rotate(new Vector3(0, -1 * rotationFactor, 0));
                }
                else
                {
                    Debug.Log("scale");
                    float scaleFactor = eventData.NormalizedOffset.x;
                    Vector3 change = new Vector3(0.01F, 0.01F, 0.01F);
                    if (scaleFactor > 0)
                    {
                        transform.localScale += change;
                    }
                    else if (scaleFactor < 0)
                    {
                        transform.localScale -= change;
                    }
                }
            }
            //if (isOperationEnabled)
            //{
            //    const string postURL = "127.0.0.1:7999/api/movement";
            //    float offsetX = eventData.NormalizedOffset.x;
            //    float offsetY = eventData.NormalizedOffset.y;
            //    float offsetZ = eventData.NormalizedOffset.z;
            //    if (offsetX > offsetY && offsetX > offsetZ ){
            //        Debug.Log("x => " + offsetX);
            //        postMessage(postURL, offsetX > 0 ? "operation=x+" : "operation=x-", Encoding.UTF8);
            //    }else if (offsetY > offsetX && offsetY > offsetZ){
            //        Debug.Log("y => " + offsetY);
            //        postMessage(postURL, offsetY > 0 ? "operation=y+" : "operation=y-", Encoding.UTF8);
            //    }else if (offsetZ > offsetX && offsetZ > offsetY){
            //        Debug.Log("z => " + offsetZ);
            //        postMessage(postURL, offsetY > 0 ? "operation=z+" : "operation=z-", Encoding.UTF8);
            //    }
            //}
        }
        IEnumerator Post()
        {
            // Let's say that this the object you want to create
            string formData = "operation=y+";
                               
            // Create the request object and use the helper function `RequestBody` to create a body from JSON
            Request request = new Request("http://172.29.39.199:7999/api/movement")
                .AddHeader("Content-Type", "application/x-www-form-urlencoded")
                .Post(RequestBody.From(formData));

            // Instantiate the client
            Client http = new Client();
            // Send the request
            yield return http.Send(request);
            
        }

        void INavigationHandler.OnNavigationCompleted(NavigationEventData eventData)
        {
            InputManager.Instance.PopModalInputHandler();
        }

        void INavigationHandler.OnNavigationCanceled(NavigationEventData eventData)
        {
            InputManager.Instance.PopModalInputHandler();
        }

        void IManipulationHandler.OnManipulationStarted(ManipulationEventData eventData)
        {
            if (!isNavigationEnabled && !isZoomEnabled && !isOperationEnabled)
            {
                InputManager.Instance.PushModalInputHandler(gameObject);

                manipulationOriginalPosition = transform.position;
            }
        }

        void IManipulationHandler.OnManipulationUpdated(ManipulationEventData eventData)
        {
            if (!isNavigationEnabled && !isZoomEnabled && !isOperationEnabled)
            {
                /* TODO: DEVELOPER CODING EXERCISE 4.a */

                // 4.a: Make this transform's position be the manipulationOriginalPosition + eventData.CumulativeDelta
                Debug.Log("move");
                transform.position = manipulationOriginalPosition + eventData.CumulativeDelta;
            }
           
        }

        void IManipulationHandler.OnManipulationCompleted(ManipulationEventData eventData)
        {
            InputManager.Instance.PopModalInputHandler();
        }

        void IManipulationHandler.OnManipulationCanceled(ManipulationEventData eventData)
        {
            InputManager.Instance.PopModalInputHandler();
        }

        void ISpeechHandler.OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            Debug.Log(eventData.RecognizedText.ToLower());
            if (eventData.RecognizedText.ToLower().Equals("move"))
            {
                isZoomEnabled = false;
                isNavigationEnabled = false;
                isOperationEnabled = false;
            }
            else if (eventData.RecognizedText.ToLower().Equals("rotate"))
            {
                isNavigationEnabled = true;
                isZoomEnabled = false;
                isOperationEnabled = false;
            }
            else if (eventData.RecognizedText.ToLower().Equals("expand"))
            {
                isZoomEnabled = true;
                isNavigationEnabled = true;
                isOperationEnabled = false;
            }
            else if (eventData.RecognizedText.ToLower().Equals("registration"))
            {
                isOperationEnabled = true;
                StartCoroutine(Post());
            }
            else
            {
                return;
            }

            eventData.Use();
        }
    }
}