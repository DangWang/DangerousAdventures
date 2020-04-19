using System;
using System.IO;
using Mirror;
using UnityEngine;

public class script_LoadBoard : NetworkBehaviour
{
    public string filePath;

    public GameObject free,
        wall,
        artifactChest,
        chest,
        door,
        chance,
        lava,
        lockedDoor,
        monsterSpawn,
        pillar,
        safeSquare,
        shop,
        startSquare,
        trigger;

    private GameObject _newTile;


    // Start is called before the first frame update
    private void Start()
    {
        LoadGrid("../Assets/Maps/Test01.damap");
    }

    public void LoadGrid(string scenePath)
    {
        var path = "";
        var splitString = scenePath.Split(new[] { "/", "." }, StringSplitOptions.RemoveEmptyEntries);
        int i, j;
        for (i = 0; i <= splitString.Length - 3; i++) path = path + splitString[i] + "/";
        path = path + splitString[splitString.Length - 2] + ".damap";
        var reader = new StreamReader(path);

        Debug.LogFormat("Loading Map Data from: {0}", path);
        var line = "";
        line = reader.ReadLine();
        j = 0;
        while (line != null)
        {
            splitString = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            for (i = 0; i < splitString.Length; i++)
            {
                switch (splitString[i])
                {
                    case "fr":
                        _newTile = Instantiate(free, new Vector3(i, -j, 0), Quaternion.identity, null);
                        break;
                    case "wa":
                        _newTile = Instantiate(wall, new Vector3(i, -j, 0), Quaternion.identity, null);
                        break;
                    case "ar":
                        _newTile = Instantiate(artifactChest, new Vector3(i, -j, 0), Quaternion.identity, null);
                        break;
                    case "ce":
                        _newTile = Instantiate(chest, new Vector3(i, -j, 0), Quaternion.identity, null);
                        break;
                    case "dr":
                        _newTile = Instantiate(door, new Vector3(i, -j, 0), Quaternion.identity, null);
                        break;
                    case "ca":
                        _newTile = Instantiate(chance, new Vector3(i, -j, 0), Quaternion.identity, null);
                        break;
                    case "la":
                        _newTile = Instantiate(lava, new Vector3(i, -j, 0), Quaternion.identity, null);
                        break;
                    case "ld":
                        _newTile = Instantiate(lockedDoor, new Vector3(i, -j, 0), Quaternion.identity, null);
                        break;
                    case "ms":
                        _newTile = Instantiate(monsterSpawn, new Vector3(i, -j, 0), Quaternion.identity, null);
                        break;
                    case "pi":
                        _newTile = Instantiate(pillar, new Vector3(i, -j, 0), Quaternion.identity, null);
                        break;
                    case "sf":
                        _newTile = Instantiate(safeSquare, new Vector3(i, -j, 0), Quaternion.identity, null);
                        break;
                    case "sh":
                        _newTile = Instantiate(shop, new Vector3(i, -j, 0), Quaternion.identity, null);
                        break;
                    case "st":
                        _newTile = Instantiate(startSquare, new Vector3(i, -j, 0), Quaternion.identity, null);
                        break;
                    case "tr":
                        _newTile = Instantiate(trigger, new Vector3(i, -j, 0), Quaternion.identity, null);
                        break;
                }

                _newTile.name = "Tile (" + i + "," + j + ")";
                _newTile.GetComponent<scr_NetworkedObject>().tempName = _newTile.name;
                NetworkServer.Spawn(_newTile);
                GameObject.Find("BoardController").GetComponent<script_BoardController>()
                    .RpcAddTileToBoardController(_newTile.name);
            }

            j++;
            line = reader.ReadLine();
        }

        reader.Close();
    }
}