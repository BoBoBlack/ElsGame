using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct ElsPos
{
    public int x;
    public int y;
    public ElsPos(int mx,int my)
    {
        x = mx;
        y = my;    
    }
}
public class elsController : MonoBehaviour
{
    public static elsController Instance;
    private void Awake()
    {
        Instance = this;
    }

    public Image mImage_Content;
    public Image mBack;

    public List<elsGroup> mGroupList = new List<elsGroup>();
    public Dictionary<ElsPos, elsItem> mItemDic = new Dictionary<ElsPos, elsItem>();

    public Dictionary<ElsPos, Image> mBackImageDic = new Dictionary<ElsPos, Image>();
    public elsGroup[] AllGroupType;
    public Image[] IdleContentArray;
    public Text Text_CurScore;
    Sequence CurScoreSequence;
    public Text Text_SaveScore;
    Sequence SaveScoreSequence;

    public Image Image_Over;

    public int curScore;
    public bool IsOver;

    //public Color[] GroupColorAry;
    public Sprite[] GroupSpriteAry;

    public int GroupUpdateCount;
    public int GroupColorUpdateCount;
    public Button[] GroupUpdateButtonAry;
    public Button[] GroupColorUpdateButtonAry;
    public Sprite baoshi_UpdateGroup;
    public Sprite baoshi_UpdateSprite;

    public GameObject[] BurstAnimAry;
    public GameObject RayAnimObj;

    public Text text_ScoreAdd;

    public Image Image_RankList;
    public RectTransform Rect_rankConten;
    public RankItem rankItemBase;
    public Text text_myRank;
    // Start is called before the first frame update
    void Start()
    {
        Text_CurScore.text = "0";
        //Text_SaveScore.text = DataCommon.Instance.GetLocalScoreData().ToString();
        DataCommon.Instance.GetMyNetScore();
        CurScoreSequence = DOTween.Sequence();
        CurScoreSequence.SetAutoKill(false);
        SaveScoreSequence = DOTween.Sequence();
        SaveScoreSequence.SetAutoKill(false);

        Image_Over.gameObject.SetActive(false);
        for (int x = 1; x < 11; x++)
        {
            for (int y = 1; y < 11; y++)
            {
                if(!mItemDic.ContainsKey(new ElsPos(x,y)))
                {
                    mItemDic.Add(new ElsPos(x, y), null);
                }
            }
        }
        Image[] backAry = mBack.GetComponentsInChildren<Image>();
        for (int i = 0; i < backAry.Length-1; i++)
        {
            int yI = 10 - i / 10;
            int xI = i % 10 + 1;
            mBackImageDic.Add(new ElsPos(xI, yI), backAry[i+1]);
        }
        CreatNewIdleItem();
    }
    private void Update()
    {
        
    }
    public void CheckLine(elsGroup curGroup)
    {
        int score = 0;
        List<int> delX_list = new List<int>();
        List<int> delY_list = new List<int>();
        for (int x = 1; x < 11; x++)
        {
            bool isFull = true;
            for (int y = 1; y < 11; y++)
            {
                if (mItemDic[new ElsPos(x, y)] == null)
                {
                    isFull = false;
                    break;
                } 
            }
            if(isFull)
            {
                delX_list.Add(x);
            }
        }
        for (int y = 1; y < 11; y++)
        {

            bool isFull = true;
            for (int x = 1; x < 11; x++)
            {
                if (mItemDic[new ElsPos(x, y)] == null)
                {
                    isFull = false;
                    break;
                }
            }
            if (isFull)
            {
                delY_list.Add(y);
            }
        }
        if(delX_list.Count>0)
        {
            List<Sprite> xLineColor = new List<Sprite>();
            for (int i = 0; i < delX_list.Count; i++)
            {
                GameObject obj = Instantiate(RayAnimObj, transform);
                obj.transform.position = mImage_Content.transform.position + new Vector3(delX_list[i] * 100 - 50, 500, 0);
                obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                obj.GetComponent<DOTweenAnimation>().DOPlay();

                for (int y = 1; y < 11; y++)
                {
                    if (mItemDic[new ElsPos(delX_list[i], y)].mProps== ElsProps.None&&!xLineColor.Contains(mItemDic[new ElsPos(delX_list[i], y)].mSprite))
                        xLineColor.Add(mItemDic[new ElsPos(delX_list[i], y)].mSprite);
                    mItemDic[new ElsPos(delX_list[i], y)].HideFromGroup();
                    mItemDic[new ElsPos(delX_list[i], y)] = null;
                }
                score += 10 - xLineColor.Count < 0 ? 0 : 10 - xLineColor.Count;
                //同时消除多行奖励分
                score += delX_list.Count * 5;
                xLineColor.Clear();
            }
        }
        if (delY_list.Count > 0)
        {
            List<Sprite> yLineColor = new List<Sprite>();
            for (int i = 0; i < delY_list.Count; i++)
            {
                GameObject obj = Instantiate(RayAnimObj, transform);
                obj.transform.position = mImage_Content.transform.position + new Vector3(500, delY_list[i] * 100 - 50, 0);
                obj.GetComponent<DOTweenAnimation>().DOPlay();

                for (int x = 1; x < 11; x++)
                {
                    if (mItemDic[new ElsPos(x, delY_list[i])] != null)
                    {
                        if (mItemDic[new ElsPos(x, delY_list[i])].mProps == ElsProps.None && !yLineColor.Contains(mItemDic[new ElsPos(x, delY_list[i])].mSprite))
                            yLineColor.Add(mItemDic[new ElsPos(x, delY_list[i])].mSprite);
                        mItemDic[new ElsPos(x, delY_list[i])].HideFromGroup();
                        mItemDic[new ElsPos(x, delY_list[i])] = null;
                    }
                }
                score += 10 - yLineColor.Count < 0 ? 0 : 10 - yLineColor.Count;
                //同时消除多行奖励分
                score += delY_list.Count * 5;
                yLineColor.Clear();
            }
        }
        if (delX_list.Count > 0 || delY_list.Count > 0) AudioController.Instance.PlayAudio(AudioType.DeletLine);
        if(score>0)
        {
            PlayScoreText(score,curGroup.transform.position);
            curScore += score;
            CurScoreSequence.Append(DOTween.To(delegate (float value) {
                int show = Mathf.FloorToInt(value);
                Text_CurScore.text = show.ToString();
            },int.Parse(Text_CurScore.text), curScore,1f));
            if (curScore> DataCommon.Instance.showMaxScore)
            {
                DataCommon.Instance.showMaxScore = curScore;
                SaveScoreSequence.Append(DOTween.To(delegate (float value) {
                    int show = Mathf.FloorToInt(value);
                    Text_SaveScore.text = show.ToString();
                }, int.Parse(Text_SaveScore.text), curScore, 1f));
            }
            if (score >= 20)
                AudioController.Instance.PlayAudio(AudioType.Nice);
        }
        CheckEmptyGroup();
    }
    public void CheckEmptyGroup()
    {
        for (int i = 0; i < mGroupList.Count; i++)
        {
            if (mGroupList[i].CurItemCount <= 0)
            {
                elsGroup gr = mGroupList[i];
                mGroupList.RemoveAt(i);
                gr.gameObject.SetActive(false);
                GroupPoolController.SaveGroup(gr);
            }
        }
    }
    public void PlayScoreText(int scoreAdd,Vector3 pos)
    {
        text_ScoreAdd.text = "+" + scoreAdd.ToString();
        text_ScoreAdd.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        text_ScoreAdd.transform.position = pos;
        text_ScoreAdd.gameObject.SetActive(true);
        text_ScoreAdd.transform.DOMoveY(text_ScoreAdd.transform.position.y + 200, 1);
        text_ScoreAdd.transform.DOScale(new Vector3(1, 1, 1), 1).onComplete = delegate
        {
            text_ScoreAdd.gameObject.SetActive(false); 
        };
    }
    /// <summary>
    /// 检查当前存在的三个组是否都放置完毕
    /// </summary>
    public bool CheckIdleImageEmpty()
    {
        for (int i = 0; i < IdleContentArray.Length; i++)
        {
            if (IdleContentArray[i].transform.childCount > 0) return false;
        }
        CreatNewIdleItem();
        return true;
    }
    /// <summary>
    /// 创建三个新的组
    /// </summary>
    public void CreatNewIdleItem()
    {
        int r1 = Random.Range(0, AllGroupType.Length);
        int r2 = Random.Range(0, AllGroupType.Length);
        int r3 = Random.Range(0, AllGroupType.Length);
        elsGroup group1 = GroupPoolController.GetGroup(AllGroupType[r1].groupName);
        elsGroup group2 = GroupPoolController.GetGroup(AllGroupType[r2].groupName);
        elsGroup group3 = GroupPoolController.GetGroup(AllGroupType[r3].groupName);

        if (group1 == null)
            group1 = Instantiate(AllGroupType[r1].gameObject, IdleContentArray[0].transform).GetComponent<elsGroup>();
        else
            group1.transform.SetParent(IdleContentArray[0].transform);
        group1.transform.localPosition = Vector3.zero;
        group1.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

        if (group2 == null)
            group2 = Instantiate(AllGroupType[r2].gameObject, IdleContentArray[1].transform).GetComponent<elsGroup>();
        else
            group2.transform.SetParent(IdleContentArray[1].transform);
        group2.transform.localPosition = Vector3.zero;
        group2.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

        if (group3 == null)
            group3 = Instantiate(AllGroupType[r3].gameObject, IdleContentArray[2].transform).GetComponent<elsGroup>();
        else
            group3.transform.SetParent(IdleContentArray[2].transform);
        group3.transform.localPosition = Vector3.zero;
        group3.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

        //Debug.Log(group1.groupName + "||" + group2.groupName + "||" + group3.groupName);

        group1.gameObject.SetActive(true);
        group2.gameObject.SetActive(true);
        group3.gameObject.SetActive(true);
    }
    /// <summary>
    /// 检测是否结束
    /// </summary>
    public void CheckGameOver()
    {
        bool over = true;

        for (int i = 0; i < IdleContentArray.Length; i++)
        {
            if (IdleContentArray[i].transform.childCount > 0)
            {
                //Debug.Log("check");
                elsGroup gp = IdleContentArray[i].transform.GetChild(0).GetComponent<elsGroup>();
                if (gp != null&& !gp.isLock)
                {
                    over = false;
                }
            }
        }

        if (over && GroupUpdateCount <= 0)
        {
            //GAME OVER
            IsOver = true;
            Image_Over.gameObject.SetActive(true);
            DataCommon.Instance.SaveMyNetScore();
            //DataCommon.Instance.SaveLocalScoreData(curScore);
        }

       
    }

    /// <summary>
    /// 更新背景显示
    /// </summary>
    /// <param name="group"></param>
    /// <param name="ep"></param>
    public void ShowDragBackElsPos(elsGroup group,ElsPos ep)
    {
        List<ElsPos> epList = GetEpListByGroupEp(group, ep);
        for (int i = 0; i < epList.Count; i++)
        {
            if (mItemDic.ContainsKey(epList[i]) && mItemDic[epList[i]] != null)
            {
                InitBack();
                return;
            }
        }
        for (int x = 1; x < 11; x++)
        {
            for (int y = 1; y < 11; y++)
            {
                ElsPos mep = new ElsPos(x, y);
                 
                if (mBackImageDic.ContainsKey(mep))
                {
                    if (epList != null && epList.Contains(mep))
                    {
                        mBackImageDic[mep].color = new Color(80f / 250, 80f / 250, 80f / 250, 180f / 250);
                    }
                    else
                    {
                        mBackImageDic[mep].color = new Color(0, 0, 0, 180f / 250);
                    }
                }
            }
        }
    }
    /// <summary>
    /// 根据当前组的ep位置获取字典中对应为每个子元素的ep位置
    /// </summary>
    /// <param name="group"></param>
    /// <param name="ep"></param>
    /// <returns></returns>
    public List<ElsPos> GetEpListByGroupEp(elsGroup group, ElsPos ep)
    {
        List<ElsPos> epList = new List<ElsPos>();
        if (ep.x * 100 < (int)group.mScale.x / 2 || ep.y * 100 < (int)group.mScale.y / 2 || 1000 - (ep.x - 1) * 100 < (int)group.mScale.x / 2 || 1000 - (ep.y - 1) * 100 < (int)group.mScale.y / 2) return null;
        for (int i = 0; i < group.mChildItem.Count; i++)
        {
            int xI = (int)group.mScale.x / 100;
            int yI = (int)group.mScale.y / 100;
            if (xI % 2 != 0)
            {
                xI = ep.x + (int)(group.mChildItem[i].transform.localPosition.x) / 100;
            }
            else
            {
                if(group.transform.position.x<mBackImageDic[ep].transform.position.x)
                {
                    xI = ep.x + (int)(group.mChildItem[i].transform.localPosition.x - 50) / 100;
                }
                else
                {
                    xI = ep.x + (int)(group.mChildItem[i].transform.localPosition.x + 50) / 100;
                }
                
            }
            if (yI % 2 != 0)
            {
                yI = ep.y + (int)(group.mChildItem[i].transform.localPosition.y) / 100;
            }
            else
            {
                if (group.transform.position.y < mBackImageDic[ep].transform.position.y)
                {
                    yI = ep.y + (int)(group.mChildItem[i].transform.localPosition.y - 50) / 100;
                }
                else
                {
                    yI = ep.y + (int)(group.mChildItem[i].transform.localPosition.y + 50) / 100;
                }           
            }
            ElsPos mep = new ElsPos(xI, yI);
            if (!epList.Contains(mep) && mep.x > 0 && mep.y > 0 && mep.x < 11 && mep.y < 11)
                epList.Add(mep);
        }
        return epList;
    }
    /// <summary>
    /// 获取位置预测时的子元素的预测ep位置
    /// </summary>
    /// <param name="group"></param>
    /// <param name="ep"></param>
    /// <returns></returns>
    public List<ElsPos> GetTestEpListByGroupEp(elsGroup group, ElsPos ep)
    {
        if (ep.x * 100 < (int)group.mScale.x / 2 || ep.y * 100 < (int)group.mScale.y / 2 || 1000 - (ep.x - 1) * 100 < (int)group.mScale.x / 2 || 1000 - (ep.y - 1) * 100 < (int)group.mScale.y / 2) return null;
        
        List<ElsPos> epList = new List<ElsPos>();
        for (int i = 0; i < group.mChildItem.Count; i++)
        {
            int xI = (int)group.mScale.x / 100;
            int yI = (int)group.mScale.y / 100;
            if (xI % 2 != 0)
            {
                xI = ep.x + (int)group.mChildItem[i].transform.localPosition.x / 100;
            }
            else
            {
                int posX = ep.x * 100;
                int itemX = (posX + (int)group.mChildItem[i].transform.localPosition.x + 50) / 100;
                xI = itemX;
            }
            if (yI % 2 != 0)
            {
                yI = ep.y + (int)group.mChildItem[i].transform.localPosition.y / 100;
            }
            else
            {
                int posY = ep.y * 100;
                int itemY = (posY + (int)group.mChildItem[i].transform.localPosition.y + 50) / 100;
                yI = itemY;
            }
            ElsPos mep = new ElsPos(xI, yI);
            if (!epList.Contains(mep) && mep.x > 0 && mep.y > 0 && mep.x < 11 && mep.y < 11)
                epList.Add(mep);
            else return null;
        }
        return epList;
    } 
    public void InitBack()
    {
        foreach (var item in mBackImageDic.Keys)
        {
            mBackImageDic[item].color = new Color(0, 0, 0, 180f / 250);
        }
    }

    public void UpdateIdleGroup(int idleIndex)
    {
        if (GroupUpdateCount <= 0|| IdleContentArray[idleIndex].transform.childCount <= 0) return;

        GroupPoolController.SaveGroup(IdleContentArray[idleIndex].transform.GetChild(0).gameObject.GetComponent<elsGroup>());
        int r = Random.Range(0, AllGroupType.Length);
        elsGroup group=GroupPoolController.GetGroup(AllGroupType[r].groupName);
        if (group == null)
            group = Instantiate(AllGroupType[r].gameObject, IdleContentArray[idleIndex].transform).GetComponent<elsGroup>();
        else
            group.transform.SetParent(IdleContentArray[idleIndex].transform);
        group.transform.localPosition = Vector3.zero;
        group.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        GroupUpdateCount--;
        UpdatePropsCountShow();
        group.gameObject.SetActive(true);
        Invoke("CheckGameOver", 0.2f);
    }
    public void UpdateGroupColor(int idleIndex)
    {
        if (GroupColorUpdateCount <= 0 || IdleContentArray[idleIndex].transform.childCount <= 0) return;
        elsGroup group= IdleContentArray[idleIndex].transform.GetChild(0).GetComponent<elsGroup>();
        if (group.isLock) return;
        if (group != null)
            group.UpdateSprite();
        GroupColorUpdateCount--;
        UpdatePropsCountShow();
    }
    public void UpdatePropsCountShow()
    {
        for (int i = 0; i < GroupUpdateButtonAry.Length; i++)
        {
            GroupUpdateButtonAry[i].GetComponentInChildren<Text>().text = GroupUpdateCount.ToString();
        }
        for (int i = 0; i < GroupColorUpdateButtonAry.Length; i++)
        {
            GroupColorUpdateButtonAry[i].GetComponentInChildren<Text>().text = GroupColorUpdateCount.ToString();
        }
    }
    public void OpenCloseAudio()
    {
        AudioController.Instance.ChangeAudioState();
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    private void OnApplicationQuit()
    {
        Application.wantsToQuit += delegate
        {
            if (curScore > DataCommon.Instance.showMaxScore)
                DataCommon.Instance.SaveMyNetScore();
            return true;
        };
    }
    public void ClickRankList()
    {
        if (Image_RankList.gameObject.activeInHierarchy) return;
        DataCommon.Instance.SaveMyNetScore();
        DataCommon.Instance.GetRankList();
        //List<UserScoreData> scoreList = DataCommon.Instance.GetRankList();
       
    }
    public void ShowRankList(List<UserScoreData> scoreList)
    {
        RankItem[] rankItemAry = Rect_rankConten.GetComponentsInChildren<RankItem>();
        Image_RankList.gameObject.SetActive(true);
        
        text_myRank.text = "我的排名：" + DataCommon.Instance.myRank.ToString();
        //删除多余的item（不主动删除数据库的情况下，基本上不触发）
        if (rankItemAry.Length > scoreList.Count)
        {
            for (int i = scoreList.Count; i < rankItemAry.Length; i++)
            {
                Destroy(rankItemAry[i].gameObject);
            }
        }
        Rect_rankConten.sizeDelta = new Vector2(0, scoreList.Count * 110 + 10);
        for (int i = 0; i < scoreList.Count; i++)
        {
            if (i < rankItemAry.Length)
            {
                rankItemAry[i].InitItem(scoreList[i].rank, scoreList[i].userName, scoreList[i].score);
            }
            else
            {
                RankItem item = Instantiate(rankItemBase, Rect_rankConten.transform).GetComponent<RankItem>();
                item.InitItem(scoreList[i].rank, scoreList[i].userName, scoreList[i].score);
            }
        }
    }
    /// <summary>
    /// 重新开始
    /// </summary>
    public void PlayAgain()
    {
        for (int i = 0; i < mGroupList.Count; i++)
        {
            mGroupList[i].gameObject.SetActive(false);
            GroupPoolController.SaveGroup(mGroupList[i]);
        }
        mGroupList.Clear();

        for (int x = 1; x < 11; x++)
        {
            for (int y = 1; y < 11; y++)
            {
                if (mItemDic.ContainsKey(new ElsPos(x, y)))
                {
                    mItemDic[new ElsPos(x, y)] = null;
                }
            }
        }
        
        for (int i = 0; i < IdleContentArray.Length; i++)
        {
            if(IdleContentArray[i].transform.childCount>0)
            {
                elsGroup group = IdleContentArray[i].transform.GetChild(0).GetComponent<elsGroup>();
                if(group!=null)
                {
                    group.gameObject.SetActive(false);
                    GroupPoolController.SaveGroup(group);
                }
                else
                {
                    Destroy(IdleContentArray[i].transform.GetChild(0).gameObject);
                }
            }
        }

        Image_Over.gameObject.SetActive(false);
        //DataCommon.Instance.SaveLocalScoreData(curScore);
        DataCommon.Instance.SaveMyNetScore();
        curScore = 0;
        GroupUpdateCount = 0;
        GroupColorUpdateCount = 0;
        IsOver = false;
        Text_CurScore.text = curScore.ToString();
        UpdatePropsCountShow();
        CreatNewIdleItem();
    }
}
