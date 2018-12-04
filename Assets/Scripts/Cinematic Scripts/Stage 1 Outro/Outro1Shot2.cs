using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outro1Shot2 : CinematicShot {

    [SerializeField]
    private FadeCanvasScript shot2;
    [SerializeField]
    private FadeCanvasScript shot3;
    [SerializeField]
    private FadeCanvasScript shot4;
    [SerializeField]
    private FadeCanvasScript shot5;
    [SerializeField]
    private FadeCanvasScript shot6;
    [SerializeField]
    private List<FadeImageScript> kuris;
    [SerializeField]
    private List<FadeImageScript> mom;
    [SerializeField]
    private RectTransform shot3Image;

    private FadeCanvasScript fader;



    private bool next = false;

    protected override void Awake()
    {
        base.Awake();
        fader = this.GetComponent<FadeCanvasScript>();
    }

    void Start()
    {
        StartCoroutine("Play");
        StartCoroutine("CheckNext");
    }
    

    IEnumerator CheckNext()
    {
        while (true)
        {
            if (Input.GetButtonDown("Attack"))
            {
                next = DialogManager.dm.Next();
                
            }
            yield return null;
        }
    }

    IEnumerator WaitForNext()
    {
        next = false;
        while (true)
        {
            yield return null;
            if (next)
            {
                next = false;
                break;
            }
        }
    }

    IEnumerator Play()
    {
        #region dialogs
        DialogManager.Dialog d1;
        d1.character = Characters.Kurogiri;
        d1.dialog = "Hmph. Nothing but dust on my blade.";

        DialogManager.Dialog d2;
        d2.character = Characters.Kurogiri;
        d2.dialog = "But it would seem that the one controlling these creatures is still-";

        DialogManager.Dialog d3;
        d3.character = Characters.Mom;
        d3.dialog = "Kuris!";

        DialogManager.Dialog d4;
        d4.character = Characters.Kuris;
        d4.dialog = "Hm?";

        DialogManager.Dialog d5;
        d5.character = Characters.Mom;
        d5.dialog = "You've made a mess out of your room again!";

        DialogManager.Dialog d6;
        d6.character = Characters.Mom;
        d6.dialog = "How many times have I told you to stop playing around in the morning?";

        DialogManager.Dialog d7;
        d7.character = Characters.Kuris;
        d7.dialog = "I wasn't playing around, Mom. I was fighting Creatures of Crimson.";

        DialogManager.Dialog d8;
        d8.character = Characters.Mom;
        d8.dialog = "Yeah, yeah, mister Shadow Hunter. Hurry up and get dressed for school.";

        DialogManager.Dialog d9;
        d9.character = Characters.Kuris;
        d9.dialog = "School? A Shadow Hunter has no need for such trivial things.";

        DialogManager.Dialog d10;
        d10.character = Characters.Mom;
        d10.dialog = "If you don't go to school, I'm not cooking you omelette rice tonight.";

        DialogManager.Dialog d11;
        d11.character = Characters.Kuris;
        d11.dialog = "W-Well, sometimes even minor duties are required of a Shadow Hunter.";

        DialogManager.Dialog d12;
        d12.character = Characters.Mom;
        d12.dialog = "(Oh, I do worry about him sometimes...)";
        #endregion

        fader.FadeIn(0.4f);
        yield return new WaitForSeconds(1);
        
        DialogManager.dm.LoadConversation(new List<DialogManager.Dialog> { d1, d2 });
        DialogManager.dm.Activate();

        yield return StartCoroutine("WaitForNext");
        
        DialogManager.dm.LoadConversation(new List<DialogManager.Dialog> { d3 });
        yield return null;
        DialogManager.dm.Next();
        
        shot2.FadeIn();

        yield return StartCoroutine("WaitForNext");
        
        DialogManager.dm.LoadConversation(new List<DialogManager.Dialog> { d4 });
        yield return null;
        DialogManager.dm.Next();
        
        shot3.FadeIn();

        yield return StartCoroutine("WaitForNext");
        float progress = 0;
        Vector3 originalPosition = shot3Image.anchoredPosition;
        Vector3 targetPosition = originalPosition + new Vector3(280, 0, 0);
        while (progress < 1)
        {
            yield return null;
            progress = Mathf.Clamp01(progress + Time.deltaTime);
            shot3Image.localScale = Vector3.Lerp(new Vector3(1.1f, 1.1f, 1), new Vector3(1, 1, 1), progress);
            shot3Image.anchoredPosition = Vector3.Lerp(originalPosition, targetPosition, progress);
        }

        yield return StartCoroutine("WaitForNext");
        
        DialogManager.dm.LoadConversation(new List<DialogManager.Dialog> { d5 });
        yield return null;
        DialogManager.dm.Next();
        
        shot4.FadeIn();

        yield return StartCoroutine("WaitForNext");
        DialogManager.dm.LoadConversation(new List<DialogManager.Dialog> { d6, d7, d8 });
        yield return null;
        DialogManager.dm.Next();
        
        shot5.FadeIn();

        yield return StartCoroutine("WaitForNext");
        DialogManager.dm.LoadConversation(new List<DialogManager.Dialog> { d9 });
        yield return null;
        
        DialogManager.dm.Next();
        kuris[0].FadeOut(0.4f);
        kuris[1].FadeIn(0.25f);

        yield return StartCoroutine("WaitForNext");
        DialogManager.dm.LoadConversation(new List<DialogManager.Dialog> { d10 });
        yield return null;
        
        DialogManager.dm.Next();
        mom[0].FadeOut(0.4f);
        mom[1].FadeIn(0.25f);
        yield return new WaitForSeconds(0.5f);
        kuris[1].FadeOut(0.4f);
        kuris[2].FadeIn(0.25f);

        yield return StartCoroutine("WaitForNext");
        DialogManager.dm.LoadConversation(new List<DialogManager.Dialog> { d11, d12 });
        yield return null;
        
        DialogManager.dm.Next();
        kuris[2].FadeOut(0.4f);
        kuris[1].FadeIn(0.25f);
        yield return new WaitForSeconds(0.5f);
        mom[1].FadeOut(0.4f);
        mom[2].FadeIn(0.25f);

        yield return StartCoroutine("WaitForNext");
        DialogManager.dm.Deactivate();
        shot6.FadeIn(3);
        yield return new WaitForSeconds(3);
        yield return StartCoroutine("WaitForNext");
        Done = true;
    }
}
