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

    private int amount;
    private int amountMax;

    private List<Coroutine> routines = new();

    private bool stopWiggle;
    private bool canRoll = true;
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
            if (canRoll) RollSlot();
            else stopWiggle = true;
        }

        if (Input.GetKeyDown(KeyCode.L)) RollSlot(false);

        if (Input.GetKeyDown(KeyCode.J)) MoveIconsDown();
        
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
            
            foreach (var spawnedIcon in spawnedIcons.Where(
                         icon => icon.GetComponent<Icon>().GetIconType() == type.GetIconType()).ToList())
            {
                StartCoroutine(Wiggle(spawnedIcon));
            }
        }

        if (connections == 0)
        {
            canRoll = true;
            return;
        }

        StartCoroutine(Wait(true));
    }

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
        return new Vector3(location.x - (float) slots.GetLength(0) / 2, location.y - (float) slots.GetLength(1) / 2);
    }
    
    private IEnumerator Wiggle(GameObject icon)
    {
        var anim = icon.GetComponent<Animation>();
        var originalRotation = icon.transform.rotation;
        anim.Play();

        while (anim.isPlaying)
        {
            yield return null;
            if (!stopWiggle) continue;
            
            anim.Stop();
            icon.transform.rotation = originalRotation;
        }
        
        spawnedIcons.Remove(icon);
            
        var location = icon.GetComponent<Icon>().location;
        slots[(int) location.x, (int) location.y] = emptyIcon;
        SpawnEmpty(icon);
        
        amount++;
    }

    private void SpawnEmpty(GameObject replacedIcon)
    {
        var obj = Instantiate(emptyIcon.gameObject, replacedIcon.transform.position, Quaternion.identity);
        obj.GetComponent<Icon>().location = replacedIcon.GetComponent<Icon>().location;
        spawnedIcons.Add(obj);

        Destroy(replacedIcon);
    }

    private IEnumerator DropIcon(GameObject icon)
    {
        var targetPosition = GetWorldPosition(icon.GetComponent<Icon>().location);

        // Loop until the GameObject reaches the target position
        while (icon.transform.position.y > targetPosition.y)
        {
            print("What");
            
            // Move the GameObject towards the target position at a constant speed
            icon.transform.position = Vector3.MoveTowards(icon.transform.position, targetPosition, 10 * Time.deltaTime);

            // Yield and wait for the next frame
            yield return null;
        }

        // Snap the GameObject to the exact target position
        icon.transform.position = targetPosition;
        amount++;
    }

    private IEnumerator Wait(bool wiggle)
    {
        while (amount < amountMax)
        {
            yield return null;
        }
        
        amount = 0;
        amountMax = 0;
        
        if (wiggle) stopWiggle = false;

        yield return new WaitForSeconds(0.5f);

        if (wiggle) MoveIconsDown();
        else StartCoroutine(DrawScreen(true));
    }
}
