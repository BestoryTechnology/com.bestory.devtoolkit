using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClassHelper
{

    public static string GetFormatedTimeSeconds(this float secondTime)
    {
        int floorSecond = Mathf.FloorToInt(secondTime);
        return string.Format("{0}:{1}", floorSecond.ToString().PadLeft(2, '0'), ((secondTime - floorSecond)*100F).ToString("N0").PadLeft(2, '0'));
    }
	
}
