using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class elsGroup : MonoBehaviour,IDragHandler,IPointerDownHandler,IPointerUpHandler
{
    //public Color mColor;
    public string groupName;
    public Sprite mSprite;
    public List<elsItem> mChildItem;

    public Vector2 mScale;

    elsController controller
    {
        get
        {
            return elsController.Instance;
        }
    }

    Transform startParent;
    Vector2 posOffset;
    public bool isSet;
    Vector2 DragOffset =new Vector3(0,200);
    public bool isLock;

    public int CurItemCount
    {
       
        get
        {
            int c = 0;
            for (int i = 0; i < mChildItem.Count; i++)
            {
                if (mChildItem[i].gameObject.activeInHierarchy)
                    c++;
            }
            return c;
        }
    }
    private void Start()
    {

    }
    private void OnEnable()
    {
        InitGroup();
    }
    void InitGroup()
    {
        if(mChildItem.Count==0)
        {
            elsItem[] elsArr = transform.GetComponentsInChildren<elsItem>();
            for (int i = 0; i < elsArr.Length; i++)
            {
                mChildItem.Add(elsArr[i]);
                elsArr[i].mGroup = this;
            }
        }
        startParent = transform.parent;
        isSet = false;
        isLock = false;
        UpdateSprite();
        DropToContentTest();
    }

    public void UpdateSprite()
    {
        int corlorIndex = Random.Range(0, controller.GroupSpriteAry.Length);
        mSprite = controller.GroupSpriteAry[corlorIndex];
        for (int i = 0; i < mChildItem.Count; i++)
        {
            mChildItem[i].InitSprite(mSprite);
            if (!mChildItem[i].gameObject.activeInHierarchy)
                mChildItem[i].gameObject.SetActive(true);
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (isSet|| controller.IsOver||isLock) return;
        AudioController.Instance.PlayAudio(AudioType.PointDown);
        transform.SetParent(controller.mImage_Content.transform);
        posOffset = eventData.position - (Vector2)transform.position;
        transform.position = eventData.position - posOffset + DragOffset;
        transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isSet || controller.IsOver || isLock) return;
        controller.InitBack();
        Vector3 dropPos = transform.position - controller.mImage_Content.transform.position;
        if (dropPos.x + 50 < mScale.x / 2 || dropPos.y + 50 < mScale.y / 2 || dropPos.x - 50 > 1000 - mScale.x / 2 || dropPos.y - 50 > 1000 - mScale.y / 2)
        {
            transform.SetParent(startParent);
            transform.localPosition = Vector3.zero;
            transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            return;
        }
       
        int xp = (int)Mathf.Round((dropPos.x - mScale.x / 2) / 100);
        int yp = (int)Mathf.Round((dropPos.y - mScale.y / 2) / 100);
        int xI;
        int yI;
        xI = (int)dropPos.x / 100 + 1;
        yI = (int)dropPos.y / 100 + 1;
        List<ElsPos> epList = controller.GetEpListByGroupEp(this, new ElsPos(xI,yI));
        for (int i = 0; i < epList.Count; i++)
        {
            if (controller.mItemDic.ContainsKey(epList[i]) && controller.mItemDic[epList[i]] != null)
            {
                transform.SetParent(startParent);
                transform.localPosition = Vector3.zero;
                transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                return;
            }
        }

        isSet = true;
        AudioController.Instance.PlayAudio(AudioType.SetItem);
        transform.position = new Vector3(xp * 100 + mScale.x / 2, yp * 100 + mScale.y / 2, 0) + controller.mImage_Content.transform.position;
        transform.localScale = new Vector3(1, 1, 1);
        if (!controller.mGroupList.Contains(this))
            controller.mGroupList.Add(this);

        
        for (int i = 0; i < mChildItem.Count; i++)
        {
            mChildItem[i].SaveToDic();
        }

        controller.CheckLine(this);
        if (!controller.CheckIdleImageEmpty())
        {
            for (int i = 0; i < controller.IdleContentArray.Length; i++)
            {
                if (controller.IdleContentArray[i].transform.childCount > 0)
                {
                    elsGroup gp = controller.IdleContentArray[i].transform.GetChild(0).GetComponent<elsGroup>();
                    if (gp != null)
                    {
                        gp.DropToContentTest();
                    }
                }
            }
        }
        controller.CheckGameOver();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (isSet || controller.IsOver || isLock) return;
        transform.position = eventData.position- posOffset+ DragOffset;
        Vector3 dragPos = transform.position - controller.mImage_Content.transform.position;
        if (dragPos.x + 50 < mScale.x / 2 || dragPos.y + 50 < mScale.y / 2 || dragPos.x - 50 > 1000 - mScale.x / 2 || dragPos.y - 50 > 1000 - mScale.y / 2)
        {
            controller.InitBack();
            return;
        }
        int xI;
        int yI;
        xI = (int)(dragPos.x) / 100+1;
        yI = (int)(dragPos.y) / 100+1;
        ElsPos ep = new ElsPos(xI, yI);
        controller.ShowDragBackElsPos(this, ep);  
    }

    void SetLockState()
    {
        if (isLock) return;
        for (int i = 0; i < mChildItem.Count; i++)
        {
            if (mChildItem[i] != null)
                mChildItem[i].SetLockState();
        }
        isLock = true;
    }
    void SetUnlockState()
    {
        if (!isLock) return;
        for (int i = 0; i < mChildItem.Count; i++)
        {
            if (mChildItem[i] != null)
                mChildItem[i].SetUnlockState();
        }
        isLock = false;
    }
    /// <summary>
    /// 检测面板是否有空间容纳该组
    /// </summary>
    /// <returns></returns>
    public bool DropToContentTest()
    {
        for (int x = 1; x < 11; x++)
        {
            for (int y = 1; y < 11; y++)
            {
                ElsPos ep = new ElsPos(x, y);
                if (controller.mItemDic[ep] == null)
                {
                    if (DropTestOnElsPos(ep))
                    {
                        SetUnlockState();
                        return true;
                    }      
                }
            }
        }
        SetLockState();
        return false;
    }
    bool DropTestOnElsPos(ElsPos elsPos)
    {
        List<ElsPos> epList = controller.GetTestEpListByGroupEp(this, elsPos);
        if (epList == null) return false;
        for (int i = 0; i < epList.Count; i++)
        {
            if (controller.mItemDic[epList[i]] != null)
                return false;
        }
        return true;
    }
}
