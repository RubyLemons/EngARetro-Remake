using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickReset : MonoBehaviour
{
    CursorLockMode defaultPointerMode = CursorLockMode.Locked;
    bool useState = true;

    [Range(0, 60)] [SerializeField] int targetFps = 60;

    string[] cheatCombo = new string[5] { "dGlja2V0c2hvdw==", "cGFjaWZ5", "Q2FtU29kYQ==", "bGlsaXRobGlu", "ZGVidWc=" };
    Dictionary<string, string> userCombo = new Dictionary<string, string>();
    Dictionary<string, float> inputTimer = new Dictionary<string, float>();

    [SerializeField] GameObject monsters;

    void Awake()
    {
        Time.timeScale = 1;
        Token.pointerMode = defaultPointerMode;
    }

    void Update()
    {
        Token.tks = Mathf.Clamp(Token.tks, 0, int.MaxValue);


        //Pointer mode

        if (Input.GetMouseButtonDown(2))
            useState = !useState;

        if (monsters != null)
            Cursor.lockState = useState  ? Token.pointerMode : CursorLockMode.None;


        //Target Fps

        Application.targetFrameRate = targetFps;


        //Reset

        bool ctrl = Input.GetKey(KeyCode.LeftControl);
        bool shift = Input.GetKey(KeyCode.LeftShift);

        if (ctrl && Input.GetKeyDown(KeyCode.R))
            Token.ReloadScene(shift);


        //Pi CBCZ

        ListenForCheat(cheatCombo[0], () => Token.tickets = 999);

        ListenForCheat(cheatCombo[1], () =>
        {
            Token.ammo[0] = 28;
            Token.ammo[1] = 112;
        });

        ListenForCheat(cheatCombo[2], () =>
        {
            Token.forbidden = true;
            Token.tks = int.MaxValue;
        });

        ListenForCheat(cheatCombo[3], () => monsters.SetActive(!monsters.activeSelf));

        ListenForCheat(cheatCombo[4], () => Token.informMonsters = !Token.informMonsters);
    }

    void ListenForCheat(string targetCombo, System.Action action)
    {
        if (!inputTimer.ContainsKey(targetCombo))
            inputTimer.Add(targetCombo, 0);

        inputTimer[targetCombo] += Time.unscaledDeltaTime;

        if (inputTimer[targetCombo] > 0.75f)
            userCombo[targetCombo] = "";

        if (Input.inputString.Length > 0)
        {
            if (!userCombo.ContainsKey(targetCombo))
                userCombo.Add(targetCombo, "");

            userCombo[targetCombo] += Input.inputString;

            string comboAtob = ColorConvert.atob(System.Convert.FromBase64String(targetCombo));
            //print($"{userCombo[targetCombo]}, {userCombo[targetCombo][userCombo[targetCombo].Length - 1] == comboAtob[userCombo[targetCombo].Length - 1]}, {comboAtob[userCombo[targetCombo].Length - 1]}"); //debug

            bool correctInput = userCombo[targetCombo][userCombo[targetCombo].Length - 1] == comboAtob[userCombo[targetCombo].Length - 1];
            bool validCombo = (userCombo[targetCombo] == comboAtob);

            inputTimer[targetCombo] = (correctInput) ? 0 : inputTimer[targetCombo];

            if (!correctInput && !validCombo)
                userCombo[targetCombo] = "";

            //Gift
            if (validCombo) {
                userCombo[targetCombo] = "";

                Inform.instance.Alert(ColorConvert.atob(System.Convert.FromBase64String("Q2hlYXQgYWN0aXZhdGVk")), 2.5f);
                action.Invoke();
            }
        }
    }
}
