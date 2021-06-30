using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ElsProps
{
    None,
    GroupUpdate,
    GroupSpriteUpdate
}
public class elsItem : MonoBehaviour
{
    public int mX;
    public int mY;
    public elsGroup mGroup;
    //public Color mColor;
    public Sprite mSprite;
    public ElsProps mProps = ElsProps.None;
    bool isSet;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnEnable()
    {
        isSet = false;
        int r = Random.Range(0, 101);
        if (r == 0)
        {
            mProps = ElsProps.GroupUpdate;
            mSprite = elsController.Instance.baoshi_UpdateGroup;
        }
        else if (r == 1)
        {
            mProps = ElsProps.GroupSpriteUpdate;
            mSprite = elsController.Instance.baoshi_UpdateSprite;
        }
        GetComponent<Image>().sprite = mSprite;
        GetComponent<Image>().color = Color.white;
    }
    private void OnDisable()
    {
        if (elsController.Instance.IsOver) return;
        if (isSet)
        {
            GameObject animObj = null;
            switch (GetSpriteIndex())
            {
                case 0:
                case 1:
                    animObj = Instantiate(elsController.Instance.BurstAnimAry[2], elsController.Instance.transform);
                    break;
                case 2:
                case 3:
                    animObj = Instantiate(elsController.Instance.BurstAnimAry[1], elsController.Instance.transform);
                    break;
                case 4:
                case 5:
                    animObj = Instantiate(elsController.Instance.BurstAnimAry[0], elsController.Instance.transform);
                    break;
                case 6:
                case 7:
                    animObj = Instantiate(elsController.Instance.BurstAnimAry[3], elsController.Instance.transform);
                    break;
            }
            if (animObj != null)
            {
                animObj.transform.position = transform.position;
            }
            if (mProps == ElsProps.GroupUpdate)
                elsController.Instance.GroupUpdateCount++;
            else if (mProps == ElsProps.GroupSpriteUpdate)
                elsController.Instance.GroupColorUpdateCount++;
            elsController.Instance.UpdatePropsCountShow();
            RemoveFromDic();
        }
    }
   
    int GetSpriteIndex()
    {
        for (int i = 0; i < elsController.Instance.GroupSpriteAry.Length; i++)
        {
            if (mSprite == elsController.Instance.GroupSpriteAry[i])
                return i;
        }
        return 0;
    }
    public void InitSprite(Sprite sp)
    {
        if (mProps != ElsProps.None) return;
        mSprite = sp;
        GetComponent<Image>().sprite = sp;
    }

    public void SaveToDic()
    {
        isSet = true;
        Vector3 dropPos = transform.position - elsController.Instance.mImage_Content.transform.position;
        int xI = (int)(dropPos.x) / 100 + 1;
        int yI = (int)(dropPos.y) / 100 + 1;
        mX = xI;
        mY = yI;
        ElsPos elsP = new ElsPos(xI, yI);
        if (elsController.Instance.mItemDic.ContainsKey(elsP))
            elsController.Instance.mItemDic[elsP] = this;
    }
    public void RemoveFromDic()
    {
        ElsPos elsP = new ElsPos(mX, mY);
        if (elsController.Instance.mItemDic.ContainsKey(elsP))
            elsController.Instance.mItemDic[elsP] = null;
        mX = mY = 0;
    }
    public void HideFromGroup()
    {
        gameObject.SetActive(false);
    }
    public void SetLockState()
    {
        GetComponent<Image>().color = Color.gray;
    }
    public void SetUnlockState()
    {
        GetComponent<Image>().color = Color.white; 
    }
}
