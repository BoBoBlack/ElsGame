using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    public GameObject Image_Login;
    public InputField input_login_username;
    public InputField input_login_password;
    public Text text_login_warn;

    public GameObject Image_Register;
    public InputField input_register_username;
    public InputField input_register_password1;
    public InputField input_register_password2;
    public Text text_register_warn;

    public GameObject Canvas_Main;

    
    string username;
    string password;

    string wwwurl = "http://39.106.160.158:90/elsGame/elslogin.php";
    // Start is called before the first frame update
    void Start()
    {
        string[] str = DataCommon.Instance.GetLocalUserInfo();
        if (str != null)
        {
            input_login_username.text = str[0];
            input_login_password.text = str[1];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ClickLogin()
    {
        username = input_login_username.text;
        password = input_login_password.text;

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        form.AddField("action", "login");

        StartCoroutine(SendLoginRrgistPost(wwwurl, form, "login"));
    }
    
    public void ClickRegister()
    {
        string rUserName = input_register_username.text;
        string rPassword1 = input_register_password1.text;
        string rPassword2 = input_register_password2.text;
        if(string.IsNullOrEmpty(rUserName)|| string.IsNullOrEmpty(rPassword1)|| string.IsNullOrEmpty(rPassword2))
        {
            SetRegisterWarn("用户名和密码不能为空！", Color.red);
        }
        else if(rPassword1!= rPassword2)
        {
            SetRegisterWarn("两次输入的密码不一致！", Color.red);
        }
        else
        {
            WWWForm form = new WWWForm();
            form.AddField("username", rUserName);
            form.AddField("password", rPassword1);
            form.AddField("action", "regist");

            StartCoroutine(SendLoginRrgistPost(wwwurl, form,"regist"));
        }
    }
    IEnumerator SendLoginRrgistPost(string url, WWWForm wForm,string action)
    {
        UnityWebRequest webRequest = UnityWebRequest.Post(url, wForm);
        yield return webRequest.SendWebRequest();
        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            switch(action)
            {
                case "login":
                    SetLoginWarn("网络连接失败!", Color.red);
                    break;
                case "regist":
                    SetRegisterWarn("网络连接失败！", Color.red);
                    break;
            } 
        }
        else
        {
            switch(action)
            {
                case "login":
                    if (webRequest.downloadHandler.text == "success")
                    {
                        SetLoginWarn("登录成功!", Color.green);
                        DataCommon.Instance.SaveLocalUserInfo(username, password);
                        Canvas_Main.gameObject.SetActive(true);
                        gameObject.SetActive(false);
                    }
                    else if (webRequest.downloadHandler.text == "error")
                    {
                        SetLoginWarn("用户名或密码错误，请重新输入!", Color.red);
                    }
                    break;
                case "regist":
                    if (webRequest.downloadHandler.text == "exist")
                    {
                        SetRegisterWarn("用户名已存在！", Color.red);
                    }
                    else if (webRequest.downloadHandler.text == "success")
                    {
                        SetRegisterWarn("注册成功，请返回登录！", Color.green);
                    }
                    break;
            }
            
        }
    }
    public void ClickToRegister()
    {
        Image_Login.SetActive(false);
        Image_Register.SetActive(true);
        SetLoginWarn("", Color.red);
        SetRegisterWarn("", Color.red);
    }
    public void ClickReturnLogin()
    {
        Image_Login.SetActive(true);
        Image_Register.SetActive(false);
        SetLoginWarn("", Color.red);
        SetRegisterWarn("", Color.red);
    }
    public void ClickExitGame()
    {
        Application.Quit();
    }
    void SetLoginWarn(string warn,Color color)
    {
        text_login_warn.text = warn;
        text_login_warn.color = color;
    }
    void SetRegisterWarn(string warn, Color color)
    {
        text_register_warn.text = warn;
        text_register_warn.color = color;
    }

}
