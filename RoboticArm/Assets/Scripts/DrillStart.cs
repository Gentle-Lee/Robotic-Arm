using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


public class DrillStart : MonoBehaviour 
{
    //public bool isRotate = true;
   // private string cubeInfo = "Start";
    GameObject cube;
    // GameObject buttonText;

    // Use this for initialization

    public void OnSelect() 
    {
        cube = GameObject.Find("drill1/drill");
        GameObject.Find("BUTTON/Start").GetComponent<DrillStart>().enabled = true;
        StartCoroutine(Upload());
    }

    void Update()
    {
            cube.transform.Rotate(0.0f, Time.deltaTime * 2000, 0.0f);
    }

    // Update is called once per frame

    IEnumerator Upload() 
    {
        WWWForm form = new WWWForm();
        form.AddField("DrillStart", "DrillStart");


        using (UnityWebRequest www = UnityWebRequest.Post("http://172.26.250.80:8080/test2/Servlet", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }


    }

}
