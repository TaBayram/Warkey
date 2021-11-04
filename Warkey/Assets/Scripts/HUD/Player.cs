using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    public int maxStamina = 100;
    public int currentStamina;
    public StaminaBar staminaBar;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);

    }
    private void Update()
    {
        //Example
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ReduceHealth(20);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ReduceStamina(10);
        }
    }

    void ReduceHealth(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

    void ReduceStamina(int damage)
    {
        currentStamina -= damage;
        staminaBar.SetStamina(currentStamina);
    }
}
