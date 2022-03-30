using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLight : MonoBehaviour
{
    public float rotationSpeed;

    public Transform redLight;

    public Transform blueLight;
    
    // Update is called once per frame
    void Update()
    {
        blueLight.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
        redLight.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
    }
}
