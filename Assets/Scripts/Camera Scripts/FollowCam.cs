using System.Collections;
using UnityEngine;

public class FollowCam : MonoBehaviour
{

    public float lerpSpeed = 7.5f;
    public float maxSpeed = 5f;
    public Vector2 desiredPos;
    
    void Start()
    {
        GameManager.SetFollowCam(this);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 targetPosition = Vector2.Lerp(transform.position, desiredPos, lerpSpeed * Time.deltaTime);
        targetPosition = Vector2.MoveTowards(targetPosition, desiredPos, Time.deltaTime);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime * maxSpeed);
    }
    
}
