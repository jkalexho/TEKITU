using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outro1Shot1 : CinematicShot
{
    [SerializeField]
    private RectTransform whiteout;
    [SerializeField]
    private RectTransform top;
    [SerializeField]
    private RectTransform bot;

    // Use this for initialization
    void Start()
    {
        StartCoroutine("Play");
    }

    IEnumerator Play()
    {
        yield return new WaitForSeconds(1);
        float progress = 0;
        while (progress < 1)
        {
            yield return null;
            progress = Mathf.Clamp01(progress + Time.deltaTime * 6);
            whiteout.localScale = new Vector3(1, (1 - progress), 1);
        }
        progress = 0;
        while (progress < 1)
        {
            yield return null;
            progress = Mathf.Clamp01(progress + Time.deltaTime * 3);
            top.anchoredPosition = Vector3.Lerp(top.anchoredPosition, new Vector3(0, 500, 0), progress);
            bot.anchoredPosition = Vector3.Lerp(bot.anchoredPosition, new Vector3(0, -400, 0), progress);
        }
        yield return new WaitForSeconds(1.5f);
        Done = true;
    }
}
