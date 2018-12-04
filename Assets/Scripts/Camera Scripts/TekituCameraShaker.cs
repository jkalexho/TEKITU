using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class TekituCameraShaker : MonoBehaviour
{

    [Space()]
    #region Camera Shake Settings
    [Header("Default Camera Shake Settings")]
    public float defaultAttackDuration = 0.1f;
    public float defaultDuration = .5f;

    public float defaultRoughness = 4f;
    public Vector3 defaultDirection = Vector3.zero;
    public float defaultShakeStrength = 1;
    public float defaultFadeIn = 0;
    public float defaultFadeOut = 0;
    #endregion

    public static TekituCameraShaker Instance;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    //Shakes the camera in the direction of dir relative to the centre of the screen. This should usually be the direction of attack
    //to simulate the attack momentum shaking the screen. If a general shake is needed, pass dir = Vector3.zero.
    public IEnumerator Shake(TekituShakeParameters  shakeparam)
    {
        //TODO: roughness? waitforseconds(roughness*time.deltatime ???) 
        Vector3 originalPos = transform.localPosition;
        //Normalises vector so we can use it for shake direction.
        if (shakeparam.dir.magnitude > 1)
        {
            shakeparam.dir.Normalize();
        }
        else if (shakeparam.dir.magnitude < 1 && shakeparam.dir.magnitude > 0)
        {
            shakeparam.dir = new Vector3(shakeparam.dir.x / shakeparam.dir.magnitude, shakeparam.dir.y / shakeparam.dir.magnitude, shakeparam.dir.z / shakeparam.dir.magnitude);
        }
        float timeElapsed = 0;

        while (timeElapsed <= shakeparam.duration)
        {
            yield return null;
            // print("shaking!");
            if (shakeparam.dir != Vector3.zero)
            {
                float randNum = Random.Range(0.2f, 1f);
                //We use the same random variable in order to randomise only the position of the shake in the dir vector given.
                float x = randNum * shakeparam.shakeStrength * shakeparam.dir.x;
                float y = randNum * shakeparam.shakeStrength * shakeparam.dir.y;
                transform.localPosition = new Vector3(x, y, originalPos.z);
            }else {
                //Default shake, randomly centred around player
                float x = Random.Range(-1f, 1f) * shakeparam.shakeStrength;
                float y = Random.Range(-1f, 1f) * shakeparam.shakeStrength;
                transform.localPosition = new Vector3(x, y, originalPos.z);
            }

            timeElapsed += Time.deltaTime;
        }

        transform.localPosition = Vector3.zero;
    }

}


