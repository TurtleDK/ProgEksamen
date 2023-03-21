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

    public Vector2 location;
    
    private bool movedDown = false;

    private float timer = 0;
    
    public int GetMultiplier()
    {
        return baseMultiplier;
    }
    
    public int GetDropChance()
    {
        return dropChance;
    }
    
    public bool GetMovedDown()
    {
        return movedDown;
    }
    
    public Icons GetIconType()
    {
        return iconType;
    }

    public void UpdateMovedDown()
    {
        movedDown = transform.position == new Vector3(location.x, location.y, transform.position.z);
    }

    IEnumerator DropDown()
    {
        if (movedDown) yield break;

        var pos = transform.position;
        
        var startPos = pos;
        var targetPos = new Vector3(location.x, location.y, pos.z);

        yield return new WaitForEndOfFrame();
        while (transform.position != targetPos)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, timer);
            timer += Time.deltaTime * 2;
            yield return null;
        }

        movedDown = true;
    }
}
