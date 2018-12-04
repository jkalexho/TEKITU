using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitScript : MonoBehaviour {

    public Image[] portraits;

    private int currentCharacter = 0;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < portraits.Length; i++)
        {
            HidePortrait(i);
        }
        DialogManager.dm.SetPortraitScript(this);
	}

    public void SetCharacter(int c)
    {
        if (currentCharacter != c)
        {
            HidePortrait(currentCharacter);
            ShowPortrait(c);
            currentCharacter = c;
        }
    }
	
	private void HidePortrait(int p)
    {
        portraits[p].color = new Color(1, 1, 1, 0);
    }

    private void ShowPortrait(int p)
    {
        portraits[p].color = new Color(1, 1, 1, 1);
    }
}
