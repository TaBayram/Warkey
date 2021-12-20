using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDPlayerContainer : MonoBehaviour
{

    public HUDBar healthBar;
    public HUDBar staminaBar;
    private Unit unit;
    [SerializeField] private WeaponUI weaponUI;
    private Inventory inventory;
    [SerializeField] private InventoryUI inventoryUI;

    private void Awake()
    {
        inventory = new Inventory();
        inventoryUI.SetInventory(inventory);
    }
    private void Start() {
        
    }

    public void BindUnit(Unit unit) {
        this.unit = unit;
        unit.FinitePropertyChanged += Unit_FinitePropertyChanged;
    }

    private void Unit_FinitePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
        if (e.PropertyName == "health") {
            SetHealth((FiniteField)sender);
        }
        else if (e.PropertyName == "stamina") {
            SetStamina((FiniteField)sender);
        }
    }

    private void Update()
    {
    }

    void SetHealth(FiniteField field)
    {
        healthBar.SetMaxValue(field.Max);
        healthBar.SetValue(field.Current);
    }

    void SetStamina(FiniteField field)
    {
        staminaBar.SetMaxValue(field.Max);
        staminaBar.SetValue(field.Current);
    }

    
    public void updateWeapon(Weapon weapon)
    {
        weaponUI.UpdateInfo(weapon.weaponIcon);
    }
    
}
