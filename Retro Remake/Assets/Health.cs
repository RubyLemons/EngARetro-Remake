using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class Health : MonoBehaviour
{

    [SerializeField] Freelook freelook;

    [SerializeField] float disorient = 10;

    [Header("Health")]

    [Range(0, 1)] public float health = 1;
    [SerializeField] float healTime = 2f;

    float elapsedTime;

    [SerializeField] float debTime = 1f;
    float elapsedTimeDeb;
    public bool deb;

    bool died;

    [Header("Gui")]

    [SerializeField] Image bar;
    [Range(0, 1)] [SerializeField] float barSmooth = 0.025f;
    string[] colorKey = new string[2] { "#93002E", "#FFFFFF" };

    [Space(10)]

    public Image deathFrame;
    GameObject content;
    public GameObject buttons;
    CanvasGroup buttonsGroup;

    [SerializeField] TMPro.TextMeshProUGUI labelRound;
    CanvasGroup labelRoundGroup;

    [Header("Volume")]

    [SerializeField] Volume indicator;
    [Range(0, 1)] [SerializeField] float volumeSmooth = 0.075f;


    void Start()
    {
        deathFrame.gameObject.SetActive(false);
        deathFrame.color = Color.black;

        content = deathFrame.transform.GetChild(0).gameObject;
        buttons = content.transform.GetChild(0).gameObject;
        buttonsGroup = buttons.GetComponent<CanvasGroup>();

        labelRoundGroup = labelRound.transform.parent.GetComponent<CanvasGroup>();

        content.SetActive(false);
        buttons.SetActive(false);
        labelRoundGroup.gameObject.SetActive(false);

        died = false;
    }


    void Update()
    {
        health = Mathf.Clamp01(health);

        bar.fillAmount = Mathf.MoveTowards(bar.fillAmount, health, barSmooth); //aniamte bar
        if (AnimateOnValue.instance)
            AnimateOnValue.instance.Animate("Health", health, bar, colorKey);
        

        elapsedTimeDeb = (deb) ? elapsedTimeDeb + Time.deltaTime : 0;

        if (elapsedTimeDeb > debTime)
            deb = false;



        elapsedTime = (!deb && health < 1 && health > 0) ? elapsedTime + Time.deltaTime : 0;

        if (health < 1 && health > 0 && (elapsedTime > healTime))
        {
            health += 0.5f;
            elapsedTime = 0;
        }

        //Died Loser

        deathFrame.color = Color.Lerp(deathFrame.color, Color.black, 0.05f);
        buttonsGroup.alpha = (buttonsGroup.alpha < 0.9f) ? Mathf.MoveTowards(buttonsGroup.alpha, 1, 0.05f) : 1;

        labelRound.text = $"Waves {Token.round}";
        labelRoundGroup.alpha = (labelRoundGroup.alpha < 0.9f) ? Mathf.MoveTowards(labelRoundGroup.alpha, 1, 0.05f) : 1;

        if (health <= 0 && !died) {
            died = true;
            Death();
        }

        //volume
        indicator.weight = (indicator.weight > 0.1f) ? Mathf.Lerp(indicator.weight, (health < 1) ? 1 : 0, volumeSmooth) : 0;
    }


    void Death()
    {
        Time.timeScale = 0;
        Token.pointerMode = CursorLockMode.None;

        deathFrame.gameObject.SetActive(true);
        deathFrame.color = Color.white;

        StartCoroutine(Token.SetTimeout(() =>
        {
            content.SetActive(true);
            
            StartCoroutine(Token.SetTimeout(() =>
            {
                buttonsGroup.alpha = 0;
                buttons.SetActive(true);
                StartCoroutine(Token.SetTimeout(() =>
                {
                    labelRoundGroup.alpha = 0;
                    labelRoundGroup.gameObject.SetActive(true);
                }, 1f));
            }, 1.5f));
        }, 1f));
    }


    public void Damage(float value)
    {
        if (deb || health < 0) return;
        deb = true;
        elapsedTimeDeb = 0;

        health -= value;

        indicator.weight = 0.15f;
        freelook.mouseDelta += new Vector2(Random.Range(-disorient, disorient), Random.Range(-disorient, disorient));
    }
}
