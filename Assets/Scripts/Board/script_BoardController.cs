using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class script_BoardController : NetworkBehaviour
{
    public List<GameObject> tiles = new List<GameObject>();
    public static float tileSize = 1f; //used to find the neighboring tiles of a given tile

    private GameObject _bC;

    private void Start()
    {
        _bC = GameObject.Find("BoardController");
        foreach (GameObject g in FindObjectsOfType(typeof(GameObject)))
            if (g.name.Contains("Tile") && !tiles.Contains(g))
                tiles.Add(g);
    }

    public static int GetTileDistance(GameObject tileA, GameObject tileB)
    {
        var distX = (int)Mathf.Abs(tileA.transform.position.x - tileB.transform.position.x);
        var distY = (int)Mathf.Abs(tileA.transform.position.y - tileB.transform.position.y);
        //       Debug.Log(distY);
        //        Debug.Log(distX);
        if (distY > distX)
            return distY;
        return distX;
    }

    public static GameObject GetTileByCoords(int x, int y)
    {
        GameObject tile = null;
        foreach (var t in GameObject.Find("BoardController").GetComponent<script_BoardController>().tiles)
            if (t.transform.position.x == x && t.transform.position.y == y)
                tile = t;
        return tile;
    }

    public static List<GameObject> GetTileNeighbors(GameObject tile)
    {
        if (GameObject.Find("BoardController").GetComponent<script_BoardController>().tiles.Count == 0)
            Debug.LogError("No tiles found. Error!");
        var neighbors = new List<GameObject>();
        Vector3 distVector;
        foreach (var t in GameObject.Find("BoardController").GetComponent<script_BoardController>().tiles)
        {
            distVector = t.transform.position - tile.transform.position;
            if (distVector.magnitude < tileSize * 2 && t != tile) neighbors.Add(t);
        }

        return neighbors;
    }

    public static GameObject GetTileNeighbor(GameObject tile, Enumerations.Direction direction)
    {
        var neighbors = GetTileNeighbors(tile);
        GameObject neighbor = null;
        foreach (var t in neighbors)
            if (direction == Enumerations.Direction.TopLeft &&
                t.transform.position.x < tile.transform.position.x &&
                t.transform.position.y > tile.transform.position.y)
            {
                neighbor = t;
            }
            else if ((int)direction == (int)Enumerations.Direction.Top)
            {
                if (Mathf.Abs(t.transform.position.x - tile.transform.position.x) < 0.1f)
                    if (t.transform.position.y > tile.transform.position.y)
                        neighbor = t;
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

        if (neighbor == null)
        {
            //            print("Neighboring tile not found");
        }

        return neighbor;
    }

    [ClientRpc]
    public void RpcAddTileToBoardController(string name)
    {
        var tile = GameObject.Find(name);
        if (tile == null)
            foreach (GameObject g in FindObjectsOfType(typeof(NetworkBehaviour)))
                if (g.GetComponent<scr_NetworkedObject>().tempName == name)
                    tile = g;
                else
                    print(g.name);

        GameObject.Find("BoardController").GetComponent<script_BoardController>().tiles.Add(tile);
    }
}