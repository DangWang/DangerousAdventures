using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour
{
    public static int RollDice(int number)
    { 
        int sum = 0;

        for(int i = 0; i < number; i++)
        {
            sum += Random.Range(1, 7); //returns value from 1 to 6
        }
        return sum; 
    }
}
