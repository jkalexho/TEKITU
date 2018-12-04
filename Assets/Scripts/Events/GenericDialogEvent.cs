using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericDialogEvent : SpecialEvent
{
    private bool ready;
    private bool displaying;

    public string[] lines;

    private List<DialogManager.Dialog> dialog;

    void Awake()
    {
        dialog = new List<DialogManager.Dialog>();
        int character = 0;
        string text;
        foreach (string s in lines)
        {
            for (int i = 1; i < Characters.Name.Length; i++)
            {
                if (s.IndexOf(Characters.Name[i]) == 0)
                {
                    character = i;
                    break;
                }
            }
            text = s.Substring(Characters.Name[character].Length + 2);
            DialogManager.Dialog d;
            d.character = character;
            d.dialog = text;
            dialog.Add(d);
        }
    }

    public override void Activate()
    {
        Done = true; //set done to be true so that current encounter ends
        StartCoroutine("Ready");
    }

    IEnumerator Ready()
    {
        // wait for current encounter to end
        while (GameManager.currentEncounter != null)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1);
        ready = true;
        TooltipManager.DisplayInteractNotifier();
        GameManager.pc.EnableAttack = false;
        GameManager.pc.speechBubble.SetBool("on", true);
        // check for player to start dialog
        while (GameManager.currentEncounter == null)
        {
            yield return null;
            bool inputAttack = Input.GetButtonDown("Attack");
            if (inputAttack)
            {
                ready = false;
                displaying = true;
                GameManager.pc.EnableMovement = false;
                GameManager.pc.speechBubble.SetBool("on", false);
                TooltipManager.HideInteractNotifier();
                DialogManager.dm.LoadConversation(dialog);
                DialogManager.dm.Activate();
                break;
            }
        }
        if (ready) // if the player started another encounter
        {
            GameManager.pc.EnableAttack = true;
            GameManager.pc.speechBubble.SetBool("on", false);
            TooltipManager.HideInteractNotifier();
        } else
        {
            yield return new WaitForSeconds(0.5f);
            // start dialog
            while (displaying)
            {
                bool inputAttack = Input.GetButtonDown("Attack");
                if (inputAttack)
                {
                    displaying = !DialogManager.dm.Next();
                }
                yield return null;
            }
            // finish dialog
            yield return new WaitForSeconds(0.25f);
            GameManager.pc.EnableAttack = true;
            GameManager.pc.EnableMovement = true;
            DialogManager.dm.Deactivate();
        }
    }
}
