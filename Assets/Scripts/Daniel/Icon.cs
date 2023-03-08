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
    
    [SerializeField] private int baseMultiplier;
    [SerializeField] private int dropChance;
    
    public int GetMultiplier()
    {
        return baseMultiplier;
    }
    
    public int GetDropChance()
    {
        return dropChance;
    }
}
