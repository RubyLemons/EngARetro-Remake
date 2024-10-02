using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public class Btn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    [HideInInspector] public Image img;
    [HideInInspector] public TextMeshProUGUI label;
    [HideInInspector] public TextMeshProUGUI levelLabel;

    Transform priceOverlay;
    [HideInInspector] public TextMeshProUGUI ticketsLabel;
    [HideInInspector] public TextMeshProUGUI tksLabel;

    public bool target;

    [Header("Function")]

    public Token.upgrades upgrade;
    [Range(0, 2)] public int level = 0;
    public int maxLevel = 2;

    public int ticketsPrice = 1;
    public int tksPrice = 10;

    [HideInInspector] public bool maxed;

    [Space(10)]

    public bool focus;


    [Header("Animation")]

    [SerializeField] bool userFeedback;
    Color initialColor;
    [SerializeField] float[] colorState = new float[2] { 0, 0.1f };


    void Start()
    {
        img = GetComponent<Image>();
        initialColor = img.color;

        label = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        levelLabel = transform.parent.Find("Level").GetComponent<TextMeshProUGUI>();

        priceOverlay = transform.parent.Find("Price");
        tksLabel = priceOverlay.Find("Tks").GetChild(0).GetComponent<TextMeshProUGUI>();
        ticketsLabel = priceOverlay.Find("Tickets").GetChild(0).GetComponent<TextMeshProUGUI>();
    }


    //MOUSEOVER

    public void OnPointerEnter(PointerEventData e)
    {
        target = true;
        ColorBtnState(1);
    }

    //MOUSEOUT

    public void OnPointerExit(PointerEventData e)
    {
        target = false;
        ColorBtnState(0);
    }

    //CLICK

    public void OnPointerClick(PointerEventData e)
    {
        StartCoroutine(SignalClick());
    }

    IEnumerator SignalClick()
    {
        focus = true;
        yield return new WaitForEndOfFrame();
        focus = false;
    }

    
    void ColorBtnState(int state)
    {
        img.color = new Color(initialColor.r - colorState[state], initialColor.g - colorState[state], initialColor.b - colorState[state]);
    }
}
