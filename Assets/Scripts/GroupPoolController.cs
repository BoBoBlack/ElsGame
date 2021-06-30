using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupPoolController : MonoBehaviour
{
    public static Dictionary<string, Stack<elsGroup>> mGroupPool = new Dictionary<string, Stack<elsGroup>>();
    
    public static void SaveGroup(elsGroup group)
    {
        group.gameObject.SetActive(false);
        group.transform.SetParent(elsController.Instance.mImage_Content.transform);
        group.mSprite = null;
        if (!mGroupPool.ContainsKey(group.groupName))
        {
            mGroupPool.Add(group.groupName, new Stack<elsGroup>()); 
        }
        mGroupPool[group.groupName].Push(group);
    }
    public static elsGroup GetGroup(string groupName)
    {
        elsGroup group = null;
        if (mGroupPool.ContainsKey(groupName)&& mGroupPool[groupName].Count>0)
        {
            group = mGroupPool[groupName].Pop();
        }
        return group;
    }
}
