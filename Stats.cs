using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour {
    [SerializeField]
    private float maxTotalHealth, maxTotalStamina, maxTotalPower ;
    [SerializeField]
    private float currentHealth, currentStamina, currentPower;

    /*
    private void Start() {
        maxTotalHealth = Utility.HEALTH_MAX;
        maxTotalStamina = Utility.STAMINA_MAX;
        maxTotalPower = Utility.POWER_MAX;

        currentHealth = maxTotalHealth;
        currentStamina = maxTotalStamina;
        currentPower = maxTotalPower;
    }
    */

    public void InitializeStats() {
        maxTotalHealth = Utility.HEALTH_MAX;
        maxTotalStamina = Utility.STAMINA_MAX;
        maxTotalPower = Utility.POWER_MAX;

        currentHealth = maxTotalHealth;
        currentStamina = maxTotalStamina;
        currentPower = maxTotalPower;
    }

    public float getCurrentHealth() {
        return currentHealth;
    }

    public float getCurrentStamina() {
        return currentStamina;
    }

    public float getCurrentPower() {
        return currentPower;
    }

    public float getMaxTotalHealth() {
        return maxTotalHealth;
    }

    public float getMaxTotalStamina() {
        return maxTotalStamina;
    }

    public float getMaxTotalPower() {
        return maxTotalPower;
    }

    public void AdjustHealth(float amount) {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0.0f, maxTotalHealth);
        //if (currentHealth < 0)
            //currentHealth = 0;
    }

    public void AdjustStamina(float amount) {
        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0.0f, maxTotalStamina);
        //if (currentStamina < 0)
        //currentStamina = 0;
    }

    public void AdjustPower(float amount) {
        currentPower += amount;
        currentPower = Mathf.Clamp(currentPower, 0.0f, maxTotalPower);
        //if (currentPower < 0)
            //currentPower = 0;
    }

    public void TakeDamage(float damageAmount) {
        AdjustHealth(-damageAmount);
    }

    public void TakeStaminaDrain(float reductionAmount) {
        AdjustStamina(-reductionAmount);
    }
}
