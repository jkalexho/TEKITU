using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Intro1Shot1 : CinematicShot {

    [SerializeField]
    private FadeImageScript blackCover;
    [SerializeField]
    private RectTransform kurogiri;
    [SerializeField]
    private RectTransform closeBuildings;
    [SerializeField]
    private RectTransform middleBuildings;
    [SerializeField]
    private RectTransform farBuildings;
    [SerializeField]
    private float duration;
    [SerializeField]
    private FadeTextScript presents;
    [SerializeField]
    private FadeTextScript tekitu;

	// Use this for initialization
	void Start () {
        StartCoroutine("Play");
        StartCoroutine("DisplayText");
	}

    IEnumerator DisplayText()
    {
        yield return new WaitForSeconds(4);
        presents.FadeIn();
        yield return new WaitForSeconds(4);
        presents.FadeOut();
        yield return new WaitForSeconds(2.5f);
        tekitu.FadeIn();
        yield return new WaitForSeconds(6);
        tekitu.FadeOut();
        yield return new WaitForSeconds(duration - 17.5f - 0.5f);
        Done = true;
    }
	
    IEnumerator Play()
    {
        blackCover.FadeOut();
        float progress = 0;
        Vector2 farBuildingsStart = farBuildings.anchoredPosition;
        Vector2 middleBuildingsStart = middleBuildings.anchoredPosition;
        Vector2 closeBuildingsStart = closeBuildings.anchoredPosition;
        Vector2 kurogiriStart = kurogiri.anchoredPosition;

        Vector2 farBuildingsStop = farBuildingsStart - new Vector2(0, 50);
        Vector2 middleBuildingsStop = middleBuildingsStart - new Vector2(0, 100);
        Vector2 closeBuildingsStop = closeBuildingsStart - new Vector2(0, 200);
        Vector2 kurogiriStop = kurogiriStart - new Vector2(0, 700);
        while (progress < 1)
        {
            yield return null;
            progress += Time.deltaTime / duration;
            farBuildings.anchoredPosition = Vector2.Lerp(farBuildingsStart, farBuildingsStop, progress);
            middleBuildings.anchoredPosition = Vector2.Lerp(middleBuildingsStart, middleBuildingsStop, progress);
            closeBuildings.anchoredPosition = Vector2.Lerp(closeBuildingsStart, closeBuildingsStop, progress);
            if (progress > 0.5f)
            {
                kurogiri.anchoredPosition = Vector2.Lerp(kurogiriStart, kurogiriStop, (progress - 0.5f) * 2);
            }
        }
    }
}
