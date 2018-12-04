using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FakeMainMenu : MonoBehaviour {

    [SerializeField]
    private FadeImageScript blackCanvas;

    private bool loading = true;

    private float esc = 0;

	// Use this for initialization
	void Start () {
        StartCoroutine("WaitAndStart");
	}

    void Update ()
    {
        if (Input.GetButtonDown("Attack") && !loading)
        {
            loading = true;
            StartCoroutine("WaitAndLoad");
        }

        if (Input.GetButton("Escape"))
        {
            esc += Time.deltaTime;
            if (esc >= 1)
            {
                Application.Quit();
            }
        } else if (esc > 0)
        {
            esc = 0;
        }
    }

    IEnumerator WaitAndLoad()
    {
        blackCanvas.FadeIn(1);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Stage 1 Intro");
    }
	
    IEnumerator WaitAndStart()
    {
        yield return new WaitForSeconds(1);
        blackCanvas.FadeOut(1);
        yield return new WaitForSeconds(1);
        loading = false;
    }
}
