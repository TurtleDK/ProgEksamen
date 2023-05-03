using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Icon : MonoBehaviour
{
    public enum Icons
    {
        Apple,
        Banana,
        Cherry,
        Grape,
        Lemon,
        Orange,
        Pear,
        Pineapple,
        Strawberry,
        Watermelon,
        Empty
    }

    [SerializeField] private Icons iconType;

    [SerializeField] private float baseMultiplier;
    [SerializeField] private int dropChance;

    public Vector2 location;
    
    private bool movedDown = false;

    private float timer = 0;
    
    public float GetMultiplier()
    {
        return baseMultiplier;
    }
    
    public int GetDropChance()
    {
        return dropChance;
    }

    public Icons GetIconType()
    {
        return iconType;
    }
}
