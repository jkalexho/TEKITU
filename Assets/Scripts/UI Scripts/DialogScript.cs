using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogScript : MonoBehaviour {

    [SerializeField]
    private Text nameText;

    [SerializeField]
    private Text dialogText;

    private TypewriterScript typewriter;

    private int currentCharacter = 0;

	// Use this for initialization
	void Start () {
        DialogManager.dm.SetDialogScript(this);
        nameText.text = "";
        dialogText.text = "";
        typewriter = dialogText.GetComponentInChildren<TypewriterScript>();
        if (typewriter == null)
        {
            Debug.LogError(gameObject.ToString() + ": No typewriter script found!");
        }
        DialogManager.dm.SetTypewriterScript(typewriter);
	}
	
    public void SetCharacter(int c)
    {
        if (currentCharacter != c)
        {
            nameText.text = Characters.Name[c];
            nameText.color = Characters.Color[c];
            currentCharacter = c;
        }
    }
}
