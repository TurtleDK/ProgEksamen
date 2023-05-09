using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Random = System.Random;

public class SlotManager : MonoBehaviour
{
    //Hvis ikonet allerede er spawnet, så skal den ikke spanwne det igen
    //
    
    [SerializeField] private Icon[] allIcons; //All possible icons except empty
    [SerializeField] Icon emptyIcon; //Icon used to fill the slot if it is empty
    
    private List<GameObject> spawnedIcons = new (); //Holds all the spawned icon GameObjects

    private Random rnd; //Random used to get random icons

    private bool stopWiggle; //Is used to stop the wiggle animation
    private bool canRoll = true; //Used to check if player can roll the slot
    private bool canSkip; //Used to check if player can skip the "Wiggle" animation
    
    private bool firstRun = true; //If the game is started the slot should not calculate any hits

    Icon[,] slots = new Icon[6, 5]; //Holds the icons in a 2D array in the current roll

    void Start()
    {
        rnd = new Random(); //Creates a new random

        RollSlot(); //Rolls the first slot
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) //If player presses space
        {
            if (canRoll) //If the player can roll
            {
                RollSlot(); //Rolls the slot
            }
            else if (canSkip) //if the player can skip the "Wiggle" animation
            {
                stopWiggle = true;
                canSkip = false;
            }
        }

        //Only used for testing and development purposes
        if (Input.GetKeyDown(KeyCode.M)) //Used to calculate the Estimated Value of the slot
        {
            var EV = 0f; //Resets the EV
            foreach (var icon in allIcons) //Loops through all the icons
            {
                EV += ((float)icon.GetDropChance() / 100) * icon.GetMultiplier() * 8; //Calculates the EV for the specific icon and adds it to the total EV
            }
            
            print("EV: " + EV); //Prints EV
        }
    }

    //Rolls the slot
    private void RollSlot(bool reRoll = true) //If the "reRoll" parameter is not set it is then true. This is used to check if the slot should be re-rolled or not
    {
        canRoll = false; //If the player re-rolls the slot they can't roll again until the slot is done rolling
        
        //Loops through the 2D array
        for (int i = 0; i < slots.GetLength(0); i++)
        {
            for (int j = 0; j < slots.GetLength(1); j++)
            {
                //If reRoll is true, then the slot should be re-rolled, which means re-rolling the entire slot 
                if (reRoll)
                {
                    slots[i, j] = GetRandomIcon();
                    continue;
                }

                //If reRoll is false, then should the empty icons be rerolled and filled.
                if (slots[i, j].GetIconType() == emptyIcon.GetIconType()) slots[i, j] = GetRandomIcon();
            }
        }
        
        //Draws the screen with the new icons in a coroutine
        StartCoroutine(DrawScreen());
    }

    private IEnumerator DrawScreen(bool reDraw = false)
    {
        foreach (var spawnedIcon in spawnedIcons) //Destroys all the spawned icons
        {
            Destroy(spawnedIcon);
        }

        spawnedIcons.Clear(); //Clears the list of spawned icons
        
        //Loops through the slots 2D array
        for (int i = 0; i < slots.GetLength(0); i++)
        {
            for (int j = 0; j < slots.GetLength(1); j++)
            {
                //Spawns the new icon and places it in the right position compared to the 2D array
                var spawnIcon = Instantiate(slots[i,j].gameObject, 
                    new Vector3(i - (float) slots.GetLength(0) / 2, j - (float) slots.GetLength(1) / 2, 0), 
                    Quaternion.identity);

                spawnIcon.GetComponent<Icon>().location = new Vector2(i, j); //Sets local variables for the spawned icon

                spawnedIcons.Add(spawnIcon); //Adds the spawned icon to the list
            }
        }

        if (firstRun) //If the game is started the slot should not calculate any hits
        {
            firstRun = false;
            canRoll = true;
            yield break;
        }

        if (reDraw) //If reDraw it then 
        {
            yield return new WaitForSeconds(0.5f);
            RollSlot(false);
        }
        else GetAllIcons();
    }

    private void GetAllIcons()
    {
        var winIcons = new List<GameObject>();
        var types = new List<Icon>();
        var connections = 0;
        
        foreach (var icon in allIcons)
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
        
        foreach (var icon in allIcons)
        {
            if (rndValue >= currentChance && rndValue <= currentChance + icon.GetDropChance()) returnIcon = icon;

            currentChance += icon.GetDropChance();
        }
        
        return returnIcon;
    }

    private int GetMaxChance()
    {
        return allIcons.Sum(icon => icon.GetDropChance());
        
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
            
            foreach (var spawnedIcon in spawnedIcons.Where
                     (icon => icon.transform.position != GetWorldPosition(icon.GetComponent<Icon>().location) &&
                              icon.GetComponent<Icon>().GetIconType() != Icon.Icons.Empty))
            {
                if (notFinished == 0) break;
                
                var worldPosition = GetWorldPosition(spawnedIcon.GetComponent<Icon>().location);

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
