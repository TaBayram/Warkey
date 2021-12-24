using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private Image icon;

    public void UpdateInfo(Sprite weaponIcon)
    {
        icon.sprite = weaponIcon;
    }
}
