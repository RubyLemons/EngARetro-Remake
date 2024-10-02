using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Upgrades : MonoBehaviour
{
    [Header("Death")]

    [SerializeField] Health health;

    public static Dictionary<BtnLabel.BtnAction, Action> btnDeathAction = new Dictionary<BtnLabel.BtnAction, Action>()
    {
        { BtnLabel.BtnAction.Live, () => Token.ReloadScene(false) },
        { BtnLabel.BtnAction.Restart, () => Token.ReloadScene(true) },
        { BtnLabel.BtnAction.End, () => Application.Quit() },
        { BtnLabel.BtnAction.Back, () => UnityEngine.SceneManagement.SceneManager.LoadScene(0) },
        { BtnLabel.BtnAction.Play, () => UnityEngine.SceneManagement.SceneManager.LoadScene(1) },
    };

    [Header("Gui")]

    [SerializeField] TextMeshProUGUI labelTokens;
    [SerializeField] TextMeshProUGUI labelTickets;

    [SerializeField] Sprite ticketShow;

    [Space(10)]

    [SerializeField] CanvasGroup group;
    [SerializeField] bool startState;

    [Header("Shop")]

    [SerializeField] KeyCode secondaryKey = KeyCode.E;

    Dictionary<Token.upgrades, Action> upgradeAction;
    Dictionary<Token.upgrades, Func<int>> getLevel;

    [SerializeField] Transform list;

    [Header("Animation")]

    [SerializeField] Image[] stroke = new Image[2];
    [SerializeField] string[] colorKey = new string[] { "#000012", "#F4C22C" };

    void Start()
    {
        Token.inShop = startState;
        ToggleShop(Token.inShop);

        upgradeAction = new Dictionary<Token.upgrades, Action>()
        {
            { Token.upgrades.Enchanment, () => Gun.enchantment++ },
            { Token.upgrades.Mag, () => Gun.magLevel++ },
            { Token.upgrades.Stock, () => Gun.spreadLevel++ },
            { Token.upgrades.RecoilSuppressor, () => Gun.recoilLevel++ },
        };
    }


    void Update()
    {
        if (getLevel == null)
            getLevel = new Dictionary<Token.upgrades, Func<int>>()
            {
                { Token.upgrades.Enchanment, () => Gun.enchantment },
                { Token.upgrades.Mag, () => Gun.magLevel },
                { Token.upgrades.Stock, () => Gun.spreadLevel },
                { Token.upgrades.RecoilSuppressor, () => Gun.recoilLevel },
            };


        labelTokens.text = Token.tks.ToString();
        labelTickets.text = Token.tickets.ToString();

        labelTickets.transform.parent.gameObject.SetActive(Token.inShop || health.deathFrame.gameObject.activeSelf);

        MatchPrices(); // and upgrade btns

        if (AnimateOnValue.instance != null)
        {
            AnimateOnValue.instance.Animate("Tks", Token.tks, stroke[0], colorKey); //aninate display Tks
            AnimateOnValue.instance.Animate("Parts", Token.tickets, stroke[1], colorKey); //aninate display Parts
        }

        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(secondaryKey))
        {
            Token.inShop = !Token.inShop;
            ToggleShop(Token.inShop);
        }

        //Death frame

        DeathBtns();
    }

    void DeathBtns()
    {
        foreach (Transform v in health.buttons.transform)
        {
            BtnLabel btn = v.GetComponent<BtnLabel>();

            bool withinFuns = (Token.tickets >= btn.ticketsPrice);

            //On click

            if (btn.focus && withinFuns)
            {
                btnDeathAction.TryGetValue(btn.btnAction, out Action action);
                action.Invoke(); //fire btn

                //charge
                Token.tickets -= btn.ticketsPrice;

                //increase the prices
                btn.ticketsPrice = Mathf.CeilToInt(btn.ticketsPrice * 1.375f);
            }
        }
    }

    void ToggleShop(bool stateEnabled)
    {
        group.blocksRaycasts = stateEnabled;


        Time.timeScale = stateEnabled ? 0 : 1;

        group.alpha = stateEnabled ? 1 : 0;

        Token.pointerMode = stateEnabled ? CursorLockMode.None : CursorLockMode.Locked;
    }

    void MatchPrices()
    {
        foreach (Transform v in list)
        {
            Btn btn = v.Find("Btn").GetComponent<Btn>();

            //match prices
            btn.tksLabel.text = btn.tksPrice.ToString();
            btn.ticketsLabel.text = btn.ticketsPrice.ToString();

            bool maxed = (btn.level >= btn.maxLevel);
            BtnMaxedOutVisuals();
            LevelLabelVisuals(); //assign level

            Token.maxUpgrades = CheckMaxed();

            //Upgrade

            bool withinFuns = (Token.tks >= btn.tksPrice) && (Token.tickets >= btn.ticketsPrice);

            if (btn.focus && (withinFuns && !maxed))
            {
                Upgrade(btn.upgrade, btn.tksPrice, btn.ticketsPrice);

                //increase the prices
                btn.tksPrice = Mathf.CeilToInt(btn.tksPrice * 1.375f);
                btn.ticketsPrice = Mathf.FloorToInt(btn.ticketsPrice * 1.5f);
            }

            bool CheckMaxed()
            {
                if (!btn.maxed)
                    return false;

                return true;
            }



            //Maxed out

            void BtnMaxedOutVisuals()
            {
                if (!maxed) return;

                btn.img.color = ColorConvert.FromHex("#05AFFA");
                btn.label.text = "Max";
                btn.label.fontSize = 25;
                btn.ticketsLabel.transform.parent.GetComponent<Image>().sprite = ticketShow;

                btn.maxed = true;
            }

            void LevelLabelVisuals()
            {
                //Match level text
                getLevel.TryGetValue(btn.upgrade, out Func<int> levelFunc);

                btn.level = levelFunc.Invoke();

                //color level text
                btn.levelLabel.text = $"Level {btn.level + 1}";
                btn.levelLabel.color = maxed ? ColorConvert.FromHex("#00FFFF") : Color.white;
            }
        }
    }

    void Upgrade(Token.upgrades key, int tksPrice, int partsPrice)
    {
        upgradeAction.TryGetValue(key, out Action action);
        action.Invoke();

        //charge
        Token.tks -= tksPrice;
        Token.tickets -= partsPrice;
    }
}
