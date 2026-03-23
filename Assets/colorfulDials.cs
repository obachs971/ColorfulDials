using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using KModkit;
using UnityEngine;

public class colorfulDials : MonoBehaviour
{
    public new KMAudio audio;
    public KMBombInfo bomb;
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    public KMBombModule module;
    public KMSelectable[] dials;
    public KMSelectable submit;
    public TextMesh[] dialScreens;
    public TextMesh[] mainScreen;
    public KMSelectable[] colorButtons;
    public MeshRenderer[] buttonColors;
    public Material[] colors;
    public MeshRenderer[] dialColors;
    public TextMesh[] dialNumbers;
    public AudioClip[] sounds;

    // Colorblind
    public KMColorblindMode colorblindMode;
    public TextMesh[] dialCbTexts;

    private string[] mainScreenColors;

    private Color[] colorHexs =
        {
            new Color(1f, 0f, 0f),
            new Color(1f, 0.5f, 0f),
            new Color(1f, 1f, 0f),
            new Color(0f, 1f, 0f),
            new Color(0f, 1f, 1f),
            new Color(0f, 0f, 1f),
            new Color(0.5f, 0f, 0.5f),
            new Color(1f, 0.5f, 0.9f)
        };
    private string[][] dialNumColors;
    private int[] DialPos;
    private int[][][] DialVals;
    private string[][][] DialValColors;
    private Color[][][] DialValMat;
    private string[] dialColorNames;
    private string toggledColor;
    private string[] clist;
    private int[] DialPosAns;
    private string[] DialColAns;
    private int submitTime;
    private string[][] cardinalChart =
    {
        new string[8]{"S", "NE", "SE", "SW", "E", "N", "NW", "W"},
        new string[8]{"E", "S", "NW", "NE", "N", "SE", "W", "SW"},
        new string[8]{"NW", "SW", "N", "W", "SE", "S", "NE", "E"},
        new string[8]{"SW", "W", "S", "SE", "NE", "E", "N", "NW"},
        new string[8]{"SE", "NW", "SW", "N", "W", "NE", "E", "S"},
        new string[8]{"N", "SE", "W", "E", "S", "NW", "SW", "NE"},
        new string[8]{"W", "E", "NE", "S", "NW", "SW", "SE", "N"},
        new string[8]{"NE", "N", "E", "NW", "SW", "W", "S", "SE"},
    };
    private string[][] colorGrid =
    {
        new string[10]{"0O", "5G", "4M", "5C", "7P", "3Y", "8C", "9Y", "3C", "3B"},
        new string[10]{"2C", "6G", "9O", "2P", "2G", "2R", "0R", "4C", "2B", "1P"},
        new string[10]{"0M", "0P", "0G", "2M", "9G", "1C", "7O", "5B", "6Y", "6P"},
        new string[10]{"1O", "6M", "1R", "8G", "6B", "4O", "0B", "4P", "1M", "1Y"},
        new string[10]{"9C", "8B", "1G", "6C", "6O", "8O", "8R", "0C", "4Y", "7Y"},
        new string[10]{"7R", "3G", "7B", "5P", "2Y", "3O", "5R", "9M", "1B", "8Y"},
        new string[10]{"5M", "9P", "9R", "7C", "0Y", "8M", "3R", "7M", "3M", "5Y"},
        new string[10]{"4G", "2O", "8P", "7G", "4R", "4B", "6R", "9B", "3P", "5O"},
    };
    private string[] dialNumCL = { "RED", "ORANGE", "YELLOW", "GREEN", "CYAN", "BLUE", "MAGENTA", "PINK" };
    void Awake()
    {
        moduleId = moduleIdCounter++;
        dials[0].OnInteract += delegate () { Turn(0); return false; };
        dials[1].OnInteract += delegate () { Turn(1); return false; };
        dials[2].OnInteract += delegate () { Turn(2); return false; };
        submit.OnInteract += delegate () { onSubmit(); return false; };
    }
    void Start()
    {
        dialColorNames = new string[3];
        dialColorNames[0] = "BLACK";
        dialColorNames[1] = "BLACK";
        dialColorNames[2] = "BLACK";
        toggledColor = "BLACK";
        DialPos = new int[3];
        DialPos[0] = 0;
        DialPos[1] = 0;
        DialPos[2] = 0;
        clist = new string[9];
        string[] colorList = { "RED", "ORANGE", "YELLOW", "GREEN", "CYAN", "BLUE", "MAGENTA", "PINK" };
        for (int aa = 0; aa < 8; aa++)
        {
            int n = UnityEngine.Random.Range(0, colorList.Length);
            clist[aa] = colorList[n].ToUpper();
            colorList = remove(colorList, n);
            switch (clist[aa])
            {
                case "RED":
                    buttonColors[aa].material = colors[0];
                    break;
                case "ORANGE":
                    buttonColors[aa].material = colors[1];
                    break;
                case "YELLOW":
                    buttonColors[aa].material = colors[2];
                    break;
                case "GREEN":
                    buttonColors[aa].material = colors[3];
                    break;
                case "CYAN":
                    buttonColors[aa].material = colors[4];
                    break;
                case "BLUE":
                    buttonColors[aa].material = colors[5];
                    break;
                case "MAGENTA":
                    buttonColors[aa].material = colors[6];
                    break;
                case "PINK":
                    buttonColors[aa].material = colors[7];
                    break;
            }
        }
        clist[8] = "BLACK";
        colorButtons[0].OnInteract += delegate () { toggleColor(clist[0]); return false; };
        colorButtons[1].OnInteract += delegate () { toggleColor(clist[1]); return false; };
        colorButtons[2].OnInteract += delegate () { toggleColor(clist[2]); return false; };
        colorButtons[3].OnInteract += delegate () { toggleColor(clist[3]); return false; };
        colorButtons[4].OnInteract += delegate () { toggleColor(clist[4]); return false; };
        colorButtons[5].OnInteract += delegate () { toggleColor(clist[5]); return false; };
        colorButtons[6].OnInteract += delegate () { toggleColor(clist[6]); return false; };
        colorButtons[7].OnInteract += delegate () { toggleColor(clist[7]); return false; };
        colorButtons[8].OnInteract += delegate () { toggleColor("BLACK"); return false; };

        dialNumColors = new string[3][];
        dialNumColors[0] = new string[10];
        dialNumColors[1] = new string[10];
        dialNumColors[2] = new string[10];
        for (int aa = 0; aa < 30; aa++)
        {
            int n = UnityEngine.Random.Range(0, colorHexs.Length);
            dialNumbers[aa].color = colorHexs[n];
            dialNumColors[aa / 10][aa % 10] = dialNumCL[n].ToUpper();
        }
        mainScreenColors = new string[3];
        for (int aa = 0; aa < 3; aa++)
        {
            int n = UnityEngine.Random.Range(0, 10);
            mainScreen[aa].text = n + "";
            n = UnityEngine.Random.Range(0, colorHexs.Length);
            mainScreen[aa].color = colorHexs[n];
            mainScreenColors[aa] = dialNumCL[n].ToUpper();
        }
        DialVals = new int[3][][];
        DialValColors = new string[3][][];
        DialValMat = new Color[3][][];
        for (int aa = 0; aa < 3; aa++)
        {
            DialVals[aa] = new int[8][];
            DialValColors[aa] = new string[8][];
            DialValMat[aa] = new Color[8][];
            for (int bb = 0; bb < 8; bb++)
            {
                DialVals[aa][bb] = new int[10];
                DialValColors[aa][bb] = new string[10];
                DialValMat[aa][bb] = new Color[10];
                for (int cc = 0; cc < 10; cc++)
                {
                    DialVals[aa][bb][cc] = UnityEngine.Random.Range(0, 100);
                    int n = UnityEngine.Random.Range(0, 8);
                    DialValColors[aa][bb][cc] = dialNumCL[n].ToUpper();
                    DialValMat[aa][bb][cc] = colorHexs[n];
                }
            }
        }
        dialScreens[0].text = "";
        dialScreens[1].text = "";
        dialScreens[2].text = "";
        getSolution();

        if (colorblindMode.ColorblindModeActive)
            ActivateColorblind();
    }

    private void ActivateColorblind()
    {
        for (var i = 0; i < 3; i++)
            dialCbTexts[i].gameObject.SetActive(true);
    }

    string[] remove(string[] c, int n)
    {
        string[] conv = new string[c.Length - 1];
        int items = 0;
        for (int aa = 0; aa < c.Length; aa++)
        {
            if (aa != n)
            {
                conv[items] = c[aa].ToUpper();
                items++;
            }
        }
        return conv;
    }
    void getSolution()
    {
        DialPosAns = new int[3];
        DialColAns = new string[3];
        for (int aa = 0; aa < 3; aa++)
        {
            Debug.LogFormat("[Colorful Dials #{0}] Main screen digit #{1}: {2} {3}", moduleId, aa + 1, mainScreenColors[aa], mainScreen[aa].text);
            int row = "ROYGCBMP".IndexOf(mainScreenColors[aa][0]);
            int col = 0;
            for (int bb = 0; bb < 8; bb++)
            {
                if (clist[bb].Equals(mainScreenColors[aa]))
                {
                    Debug.LogFormat("[Colorful Dials #{0}] Position of {1} square: {2}", moduleId, mainScreenColors[aa].ToLower(), bb + 1);
                    col = bb;
                    break;
                }
            }
            Debug.LogFormat("[Colorful Dials #{0}] Cardnial direction: {1}", moduleId, cardinalChart[row][col]);
            string cardinal = cardinalChart[row][col].ToUpper();
            int numTimes = 1;
            for (int bb = 0; bb < 10; bb++)
            {
                if (dialNumColors[aa][bb].Equals(mainScreenColors[aa]))
                    numTimes++;
            }
            Debug.LogFormat("[Colorful Dials #{0}] Number of {1} colors: {2}", moduleId, mainScreenColors[aa].ToLower(), numTimes - 1);
            row = -1;
            col = -1;
            for (int bb = 0; bb < colorGrid.Length; bb++)
            {
                for (int cc = 0; cc < colorGrid[bb].Length; cc++)
                {
                    if (colorGrid[bb][cc][1] == mainScreenColors[aa][0] && colorGrid[bb][cc][0] == mainScreen[aa].text[0])
                    {
                        row = bb;
                        col = cc;
                        break;
                    }
                }
                if (row >= 0)
                    break;
            }
            switch (cardinal)
            {
                case "N":
                    row -= numTimes;
                    break;
                case "NE":
                    row -= numTimes;
                    col += numTimes;
                    break;
                case "E":
                    col += numTimes;
                    break;
                case "SE":
                    row += numTimes;
                    col += numTimes;
                    break;
                case "S":
                    row += numTimes;
                    break;
                case "SW":
                    row += numTimes;
                    col -= numTimes;
                    break;
                case "W":
                    col -= numTimes;
                    break;
                case "NW":
                    row -= numTimes;
                    col -= numTimes;
                    break;
            }
            row = mod(row, 8);
            col = mod(col, 10);
            DialColAns[aa] = dialNumCL["ROYGCBMP".IndexOf(colorGrid[row][col][1])];
            DialPosAns[aa] = colorGrid[row][col][0] - '0';
            Debug.LogFormat("[Colorful Dials #{0}] Initial dial #{1} configuration: {2}", moduleId, aa + 1, DialColAns[aa] + " " + DialPosAns[aa]);
        }
        for (int aa = 0; aa < 3; aa++)
        {
            Debug.LogFormat("[Colorful Dials #{0}] Dial color #{1}: {2}", moduleId, aa + 1, DialColAns[aa]);
            int row = "ROYGCBMP".IndexOf(DialColAns[aa][0]);
            int col = 0;
            for (int bb = 0; bb < 8; bb++)
            {
                if (clist[bb].Equals(DialColAns[aa]))
                {
                    Debug.LogFormat("[Colorful Dials #{0}] Position of {1} square: {2}", moduleId, DialColAns[aa].ToLower(), bb + 1);
                    col = bb;
                    break;
                }
            }
            Debug.LogFormat("[Colorful Dials #{0}] Cardnial direction: {1}", moduleId, cardinalChart[row][col]);
            string cardinal = cardinalChart[row][col].ToUpper();
            int numA;
            int numB;
            string color = DialValColors[aa]["ROYGCBMP".IndexOf(DialColAns[aa][0])][DialPosAns[aa]].ToUpper();
            if (bomb.GetSerialNumberNumbers().Last() % 2 == 0)
            {
                numA = DialVals[aa]["ROYGCBMP".IndexOf(DialColAns[aa][0])][DialPosAns[aa]] / 10;
                numB = DialVals[aa]["ROYGCBMP".IndexOf(DialColAns[aa][0])][DialPosAns[aa]] % 10;
            }
            else
            {
                numB = DialVals[aa]["ROYGCBMP".IndexOf(DialColAns[aa][0])][DialPosAns[aa]] / 10;
                numA = DialVals[aa]["ROYGCBMP".IndexOf(DialColAns[aa][0])][DialPosAns[aa]] % 10;
            }
            numB++;
            Debug.LogFormat("[Colorful Dials #{0}] Number A: {1}", moduleId, numA);
            Debug.LogFormat("[Colorful Dials #{0}] Number B: {1}", moduleId, numB - 1);
            Debug.LogFormat("[Colorful Dials #{0}] Color: {1}", moduleId, color.ToUpper());

            row = -1;
            col = -1;
            for (int bb = 0; bb < colorGrid.Length; bb++)
            {
                for (int cc = 0; cc < colorGrid[bb].Length; cc++)
                {
                    if (colorGrid[bb][cc][1] == color[0] && (colorGrid[bb][cc][0] - '0') == numA)
                    {
                        row = bb;
                        col = cc;
                        break;
                    }
                }
                if (row >= 0)
                    break;
            }
            switch (cardinal)
            {
                case "N":
                    row -= numB;
                    break;
                case "NE":
                    row -= numB;
                    col += numB;
                    break;
                case "E":
                    col += numB;
                    break;
                case "SE":
                    row += numB;
                    col += numB;
                    break;
                case "S":
                    row += numB;
                    break;
                case "SW":
                    row += numB;
                    col -= numB;
                    break;
                case "W":
                    col -= numB;
                    break;
                case "NW":
                    row -= numB;
                    col -= numB;
                    break;
            }
            row = mod(row, 8);
            col = mod(col, 10);
            DialColAns[aa] = dialNumCL["ROYGCBMP".IndexOf(colorGrid[row][col][1])];
            DialPosAns[aa] = colorGrid[row][col][0] - '0';
            Debug.LogFormat("[Colorful Dials #{0}] Final dial #{1} configuration: {2}", moduleId, aa + 1, DialColAns[aa] + " " + DialPosAns[aa]);
        }
        int[] nums = new int[3];
        for (int aa = 0; aa < 3; aa++)
        {
            string color = DialValColors[aa]["ROYGCBMP".IndexOf(DialColAns[aa][0])][DialPosAns[aa]].ToUpper();
            int num = DialVals[aa]["ROYGCBMP".IndexOf(DialColAns[aa][0])][DialPosAns[aa]];
            int nl = num / 10;
            int nr = num % 10;
            switch (color)
            {
                case "RED":
                    nums[aa] = (nl * nl) + (nr * nr);
                    Debug.LogFormat("[Colorful Dials #{0}] Red {1}: {2} + {3} = {4}", moduleId, num, nl * nl, nr * nr, nums[aa]);
                    break;
                case "ORANGE":
                    nums[aa] = (nl - nr) * (nl - nr);
                    Debug.LogFormat("[Colorful Dials #{0}] Orange {1}: ({2} - {3})squared = {4}", moduleId, num, nl, nr, nums[aa]);
                    break;
                case "YELLOW":
                    nums[aa] = (nr - nl) * (nr - nl);
                    Debug.LogFormat("[Colorful Dials #{0}] Yellow {1}: ({2} - {3})squared = {4}", moduleId, num, nr, nl, nums[aa]);
                    break;
                case "GREEN":
                    nums[aa] = nr * nl;
                    Debug.LogFormat("[Colorful Dials #{0}] Green {1}: {2} * {3} = {4}", moduleId, num, nl, nr, nums[aa]);
                    break;
                case "CYAN":
                    nums[aa] = num / (nl + 1);
                    Debug.LogFormat("[Colorful Dials #{0}] Cyan {1}: {2} / ({3} + 1) = {4}", moduleId, num, num, nl, nums[aa]);
                    break;
                case "BLUE":
                    nums[aa] = num / (nr + 1);
                    Debug.LogFormat("[Colorful Dials #{0}] Blue {1}: {2} / ({3} + 1) = {4}", moduleId, num, num, nr, nums[aa]);
                    break;
                case "MAGENTA":
                    nums[aa] = (num * num) % (nl + 1);
                    Debug.LogFormat("[Colorful Dials #{0}] Magenta {1}: {2} % ({3} + 1) = {4}", moduleId, num, num * num, nl, nums[aa]);
                    break;
                case "PINK":
                    nums[aa] = (num * num) % (nr + 1);
                    Debug.LogFormat("[Colorful Dials #{0}] Magenta {1}: {2} % ({3} + 1) = {4}", moduleId, num, num * num, nr, nums[aa]);
                    break;
            }
        }
        submitTime = nums[0] + nums[1] + nums[2];
        Debug.LogFormat("[Colorful Dials #{0}] {1} + {2} + {3} = {4}", moduleId, nums[0], nums[1], nums[2], submitTime);
        while (submitTime > 9)
        {
            string conv = submitTime + "";
            submitTime = 0;
            for (int aa = 0; aa < conv.Length; aa++)
                submitTime += (conv[aa] - '0');
            Debug.LogFormat("[Colorful Dials #{0}] {1} => {2}", moduleId, conv, submitTime);
        }
        Debug.LogFormat("[Colorful Dials #{0}] Submit when the last digit is a {1}", moduleId, submitTime);
    }
    int mod(int n, int m)
    {
        while (n < 0)
            n += m;
        return (n % m);
    }
    void toggleColor(string c)
    {
        if (!(moduleSolved))
        {
            audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            toggledColor = c.ToUpper();
            if (toggledColor.Equals("BLACK"))
            {
                for (int aa = 0; aa < 3; aa++)
                {
                    while (DialPos[aa] != 0)
                    {
                        DialPos[aa] = (DialPos[aa] + 1) % 10;
                        dials[aa].transform.Rotate(Vector3.up, 36f);
                    }
                    dialColors[aa].material = colors[8];
                    dialColorNames[aa] = clist[8].ToUpper();
                    dialScreens[aa].text = "";
                }
            }
        }
    }
    void Turn(int d)
    {
        if (!(moduleSolved))
        {
            if (!(dialColorNames[d].Equals("BLACK")))
            {
                audio.PlaySoundAtTransform(sounds[2].name, transform);
                DialPos[d] = (DialPos[d] + 1) % 10;
                dials[d].transform.Rotate(Vector3.up, 36f);
                getScreen(d, dialColorNames[d][0]);
            }
            else if (!(toggledColor.Equals("BLACK")))
            {
                for (int aa = 0; aa < clist.Length; aa++)
                {
                    if (toggledColor.Equals(clist[aa]))
                    {
                        audio.PlaySoundAtTransform(sounds[3].name, transform);
                        dialColorNames[d] = clist[aa].ToUpper();
                        dialColors[d].material = buttonColors[aa].material;
                        getScreen(d, dialColorNames[d][0]);
                        break;
                    }
                }
            }
        }
    }
    void getScreen(int d, char c)
    {
        dialScreens[d].text = DialVals[d]["ROYGCBMP".IndexOf(c)][DialPos[d]].ToString().PadLeft(2, '0');
        dialScreens[d].color = DialValMat[d]["ROYGCBMP".IndexOf(c)][DialPos[d]];
    }
    void onSubmit()
    {
        if (!moduleSolved)
        {
            submit.AddInteractionPunch();
            audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            if ((int) (bomb.GetTime() % 10) == submitTime)
            {
                bool flag = true;
                for (int aa = 0; aa < 3; aa++)
                {
                    if (DialPosAns[aa] != DialPos[aa])
                    {
                        flag = false;
                        break;
                    }
                    if (!(DialColAns[aa].Equals(dialColorNames[aa])))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    audio.PlaySoundAtTransform(sounds[1].name, transform);
                    Debug.LogFormat("[Colorful Dials {0}] Module solved. Everybody boogie!", moduleId);
                    mainScreen[0].color = Color.white;
                    mainScreen[1].color = Color.white;
                    mainScreen[2].color = Color.white;
                    mainScreen[0].text = "W";
                    mainScreen[1].text = "I";
                    mainScreen[2].text = "N";
                    for (int aa = 0; aa < 3; aa++)
                    {
                        while (DialPos[aa] != 0)
                        {
                            DialPos[aa] = (DialPos[aa] + 1) % 10;
                            dials[aa].transform.Rotate(Vector3.up, 36f);
                        }
                        dialColors[aa].material = colors[8];
                        dialScreens[aa].text = "";
                    }
                    moduleSolved = true;
                    module.HandlePass();
                }
                else
                {
                    audio.PlaySoundAtTransform(sounds[0].name, transform);
                    module.HandleStrike();
                    Debug.LogFormat("[Colorful Dials {0}] Strike! You tried to submit {1} {2} {3} at {4} seconds", moduleId, dialColorNames[0][0] + "" + DialPos[0], dialColorNames[1][0] + "" + DialPos[1], dialColorNames[2][0] + "" + DialPos[2], (int) (bomb.GetTime() % 10));
                }
            }
            else
            {
                audio.PlaySoundAtTransform(sounds[0].name, transform);
                module.HandleStrike();
                Debug.LogFormat("[Colorful Dials {0}] Strike! You tried to submit {1} {2} {3} at {4} seconds", moduleId, dialColorNames[0][0] + "" + DialPos[0], dialColorNames[1][0] + "" + DialPos[1], dialColorNames[2][0] + "" + DialPos[2], (int) (bomb.GetTime() % 10));
            }
        }
    }
#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} set B9 M7 R1 sets the 1st, 2nd, and 3rd dials to Blue 9, Magenta 7, Red 1, !{0} submit 5 presses the submit button when the countdown timer's last digit is a 5.";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] param = command.ToUpper().Split(' ');
        if (Regex.IsMatch(param[0], @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) && param.Length == 2)
        {
            if ("0123456789".IndexOf(param[1]) >= 0 && param[1].Length == 1)
            {
                yield return null;
                int timepress = "0123456789".IndexOf(param[1]);
                while (((int) (bomb.GetTime())) % 10 != timepress)
                    yield return "trycancel The button was not pressed due to a request to cancel.";
                submit.OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
        }
        if (Regex.IsMatch(param[0], @"^\s*set\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) && param.Length == 4)
        {
            bool flag = true;
            for (int aa = 1; aa < param.Length; aa++)
            {
                if (param[aa].Length != 2)
                {
                    flag = false;
                    break;
                }
                if ("ROYGCBMP".IndexOf(param[aa][0]) < 0 || "0123456789".IndexOf(param[aa][1]) < 0)
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                colorButtons[8].OnInteract();
                yield return new WaitForSeconds(0.1f);
                for (int aa = 1; aa < 4; aa++)
                {
                    for (int bb = 0; bb < clist.Length; bb++)
                    {
                        if (clist[bb][0] == param[aa][0])
                        {
                            colorButtons[bb].OnInteract();
                            yield return new WaitForSeconds(0.1f);
                            break;
                        }
                    }
                    while (DialPos[aa - 1] != (param[aa][1] - '0'))
                    {
                        dials[aa - 1].OnInteract();
                        yield return new WaitForSeconds(0.1f);
                    }
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
        yield break;
    }
}
