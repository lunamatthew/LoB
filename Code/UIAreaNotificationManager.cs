using System.Collections;
using System.Collections.Generic;
//using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class UIAreaNotificationManager : MonoBehaviour
{
    private Canvas canvas;
    [SerializeField]
    private TextMeshProUGUI headerText;
    [SerializeField]
    private TextMeshProUGUI subText;
    [SerializeField]
    private GameObject tmpGO;


    private void Start() {
        canvas = GetComponent<Canvas>();
        //tmpObj = canvas.GetComponentInChildren<TextMeshProUGUI>();
        //FadeNotificationOverTime(0f, 4f, -1f, 3f, 1f);
    }

    public void ChangeText(string headerText, string subText) {
        this.headerText.text = headerText;
        this.subText.text = subText;
    }

    public void FadeNotificationOverTime(float targetVal, float time, float fadeTargetVal, float fadeTime, float fadeDelay) {
        tmpGO.SetActive(true);
        StartCoroutine(SmoothFaceDilate(targetVal, time));
        StartCoroutine(SmoothFaceDilate(fadeTargetVal, fadeTime, time + fadeDelay));
        StartCoroutine(Utility.GameObjectToggle(tmpGO, false, time + fadeDelay + fadeTime)); ;
    }

    private IEnumerator SmoothFaceDilate(float targetValue, float time, float delay = 0f) {
        yield return new WaitForSeconds(delay);

        float elapsedTime = 0f;
        float headerStartValueFace = headerText.fontMaterial.GetFloat(ShaderUtilities.ID_FaceDilate);
        float headerStartValueUnderlay = headerText.fontMaterial.GetFloat(ShaderUtilities.ID_UnderlayDilate);

        while (elapsedTime < time) {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / time);
            float currentValueF = Mathf.Lerp(headerStartValueFace, targetValue, t);
            float currentValueU = Mathf.Lerp(headerStartValueUnderlay, targetValue, t);

            headerText.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, currentValueF);
            headerText.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, currentValueU);

            subText.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, currentValueF);
            subText.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, currentValueU);

            yield return null;
        }

        // Ensure the target value is set at the end of the coroutine
        headerText.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, targetValue);
        headerText.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, targetValue);

        subText.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, targetValue);
        subText.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, targetValue);
    }

}
