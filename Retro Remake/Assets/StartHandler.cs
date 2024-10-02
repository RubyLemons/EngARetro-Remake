using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartHandler : MonoBehaviour
{

    void Update()
    {
        Btns();
    }

    void Btns()
    {
        foreach (Transform v in transform.GetChild(0))
        {
            BtnLabel btn = v.GetComponent<BtnLabel>();

            bool withinFuns = (Token.tickets >= btn.ticketsPrice);

            //On click

            if (btn.focus && withinFuns)
            {
                Upgrades.btnDeathAction.TryGetValue(btn.btnAction, out System.Action action);
                action.Invoke(); //fire btn
            }
        }
    }
}
