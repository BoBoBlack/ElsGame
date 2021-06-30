using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIAnimObj : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(GetComponent<UGUISpriteAnimation>()!=null)
        {
            GetComponent<UGUISpriteAnimation>().onAnimEnd += OnSpEndEvent;
        }
        if(GetComponent<DOTweenAnimation>() != null)
        {
            GetComponent<DOTweenAnimation>().onComplete.AddListener(OnDoTweenEndEvent);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnSpEndEvent()
    {
        Destroy(gameObject);
    }
    void OnDoTweenEndEvent()
    {
        Destroy(gameObject);
    }
}
