using LitJson;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DataCommon : MonoBehaviour
{
    public static DataCommon Instance;
    private void Awake()
    {
        Instance = this;
    }
    public string username;
    public int showMaxScore;
    public int netMaxScore;
    public int myRank;
    
    string wurl = "http://39.106.160.158:90/elsGame/elsscore.php";

    List<UserScoreData> showRankList = new List<UserScoreData>();
    private void Start()
    {
        
    }

    #region 本地数据
    //public void SaveLocalScoreData(int score)
    //{
    //    int saveScore= PlayerPrefs.GetInt("score", 0);
    //    if (saveScore < score)
    //        PlayerPrefs.SetInt("score", score);
    //    PlayerPrefs.Save();
    //}
    //public int GetLocalScoreData()
    //{
    //    return PlayerPrefs.GetInt("score", 0);
    //}
    public void SaveLocalUserInfo(string userName,string password)
    {
        username = userName;
        PlayerPrefs.SetString("username", userName);
        PlayerPrefs.SetString("password", password);
    }
    public string[] GetLocalUserInfo()
    {
        string[] str = new string[] { PlayerPrefs.GetString("username", ""), PlayerPrefs.GetString("password", "") };
        if (string.IsNullOrEmpty(str[0]) || string.IsNullOrEmpty(str[1])) return null;
        return str;
    }
    #endregion

    #region 服务器数据
    public void GetMyNetScore()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("score", 0);
        form.AddField("action", "GetUserScore");

        StartCoroutine(SendScorePost(wurl, form, "GetUserScore"));
    }
    IEnumerator SendScorePost(string url, WWWForm wForm,string action)
    {
        UnityWebRequest webRequest = UnityWebRequest.Post(url, wForm);
        yield return webRequest.SendWebRequest();
        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            switch(action)
            {
                case "GetUserScore":
                    JsonData json = JsonMapper.ToObject(webRequest.downloadHandler.text);
                    showMaxScore = netMaxScore = int.Parse(json[0]["score"].ToString());
                    elsController.Instance.Text_SaveScore.text = showMaxScore.ToString();
                    break;
                case "SaveUserScore":
                    netMaxScore = showMaxScore;
                    break;
                case "GetRankList":
                    showRankList.Clear();
                    JsonData jsonRank = JsonMapper.ToObject(webRequest.downloadHandler.text);
                    myRank= int.Parse(jsonRank["myrank"].ToString());
                    for (int i = 0; i < jsonRank["ranks"].Count; i++)
                    {
                        UserScoreData scoredata = new UserScoreData(i + 1, jsonRank["ranks"][i]["username"].ToString(), int.Parse(jsonRank["ranks"][i]["score"].ToString()));
                        showRankList.Add(scoredata);
                    }
                    elsController.Instance.ShowRankList(showRankList);
                    break;
            }
        }
    }
    public void SaveMyNetScore()
    {
        if (showMaxScore <= netMaxScore) return;
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("score", showMaxScore);
        form.AddField("action", "SaveUserScore");

        StartCoroutine(SendScorePost(wurl, form, "SaveUserScore"));      
    }
    public void GetRankList()
    {

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("score", 0);
        form.AddField("action", "GetRankList");

        StartCoroutine(SendScorePost(wurl, form, "GetRankList"));

        //MySqlConnection conn = new MySqlConnection(dataUrl);
        //conn.Open();
        ////先要查询一下要获取的账号是否在目前数据库中。
        //MySqlCommand myCommand = new MySqlCommand("select*from score", conn);
        //MySqlDataReader reader = myCommand.ExecuteReader();
        //List<UserScoreData> scoreList = new List<UserScoreData>();
         
        //while (reader.Read())
        //{
        //    UserScoreData scoreData = new UserScoreData(0, reader.GetString("username"), reader.GetInt32("score"));
        //    scoreList.Add(scoreData);
        //}
        //reader.Close();
        //conn.Close();

        //QuickSort(scoreList, 0, scoreList.Count - 1);

        //showRankList.Clear();

        //for (int i = 0; i < scoreList.Count; i++)
        //{
        //    scoreList[i].rank = i + 1;
        //    if(scoreList[i].userName==username)
        //        myRank= i + 1;
        //    if(i<100)
        //    {
        //        showRankList.Add(scoreList[i]);
        //    }
        //}
    }
    #endregion
    
    void QuickSort(List<UserScoreData> scoreList,int left,int right)
    {
        if(left<right)
        {
            int middle = scoreList[(left + right) / 2].score;
            int i = left - 1;
            int j = right + 1;
            while(true)
            {
                while (scoreList[++i].score > middle) ;
                while (scoreList[--j].score < middle) ;
                if (i >= j)
                    break;
                Swap(scoreList, i, j);
            }
            QuickSort(scoreList, left, i - 1);
            QuickSort(scoreList, j + 1, right);
        }
    }
    void Swap(List<UserScoreData> scoreList,int i,int j)
    {
        UserScoreData scoreData = scoreList[i];
        scoreList[i] = scoreList[j];
        scoreList[j] = scoreData;
    }
}
public class UserScoreData
{
    public int rank;
    public string userName;
    public int score;

    public UserScoreData(int mrank, string muserName,int mscore)
    {
        rank = mrank;
        userName = muserName;
        score = mscore;
    }
}
