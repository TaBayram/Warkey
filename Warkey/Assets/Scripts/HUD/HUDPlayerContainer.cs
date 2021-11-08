using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDPlayerContainer : MonoBehaviour
{

    public int maxHealth = 100;
    public int currentHealth;
    public HUDBar healthBar;

    public int maxStamina = 100;
    public int currentStamina;
    public HUDBar staminaBar;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxValue(maxHealth);

        currentStamina = maxStamina;
        staminaBar.SetMaxValue(maxStamina);

    }
    private void Update()
    {
        //Example
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetHealth(10);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetStamina(10);
        }
    }

    void SetHealth(float damage)
    {
        currentHealth -= (int)damage;
        healthBar.SetValue(currentHealth);
    }

    void SetStamina(float damage)
    {
        currentStamina -= (int)damage;
        staminaBar.SetValue(currentStamina);
    }
}
