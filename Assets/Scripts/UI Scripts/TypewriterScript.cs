using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypewriterScript : MonoBehaviour
{

    #region Editor Variables
    [SerializeField]
    [Tooltip("Use this symbol in your strings to represent a newline. (You can change this symbol)")]
    private string newlineDelimiter = "/n";
    [SerializeField]
    [Tooltip("Use this symbol in your strings to represent a pause in the display. (You can change this symbol)")]
    private string pauseDelimiter = "/p";
    [SerializeField]
    [Tooltip("The length of the dramatic pauses in your dialog.")]
    private float pauseDuration = 0.5f;
    [SerializeField]
    [Tooltip("The amount of characters that will be displayed per second.")]
    private float textSpeed = 12.0f;
    [SerializeField]
    [Tooltip("The maximum number of lines that will be displayed. When a new line is displayed, the oldest one vanishes.")]
    private int maxLinesDisplayed = 2;
    [SerializeField]
    [Tooltip("Choose to display a blinking cursor or not.")]
    private bool includeBlinkingCursor = true;
    [SerializeField]
    [Tooltip("The symbol to use for the cursor.")]
    private string cursorSymbol = "▁";
    [SerializeField]
    [Tooltip("The duration of the cursor blink.")]
    private float cursorBlinkDuration = 1.2f;
    [SerializeField]
    [Tooltip("The sounds to use for the typewriter. Sounds are randomized evenly.")]
    private List<AudioClip> sounds;
    [SerializeField]
    [Tooltip("The frequency at which sounds will be emitted, measured in characters.")]
    private int charactersPerSound = 2;
    [SerializeField]
    [Tooltip("The location at which the sound will be emitted.")]
    public Transform soundLocation;
    #endregion

    #region Private Variables
    // Private Variables
    private List<string> text;

    private int curLine;

    private int curLineProgress;

    private int curLineLength;

    private int progressToNextSound;

    private Text textBox;

    private string prefix;

    private string curLineText;

    private string suffix;

    private int clearPoint;
    #endregion

    #region Awake/Start/Update
    void Start()
    {
        text = new List<string>();
        textBox = this.GetComponent<Text>();
        if (textBox == null)
        {
            Debug.LogError("No text box found!");
        }
        curLine = -1;
        curLineProgress = 0;
        curLineLength = 0;
        progressToNextSound = charactersPerSound - 1;
        clearPoint = -1;
        if (includeBlinkingCursor)
        {
            EnableBlinkingCursor();
        }
        else
        {
            suffix = "";
        }
    }
    #endregion

    #region Public Control Functions
    // this is what happens when the user presses the submit key. The Typewriter will either start writing the next assigned line or immediately finish the line it is currently writing. 
    // Next() returns false if it is not done displaying the line.
    // Next() returns true if there are no more lines to be displayed.
    public bool Next()
    {
        if (curLineProgress == curLineLength) // line finished. advance dialog
        {
            return StartNextLine();
        }
        else // player is impatient and wants to see the entire line
        {
            FinishCurrentLine();
        }
        return false;
    }

    // loads a new line of dialog into the typewriter.
    public void LoadLine(string line)
    {
        text.Add(line);
        FixNewlines();
    }

    // bulk loads a bunch of lines of dialog into the typewriter.
    public void LoadLines(List<string> lines)
    {
        text.AddRange(lines);
        FixNewlines();
    }

    // clears everything except the blinking cursor
    public void Clear()
    {
        clearPoint = curLine;
        curLineProgress = curLineLength;
        prefix = "";
        curLineText = "";
        textBox.text = "";
    }

    public void EnableBlinkingCursor()
    {
        suffix = cursorSymbol;
        includeBlinkingCursor = true;
        DisplayText();
        StopCoroutine("Blink");
        StartCoroutine("Blink");
    }

    public void DisableBlinkingCursor()
    {
        suffix = "";
        includeBlinkingCursor = false;
        StopCoroutine("Blink");
        DisplayText();
    }

    public int GetCurrentLine()
    {
        return curLine;
    }
    #endregion
    

    #region Private Helper Functions and Coroutines

    // converts newline delimiters into System.Environment.NewLines
    private void FixNewlines()
    {
        for (int i = 0; i < text.Count; i++)
        {
            text[i] = text[i].Replace(newlineDelimiter, System.Environment.NewLine);
        }
    }

    private void DisplayText()
    {
        textBox.text = prefix + curLineText + suffix;
    }

    // finish writing the current line
    private void FinishCurrentLine()
    {
        StopCoroutine("TypeText");
        curLineProgress = curLineLength;
        curLineText = text[curLine];
        DisplayText();
    }

    private bool StartNextLine()
    {
        curLine++;
        if (curLine >= text.Count)
        {
            curLine--;
            return true;
        }
        curLineProgress = 0;

        curLineLength = text[curLine].Length;

        RefreshPrefix(curLine);
        StartCoroutine("TypeText");
        return false;
    }

    // recalculates the lines that should go in front of the line being passed in
    private void RefreshPrefix(int line)
    {
        prefix = "";
        if (clearPoint >= line || clearPoint > text.Count)
        {
            return;
        }
        for (int i = line - maxLinesDisplayed + 1; i < line; i++)
        {
            if (i > clearPoint)
            {
                prefix += text[i];
            }
        }
    }

    private IEnumerator TypeText()
    {
        int randomSound;
        if (includeBlinkingCursor)
        {
            suffix = cursorSymbol;
        }
        while (curLineProgress < curLineLength)
        {
            // if we encounter a pause delimiter
            if (curLineProgress + pauseDelimiter.Length < curLineLength && text[curLine].Substring(curLineProgress + 1, pauseDelimiter.Length) == pauseDelimiter)
            {
                curLineProgress = curLineProgress + pauseDelimiter.Length + 1;
                yield return new WaitForSeconds(pauseDuration);
                continue;
            }
            curLineText = text[curLine].Substring(0, curLineProgress + 1);
            DisplayText();
            progressToNextSound++;
            if (progressToNextSound >= charactersPerSound && sounds.Capacity > 0)
            {
                randomSound = (int)Mathf.Clamp(Mathf.Floor(Random.value * sounds.Count), 0, sounds.Count - 1);
                AudioSource.PlayClipAtPoint(sounds[randomSound], soundLocation.position);
                progressToNextSound = 0;
            }
            curLineProgress++;
            yield return new WaitForSeconds(1.0f / textSpeed);
        }
        yield return null;
    }

    private IEnumerator Blink()
    {
        bool blinkOn = false;
        while (includeBlinkingCursor)
        {
            if (curLineProgress == 0 || curLineProgress == curLineLength) // blinking time
            {
                if (blinkOn)
                {
                    suffix = "";
                }
                else
                {
                    suffix = cursorSymbol;
                }
                blinkOn = !blinkOn;
                DisplayText();
            }
            yield return new WaitForSeconds(cursorBlinkDuration);
        }
    }
    #endregion
}
