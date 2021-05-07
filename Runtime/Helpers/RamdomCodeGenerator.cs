using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RamdomCodeGenerator
{

    static string[] codeSerial = new string[] { "x", "2", "C", "K", "5", "u", "G", "t", "i", "n", "D", "f",
        "r", "N", "O", "J", "3", "o", "0", "F", "U", "Z", "M", "q", "Y", "V", "s", "w", "h", "z",
        "g", "E", "I", "a", "7", "S", "c", "9", "6", "m", "e", "v", "b", "j", "k", "Q", "H", "8",
        "L", "P", "W", "d", "T", "X", "l", "1", "R", "4", "B", "A", "y", "p" };

    public static string RandomCode()
    {

        string temp = string.Empty;

        List<int> index = new List<int>();

        index.Add(DateTime.Now.Year - 2020);
        index.Add(DateTime.Now.Month);
        index.Add(DateTime.Now.Day);
        index.Add(DateTime.Now.Hour);
        index.Add(DateTime.Now.Minute);
        index.Add(DateTime.Now.Second);

        for (int i = 0; i < index.Count; i++)
        {
            temp += codeSerial[index[i]];
        }

        return temp;
    }

}
