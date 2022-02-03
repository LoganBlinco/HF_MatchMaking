using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelConfigVariables : MonoBehaviour
{
    public static Dictionary<string, Action<string>> commandDictionary = new Dictionary<string, Action<string>>()
    {
        {"DEFAULT_MATCHMAKING", variable_DEFAULT_MATCHMAKING},
        {"MatchMakeFreq",variable_MatchMakeFreq },
        {"MatchMakeRounds",variable_MatchMakeRounds },
        {"DEFAULT_MATCHMAKING_PRIORITY",command_matchmaking_priority }
    };

    private static void command_matchmaking_priority(string obj)
    {
        string[] classTypes = System.Enum.GetNames(typeof(PlayerPriority));
        if (DuelHelper.CustomContains(classTypes,obj))
        {
            Player.DEFAULT_MATCHMAKING_PRIORITY = (PlayerPriority)System.Enum.Parse(typeof(PlayerPriority), obj);
        }
    }

    private static void variable_MatchMakeRounds(string obj)
    {
        int defaultV;
        if (int.TryParse(obj, out defaultV) && defaultV >= 3)
        {
            Matchmaking.MatchMakeDefaultRounds = defaultV;
            Debug.Log("Matchmaking default rounds is now: " + defaultV);
        }
        Debug.Log("Error, matchmaking default rounds cannot be changed to: " + obj);
    }

    private static void variable_MatchMakeFreq(string obj)
    {
        float defaultV;
        if (float.TryParse(obj, out defaultV) && defaultV >= 3)
        {
            Matchmaking.MatchMakeFreq = defaultV;
            Debug.Log("Matchmaking frequency is now: " + defaultV);
        }
        Debug.Log("Error, matchmaking frequency cannot be changed to: " + obj);
    }

    private static void variable_DEFAULT_MATCHMAKING(string obj)
    {
        bool defaultV;
        if (bool.TryParse(obj,out defaultV))
        {
            Player.DEFAULT_MATCHMAKING = defaultV;
            Debug.Log("Matchmaking default is now: " + defaultV);
        }
        Debug.Log("Error, matchmaking cannot be changed to: " + obj);
    }




    //using wrex notation
    //<modid>:<class>:[playerLayer,safeZone,damgeZone,damgeMod,minPlayers,maxSlap]
    public static void PassConfigVariables(string[] value)
    {
        string modID = "2725619766";
        for (int i = 0; i < value.Length; i++)
        {
            Debug.Log("Attempting to parse: " + value[i]);
            var splitData = value[i].Split(':');
            if (splitData.Length != 3)
            {
                Debug.LogError("invalid number of variables");
                continue;
            }

            //so first variable should be the mod id
            if (splitData[0] != modID) { continue; }
            Action<string> function;
            if (commandDictionary.TryGetValue(splitData[1], out function))
            {
                function(splitData[2]);
                continue;
            }
        }
    }
}
