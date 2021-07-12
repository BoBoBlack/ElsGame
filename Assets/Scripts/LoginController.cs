using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    string dataUrl = "datasource=39.106.160.158;port=3306;database=elsgamedb;user=root;pwd=Yrx246;charset=utf8";
    string username;
    string password;
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
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            SetLoginWarn("用户名和密码不能为空！",Color.red);
        }
        else
        {
            text_login_warn.text = null;
            Dictionary<string, string> myDic = new Dictionary<string, string>();
            myDic.Clear();
            MySqlConnection conn = new MySqlConnection(dataUrl);
            //"数据库连接成功";
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("select * from users", conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string _usernames = reader.GetString("username");
                string _password = reader.GetString("password");
                myDic.Add(_usernames, _password);
            }
            if (myDic.ContainsKey(username))
            {
                string vale;
                if (myDic.TryGetValue(username, out vale))
                {
                    if (vale == password)
                    {
                        conn.Close();
                        SetLoginWarn("登录成功!", Color.green);
                        DataCommon.Instance.SaveLocalUserInfo(username, password);
                        Canvas_Main.gameObject.SetActive(true);
                        gameObject.SetActive(false);
                    }
                    else
                    {       
                        SetLoginWarn("密码错误，请重新输入!", Color.red);
                    }
                }
            }
            else
            { 
                SetLoginWarn("账号不存在!", Color.red);
            }
        }
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
            MySqlConnection conn = new MySqlConnection(dataUrl);
            conn.Open();
            //先要查询一下要注册的账号是否在目前数据库中。
            MySqlCommand myCommand = new MySqlCommand("select*from users", conn);
            MySqlDataReader reader = myCommand.ExecuteReader();
            List<string> user = new List<string>();
            while (reader.Read())
            {
                string username = reader.GetString("username");
                string password = reader.GetString("password");
                user.Add(username);
            }
            if (user.Contains(rUserName))
            {
                SetRegisterWarn("用户名已存在！", Color.red);
            }
            else
            {
                reader.Close();//先将查询的功能关闭
                MySqlCommand cmd = new MySqlCommand("insert into users set username ='" + rUserName + "'" + ",password='" + rPassword1 + "'", conn);
                cmd.Parameters.AddWithValue("username", rUserName);
                cmd.Parameters.AddWithValue("password", rPassword1);
                cmd.ExecuteNonQuery();
                conn.Close();
                SetRegisterWarn("注册成功，请返回登录！", Color.green);
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
