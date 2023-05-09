namespace Daniel
{
    public class OldIteration
    { 
        // Used before to wait for the "Wiggle" icons to finish animating
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
    
    //Used when trying to make icons fall down
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
    
    //Old version of moving icons down (Did not work and a lot of bugs)
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
    
    //Old version of "MoveIconsDown" (Did not work and caused crashes)
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
    */
    
    //was used for checking if there was any empty icons in a row (Did not work and caused crashes)
    //Different iterations
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
    
    //Different iteration on "MoveRowDown" (Did not work and caused bugs)
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

    private void MoveRowDown(int row, int startPoint)
    {
        for (int i = startPoint; i < slots.GetLength(1) - 1; i++)
        {
            slots[row, i] = slots[row, i + 1];
            slots[row, i].location = new Vector2(row, i);
            slots[row, i + 1] = emptyIcon;
        }
    }
    */
    }
}