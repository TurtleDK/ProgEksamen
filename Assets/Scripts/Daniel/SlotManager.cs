using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class SlotManager : MonoBehaviour
{
    [SerializeField] private Icon[] allIcons; //All possible icons except empty
    [SerializeField] Icon emptyIcon; //Icon used to fill the slot if it is empty
    
    private List<GameObject> spawnedIcons = new (); //Holds all the spawned icon GameObjects

    private Random rnd; //Random used to get random icons

    private bool stopWiggle; //Is used to stop the wiggle animation
    private bool canRoll = true; //Used to check if player can roll the slot
    private bool canSkip; //Used to check if player can skip the "Wiggle" animation
    
    private bool firstRun = true; //If the game is started the slot should not calculate any hits

    Icon[,] slots = new Icon[6, 5]; //Holds the icons in a 2D array in the current roll

    //Is run on the first frame
    void Start()
    {
        rnd = new Random(); //Creates a new random

        RollSlot(); //Rolls the first slot
    }
    
    //Runs every frame
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

        if (reDraw) //If reDraw it then needs to roll for the empty slots
        {
            yield return new WaitForSeconds(0.5f);
            RollSlot(false);
        }
        else GetAllIcons(); //If not reDraw then it should calculate the hits
    }

    private void GetAllIcons() //Calculates the icon hits
    {
        var winIcons = new List<GameObject>();
        var types = new List<Icon>();
        var connections = 0;
        
        foreach (var icon in allIcons) //Gets all icon types
        {
            if (!types.Contains(icon)) types.Add(icon);
        }
        
        foreach (var type in types) //Loops through all the icon types gets the amount of each type
        {
            //Contains the amount of the specific icon type in the slot
            var typeAmount = slots.Cast<Icon>().Count(icon => icon.GetIconType() == type.GetIconType()); 

            /* Same as the LINQ above
            var amount = 0;
            
            foreach (var icon in slots)
            {
                if (icon.GetIconType() == type.GetIconType()) amount++;
            }
            */

            if (typeAmount <= 8) continue; //Goes to the next type if the type amount is less than 8
            
            connections++; //Adds one to the total connections
            
            //Loops through all the icons in the slot where the icon type is the same as the current type
            foreach (var spawnedIcon in spawnedIcons.Where
                     (icon => icon.GetComponent<Icon>().GetIconType() == type.GetIconType()).ToList())
            {
                winIcons.Add(spawnedIcon);
            }
        }

        if (connections == 0) //If there is zero connections the player can then re-roll the slot
        {
            canRoll = true;
            return;
        }

        StartCoroutine(Wiggle(winIcons)); //Starts the wiggle animation for all the winning icons
    }
    
    private IEnumerator Wiggle(List<GameObject> winIcons) //Makes all the GameObjects inside the "winIcons" list wiggle
    {
        canSkip = true; //Makes it possible to skip the animation
        
        foreach (var icon in winIcons) //Starts the animation foreach GameObject in the list
        {
            var anim = icon.GetComponent<Animation>();
            anim.Play();
        }

        while (!stopWiggle) //While the player hasn't skipped the animation, wait
        {
            if (winIcons.Any(icon => icon.GetComponent<Animation>().isPlaying)) yield return null;
            else stopWiggle = true; //If the animation is done, stop the while loop
        }

        foreach (var icon in winIcons) //Stops the animation foreach GameObject in the list
        {
            var anim = icon.GetComponent<Animation>();
            anim.Stop();
            
            spawnedIcons.Remove(icon);
            
            var location = icon.GetComponent<Icon>().location;
            slots[(int) location.x, (int) location.y] = emptyIcon; //Changes all the winIcons to an empty icon
            SpawnEmpty(icon); //Spawn an empty icon GameObject at the position
        }

        yield return new WaitForSeconds(0.5f); //Wait 0.5 seconds

        stopWiggle = false; //Reset the stopWiggle bool
        
        MoveIconsDown(); //Function to move all the icons down
    }
    
    private void SpawnEmpty(GameObject replacedIcon) //Function to spawn an empty icon GameObject at the position of the replaced icon
    {
        //Spawns an empty icon GameObject at the position of the replaced icon
        var obj = Instantiate(emptyIcon.gameObject, replacedIcon.transform.position, Quaternion.identity); 
        obj.GetComponent<Icon>().location = replacedIcon.GetComponent<Icon>().location;
        spawnedIcons.Add(obj);

        Destroy(replacedIcon); //Destroys the replaced icon
    }

    private void MoveIconsDown() //Moves all the icons down
    {
        //Loops through the x axis in the 2D array
        for (int i = 0; i < slots.GetLength(0); i++)
        {
            //Contains all the non-empty icons in the current y axis
            var nonEmptyIcons = new List<Icon>();
        
            //Loops through the y axis in the 2D array
            for (int j = 0; j < slots.GetLength(1); j++)
            {
                //If the current icon type is not empty then add it to the list
                if (slots[i, j].GetIconType() != emptyIcon.GetIconType()) nonEmptyIcons.Add(slots[i,j]);
            }

            //Move all the icons down
            for (int j = 0; j < nonEmptyIcons.Count; j++)
            {
                slots[i, j] = nonEmptyIcons[j];
                slots[i, j].location = new Vector2(i, j);
            }

            //Fill the rest of the slots with empty icons
            for (int j = nonEmptyIcons.Count; j < slots.GetLength(1); j++)
            {
                slots[i, j] = emptyIcon;
            }
        }

        StartCoroutine(ReDraw()); //Redraw the slot with the moved icons
    }
    
    private IEnumerator ReDraw() 
    {
        yield return new WaitForSeconds(0.5f); //Waits 0.5 seconds
        StartCoroutine(DrawScreen(true)); //Redraws the screen
    }

    private Icon GetRandomIcon() //Gets a random icon based on the drop chance
    {
        var rndValue = rnd.Next(0, GetMaxChance() + 1); //Gets a random value between 0 and the max chance

        var currentChance = 0; //Sets currentChance
        var returnIcon = emptyIcon; //Sets the return icon to the empty icon
        
        foreach (var icon in allIcons) //Loops through all the icons
        {
            //Checks if the random value is between the current chance and the current chance + the icon drop chance
            if (rndValue >= currentChance && rndValue <= currentChance + icon.GetDropChance())
            {
                returnIcon = icon; //return icon is set to the current icon
                break;
            } 

            //Adds the current icon drop chance to the current chance
            currentChance += icon.GetDropChance();
        }
        
        return returnIcon; 
    }

    private int GetMaxChance()
    {
        return allIcons.Sum(icon => icon.GetDropChance()); //Gets the sum of all the icon drop chances
        
        /* Is the same as the LINQ above
        var returnValue = 0;
        
        foreach (var icon in icons)
        {
            returnValue += icon.GetDropChance();
        }

        return returnValue;
        */
    }
}
