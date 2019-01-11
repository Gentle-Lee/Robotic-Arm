using System.Collections;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

public class Register : MonoBehaviour 
{
    bool isOnce = true;
    //bool check = false;
    //GameObject regC;
    // Use this for initialization

    public void OnSelect () 
    {
        GameObject.Find("drill1").GetComponent<PostMessage>().enabled = true;
       // check = true;
        //regC = GameObject.Find("BUTTON/Reg_Complete");
        if(isOnce) StartCoroutine(Upload());
        isOnce = false;
    }


    // Update is called once per frame

    IEnumerator Upload()
    {
        WWWForm form = new WWWForm();
        form.AddField("regC", "regC");


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
