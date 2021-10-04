using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChanceSystem
{
    private static readonly System.Random random = new System.Random();
    private static readonly object syncLock = new object();

    public static bool PercentChance(int chance)
    {
        int numberOfTrues = 0;
        int numberOfFalse = 0;
        for (int i = 0; i < 100; i++)
        {
            if(RandomNumber(1,100) < chance)
            {
                numberOfTrues++;
            }
            else
            {
                numberOfFalse++;
            }
        }
        if(numberOfTrues >= numberOfFalse)
        {
            return true;
        }
        return false;
    }
    public static int RandomNumber(int min, int max)
    {
        lock(syncLock)
        {
            return random.Next(min, max);
        }
    }
}
