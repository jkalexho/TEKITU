using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipBackgroundAnimator : MonoBehaviour {

    private RectTransform rectTransform;

    private float currentHeight = 0;

    [SerializeField]
    public float animationDuration = 0.5f;

	// Use this for initialization
	void Awake () {
        rectTransform = gameObject.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError(gameObject.ToString() + ": No rectangle transform found!");
        }
	}

    void Start()
    {
        TooltipManager.SetTooltipBackground(this);
    }
	
	public void Activate()
    {
        StopCoroutine("DoAnimation");
        StartCoroutine("DoAnimation", 600);
    }

    public void Deactivate()
    {
        StopCoroutine("DoAnimation");
        StartCoroutine("DoAnimation", 0);
    }

    private IEnumerator DoAnimation(float destination)
    {
        float progress = 0;
        yield return null;
        while (progress < 1)
        {
            progress += Time.deltaTime / animationDuration;
            float h = Mathf.Lerp(currentHeight, destination, progress);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
            currentHeight = (h + currentHeight) * 0.5f;
            yield return null;  
        }
    }
}
