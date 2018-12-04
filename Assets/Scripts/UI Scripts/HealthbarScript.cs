using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarScript : MonoBehaviour {

    public int health = 5;
    public float edge = 0;

    [SerializeField]
    private int maximumEdge = 3;

    private int[] edgeLevels;

    private int displayHealth = 5;
    private float displayEdge = 3;

    private float displayHealthX;
    private float displayEdgeX;

    private GameObject healthbar;
    private GameObject edgebar;
    [SerializeField]
    private Image[] edgebarGlowSegments;

    // private bool fullEdge = true;

    private float defaultFullHealthX;
    private float defaultFullEdgeX;

    private float edgeLength = 246;

    void Awake()
    {
        healthbar = GameObject.FindWithTag("healthbar");
        if (healthbar == null)
        {
            Debug.LogError(gameObject.ToString() + ": No healthbar found!");
        }
        defaultFullHealthX = healthbar.transform.localPosition.x;
        edgebar = GameObject.FindWithTag("edgebar");
        if (edgebar == null)
        {
            Debug.LogError(gameObject.ToString() + ": No edgebar found!");
        }
        defaultFullEdgeX = edgebar.transform.localPosition.x;
        GameManager.SetHealthBarScript(this);
    }

	// Use this for initialization
	void Start () {
        foreach (Image r in edgebarGlowSegments)
        {
            r.enabled = false;
        }
        
	}

    public void SetEdgeLevels(int[] levels)
    {
        edgeLevels = levels;
        maximumEdge = levels[levels.Length - 1];
    }
	
	// Update is called once per frame
	void Update () {
		if (displayHealth != health)
        {
            displayHealth = health;
            StopCoroutine("MoveHealth");
            StartCoroutine("MoveHealth");
        }
        if (displayEdge != edge)
        {
            StopCoroutine("MoveEdge");
            StartCoroutine("MoveEdge");
            displayEdge = edge;
        }
        for (int i = 0; i < edgeLevels.Length; i++)
        {
            if (edgeLevels[i] <= displayEdge)
            {
                edgebarGlowSegments[i].enabled = true;
            } else
            {
                edgebarGlowSegments[i].enabled = false;
            }
        }
    }
    
    IEnumerator MoveHealth()
    {
        float progress = 0;
        Vector3 target = new Vector3(defaultFullHealthX - (5 - displayHealth) * 61, healthbar.transform.localPosition.y, 0);
        Vector3 start = healthbar.transform.localPosition;
        while (progress < 1)
        {
            yield return null;
            progress += 2 * Time.deltaTime;
            healthbar.transform.localPosition = Vector3.Lerp(start, target, progress);
        }
    }

    IEnumerator MoveEdge()
    {
        float distance = Mathf.Abs(edge - displayEdge);
        float progress = 0;
        Vector3 target = new Vector3(defaultFullEdgeX - (maximumEdge - edge) * (edgeLength / maximumEdge), edgebar.transform.localPosition.y, 0);
        Vector3 start = edgebar.transform.localPosition;
        while (progress < 1)
        {
            yield return null;
            progress = Mathf.Clamp01(progress + Time.deltaTime / distance * 4);
            edgebar.transform.localPosition = Vector3.Lerp(start, target, progress);
        }
    }
}
