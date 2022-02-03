using HoldfastSharedMethods;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class DuelController : MonoBehaviour
{
	private static DuelController _instance;
	public static DuelController Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType<DuelController>();
			}
			return _instance;
		}
	}

	public int currentTime = -1;

	//public List<DuelArena> DuelArenaList;
	public DuelArena[] DuelArenaList;
	public List<DuelArena> CurrentDuelArenasIdle;
	public List<DuelArena> CurrentDuelArenasInUse;

	public Dictionary<int, Player> idToPlayer;

	public Random rand;

	public void InitializeRandom(int seed)
	{
		rand = new Random(seed);
	}

	public void OnPlayerJoined(int playerId, ulong steamId, string playerName, string regimentTag, bool isBot)
	{
		//if (isBot) { return; }

		Player newP = new Player(playerId, steamId, playerName, regimentTag, isBot);
	}

	public void OnPlayerLeft(int playerId)
	{
		DuelInfomation duelInfo;
		if (Matchmaking.Instance.IdToDuel.TryGetValue(playerId, out duelInfo))
		{
			duelInfo.EndDuel();
		}

		Player temp;
		if (idToPlayer.TryGetValue(playerId, out temp))
		{
			temp.RemoveFromMatchMaking();
			idToPlayer.Remove(playerId);
		}
	}


	internal void OnPlayerSpawned(int playerId, int spawnSectionId, FactionCountry playerFaction, PlayerClass playerClass, int uniformId, GameObject playerObject)
	{
		Player temp;
		if (!idToPlayer.TryGetValue(playerId,out temp))
		{
			Debug.LogError("Error. Player with id: " + playerId + " does not have an instance.");
			return;
		}
		temp.PlayerSpawned(spawnSectionId, playerFaction, playerClass, playerObject);

	}

	internal void OnTextMessage(int playerId, TextChatChannel channel, string text)
	{
		if (text[0] != '/') { return; }

		string[] inputArray = text.Substring(1).Split(' ');
		Debug.Log(inputArray[0]);
		Action<passedVar> function;
		if (DuelCommandController.Instance.commands.TryGetValue(inputArray[0], out function))
		{
			passedVar temp = new passedVar()
			{
				id = playerId,
				data = inputArray
			};
			function(temp);
		}
	}


	void Start()
	{
		DuelArenaList = FindObjectsOfType<DuelArena>();
		CurrentDuelArenasIdle = new List<DuelArena>(DuelArenaList);
		CurrentDuelArenasInUse = new List<DuelArena>();

		idToPlayer = new Dictionary<int, Player>();
	}

	public void OnPlayerKilledPlayer(int killerPlayerId, int victimPlayerId)
	{
		Matchmaking.Instance.PlayerDied(killerPlayerId, victimPlayerId);
	}

	public void OnPlayerHurt(int playerID, float newHP)
	{
		Player temp;
		if (idToPlayer.TryGetValue(playerID,out temp))
		{
			temp.CurrentHP = newHP;
		}
	}


}
