using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour
{
    
    [SerializeField] CanvasGroup group;
    [SerializeField] float tweenTime = 1;

    [SerializeField] MonsterSpawner spawner;

    void Start()
    {
        group.alpha = 1;

        StartCoroutine(Token.SetTimeout(() =>
        {
            LeanTween.value(1, 0, tweenTime).setOnUpdate((v) => { 
                if (group == null) return;
                group.alpha = v;
            });
        }, spawner.delayTime - tweenTime));
    }
}
