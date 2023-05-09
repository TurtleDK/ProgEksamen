using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Icon : MonoBehaviour
{
    //Enum for the different icons
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

    //Holds the icon type from the enum Icons
    [SerializeField] private Icons iconType;

    //Icon multiplier if icon hits
    [SerializeField] private float baseMultiplier;
    
    //Drop chance for the icon
    [SerializeField] private int dropChance;

    //Location of the icon in the 2D array
    public Vector2 location;

    //Makes it possible to get the base multiplier from other scripts
    public float GetMultiplier()
    {
        return baseMultiplier;
    }
    
    //Makes it possible to get the drop chance from other scripts
    public int GetDropChance()
    {
        return dropChance;
    }

    //Makes it possible to get the icon type from other scripts
    public Icons GetIconType()
    {
        return iconType;
    }
}
