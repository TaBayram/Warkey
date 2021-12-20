using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Unit : MonoBehaviour,IWidget
{
    public const float regenInterval = 0.25f;

    [SerializeField] protected UnitData unitData;
    [SerializeField] protected Disabler disabler;
    [SerializeField] protected WidgetAudio widgetAudio;
    protected FiniteField health;
    protected FiniteField stamina;

    public IWidget.State state = IWidget.State.alive;

    public event PropertyChangedEventHandler FinitePropertyChanged;
    public event System.Action<float> onDamageTaken;

    

    protected void Start() {
        if (unitData) {
            health = new FiniteField(unitData.health, unitData.healthRegen);
            stamina = new FiniteField(unitData.stamina, unitData.staminaRegen);

            health.PropertyChanged += Health_PropertyChanged;
            stamina.PropertyChanged += Stamina_PropertyChanged;

            InvokeRepeating("RegenerateFields", 0.0f, regenInterval);
        }
    }

    private void Stamina_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        OnPropertyChanged(nameof(stamina),(FiniteField) sender);
    }
    private void Health_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        OnPropertyChanged(nameof(health),(FiniteField) sender);
    }
    protected void OnPropertyChanged(string name,FiniteField field) {
        FinitePropertyChanged?.Invoke(field, new PropertyChangedEventArgs(name));
    }

    public void Die() {
        state = IWidget.State.dead;
        disabler?.DisableComponents(0);
        disabler?.RemoveComponents(0);
    }

    public void Destroy() {
        Destroy(gameObject);
    }

    public virtual void TakeDamage(float damage) {
        onDamageTaken?.Invoke(damage);
        health.Current -= damage;
        if (health.Current <= 0) {
            Die();
        }
    }

    void Update() {

    }

    void RegenerateFields() {
        health.Current += health.Regen*regenInterval;
        stamina.Current += stamina.Regen*regenInterval;
    }

}


public class FiniteField
{
    private float max;
    private float current;
    private float regen;

    public float Current { get => current; set { current = Mathf.Min(value, max); OnPropertyChanged(); } }
    public float Regen { get => regen; set => regen = value; }
    public float Max { get => max; set { max = value; current = Mathf.Min(current, max); OnPropertyChanged(); }  }

    public event PropertyChangedEventHandler PropertyChanged;

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
    

    private void OnPropertyChanged(string name = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
