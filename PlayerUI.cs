using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private Stats playerStats;
    [SerializeField]
    private Canvas PlayerCanvas;
    [SerializeField]
    private RectTransform healthBar, staminaBar, powerBar;

    private void Start() {
        UpdateHealthBar();
        UpdateStaminaBar();
        UpdatePowerBar();
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
