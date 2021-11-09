using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

//[RequireComponent(typeof(WeaponController))]
public class Unit : MonoBehaviour,IWidget
{
    public UnitData unitData;
    public FiniteField health;
    public FiniteField stamina;



    private void Start() {
        if (unitData) {
            health = new FiniteField(unitData.health, unitData.healthRegen);
            stamina = new FiniteField(unitData.stamina, unitData.staminaRegen);
        }

        
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    public void Die() {
        this.gameObject.SetActive(false);
        Debug.Log(gameObject.name + "Ded");
        Destroy(gameObject);
    }

    public void Destroy() {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(float damage) {
        Debug.Log( gameObject.name + "took damage" +damage);
        health.Current -= damage;

        if (health.Current <= 0) {
            Die();
        }
    }

    void Update() {

    }
}


public struct FiniteField
{
    private float max;
    private float current;
    private float regen;

    public float Current { get => current; set => current = Mathf.Min(value,max); }
    public float Regen { get => regen; set => regen = value; }
    public float Max { get => max; set { max = value; current = Mathf.Min(current, max); }  }

    /**
     * Use this if you want to change current based on new value
     */
    public void SetMax(float value) {
        float increase = (value)/ max - 1;
        max = value;
        current += current * increase;
    }

    public FiniteField(float max, float regen = 0) {
        this.max = max;
        this.current = max;
        this.regen = regen;
    }
}
