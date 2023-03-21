using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

public class SlotManager : MonoBehaviour
{
    //Hvis ikonet allerede er spawnet, så skal den ikke spanwne det igen
    //
    
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

        if (Input.GetKeyDown(KeyCode.Space)) StartCoroutine(GetAllIcons());

        if (Input.GetKeyDown(KeyCode.L)) RollSlot(false);

        if (Input.GetKeyDown(KeyCode.J)) MoveIconsDown();
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

                if (slots[i, j].GetIconType() == emptyIcon.GetIconType())
                {
                    slots[i, j] = GetRandomIcon();
                    print("Virkede");
                }
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
                var spawnIcon = Instantiate(slots[i,j].gameObject, 
                    new Vector3(i - slots.GetLength(0) / 2, j - slots.GetLength(1) / 2, 0), 
                    Quaternion.identity);

                spawnIcon.GetComponent<Icon>().location = new Vector2(i, j);

                spawnedIcons.Add(spawnIcon);
            }
        }
    }

    private IEnumerator GetAllIcons()
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

            foreach (var spawnedIcon in spawnedIcons.Where(
                         icon => icon.GetComponent<Icon>().GetIconType() == type.GetIconType()).ToList())
            {
                spawnedIcons.Remove(spawnedIcon);
                var location = spawnedIcon.GetComponent<Icon>().location;
                slots[(int) location.x, (int) location.y] = emptyIcon;
                Destroy(spawnedIcon);
            }

            DrawScreen();
        }

        yield return null;
    }

    private void MoveIconsDown()
    {
        foreach (var spawnedIcon in spawnedIcons)
        {
            var icon = spawnedIcon.GetComponent<Icon>();
            print("wtf");

            if (icon.GetIconType() != Icon.Icons.Empty) continue;
                
            print("Fundet");

            var location = icon.location;
            
            while (IsThereEmpty((int) location.x)) 
                MoveRowDown((int) location.x, (int) location.y);
        }
        
        DrawScreen();
    }

    private void MoveRowDown(int row, int startPoint)
    {
        for (int i = startPoint; i < slots.GetLength(1) - 1; i++)
        {
            slots[row, i] = slots[row, i + 1];
            slots[row, i].location = new Vector2(row, i);
            slots[row, i + 1] = emptyIcon;
        }
    }

    private bool IsThereEmpty(int row)
    {
        for (int i = 0; i < slots.GetLength(1) - 1; i++)
        {
            if (slots[row, i].GetIconType() == Icon.Icons.Empty &&
                slots[row, i + 1].GetIconType() != Icon.Icons.Empty) return true;
        }
        return false;
    }

    private Icon GetRandomIcon()
    {
        var rndValue = rnd.Next(0, GetMaxChance() + 1);

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
