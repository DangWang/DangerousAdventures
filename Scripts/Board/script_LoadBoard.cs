using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.CodeDom.Compiler;
using System;
using System.Linq;

public class script_LoadBoard : MonoBehaviour
{
    public GameObject free, wall, artifactChest, chest, door, chance, lava, lockedDoor, monsterSpawn, pillar, safeSquare, shop, startSquare, trigger;
    public string filePath;

    GameObject newTile;


    // Start is called before the first frame update
    void Start()
    {
        LoadGrid("../Assets/Maps/Test01.damap");
    }

    public void LoadGrid(string scenePath)
    {
        string path = "";
        string[] splitString = scenePath.Split(new string[] { "/", "." }, StringSplitOptions.RemoveEmptyEntries);
        int i, j;
        for (i = 0; i <= splitString.Length - 3; i++)
        {
            path = path + splitString[i] + "/";
        }
        path = path + splitString[splitString.Length - 2] + ".damap";
        StreamReader reader = new StreamReader(path);

        Debug.LogFormat("Loading Map Data from: {0}", path);
        string line = "";
        line = reader.ReadLine();
        j = 0;
        while(line != null)
        {
            splitString = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            for (i = 0; i < splitString.Length; i++)
            {
                switch (splitString[i])
                {
                    case "fr":
                        newTile = Instantiate(free, new Vector3(i, -j, 0), Quaternion.identity, this.transform);
                        break;
                    case "wa":
                        newTile = Instantiate(wall, new Vector3(i, -j, 0), Quaternion.identity, this.transform);
                        break;
                    case "ar":
                        newTile = Instantiate(artifactChest, new Vector3(i, -j, 0), Quaternion.identity, this.transform);
                        break;
                    case "ce":
                        newTile = Instantiate(chest, new Vector3(i, -j, 0), Quaternion.identity, this.transform);
                        break;
                    case "dr":
                        newTile = Instantiate(door, new Vector3(i, -j, 0), Quaternion.identity, this.transform);
                        break;
                    case "ca":
                        newTile = Instantiate(chance, new Vector3(i, -j, 0), Quaternion.identity, this.transform);
                        break;
                    case "la":
                        newTile = Instantiate(lava, new Vector3(i, -j, 0), Quaternion.identity, this.transform);
                        break;
                    case "ld":
                        newTile = Instantiate(lockedDoor, new Vector3(i, -j, 0), Quaternion.identity, this.transform);
                        break;
                    case "ms":
                        newTile = Instantiate(monsterSpawn, new Vector3(i, -j, 0), Quaternion.identity, this.transform);
                        break;
                    case "pi":
                        newTile = Instantiate(pillar, new Vector3(i, -j, 0), Quaternion.identity, this.transform);
                        break;
                    case "sf":
                        newTile = Instantiate(safeSquare, new Vector3(i, -j, 0), Quaternion.identity, this.transform);
                        break;
                    case "sh":
                        newTile = Instantiate(shop, new Vector3(i, -j, 0), Quaternion.identity, this.transform);
                        break;
                    case "st":
                        newTile = Instantiate(startSquare, new Vector3(i, -j, 0), Quaternion.identity, this.transform);
                        break;
                    case "tr":
                        newTile = Instantiate(trigger, new Vector3(i, -j, 0), Quaternion.identity, this.transform);
                        break;
                }
                newTile.name = "Tile (" + i.ToString() + "," + j.ToString() + ")";
                script_BoardController.tiles.Add(newTile);
            }
            j++;
            line = reader.ReadLine();
        }
        reader.Close();
    }
}
