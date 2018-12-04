using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Intro1Shot8 : CinematicShot {

    [SerializeField]
    private FadeImageScript body1;
    [SerializeField]
    private FadeImageScript body2;
    [SerializeField]
    private RectTransform rightHand;
    [SerializeField]
    private RectTransform leftHand;
    [SerializeField]
    private FadeImageScript whiteout;
    [SerializeField]
    private RectTransform shiny;

    // Use this for initialization
    void Start () {
        StartCoroutine("Play");
        this.GetComponent<FadeCanvasScript>().FadeIn();
    }

    IEnumerator Play()
    {
        yield return new WaitForSeconds(0.3f);
        body1.FadeOut(0.4f);
        body2.FadeIn(0.25f);
        rightHand.GetComponent<FadeImageScript>().FadeIn(0.25f);
        leftHand.GetComponent<FadeImageScript>().FadeIn(0.25f);
        yield return new WaitForSeconds(1f);
        float progress = 0;
        Vector2 rightHandOriginalPos = rightHand.anchoredPosition;
        Vector2 leftHandOriginalPos = leftHand.anchoredPosition;
        Vector2 rightHandTargetPos = rightHandOriginalPos + new Vector2(-100, 0);
        Vector2 leftHandTargetPos = leftHandOriginalPos + new Vector2(100, 0);
        shiny.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        StartCoroutine("Whiteout");
        while (progress < 1)
        {
            yield return null;
            progress = Mathf.Clamp01(progress + Time.deltaTime * 3);
            rightHand.anchoredPosition = Vector2.Lerp(rightHandOriginalPos, rightHandTargetPos, progress);
            leftHand.anchoredPosition = Vector2.Lerp(leftHandOriginalPos, leftHandTargetPos, progress);
            shiny.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(shiny.localRotation.z, 135, progress));
            shiny.localScale = Vector3.Lerp(shiny.localScale, new Vector3(1.25f, 1.25f, 1), progress);
        }
        
    }
    

    IEnumerator Whiteout()
    {
        yield return new WaitForSeconds(0.15f);
        whiteout.FadeIn(0.25f);
        yield return new WaitForSeconds(1);
        Done = true;
    }
}
