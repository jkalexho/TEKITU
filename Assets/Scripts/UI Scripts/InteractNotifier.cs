using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractNotifier : MonoBehaviour
{

    private Oscillator osci;

    private FadeImageScript imageFader;

    private Image image;

    private bool active;

    void Awake()
    {
        imageFader = this.GetComponent<FadeImageScript>();
        if (imageFader == null)
        {
            Debug.LogError(gameObject.ToString() + ": No Fade Image Script found!");
        }
        osci = this.GetComponent<Oscillator>();
        if (osci == null)
        {
            Debug.LogError(gameObject.ToString() + ": No oscillator script found!");
        }
        image = this.GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError(gameObject.ToString() + ": No Image script found!");
        }
    }
    // Use this for initialization
    void Start()
    {
        TooltipManager.SetInteractNotifier(this);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (active)
        {
            float c = 0.5f + (osci.SinValue + 1) * 0.25f;
            image.color = new Color(c, c, c, image.color.a);
        }
    }

    public void FadeIn()
    {
        imageFader.FadeIn();
        active = true;
    }

    public void FadeOut()
    {
        imageFader.FadeOut();
        active = false;
    }
}
