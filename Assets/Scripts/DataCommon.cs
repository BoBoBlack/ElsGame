using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    string dataUrl = "datasource=39.106.160.158;port=3306;database=elsgamedb;user=root;pwd=Yrx246;charset=utf8";

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
    public int GetMyNetScore()
    {
        Dictionary<string, string> myDic = new Dictionary<string, string>();
        myDic.Clear();
        MySqlConnection conn = new MySqlConnection(dataUrl);
        //"数据库连接成功";
        conn.Open();
        MySqlCommand cmd = new MySqlCommand("select * from score", conn);
        MySqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            string _usernames = reader.GetString("username");
            string _score = reader.GetString("score");
            myDic.Add(_usernames, _score);
        }
        if (myDic.ContainsKey(username))
        {
            string vale;
            if (myDic.TryGetValue(username, out vale))
            {
                reader.Close();
                conn.Close();
                return int.Parse(vale);
            }
            return 0;
        }
        else
        {
            return 0;
        }
       
    }
    public void SaveMyNetScore()
    {
        if (showMaxScore <= netMaxScore) return;
        MySqlConnection conn = new MySqlConnection(dataUrl);
        conn.Open();
        //先要查询一下要获取的账号是否在目前数据库中。
        MySqlCommand myCommand = new MySqlCommand("select*from score", conn);
        MySqlDataReader reader = myCommand.ExecuteReader();
        Dictionary<string, int> scoreDic = new Dictionary<string, int>();
        while (reader.Read())
        {
            scoreDic.Add(reader.GetString("username"), reader.GetInt32("score"));
        }
        if (scoreDic.ContainsKey(username))
        {
            reader.Close();//先将查询的功能关闭
            MySqlCommand cmd = new MySqlCommand("update score set score='" + showMaxScore + "'"+ "where username = '" + username + "'", conn);
            cmd.ExecuteNonQuery();
            conn.Close();
            netMaxScore = showMaxScore;
        }
        else
        {
            reader.Close();//先将查询的功能关闭
            MySqlCommand cmd = new MySqlCommand("insert into score set username ='" + username + "'" + ",score='" + showMaxScore + "'", conn);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Parameters.AddWithValue("score", showMaxScore);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
    public List<UserScoreData> GetRankList()
    {
        MySqlConnection conn = new MySqlConnection(dataUrl);
        conn.Open();
        //先要查询一下要获取的账号是否在目前数据库中。
        MySqlCommand myCommand = new MySqlCommand("select*from score", conn);
        MySqlDataReader reader = myCommand.ExecuteReader();
        List<UserScoreData> scoreList = new List<UserScoreData>();
         
        while (reader.Read())
        {
            UserScoreData scoreData = new UserScoreData(0, reader.GetString("username"), reader.GetInt32("score"));
            scoreList.Add(scoreData);
        }
        reader.Close();
        conn.Close();

        QuickSort(scoreList, 0, scoreList.Count - 1);

        List<UserScoreData> showRankList = new List<UserScoreData>();

        for (int i = 0; i < scoreList.Count; i++)
        {
            scoreList[i].rank = i + 1;
            if(scoreList[i].userName==username)
                myRank= i + 1;
            if(i<100)
            {
                showRankList.Add(scoreList[i]);
            }
        }

        return showRankList;
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
