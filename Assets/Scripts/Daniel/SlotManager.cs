using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using Random = System.Random;

public class SlotManager : MonoBehaviour
{
    //Hvis ikonet allerede er spawnet, så skal den ikke spanwne det igen
    //
    
    [SerializeField] Icon[] icons;
    [SerializeField] Icon emptyIcon;
    
    private List<GameObject> spawnedIcons = new ();
    
    private List<Coroutine> runningCoroutine = new ();

    private Random rnd;

    private int amount;
    private int amountMax;

    [SerializeField] private Icon[] allIcons;

    private bool stopWiggle;
    private bool canRoll = true;
    private bool canSkip = false;
    
    private bool firstRun = true;

    Icon[,] slots = new Icon[6, 5];

    void Start()
    {
        rnd = new Random();

        RollSlot();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canRoll)
            {
                RollSlot();
            }
            else if (canSkip)
            {
                stopWiggle = true;
                canSkip = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            var EV = 0f;
            foreach (var icon in allIcons)
            {
                EV += ((float)icon.GetDropChance() / 100) * icon.GetMultiplier() * 8;
            }
            
            print("EV: " + EV);
        }
    }

    private void RollSlot(bool reRoll = true)
    {
        canRoll = false;
        
        for (int i = 0; i < slots.GetLength(0); i++)
        {
            for (int j = 0; j < slots.GetLength(1); j++)
            {
                if (reRoll)
                {
                    slots[i, j] = GetRandomIcon();
                    continue;
                }

                if (slots[i, j].GetIconType() == emptyIcon.GetIconType()) slots[i, j] = GetRandomIcon();
            }
        }
        
        StartCoroutine(DrawScreen());
    }

    private IEnumerator DrawScreen(bool reDraw = false)
    {
        foreach (var spawnedIcon in spawnedIcons)
        {
            Destroy(spawnedIcon);
        }
        
        print("Wtf");

        spawnedIcons.Clear();
        
        for (int i = 0; i < slots.GetLength(0); i++)
        {
            for (int j = 0; j < slots.GetLength(1); j++)
            {
                var spawnIcon = Instantiate(slots[i,j].gameObject, 
                    new Vector3(i - (float) slots.GetLength(0) / 2, j - (float) slots.GetLength(1) / 2, 0), 
                    Quaternion.identity);

                spawnIcon.GetComponent<Icon>().location = new Vector2(i, j);

                spawnedIcons.Add(spawnIcon);
            }
        }

        if (firstRun)
        {
            firstRun = false;
            canRoll = true;
            yield break;
        }
        
        switch (reDraw)
        {
            case true:
                yield return new WaitForSeconds(0.5f);
                RollSlot(false);
                break;
            
            case false:
                GetAllIcons();
                break;
        }
    }

    private void GetAllIcons()
    {
        var winIcons = new List<GameObject>();
        var types = new List<Icon>();
        var connections = 0;
        
        foreach (var icon in icons)
        {
            if (!types.Contains(icon)) types.Add(icon);
        }
        
        foreach (var type in types)
        {
            var typeAmount = slots.Cast<Icon>().Count(icon => icon.GetIconType() == type.GetIconType());

            /* Samme som den over
            var amount = 0;
            
            foreach (var icon in slots)
            {
                if (icon.GetIconType() == type.GetIconType()) amount++;
            }
            */

            if (typeAmount < 8) continue;

            amountMax += typeAmount;
            connections++;
            
            foreach (var spawnedIcon in spawnedIcons.Where
                     (icon => icon.GetComponent<Icon>().GetIconType() == type.GetIconType()).ToList())
            {
                winIcons.Add(spawnedIcon);
            }
        }

        if (connections == 0)
        {
            canRoll = true;
            return;
        }

        StartCoroutine(Wiggle(winIcons));
    }

    /*
    private bool IsThereEmpty(int row)
    {
        for (int i = 0; i < slots.GetLength(1) - 1; i++)
        {
            if (slots[row, i].GetIconType() == Icon.Icons.Empty && slots[row, i + 1].GetIconType() != Icon.Icons.Empty)
            {
                return true;
            }
        }
        return false;
    }
    */
    
    private bool IsThereEmpty(int row)
    {
        for (int i = 0; i < slots.GetLength(1) - 1; i++)
        {
            if (slots[row, i].GetIconType() == Icon.Icons.Empty &&
                slots[row, i + 1].GetIconType() != Icon.Icons.Empty) return true;
        }
        return false;
    }

    private void MoveIconsDown()
    {
        
        /*
        var anyIconsMoved = false; // Add a flag to check if any icons were moved
        
        foreach (var spawnedIcon in spawnedIcons)
        {
            var icon = spawnedIcon.GetComponent<Icon>();

            if (icon.GetIconType() != Icon.Icons.Empty) continue;

            var location = icon.location;

            var catchInt = 0;
            
            while (IsThereEmpty((int) location.x) && catchInt < slots.GetLength(1))
            {
                anyIconsMoved = true; // Set the flag to true if you're moving any icons
                MoveRowDown((int) location.x, (int) location.y);

                catchInt++;
            }
        }
        

        // Check if any icons were moved before starting the DropIcon coroutine
        if (!anyIconsMoved) return;

        */

        for (int i = 0; i < slots.GetLength(0); i++)
        {
            var nonEmptyIcons = new List<Icon>();
        
            for (int j = 0; j < slots.GetLength(1); j++)
            {
                if (slots[i, j].GetIconType() != emptyIcon.GetIconType())
                {
                    nonEmptyIcons.Add(slots[i,j]);
                }
            }

            for (int j = 0; j < nonEmptyIcons.Count; j++)
            {
                slots[i, j] = nonEmptyIcons[j];
                slots[i, j].location = new Vector2(i, j);
            }

            for (int j = nonEmptyIcons.Count; j < slots.GetLength(1); j++)
            {
                slots[i, j] = emptyIcon;
            }
        }

        StartCoroutine(DropIcon());

        //StartCoroutine(Wait(false));
        
    }
    
    /*
    private void MoveRowDown(int row, int startPoint)
    {
        for (int i = startPoint; i < slots.GetLength(1) - 1; i++)
        {
            if (slots[row, i].GetIconType() != Icon.Icons.Empty) break;
            slots[row, i] = slots[row, i + 1];
            slots[row, i].location = new Vector2(row, i);
            slots[row, i + 1] = emptyIcon;
        }
    }
    */
    
    /*
    private void MoveIconsDown()
    {
        foreach (var spawnedIcon in spawnedIcons)
        {
            var icon = spawnedIcon.GetComponent<Icon>();

            if (icon.GetIconType() != Icon.Icons.Empty) continue;

            var location = icon.location;
            
            while (IsThereEmpty((int) location.x)) 
                MoveRowDown((int) location.x, (int) location.y);
        }

        foreach (var spawnedIcon in spawnedIcons.Where
                     (icon => icon.transform.position != GetWorldPosition(icon.GetComponent<Icon>().location) &&
                              icon.GetComponent<Icon>().GetIconType() != Icon.Icons.Empty))
        {
            amountMax++;

            StartCoroutine(DropIcon(spawnedIcon));
        }
        
        StartCoroutine(Wait(false));
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

    /*
    private bool IsThereEmpty(int row)
    {
        for (int i = 0; i < slots.GetLength(1) - 1; i++)
        {
            if (slots[row, i].GetIconType() == Icon.Icons.Empty &&
                slots[row, i + 1].GetIconType() != Icon.Icons.Empty) return true;
        }
        return false;
    }
    */

    private Icon GetRandomIcon()
    {
        var rndValue = rnd.Next(0, GetMaxChance() + 1);

        var currentChance = 0;
        var returnIcon = emptyIcon;
        
        foreach (var icon in icons)
        {
            if (rndValue >= currentChance && rndValue <= currentChance + icon.GetDropChance()) returnIcon = icon;

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

    private Vector3 GetWorldPosition(Vector2 location)
    {
        return new Vector3(location.x - (float)slots.GetLength(0) / 2f, location.y - (float)slots.GetLength(1) / 2f);
    }
    
    private IEnumerator Wiggle(List<GameObject> winIcons)
    {
        canSkip = true;
        
        foreach (var icon in winIcons)
        {
            var anim = icon.GetComponent<Animation>();
            anim.Play();
        }

        while (!stopWiggle)
        {
            if (winIcons.Any(icon => icon.GetComponent<Animation>().isPlaying)) yield return null;
            else stopWiggle = true;
        }

        foreach (var icon in winIcons)
        {
            var anim = icon.GetComponent<Animation>();
            anim.Stop();
            
            spawnedIcons.Remove(icon);
            
            var location = icon.GetComponent<Icon>().location;
            slots[(int) location.x, (int) location.y] = emptyIcon;
            SpawnEmpty(icon);
        }

        yield return new WaitForSeconds(0.5f);

        stopWiggle = false;
        
        MoveIconsDown();
    }

    private void SpawnEmpty(GameObject replacedIcon)
    {
        var obj = Instantiate(emptyIcon.gameObject, replacedIcon.transform.position, Quaternion.identity);
        obj.GetComponent<Icon>().location = replacedIcon.GetComponent<Icon>().location;
        spawnedIcons.Add(obj);

        Destroy(replacedIcon);
    }

    private IEnumerator DropIcon()
    {
        /*
        const float step = 2f;

        var counted = false;

        var notFinished = 0;

        
        while (notFinished > 0)
        {
            var icon = spawnedIcons[0].GetComponent<Icon>();
            var worldPos = GetWorldPosition(icon.location);
            var iconPos = icon.gameObject.transform.position;

            print("worldPosition: " + worldPos + " | " + "spawnedIcon.transform.position: " +
                  icon.location);
            
            foreach (var spawnedIcon in spawnedIcons.Where
                     (icon => icon.transform.position != GetWorldPosition(icon.GetComponent<Icon>().location) &&
                              icon.GetComponent<Icon>().GetIconType() != Icon.Icons.Empty))
            {
                if (notFinished == 0) break;
                
                var worldPosition = GetWorldPosition(spawnedIcon.GetComponent<Icon>().location);
                print("worldPosition: " + worldPosition + " | " + "spawnedIcon.transform.position: " + spawnedIcon.transform.position + " | " + "notFinished: " + notFinished);

                spawnedIcon.transform.position = Vector3.MoveTowards( spawnedIcon.transform.position, 
                    worldPosition, 
                    step * Time.deltaTime);
                
                if (Vector2.Distance(spawnedIcon.transform.position, worldPosition) < 0.01f) notFinished--;
            }
            yield return null;
        }
        */

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(DrawScreen(true));
    }

    // Brugte før til at vente på at alle coroutine var færdige men det tog for meget computer kraft
    /*
    private IEnumerator Wait(bool wiggle)
    {
        while (amount < amountMax)
        {
            yield return null;
        }
        
        amount = 0;
        amountMax = 0;
        
        if (wiggle) stopWiggle = false;

        if (wiggle) MoveIconsDown();
        else
        {
            var coroutine = StartCoroutine(DrawScreen(true));
            runningCoroutine.Add(coroutine);
        }
    }
    */
}
