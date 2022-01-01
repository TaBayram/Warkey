using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Photon.Pun;

public class Unit : MonoBehaviour,IWidget
{
    public const float regenInterval = 0.10f;

    [SerializeField] protected bool isHero;
    [SerializeField] protected UnitData unitData;
    [SerializeField] protected Disabler disabler;
    [SerializeField] protected WidgetAudio widgetAudio;
    protected FiniteField health;
    protected FiniteField stamina;
    protected float armor;
    private bool isBlocking;

    private IWidget.State state = IWidget.State.alive;

    public event PropertyChangedEventHandler FinitePropertyChanged;
    public event System.Action<float> onDamageTaken;
    public event System.Action<IWidget.State> onStateChange;

    private float staminaRegenCooldown = 1f;

    protected PhotonView photonView;

    public IWidget.State State { get => state; set { state = value; onStateChange?.Invoke(value); } }

    public bool IsHero { get => isHero; }
    public FiniteField Health { get => health; }
    public FiniteField Stamina { get => stamina; }
    public float Armor { get => armor; set => armor = value; }
    public bool IsBlocking { get => isBlocking; set => isBlocking = value; }

    protected void Awake() {
        if (unitData) {
            health = new FiniteField(unitData.health, unitData.healthRegen, unitData.healthCooldown);
            stamina = new FiniteField(unitData.stamina, unitData.staminaRegen, unitData.staminaCooldown);

            health.PropertyChanged += Health_PropertyChanged;
            stamina.PropertyChanged += Stamina_PropertyChanged;

            Armor = unitData.armor;
        }
    }

    protected void Start() {
        photonView = GetComponent<PhotonView>();
        InvokeRepeating("RegenerateFields", 0.0f, regenInterval);
        
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
        State = IWidget.State.dead;
        if (isHero) {

        }
        else {
            disabler?.DisableComponents(0);
            disabler?.RemoveComponents(0);
            Invoke(nameof(Destroy),3);
        }
    }

    public void Destroy() {
        PhotonNetwork.Destroy(gameObject);
    }

    public virtual void TakeDamage(float damage) {
        if (IsBlocking && UseStamina(damage)) return;
        damage = damage * (1 - Armor / 100);
        if (damage == 0) return;
        photonView.RPC("RPC_TakeDamage", RpcTarget.All, damage);
        Debug.Log(damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        onDamageTaken?.Invoke(damage);
        health.Current -= damage;
        if (health.Current <= 0)
        {
            Die();
        }
        Debug.Log(damage);
    }

    public void Heal(float heal) {
        health.Current += heal;
    }

    public void RegainStamina(float value) {
        stamina.Current += value;
    }

    public bool UseStamina(float value,float minV = 0) {
        if (minV != 0 && stamina.Current < minV) return false;
        if(stamina.Current >= value) {
            stamina.Current -= value;
            staminaRegenCooldown = stamina.Cooldown;
            return true;
        }
        else {
            return false;
        }
    }

    void Update() {
        if(unitData.canDrown && Physics.Raycast(transform.position, Vector3.up, out RaycastHit raycastHit, 50f, 1 << LayerMask.NameToLayer("Water"))) {
            if(raycastHit.distance > 2) {
                if (Time.time > drownCooldown) {
                    drownCooldown = Time.time + 1f;
                    if (!UseStamina(5)) {
                        TakeDamage(5);
                    }
                }
            }
        }
    }

    float drownCooldown;

    void RegenerateFields() {
        health.Current += health.Regen*regenInterval;
        if (staminaRegenCooldown <= 0)
            stamina.Current += stamina.Regen * regenInterval;
        else
            staminaRegenCooldown -= regenInterval;
    }




}


public class FiniteField
{
    private float max;
    private float current;
    private float regen;
    private float cooldown;

    public float Current { get => current; set { current = Mathf.Min(value, max); OnPropertyChanged(); } }
    public float Regen { get => regen; set => regen = value; }
    public float Max { get => max; set { max = value; current = Mathf.Min(current, max); OnPropertyChanged(); }  }
    public float Cooldown { get => cooldown; set => cooldown = value; }

    public event PropertyChangedEventHandler PropertyChanged;

    /**
     * Use this if you want to change current based on new value
     */
    public void SetMax(float value) {
        float increase = (value)/ max - 1;
        max = value;
        current += current * increase;
    }

    public FiniteField(float max, float regen = 0, float cooldown = 0) {
        this.max = max;
        this.current = max;
        this.regen = regen;
        this.cooldown = cooldown;
    }


    private void OnPropertyChanged(string name = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
