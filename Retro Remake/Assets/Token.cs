using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Token
{
    public static int tks = 0;
    public static int tickets = 0;

    public static int[] ammo = new int[2] { 28, 112 };

    public static bool forbidden = false;

    public static int numberMonsters = 0;
    public static float leastMonsters = 0; //like non monsters (before can spawn more)
    public static float monsterSpeed = 0;
    public static int round = 0;
    public static bool informMonsters = false;

    public static bool firstTime = true;

    public static bool maxUpgrades = false;

    [System.Serializable]
    public enum upgrades
    {
        Enchanment,
        Mag,
        Stock,
        RecoilSuppressor,
    }


    public static bool inShop;
    public static CursorLockMode pointerMode;

    public static IEnumerator SetTimeout(System.Action action, float delayTime, bool ignoreTimeScale = true)
    {
        if (!ignoreTimeScale)
            yield return new WaitForSeconds(delayTime);
        else
            yield return new WaitForSecondsRealtime(delayTime);

        action.Invoke();
    }

    public static void ReloadScene(bool clear = false)
    {
        if (clear)
            Reset();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void Reset()
    {
        //enemies
        numberMonsters = 0;
        leastMonsters = 0;
        monsterSpeed = 0;
        round = 0;

        //currency
        tks = 0;
        tickets = 0;

        //gun
        Gun.enchantment = 0;
        Gun.magLevel = 0;
        Gun.spreadLevel = 0;
        Gun.recoilLevel = 0;

        //ammo
        ammo[0] = 28;
        ammo[1] = 112;
    }
}
