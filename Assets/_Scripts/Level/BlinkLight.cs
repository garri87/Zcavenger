using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlinkLight : MonoBehaviour
{
    public bool randomActivation;
    public int probability;
    public Light[] targetLights;
    public float blinkRate;
    private bool power = false;
    public bool randomBlink;
    public float minBlinkRate;
    public float maxBlinkRate;
    private Coroutine blinkCoroutine;

    private void Start()
    {
        if (randomActivation)
        {
            int randomNum = Random.Range(0, probability);

            if (randomNum == probability-1)
            {
                this.enabled = true;
                randomBlink = true;
            }
            else
            {
                this.enabled = false;
            }
        }
    }

    void Update()
    {
        if (randomBlink)
        {
            blinkRate = Random.Range(minBlinkRate, maxBlinkRate);
        }
        if (blinkCoroutine == null)
        {
            blinkCoroutine = StartCoroutine(BlinkCycle());
        }
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
