using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private Stats playerStats;
    [SerializeField]
    private Canvas PlayerCanvas;
    [SerializeField]
    private RectTransform healthBar, staminaBar, powerBar;
    [SerializeField]
    RectTransform healthBG, stamBG, powerBG;
    [SerializeField]
    private UIHitIndicator hitIndicator;

    private Color darkerColor = new Color(0.2f, 0.2f, 0.2f);

    /*
    private void Start() {
        playerStats = Camera.main.GetComponentInParent<FPSController>().getStats();

        UpdateHealthBar();
        UpdateStaminaBar();
        UpdatePowerBar();
        DrawAllUIBarsBackground();
    }
    */
    public UIHitIndicator GetHitIndicator() {
        return hitIndicator;
    }

    public void InitializeBars(Stats stats) {
        playerStats = stats;
        UpdateHealthBar();
        UpdateStaminaBar();
        UpdatePowerBar();
        DrawAllUIBarsBackground();
    }

    private void DrawAllUIBarsBackground() {

        //Debug.Log(healthBG.sizeDelta);
        //Debug.Log(playerStats.getCurrentHealth() + " " + playerStats.getMaxTotalHealth());

        float healthRatio = playerStats.getCurrentHealth() / playerStats.getMaxTotalHealth();
        float staminaRatio = playerStats.getCurrentStamina() / playerStats.getMaxTotalStamina();
        float powerRatio = playerStats.getCurrentPower() / playerStats.getMaxTotalPower();

        healthBG.sizeDelta = new Vector2(healthRatio * 100.0f, healthBar.sizeDelta.y);
        stamBG.sizeDelta = new Vector2(staminaRatio * 100.0f, staminaBar.sizeDelta.y);
        powerBG.sizeDelta = new Vector2(powerRatio * 100.0f, powerBar.sizeDelta.y);

        Image healthBGImage = healthBG.GetComponent<Image>();
        Image stamBGImage = stamBG.GetComponent<Image>();
        Image powerBGImage = powerBG.GetComponent<Image>();

        healthBGImage.color *= darkerColor;
        stamBGImage.color *= darkerColor;
        powerBGImage.color *= darkerColor;

        //Debug.Log(healthBG.sizeDelta);
    }

    public void UpdateHealthBar() {
        float healthRatio = playerStats.getCurrentHealth() / playerStats.getMaxTotalHealth();
        healthBar.sizeDelta = new Vector2(healthRatio * 100f, healthBar.sizeDelta.y);
    }

    public void UpdateStaminaBar() {
        float staminaRatio = playerStats.getCurrentStamina() / playerStats.getMaxTotalStamina();
        staminaBar.sizeDelta = new Vector2(staminaRatio * 100.0f, staminaBar.sizeDelta.y);
    }

    public void UpdatePowerBar() {
        float powerRatio = playerStats.getCurrentPower() / playerStats.getMaxTotalPower();
        powerBar.sizeDelta = new Vector2(powerRatio * 100.0f, powerBar.sizeDelta.y);
    }

    public void UpdateAllUIBars() {
        //Debug.Log("Update All UI Bars");
        UpdateHealthBar();
        UpdateStaminaBar();
        UpdatePowerBar();
    }
}
