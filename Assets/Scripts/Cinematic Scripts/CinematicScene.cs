using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CinematicScene : MonoBehaviour {

    public CinematicShot[] shots;

    public string nextScene;

    private float esc = 0;

	// Use this for initialization
	void Start () {
        shots[0].gameObject.SetActive(true);
        StartCoroutine("Play");
	}

    void Update()
    {
        if (Input.GetButton("Escape"))
        {
            esc = Mathf.Clamp01(esc + Time.deltaTime);
            if (esc >= 1)
            {
                StopAllCoroutines();
                SceneManager.LoadScene(nextScene);
            }
        } else if (esc > 0)
        {
            esc = Mathf.Clamp01(esc - Time.deltaTime * 5);
        }
    }
	
    IEnumerator Play()
    {
        int currentShot = 0;
        while (currentShot < shots.Length)
        {
            if (shots[currentShot].Done)
            {
                currentShot++;
                if (currentShot < shots.Length)
                {
                    shots[currentShot].gameObject.SetActive(true);
                    yield return new WaitForSeconds(2f);
                    shots[currentShot - 1].gameObject.SetActive(false);
                }
            }
            yield return null;
        }
        SceneManager.LoadScene(nextScene);
    }
}
