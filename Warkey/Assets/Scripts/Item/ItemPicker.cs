using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPicker : MonoBehaviour
{
    [SerializeField] LayerMask itemLayer;

    private List<GameObject> items = new List<GameObject>();
    private GameObject closestItem;
    private Inventory inventory = new Inventory();
    public Inventory Inventory { get => inventory; set => inventory = value; }

    [SerializeField] Entity entity;

    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (1 << other.gameObject.layer == itemLayer && other.isTrigger)
        {
            items.Add(other.gameObject);
            if (closestItem == null)
                closestItem = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (1 << other.gameObject.layer == itemLayer && other.isTrigger)
        {
            items.Remove(other.gameObject);
            if (closestItem == other.gameObject)
                closestItem = null;
        }
    }

    private void FindTheClosestItem()
    {
        RemoveDestroyed();
        if (closestItem == null)
        {
            if (items.Count != 0)
            {
                closestItem = items[0];
            }
            else
                return;
        }
        float sqrMag = (closestItem.transform.position - transform.position).sqrMagnitude;
        foreach (GameObject item in items)
        {
            float canditateSqrMag = (item.transform.position - transform.position).sqrMagnitude;
            if (canditateSqrMag < sqrMag)
            {
                closestItem = item;
                sqrMag = canditateSqrMag;
            }
        }
    }

    private void RemoveDestroyed()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items.RemoveAt(i);
                i--;
            }
            else if (items[i].GetComponent<ItemObject>().IsPicked)
            {
                items.RemoveAt(i);
                i--;
            }
        }
    }


    private void Update()
    {
        FindTheClosestItem();
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (closestItem != null && Inventory.TypeCount(closestItem.GetComponent<ItemObject>().Type) == 0)
            {
                inventory.AddItem(closestItem.GetComponent<ItemObject>().PickUp());
                closestItem = null;
            }
        }

        if (entity == null) return;
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            inventory.UseItem(0,entity);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            inventory.UseItem(1, entity);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            inventory.UseItem(2, entity);
        }
    }

}
