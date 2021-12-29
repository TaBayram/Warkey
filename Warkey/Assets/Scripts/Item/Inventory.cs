using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory
{

	private List<ItemPicked> itemList;

	public event System.Action<ItemPicked> onItemAdded;
	public event System.Action<ItemPicked> onItemRemoved;
	public Inventory()
    {
		itemList = new List<ItemPicked>();
	}
	public void AddItem(ItemPicked item)
    {
		itemList.Add(item);
		onItemAdded?.Invoke(item);
    }

	public void RemoveItem(ItemPicked item)
    {
		itemList.Remove(item);
		onItemRemoved?.Invoke(item);
	}

	public List<ItemPicked> GetItemList()
    {
		return new List<ItemPicked>(itemList);
    }

	public bool Contains(ItemPicked item)
    {
		if (itemList.Contains(item))
			return true;
		return false;
    }

	public int TypeCount(IItem.type type)
    {
		int count = 0;
		for(int i = 0; i < itemList.Count; i++)
        {
			if (itemList[i].Type == type)
				count++;
        }

		return count;
    }
}
