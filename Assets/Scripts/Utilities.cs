using UnityEngine;

public class Utilities : MonoBehaviour
{
    public static int RollDice(int number)
    {
        var sum = 0;

        for (var i = 0; i < number; i++) sum += Random.Range(1, 7); //returns value from 1 to 6
        //        print(sum);
        return sum;
    }
}