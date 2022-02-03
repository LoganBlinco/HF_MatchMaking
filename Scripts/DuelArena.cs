using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DuelArenaSelection
{
	random,
	itterative
}

public class DuelArena : MonoBehaviour
{
	public static DuelArenaSelection defaultSelection = DuelArenaSelection.itterative;

	public List<GameObject> SpawnLocation1;
	public List<GameObject> SpawnLocation2;

	private int spawnLocation1Counter = 0;
	private int spawnLocation2Counter = 0;

	public GameObject GetLocation(int teamID)
	{
		if (teamID == 1)
		{
			return getLocation1();
		}
		else if (teamID == 2) //player is in team2
		{
			return getLocation2();
		}
		throw new System.Exception("GetLocation() -> Error. Invalid TeamID ");
	}

	private GameObject getLocation1()
	{
		int index = getIndex(SpawnLocation1.Count - 1,1);
		return SpawnLocation1[index];
	}

	private GameObject getLocation2()
	{
		int index = getIndex(SpawnLocation2.Count-1,2);
		return SpawnLocation2[index];
	}

	private int getItterative(int max, int location)
	{
		int value;
		if (location == 1)
		{
			value = spawnLocation1Counter;
			spawnLocation1Counter = (spawnLocation1Counter +1) % (max+1);
		}
		else
		{
			value = spawnLocation2Counter;
			spawnLocation2Counter = (spawnLocation2Counter + 1) % (max+1);
		}
		return value;
	}


	private int getIndex(int max,int teamID)
	{
		switch(defaultSelection)
		{
			case DuelArenaSelection.itterative:
				return getItterative(max, teamID);
			case DuelArenaSelection.random:
				return getRandom(max);
		}
		//default.
		return getRandom(max);
	}

	private int getRandom(int max)
	{
		var random = DuelController.Instance.rand;
		return random.Next(0, max);
	}
}
