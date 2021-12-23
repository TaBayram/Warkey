using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    //public GameObject pfItemWorld;
    
    public Sprite healthPotionSprite;
    public Sprite staminaPotionSprite;
    public Sprite BreadSprite;
    
    public GameObject healthPotionObject;
    public GameObject staminaPotionObject;
    public GameObject BreadObject;

}
