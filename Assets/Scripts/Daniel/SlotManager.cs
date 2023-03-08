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
    
    private List<GameObject> spawnedIcons = new ();

    private Random rnd;

    Icon[,] slots = new Icon[6, 5];

    void Start()
    {
        rnd = new Random();

        RollSlot(true);

        DrawScreen();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) RollSlot(true);

        if (Input.GetKeyDown(KeyCode.Space)) GetAllIcons();
    }

    private void RollSlot(bool reRoll)
    {
        for (int i = 0; i < slots.GetLength(0); i++)
        {
            for (int j = 0; j < slots.GetLength(1); j++)
            {
                if (reRoll)
                {
                    slots[i, j] = GetRandomIcon();
                    continue;
                }
                
                if (slots[i, j].GetIconType() == Icon.Icons.Empty) slots[i, j] = GetRandomIcon();
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

    private void GetAllIcons()
    {
        var types = new List<Icon>();
        
        foreach (var icon in icons)
        {
            if (!types.Contains(icon)) types.Add(icon);
        }

        foreach (var type in types)
        {
            var amount = slots.Cast<Icon>().Count(icon => icon.GetIconType() == type.GetIconType());

            /* Samme som den over
            var amount = 0;
            
            foreach (var icon in slots)
            {
                if (icon.GetIconType() == type.GetIconType()) amount++;
            }
            */

            if (amount < 8) continue;
    
            foreach (var icon in spawnedIcons.Where(
                         icon => icon.GetComponent<Icon>().GetIconType() == type.GetIconType()).ToList())
            {
                spawnedIcons.Remove(icon);
                Destroy(icon);
            }

            for (int i = 0; i < slots.GetLength(0); i++)
            {
                for (int j = 0; i < slots.GetLength(1); i++)
                {
                    if (slots[i, j].GetIconType() == type.GetIconType()) slots[i, j] = emptyIcon;
                }
            }
        }

        RollSlot(false);
    }
    
    private Icon GetRandomIcon()
    {
        var rndValue = rnd.Next(0, GetMaxChance() + 1);

        //print("rndValue: " + rndValue + " maxChance: " + GetMaxChance() + "");
        
        var currentChance = 0;
        var returnIcon = emptyIcon;
        
        foreach (var icon in icons)
        {
            if (rndValue >= currentChance && rndValue <= currentChance + icon.GetDropChance())
            {
                //print(icon.name);
                returnIcon = icon;
            }
            
            currentChance += icon.GetDropChance();
        }
        
        return returnIcon;
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
