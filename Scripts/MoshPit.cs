using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoshPit : MonoBehaviour
{
	public static DuelArenaSelection defaultSelection = DuelArenaSelection.itterative;

	public List<GameObject> SpawnLocations;

	private int spawnLocationCounter = 0;

	public GameObject GetLocation()
	{
		return getLocation();
	}

	private GameObject getLocation()
	{
		int index = getIndex(SpawnLocations.Count-1);
		return SpawnLocations[index];
	}

	private int getItterative(int max)
	{
		int value = spawnLocationCounter;
		spawnLocationCounter = (spawnLocationCounter + 1) % (max + 1);
		return value;
	}


	private int getIndex(int max)
	{
		switch (defaultSelection)
		{
			case DuelArenaSelection.itterative:
				return getItterative(max);
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
