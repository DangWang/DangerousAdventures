using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script_MovementParser : MonoBehaviour
{

    public List<GameObject> GetAllowedMovement(GameObject myTile, ref Enumerations.Direction myDirection, int availableMovement)
    {
        List<GameObject> tiles = new List<GameObject>();
        GameObject neighbor, tempTile = myTile;
        int i = 0;
        
        while(i++ < availableMovement)
        {
            neighbor = script_BoardController.GetTileNeighbor(tempTile, myDirection);
            if(neighbor == null)
                return tiles;
            if(myDirection != Enumerations.Direction.Choose)
            {
                if (neighbor.tag == "Free")
                {
                    tempTile = neighbor;
                    tiles.Add(neighbor);
                }
                else if (neighbor.tag == "Wall")
                {
                    if(tiles.Count == 0)
                    {
                        myDirection = Enumerations.Direction.Choose;
                    }
                    return tiles;
                }
                else
                {
                    //print("Undefined Tile Tag");
                }
            }else{
                return tiles;
            }
        }
        if(i >= availableMovement)
        {
            return tiles;
        }
        Debug.LogError("problem");
        return null;
    }
}
