using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    private static float STAMINA_MAX = 100.0f;
    private static float HEALTH_MAX = 100.0f;
    private static float POWER_MAX = 100.0f;

    // returns int between bounds flr and cel, excluding lst
    public static int GetRandomNonRepeatInt(int cel, int lst, int flr = 0) {
        int freshInt = Random.Range(flr, cel);
        while (freshInt == lst)
            freshInt = Random.Range(flr, cel);
        return freshInt;
    }
}
