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
    //[SerializeField] private InventoryUI inventoryUI;

    private void Awake()
    {
    }
    private void Start() {
<<<<<<< HEAD
        inventory = new Inventory();
        inventoryUI.SetInventory(inventory);
        /*
        ItemWorld.SpawnBread(new Vector3(10, 0.5f), new Item { itemType = Item.ItemType.HealthPotion, amount = 1 });
        ItemWorld.SpawnHealthPotion(new Vector3(15, 0.5f), new Item { itemType = Item.ItemType.StaminaPotion, amount = 1 });
        ItemWorld.SpawnStaminaPotion(new Vector3(20, 0.5f), new Item { itemType = Item.ItemType.Bread, amount = 1 });*/

=======
      
>>>>>>> MoEl-Branch
    }


    public void BindUnit(Unit unit) {
        this.unit = unit;
        unit.FinitePropertyChanged += Unit_FinitePropertyChanged;
    }

    public void SubscribeInventory(Inventory inventory)
    {
        inventory.onItemAdded += Inventory_onItemAdded;
        inventory.onItemRemoved += Inventory_onItemRemoved;
    }

    private void Inventory_onItemRemoved(ItemPicked obj)
    {
        throw new System.NotImplementedException();
    }

    private void Inventory_onItemAdded(ItemPicked obj)
    {
        throw new System.NotImplementedException();
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


    public void ConsumeHealth()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //inventory.RemoveItem();
        }
    }
    
}
