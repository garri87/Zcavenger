using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkLight : MonoBehaviour
{
    public Light[] targetLights;
    public float blinkRate;
    private bool power = false;
    public bool randomBlink;
    public float minBlinkRate;
    public float maxBlinkRate;
    void Update()
    {
        if (randomBlink)
        {
            blinkRate = Random.Range(minBlinkRate, maxBlinkRate);
        }
        StartCoroutine(BlinkCycle());
    }

    IEnumerator BlinkCycle()
    {
        foreach (Light light in targetLights)
        {
            if (power == false)
            {
                power = true;
                light.enabled = false;
                yield return new WaitForSeconds(blinkRate);
                light.enabled = true;
                yield return new WaitForSeconds(blinkRate);
                power = false;
            }   
        }
    }
}
