using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathHelper
{
    
    public static float LoopRange(this float inputVal, float min, float max)
    {
        if (inputVal > max)
            return min + (inputVal - max);
        if (inputVal < min)
            return max - (inputVal - min);

        return inputVal;
    }

    public static float LoopRangeClamped(this float inputVal, float min, float max, float clampVal)
    {
        if (inputVal > max)
            inputVal =  min + (inputVal - max);
        if (inputVal < min)
            inputVal = max - (inputVal - min);

        if (inputVal > clampVal)
            inputVal = clampVal;

        return inputVal;
    }

    public static int LoopRange(this int inputVal, int min, int max)
    {
        if (inputVal > max)
            return min + (inputVal - max);
        if (inputVal < min)
            return max - (inputVal - min);

        return inputVal;
    }

    public static int LoopRangeClamped(this int inputVal, int min, int max, int clampVal)
    {
        if (inputVal > max)
            inputVal = min + (inputVal - max);
        if (inputVal < min)
            inputVal = max - (inputVal - min);

        if (inputVal > clampVal)
            inputVal = clampVal;

        return inputVal;
    }

}
