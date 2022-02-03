using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelHelper
{
    public static bool CustomContains(string[] array, string obj)
    {
        foreach (var e in array)
        {
            if (e == obj)
            {
                return true;
            }
        }
        return false;
    }
}
