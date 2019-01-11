using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


public class DrillStop : MonoBehaviour
{
    // Use this for initialization
 
    public void OnSelect()
    {
        GameObject.Find("BUTTON/Start").GetComponent<DrillStart>().enabled = false;
        Debug.Log("stopdrill");
        StartCoroutine(Upload());
    }


    /*private void Update()
    {
        cube.transform.Rotate(0.0f, -Time.deltaTime * 200, 0.0f);
    }*/
    // Update is called once per frame

    IEnumerator Upload()
    {
        WWWForm form = new WWWForm();
        form.AddField("DrillStop", "DrillStop");


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