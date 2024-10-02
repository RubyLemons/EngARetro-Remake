using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropCollectable : MonoBehaviour
{

    [Serializable]
    public enum DropType
    {
        Tks,
        Tickets,
        Supplize
    }

    public DropType type;

    Dictionary<DropType, Action> getAction = new Dictionary<DropType, Action>()
    {
        { DropType.Tks, () => Token.tks++ },
        { DropType.Tickets, () => {
            Token.tickets++;
            if (Token.firstTime)
            {
                Token.firstTime = false;
                Inform.instance.Alert("<color=#00FFFF>[E]</color> view <color=#FFFF00>Upgrades</color>", 30);
            }
        } },
        { DropType.Supplize, () => { 
            Token.ammo[0] = (Token.ammo[0] > 0) ? 28 : Token.ammo[0];
            Token.ammo[1] = 112;
            }
        },
    };

    Dictionary<DropType, Func<Sprite>> imgSrc;
    Image ico;

    [SerializeField] Sprite imgTks;
    [SerializeField] Sprite imgTicket;
    [SerializeField] Sprite imgSupplize;

    [Space(10)]

    [SerializeField] float tweenTime = 1.65f;

    float elaspedTime;
    [SerializeField] float delay = 1;

    bool triggered;

    void Update() {
        elaspedTime += Time.deltaTime;
    }

    void OnEnable()
    {
        //set img
        ico = transform.GetChild(0).gameObject.GetComponent<Image>();

        imgSrc = new Dictionary<DropType, Func<Sprite>>()
        {
            { DropType.Tks, () => imgTks },
            { DropType.Tickets, () => imgTicket },
            { DropType.Supplize, () => imgSupplize },
        };

        //Set the img src depending on the drop type
        imgSrc.TryGetValue(type, out Func<Sprite> imgSrcFunc);
        ico.sprite = imgSrcFunc.Invoke();
    }

    void OnTriggerEnter(Collider hit)
    {
        if (!hit.CompareTag("Player") || triggered || elaspedTime < delay) return;
        triggered = true;

        StopPhysics();

        Get();

        LeanTween.scale(gameObject, Vector3.zero, tweenTime);
    }

    void Get()
    {
        getAction.TryGetValue(type, out Action action);
        action.Invoke();

        Destroy(gameObject);
    }


    void StopPhysics()
    {
        GetComponent<Rigidbody>().isKinematic = true;

        for (int i = 0; i < GetComponents<SphereCollider>().Length; i++)
            GetComponents<SphereCollider>()[i].enabled = false;
    }
}
