using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script_BoardController : MonoBehaviour
{
    public static List<GameObject> tiles = new List<GameObject>();
    public static float tileSize = 1f; //used to find the neighboring tiles of a given tile

    public static int GetTileDistance(GameObject tileA, GameObject tileB)
    {
        int distX = (int)(Mathf.Abs(tileA.transform.position.x - tileB.transform.position.x));
        int distY = (int)(Mathf.Abs(tileA.transform.position.y - tileB.transform.position.y));
        Debug.Log(distY);
        Debug.Log(distX);
        if(distY > distX)
            return distY;
        else
            return distX;
    }   

    public static List<GameObject> GetTileNeighbors(GameObject tile)
    {
        List<GameObject> neighbors = new List<GameObject>();
        Vector3 distVector;
        foreach(GameObject t in tiles)
        {
            distVector = t.transform.position - tile.transform.position;
            if(distVector.magnitude < tileSize * 2 && t != tile)
            {
                neighbors.Add(t);
            }
        }

        return neighbors;
    }

    public static GameObject GetTileNeighbor(GameObject tile, Enumerations.Direction direction)
    {
        List<GameObject> neighbors = GetTileNeighbors(tile);
        GameObject neighbor = null;
        foreach (GameObject t in neighbors)
        {
            if (direction == Enumerations.Direction.TopLeft && 
                t.transform.position.x < tile.transform.position.x && 
                t.transform.position.y > tile.transform.position.y)
            {
                neighbor = t;
            }
            else if ((int)direction == (int)Enumerations.Direction.Top)
            {
                if(Mathf.Abs(t.transform.position.x - tile.transform.position.x) < 0.1f)
                {
                    if (t.transform.position.y > tile.transform.position.y)
                    {
                        neighbor = t;
                    }
                }
            }
            else if (direction == Enumerations.Direction.TopRight &&
                t.transform.position.x > tile.transform.position.x &&
                t.transform.position.y > tile.transform.position.y)
            {
                neighbor = t;
            }
            else if (direction == Enumerations.Direction.BottomLeft &&
                t.transform.position.x < tile.transform.position.x &&
                t.transform.position.y < tile.transform.position.y)
            {
                neighbor = t;
            }
            else if (direction == Enumerations.Direction.Bottom &&
                Mathf.Abs(t.transform.position.x - tile.transform.position.x) < 0.1f &&
                t.transform.position.y < tile.transform.position.y)
            {
                neighbor = t;
            }
            else if (direction == Enumerations.Direction.BottomRight &&
                t.transform.position.x > tile.transform.position.x &&
                t.transform.position.y < tile.transform.position.y)
            {
                neighbor = t;
            }
            else if (direction == Enumerations.Direction.Right &&
                t.transform.position.x > tile.transform.position.x &&
                Mathf.Abs(t.transform.position.y - tile.transform.position.y) < 0.1f)
            {
                neighbor = t;
            }
            else if (direction == Enumerations.Direction.Left &&
               t.transform.position.x < tile.transform.position.x &&
               Mathf.Abs(t.transform.position.y - tile.transform.position.y) < 0.1f)
            {
                neighbor = t;
            }
        }

        if(neighbor == null)
        {
            print("Neighboring tile not found");
        }

        return neighbor;
    }
}
