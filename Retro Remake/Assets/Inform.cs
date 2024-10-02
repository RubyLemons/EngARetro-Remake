using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inform : MonoBehaviour
{
    public static Inform instance { get; private set; }

    void Awake() {
        Assign();
    }

    void Update() {
        Assign();
    }

    void Assign()
    {
        if (instance == null)
            instance = this;
    }

    public void Alert(string msg, float ttl = 5)
    {
        StartCoroutine(Message(msg, ttl));

        IEnumerator Message(string msg, float ttl = 1)
        {
            GameObject alert = Instantiate(gameObject, transform.parent);
            CanvasGroup alertGroup = alert.GetComponent<CanvasGroup>();
            TMPro.TextMeshProUGUI alertLabel = alert.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();

            alertGroup.alpha = 1;

            alertLabel.text = msg.ToString();

            alert.transform.localScale = Vector3.one * 1.1f;
            LeanTween.scale(alert, Vector3.one, 0.25f).setEaseOutBack();

            yield return new WaitForSecondsRealtime(ttl);

            LeanTween.value(1, 0, 1)
                .setOnUpdate((v) => {
                    if (alertGroup == null) return;

                    alertGroup.alpha = v;
                })
                .setOnComplete(() => Destroy(alert));

            yield return new WaitForSecondsRealtime(1);
            if (alert != null)
                Destroy(alert);
        }
    }
}
