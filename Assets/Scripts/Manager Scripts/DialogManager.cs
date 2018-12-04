using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour {

    public struct Dialog
    {
        public int character;
        public string dialog;
    }

    private List<Dialog> dialogs;

    public static DialogManager dm;

    public PortraitScript portrait;

    public DialogScript dialog;

    private TypewriterScript typewriter;

    public FadeCanvasScript dialogCanvasFader;

    public FadeCanvasScript healthbarCanvasFader;

    private int currentLine = 0;

    void Awake()
    {
        if (dm == null)
        {
            dm = this;
        } 
        dialogs = new List<Dialog>();
    }

    public void Activate()
    {
        portrait.SetCharacter(dialogs[currentLine].character);
        dialogCanvasFader.FadeIn();
        healthbarCanvasFader.FadeOut();
        StartCoroutine("WaitAndStart");
    }

    public void Deactivate()
    {
        healthbarCanvasFader.FadeIn();
        dialogCanvasFader.FadeOut();
    }

    private IEnumerator WaitAndStart()
    {
        typewriter.Clear();
        yield return new WaitForSeconds(0.5f);
        Next();
    }
    
    public void LoadConversation(List<Dialog> d)
    {
        dialogs.AddRange(d);
        foreach (Dialog b in d)
        {
            typewriter.LoadLine(b.dialog);
        }
    }

    public bool Next()
    {
        bool done = typewriter.Next();
        currentLine = typewriter.GetCurrentLine();
        int currentChar = dialogs[currentLine].character;
        portrait.SetCharacter(currentChar);
        dialog.SetCharacter(currentChar);
        return done;
    }
    
    public void SetPortraitScript(PortraitScript p)
    {
        portrait = p;
    }

    public void SetDialogScript(DialogScript d)
    {
        dialog = d;
    }

    public void SetTypewriterScript(TypewriterScript t)
    {
        typewriter = t;
    }
}
