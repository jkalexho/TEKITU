using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VultureEncounter : EnemyEncounter {

    [Header("Vulture Boss")]
    [SerializeField]
    private VultureScript vulture;

    [SerializeField]
    private GameObject chasm;

    [SerializeField]
    private GameObject bridge;

    [SerializeField]
    private Transform cameraZone;

    [SerializeField]
    private BossHealthbarScript healthBar;

    private Vector3 bridgeStart;

    private Vector3 bridgeEnd;

    protected override void Start()
    {
        Active = false;
        Encountered = false;
        chasm.SetActive(false);
        bridgeStart = bridge.transform.position;
        bridgeEnd = bridgeStart - new Vector3(0, 5, 0);
    }

    protected override void Update()
    {
        if (!Encountered)
        {
            if (CheckPlayerEntered())
            {
                StartEncounter();
            }
        }
    }

    public override void Activate()
    {
        Active = true;
        chasm.SetActive(true);
        StartCoroutine("RetractBridge");
        StartCoroutine("Appear");
        cameraZone.position = this.transform.position;
        healthBar.Show();
    }

    public override void Deactivate()
    {
        Active = false;
        chasm.SetActive(false);
        bridge.transform.position = bridgeStart;
        cameraZone.position = this.transform.position + new Vector3(0, 30, 0);
        healthBar.Hide();
    }

    private IEnumerator Appear()
    {
        yield return new WaitForSeconds(1);
        vulture.Activate();
    }

    private IEnumerator RetractBridge()
    {
        float progress = 0;
        while (progress < 1)
        {
            yield return new WaitForFixedUpdate();
            progress = Mathf.Clamp01(progress + Time.fixedDeltaTime);
            bridge.transform.position = Vector3.Lerp(bridgeStart, bridgeEnd, progress);
        }
    }

    public override void Reset()
    {
        Encountered = false;
        vulture.Reset();
        Deactivate();
    }
}
