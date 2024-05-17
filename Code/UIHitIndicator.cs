using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHitIndicator : MonoBehaviour {
    [SerializeField] private float radius = 100.0f;
    [SerializeField] private float fadeInDuration = 0.1f;
    [SerializeField] private float fadeOutDuration = 1.0f;
    private RectTransform indicatorRectTransform;
    private Image indicatorImage;
    private float fadeOutTimer = 0f;
    private bool isFadingOut = false;

    private void Start() {
        indicatorRectTransform = GetComponent<RectTransform>();
        indicatorImage = GetComponent<Image>();

        SetOpacity(0f);
    }

    public void SetDirection(Vector3 hitDirection) {
        Vector3 cameraForward = Camera.main.transform.forward;
        float angle = Vector3.SignedAngle(cameraForward, hitDirection, Camera.main.transform.up);

        float radians = angle * Mathf.Deg2Rad;

        float x = Mathf.Cos(radians) * radius;
        float y = Mathf.Sin(radians) * radius;

        indicatorRectTransform.anchoredPosition = new Vector2(x, y);

        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, hitDirection);
        indicatorRectTransform.rotation = rotation;

        SetOpacity(1f);

        fadeOutTimer = 0f;
        isFadingOut = true;
    }

    private void Update() {
        if (isFadingOut) {
            fadeOutTimer += Time.deltaTime;

            float fadeOutOpacity = 1f - (fadeOutTimer / fadeOutDuration);

            fadeOutOpacity = Mathf.Clamp01(fadeOutOpacity);

            SetOpacity(fadeOutOpacity);

            if (fadeOutOpacity <= 0f) {
                isFadingOut = false;
            }
        }
    }

    private void SetOpacity(float opacity) {
        Color color = indicatorImage.color;
        color.a = opacity;
        indicatorImage.color = color;
    }
}