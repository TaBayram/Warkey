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

    private bool hasBinded = false;

    private void Start() {
        if (onlyForTest) {
            BindUnit(onlyForTest.GetComponent<Unit>());
            SubscribeInventory(onlyForTest.GetComponentInChildren<ItemPicker>().Inventory);
        }

        experienceBar.SetMaxValue(PlayerTracker.levelExperienceCost);
        BindListeners();
    }

    private void BindListeners() {
        if (hasBinded) return;
        var localPlayer = GameTracker.Instance.GetLocalPlayerTracker();
        if (localPlayer == null) Invoke(nameof(BindListeners), 2);
        localPlayer.onLevelUp += HUDPlayerContainer_onLevelUp;
        localPlayer.onExperienceChange += HUDPlayerContainer_onExperienceChange;
        localPlayer.onHeroChanged += LocalPlayer_onHeroChanged;

        levelText.text = "" + localPlayer.Level;
        experienceBar.SetValue(localPlayer.Experience);
    }

    private void LocalPlayer_onHeroChanged(GameObject obj) {
        BindUnit(obj.GetComponent<Unit>());
        SubscribeInventory(obj.GetComponentInChildren<ItemPicker>().Inventory);
    }

    private void HUDPlayerContainer_onExperienceChange(PlayerTracker obj) {
        experienceBar.SetValue(obj.Experience);
    }

    private void HUDPlayerContainer_onLevelUp(PlayerTracker obj) {
        levelText.text = ""+ obj.Level;
    }

    public void BindUnit(Unit unit) {
        unit.FinitePropertyChanged += Unit_FinitePropertyChanged;
        SetHealth(unit.Health);
        SetStamina(unit.Stamina);
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

    private void OnDestroy() {
        var localPlayer = GameTracker.Instance.GetLocalPlayerTracker();
        localPlayer.onLevelUp -= HUDPlayerContainer_onLevelUp;
        localPlayer.onExperienceChange -= HUDPlayerContainer_onExperienceChange;
        localPlayer.onHeroChanged -= LocalPlayer_onHeroChanged;
    }

}
