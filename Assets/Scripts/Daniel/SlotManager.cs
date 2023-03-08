using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class SlotManager : MonoBehaviour
{
    [SerializeField] Icon[] icons;
    [SerializeField] Icon emptyIcon;
    
    private List<GameObject> spawnedIcons = new List<GameObject>();

    private Random rnd;

    Icon[,] slots = new Icon[6, 5];

    void Start()
    {
        rnd = new Random();
        for (int i = 0; i < slots.GetLength(0); i++)
        {
            for (int j = 0; j < slots.GetLength(1); j++)
            {
                slots[i, j] = emptyIcon;
            }
        }
        
        DrawScreen();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) RollSlot();
    }

    private void RollSlot()
    {
        for (int i = 0; i < slots.GetLength(0); i++)
        {
            for (int j = 0; j < slots.GetLength(1); j++)
            {
                slots[i, j] = GetRandomIcon();
            }
        }
        
        DrawScreen();
    }

    private void DrawScreen()
    {
        foreach (var spawnedIcon in spawnedIcons)
        {
            Destroy(spawnedIcon);
        }
        
        spawnedIcons.Clear();
        
        for (int i = 0; i < slots.GetLength(0); i++)
        {
            for (int j = 0; j < slots.GetLength(1); j++)
            {
                print(slots[i,j].name);
                var spawnIcon = Instantiate(slots[i,j].gameObject, 
                    new Vector3(i - slots.GetLength(0) / 2, j - slots.GetLength(1) / 2, 0), 
                    Quaternion.identity);

                spawnedIcons.Add(spawnIcon);
            }
        }
    }
    
    private Icon GetRandomIcon()
    {
        var rndValue = rnd.Next(0, GetMaxChance() + 1);

        //print("rndValue: " + rndValue + " maxChance: " + GetMaxChance() + "");
        
        var currentChance = 0;
        Icon retrunIcon = emptyIcon;
        
        foreach (var icon in icons)
        {
            if (rndValue >= currentChance && rndValue <= currentChance + icon.GetDropChance())
            {
                //print(icon.name);
                retrunIcon = icon;
            }
            
            currentChance += icon.GetDropChance();
        }
        
        return retrunIcon;
    }

    private int GetMaxChance()
    {
        return icons.Sum(icon => icon.GetDropChance());
        
        /* Det samme som den her
        var returnValue = 0;
        
        foreach (var icon in icons)
        {
            returnValue += icon.GetDropChance();
        }

        return returnValue;
        */
    }
}
