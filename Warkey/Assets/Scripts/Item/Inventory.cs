using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Inventory
{
	private List<ItemPicked> itemList;

	public event System.Action<ItemPicked> onItemAdded;
	public event System.Action<ItemPicked> onItemRemoved;
	public Inventory(){
		itemList = new List<ItemPicked>();
	}
	public void AddItem(ItemPicked item){
		itemList.Add(item);
		onItemAdded?.Invoke(item);
    }

	public void RemoveItem(ItemPicked item){
		itemList.Remove(item);
		onItemRemoved?.Invoke(item);
	}

	public List<ItemPicked> GetItemList(){
		return new List<ItemPicked>(itemList);
    }

	public bool Contains(ItemPicked item){
		return (itemList.Contains(item));
    }

	public int TypeCount(IItem.type type){
		int count = 0;
		for(int i = 0; i < itemList.Count; i++)
        {
			if (itemList[i].Type == type)
				count++;
        }
		return count;
    }

	public ItemPicked GetFirstItemByType(IItem.type type) {
		for (int i = 0; i < itemList.Count; i++) {
			if (itemList[i].Type == type)
				return itemList[i];
		}
		return null;
	}

    internal void UseItem(int v,Entity entity) {
		var item = GetFirstItemByType((IItem.type)v);
		if (item == null) return;
        if (item.Use(entity)) {
			RemoveItem(item);
        }
    }
}
