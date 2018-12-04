using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthbarScript : MonoBehaviour {

    
    private int maxHitPoints;

    [SerializeField]
    private GenericEnemyScript boss;

    [SerializeField]
    private RectTransform healthbar;

    private FadeCanvasScript canvasFader;

    private float baseWidth;

    // Use this for initialization
    void Awake () {
        canvasFader = this.GetComponent<FadeCanvasScript>();
        // baseWidth = healthbar.rect.width;
        maxHitPoints = boss.healthPoint;
	}
	
	// Update is called once per frame
	void Update () {
        float size = (float) boss.healthPoint / (float) maxHitPoints;
        healthbar.localScale = new Vector3(size, 1, 1);
	}

    public void Show()
    {
        StartCoroutine("DoShow");
    }

    IEnumerator DoShow()
    {
        yield return new WaitForSeconds(1);
        canvasFader.FadeIn();
    }

    public void Hide()
    {
        canvasFader.FadeOut();
    }
}
