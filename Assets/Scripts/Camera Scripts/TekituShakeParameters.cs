using UnityEngine;

//Data type for making camera shake parameters.
public struct TekituShakeParameters{
    public float duration;
    public float roughness;
    public Vector3 dir;
    public float shakeStrength;
    public float fadeIn;
    public float fadeOut;
    public TekituShakeParameters(float dur, Vector3 direction, float rough = 0, float shakeStr = 1, float fadeInTime = 0, float fadeOutTime = 0){
        duration = dur;
        roughness = rough;
        dir = direction;
        shakeStrength = shakeStr;
		fadeIn = fadeInTime;
		fadeOut = fadeOutTime;
    }
}