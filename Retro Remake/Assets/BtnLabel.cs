using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public class BtnLabel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [System.Serializable]
    public enum BtnAction
    {
        Live,
        Restart,
        Back,
        End,
        Play,
    }

    public BtnAction btnAction;

    [HideInInspector] public TextMeshProUGUI label;
    [SerializeField] TextMeshProUGUI priceLabel;
    public int ticketsPrice = 0;

    public bool target;
    public bool focus;

    [Header("Animation")]

    [SerializeField] bool userFeedback;
    Color initialColor;
    [SerializeField] float[] colorState = new float[2] { 0, 0.1f };


    void Start()
    {
        label = transform.GetComponent<TextMeshProUGUI>();
        initialColor = label.color;

        if (priceLabel != null)
            priceLabel.text = ticketsPrice.ToString();

        ColorBtnState(0);
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
        label.color = new Color(initialColor.r - colorState[state], initialColor.g - colorState[state], initialColor.b - colorState[state]);
    }
}
