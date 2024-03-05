using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class HealthBasedObject
{
    public GameObject objectToActivateOrDeactivate;
    public float activationHealthThreshold;
}

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private List<HealthBasedObject> healthBasedObjects;

    public HealthBar healthBar;
    public GameObject deathScreen;

    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetSliderMax(maxHealth);
    }

    private void Update()
    {
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }

        // Sprawdzenie, czy aktualne zdrowie osi¹gnê³o lub przekroczy³o próg aktywacji/dezaktywacji
        foreach (var healthBasedObject in healthBasedObjects)
        {
            if (currentHealth <= healthBasedObject.activationHealthThreshold && healthBasedObject.objectToActivateOrDeactivate != null)
            {
                healthBasedObject.objectToActivateOrDeactivate.SetActive(true);
            }
            else
            {
                healthBasedObject.objectToActivateOrDeactivate.SetActive(false);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        healthBar.SetSlider(currentHealth);
    }

    public void HealPlayer(float amount)
    {
        currentHealth += amount;
        healthBar.SetSlider(currentHealth);
    }

    private void Die()
    {
        Debug.Log("You died!");
        deathScreen.SetActive(true);
    }
}
