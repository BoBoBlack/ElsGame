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
    
    public void SaveScoreData(int score)
    {
        int saveScore= PlayerPrefs.GetInt("score", 0);
        if (saveScore < score)
            PlayerPrefs.SetInt("score", score);
        PlayerPrefs.Save();
    }
    public int GetScoreData()
    {
        return PlayerPrefs.GetInt("score", 0);
    }
}
