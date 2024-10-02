using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimateOnValue : MonoBehaviour
{
    public static AnimateOnValue instance { get; private set; }
    
    Dictionary<string, float> check = new Dictionary<string, float>();
    Dictionary<string, float> late = new Dictionary<string, float>();

    void Awake()
    {
        Assign();
    }

    void Update()
    {
        Assign();
    }

    void Assign()
    {
        if (instance == null)
            instance = this;
    }

    public void Animate(string key, float focusValue, Image img, string[] hexKey, float smooth = 0.1f)
    {
        if (!check.ContainsKey(key)) {
            check.Add(key, 0);
            late.Add(key, 0);
        }

        check[key] = focusValue;

        bool changed = (check[key] != late[key]);

        img.color = changed ? ColorConvert.FromHex(hexKey[1]) : img.color;
        img.color = Color.Lerp(img.color, ColorConvert.FromHex(hexKey[0]), smooth);

        late[key] = focusValue;
    }
}
