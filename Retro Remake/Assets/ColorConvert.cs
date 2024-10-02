using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorConvert
{
    public static Color FromHex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color hexColor);
        return hexColor;
    }

    public static string atob(byte[] encoded)
    {
        return System.Text.Encoding.UTF8.GetString(encoded);
    }
}
