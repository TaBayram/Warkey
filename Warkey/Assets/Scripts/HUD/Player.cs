using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

    public GameObject player;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);

        player.GetComponent<Unit>().PropertyChanged += A_PropertyChanged;


    }
    private void A_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (sender.GetType() != typeof(Unit)) return;
        Unit unit = sender as Unit;
        if (e.PropertyName == nameof(unit.health)) {
            HealthChange(unit.health.Current);
        }
        else if (e.PropertyName == nameof(unit.stamina)) {
            StaminaChange(unit.stamina.Current);
        }
        

    }

    private void Update()
    {
        //Example
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            HealthChange(20);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            StaminaChange(10);
        }
    }

    void HealthChange(float damage)
    {
        currentHealth =(int) damage;
        healthBar.SetHealth(currentHealth);
    }

    void StaminaChange(float damage)
    {
        currentStamina = (int)damage;
        staminaBar.SetStamina(currentStamina);
    }
}
