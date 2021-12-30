using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HUDPlayerContainer : MonoBehaviour
{

    [SerializeField] private HUDBar healthBar;
    [SerializeField] private HUDBar staminaBar;
    [SerializeField] private HUDBar experienceBar;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private WeaponUI weaponUI;
    [SerializeField] private InventoryUI inventoryUI;


    [SerializeField] private GameObject onlyForTest;

    private void Start() {
        if (onlyForTest) {
            BindUnit(onlyForTest.GetComponent<Unit>());
            SubscribeInventory(onlyForTest.GetComponentInChildren<ItemPicker>().Inventory);
        }

        experienceBar.SetMaxValue(PlayerTracker.levelExperienceCost);
        GameTracker.Instance.GetLocalPlayerTracker().onLevelUp += HUDPlayerContainer_onLevelUp;
        GameTracker.Instance.GetLocalPlayerTracker().onExperienceChange += HUDPlayerContainer_onExperienceChange;


    }

    private void HUDPlayerContainer_onExperienceChange(PlayerTracker obj) {
        experienceBar.SetValue(obj.Experience);
    }

    private void HUDPlayerContainer_onLevelUp(PlayerTracker obj) {
        levelText.text = ""+ obj.Level;
    }

    public void BindUnit(Unit unit) {
        unit.FinitePropertyChanged += Unit_FinitePropertyChanged;
    }

    public void SubscribeInventory(Inventory inventory)
    {
        inventoryUI.SubscribeInventory(inventory);
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
        weaponUI.UpdateInfo(weapon.icon);
    }

}
