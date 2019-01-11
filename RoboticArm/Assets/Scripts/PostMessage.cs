using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
//using Net;
using System;
using UnityEngine.XR.WSA.Input;

public class PostMessage : MonoBehaviour
{
    //GestureRecognizer recognizer;//生命手势捕捉函数GestureRecognizer（）
    // public GameObject FocusedObject { get; private set; }
    //  public Register reg;
    //  bool startMoving = false;

    public float x = 0.0f;
    public float y = 0.0f;
    public float z = 0.0f;
    public float Old_x = 0.0f;
    public float Old_y = 0.0f;
    public float Old_z = 0.0f;
    float delta_x = 0.0f;
    float delta_y = 0.0f;
    float delta_z = 0.0f;
    //---------------------------------------------------------------------------------------------------------
    //添加cube重定位
    GameObject Cube_obj;
    
    string xOrit = "";
    string yOrit = "";
    string zOrit = "";

    bool isPostMessage = true;
    public bool getIsPostMessage()
    {
        return isPostMessage;
    }
    public void setIsPostMessage(bool isPostMessage1)
    {
        isPostMessage = isPostMessage1; 
    }

    String s_message = null;
    GameObject obj;
    GameObject regC;
    GameObject notSee;
    GameObject pos;
    //ClientSocket mSocket;


    // Use this for initialization

    void Start () 
    {
        obj = GameObject.Find("drill1");
        regC = GameObject.Find("BUTTON/Reg_Complete");
        pos = GameObject.Find("pos");
        x = obj.transform.position.x;
        y = obj.transform.position.y;
        z = obj.transform.position.z;
        Old_x = x;
        Old_y = y;
        Old_z = z;
        // yield return new WaitForSeconds(10);
        //连接服务器
        //mSocket = new ClientSocket();
        //mSocket.ConnectServer("172.31.70.197", 8088);
    }
	
	
// Update is called once per frame
	
    void FixedUpdate () 
    {
        //获取当前坐标
        x = obj.transform.position.x;
        y = obj.transform.position.y;
        z = obj.transform.position.z;
        //计算差值
        delta_x = x - Old_x; 
        delta_y = y - Old_y;
        delta_z = z - Old_z;
        //存储老值
        Old_x = x;
        Old_y = y;
        Old_z = z;


        Vector3 pos_p = new Vector3();
        //计算真实值
        if (delta_x > 0)
            {
            pos_p.x += (float)(7 / 1000);
            xOrit = "x+";
            }
        if (delta_x < 0)
            {
            pos_p.x -= (float)(7 / 1000);
            xOrit = "x-";
            }
        if (delta_y > 0)
            {
            pos_p.y += (float)(3.6 / 1000);
                yOrit = "y+";
            }
         if (delta_y < 0)
            {
            pos_p.y -= (float)(3.6 / 1000);
            yOrit = "y-";
            }
        if (delta_z > 0)
            {
            pos_p.z += (float)(3.6 / 1000);
            zOrit = "z+";
            }
         if (delta_z < 0)
            {
            pos_p.z -= (float)(3.6 / 1000);
            zOrit = "z-";
            }

        Debug.Log("Ordinate:" + pos_p.x + "," + pos_p.y + "," + pos_p.z);
        pos.transform.position += pos_p;

        //发送坐标操作机械手

        if (isPostMessage)
        {
            s_message = xOrit + "," + yOrit + "," + zOrit;
            if (delta_x != 0 || delta_y != 0 || delta_z != 0)
            {
                StartCoroutine(Upload());
            }
        }
        
    }

    IEnumerator Upload()
    {
        WWWForm form = new WWWForm();
        form.AddField("MyForm", s_message);
        

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
    
    public void set_Old(Vector3 trans)
    {
        Old_x = trans.x;
        Old_y = trans.y;
        Old_z = trans.z;
    }
}
