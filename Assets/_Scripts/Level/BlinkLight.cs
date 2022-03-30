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
        
        for (int i = 0; i < targetLights.Length; i++)
        {
            
        
            if (power == false)
            {
                power = true;
                targetLights[i].enabled = false;
                yield return new WaitForSeconds(blinkRate);
                targetLights[i].enabled = true;
                yield return new WaitForSeconds(blinkRate);
                power = false;
            }  
        }
        

    }
}
