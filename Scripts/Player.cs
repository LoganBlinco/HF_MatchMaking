using HoldfastSharedMethods;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player 
{
	public static bool DEFAULT_MATCHMAKING = true;
	public static PlayerPriority DEFAULT_MATCHMAKING_PRIORITY = PlayerPriority.Both;
	public int ID;
	public ulong steamID;
	public string Name;
	public string regimentTag;
	public bool isBot;

	//state infomation
	public PlayerStates currentState = PlayerStates.idle;
	public float CurrentHP = 0;

	public bool wantsToMatchMake = DEFAULT_MATCHMAKING;


	private int spawnSectionID = -1;
	private FactionCountry playerFaction;
	private PlayerClass playerClass;
	private GameObject playerObject;

	public float ELO;

	public bool isAdmin = false;
	public PlayerPriority matchMakingPriority;


	public Player(int ID,ulong steamID, string name, string regTag, bool isBot)
	{
		this.ID = ID;
		this.steamID = steamID;
		this.Name = name;
		this.regimentTag = regTag;
		this.matchMakingPriority = DEFAULT_MATCHMAKING_PRIORITY;
		this.isBot = isBot;
		SetElo(DuelELOController.DEFAULT_ELO);

		DuelController.Instance.idToPlayer[ID] = this;
		Debug.Log("created player: " + name);

		if (wantsToMatchMake)
		{
			AddToMatchMaking();
		}
	}

	internal void PlayerSpawned(int spawnSectionId, FactionCountry playerFaction, PlayerClass playerClass, GameObject playerObject)
	{
		this.spawnSectionID = spawnSectionId;
		this.playerFaction = playerFaction;
		this.playerClass = playerClass;
		this.playerObject = playerObject;
		this.CurrentHP = 100;

		if (currentState == PlayerStates.dueling)
		{
			//check if correct spawn point.
			DuelInfomation duelInfo = Matchmaking.Instance.IdToDuel[ID];
			if (ID == duelInfo.challenged.ID)
			{
				//teleport to that one.
				DuelConsoleController.teleport_delayed(ID, duelInfo.challengedSpawnLocation);
			}
			else
			{
				DuelConsoleController.teleport_delayed(ID, duelInfo.challengerSpawnLocation);

			}
		}
		if (currentState == PlayerStates.groupfighting)
		{
			//check if correct spawn point.
			GroupfightInfo duelInfo = Matchmaking.Instance.IdToGroupfight[ID];
			duelInfo.PlayerSpawned(ID);
		}
	}

	public void command_duel(int opponentID, int rounds)
	{
		Player opponent = DuelController.Instance.idToPlayer[opponentID]; //check occurs before calling.


		//both players must be in idle state.
		if (currentState != PlayerStates.idle || opponent.currentState != PlayerStates.idle)
		{
			DuelConsoleController.privateMessage(ID, "Both players must be in idle state to challenge for a duel");
			return;
		}

		Matchmaking.Instance.BeginDuel(ID, opponentID, rounds, this, opponent);


		//can challenge the player to a duel.
	}



	public void HealMe()
	{
		if (CurrentHP <= 0 || CurrentHP >= 100) { return; }

		float amountToHeal = 100 - CurrentHP;
		DuelConsoleController.slap_delayed(ID, -(int)amountToHeal);
	}

	public void RemoveFromMatchMaking()
	{
		wantsToMatchMake = false;
		Matchmaking.Instance.RemoveFromMatchMakingPool(ID);
		DuelConsoleController.privateMessage(ID, "Removed from matchmaking.");
	}
	public void AddToMatchMaking()
	{
		wantsToMatchMake = true;
		Matchmaking.Instance.AddToMatchMaking(this);
		DuelConsoleController.privateMessage(ID, "Added to matchmaking.");
	}

	public void SetElo(float newVal)
	{
		ELO = newVal;
		Matchmaking.Instance.ELOChanged(ID, ELO);
	}

	public void MakePlayerLost()
	{
		Matchmaking.Instance.playersLost.Add(ID);
	}
}
