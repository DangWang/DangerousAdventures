using System.Collections.Generic;
using UnityEngine;

public class script_MovementParser : MonoBehaviour
{
    public string[] tagsOfMovementTiles;

    public List<GameObject> GetAllowedMovement(GameObject myTile, ref Enumerations.Direction myDirection,
        int availableMovement)
    {
        var tiles = new List<GameObject>();
        GameObject neighbor, tempTile = myTile;
        var i = 0;

        while (i++ < availableMovement)
        {
            neighbor = script_BoardController.GetTileNeighbor(tempTile, myDirection);
            if (neighbor == null)
                return tiles;
            if (myDirection != Enumerations.Direction.Choose)
            {
                var check = false;
                foreach (var tag in tagsOfMovementTiles)
                    if (neighbor.tag == tag)
                    {
                        check = true;
                        tempTile = neighbor;
                        tiles.Add(neighbor);
                    }

                if (check == false)
                {
                    if (tiles.Count == 0) myDirection = Enumerations.Direction.Choose;
                    return tiles;
                }
            }
            else
            {
                return tiles;
            }
        }

        if (i >= availableMovement) return tiles;
        Debug.LogError("problem");
        return null;
    }
}