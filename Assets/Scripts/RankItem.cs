using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankItem : MonoBehaviour
{
    public Text text_top;
    public Text text_name;
    public Text text_score;

    public void InitItem(int top,string userName,int score)
    {
        text_top.text = top.ToString();
        text_name.text = userName.ToString();
        text_score.text = score.ToString();
    }
    
}
