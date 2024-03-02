using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlickerEffect : MonoBehaviour
{
    public float intensityModifier = 0.1f;

    private Light lightComponent;
    private float baseIntensity;
    private float targetIntensity;

    [SerializeField]
    private float intensityAdjustSpeed = 1.0f;

    private void Start() {
        lightComponent = GetComponent<Light>();
        baseIntensity = lightComponent.intensity;
        targetIntensity = baseIntensity;
    }

    private void FixedUpdate() {
        targetIntensity = baseIntensity + Random.Range(-intensityModifier, intensityModifier);
        targetIntensity = Mathf.Clamp(targetIntensity, 0f, Mathf.Infinity);
        lightComponent.intensity = Mathf.Lerp(lightComponent.intensity, targetIntensity, Time.deltaTime * intensityAdjustSpeed);
    }
}
