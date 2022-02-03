using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupfightInfo 
{
	public static int TEAM_KILL_AMOUNT = 2;

	public List<Player> team1;
	public List<Player> team2;

	public int roundsToPlay = 0;

	public Vector3 challengerSpawnLocation;
	public Vector3 challengedSpawnLocation;

	private float team1ELO = 0;
	private float team2ELO = 0;

	public Dictionary<int, int> idToEnemyKills;

	public Dictionary<int, int> idToTeam;
	public Dictionary<int, bool> idToAlive;
	public Dictionary<int, Vector3> idToSpawnPoint;

	public Dictionary<int, int> teamIDToAlive;

	DuelArena arenaInuse;

	public GroupfightInfo(List<Player> team1, List<Player> team2, int rounds, DuelArena arena)
	{
		InitializeValues(team1, team2, rounds, arena);


	}

	private void InitializeValues(List<Player> team1, List<Player> team2, int rounds, DuelArena arena)
	{
		this.arenaInuse = arena;
		this.team1 = team1;
		this.team2 = team2;
		this.roundsToPlay = rounds;
		this.teamIDToAlive = new Dictionary<int, int>();
		this.idToAlive = new Dictionary<int, bool>();
		this.idToSpawnPoint = new Dictionary<int, Vector3>();

		challengerSpawnLocation = arena.GetLocation(1).transform.position;
		challengedSpawnLocation = arena.GetLocation(2).transform.position;

		idToEnemyKills = new Dictionary<int, int>();
		idToTeam = new Dictionary<int, int>();

		int teamSize = team1.Count;
		string messageBegin = "Begin: " + teamSize + "v" + teamSize + " - Qapla!'";

		for (int i = 0; i < team1.Count; i++)
		{
			idToEnemyKills.Add(team1[i].ID, 0);
			idToEnemyKills.Add(team2[i].ID, 0);

			idToTeam.Add(team1[i].ID, 1);
			idToTeam.Add(team2[i].ID, 2);

			team1ELO += team1[i].ELO;
			team2ELO += team2[i].ELO;

			idToAlive.Add(team1[i].ID, true);
			idToAlive.Add(team2[i].ID, true);

			team1[i].HealMe();
			team2[i].HealMe();

			team1[i].currentState = PlayerStates.groupfighting;
			team2[i].currentState = PlayerStates.groupfighting;

			TeleportPlayer(team1[i].ID);
			TeleportPlayer(team2[i].ID);

			DuelConsoleController.privateMessage_delayed(team1[i].ID, messageBegin);
			DuelConsoleController.privateMessage_delayed(team2[i].ID, messageBegin);
		}

		teamIDToAlive.Add(1, team1.Count);
		teamIDToAlive.Add(2, team2.Count);

	}

	public void PlayerSpawned(int playerID)
	{
		if (!idToAlive[playerID]) { return; }//player died from fight so we can ignore.

		TeleportPlayer(playerID);

	}

	public void TeleportPlayer(int playerID)
	{
		Vector3 spawnPos;
		if (idToSpawnPoint.TryGetValue(playerID,out spawnPos))
		{
			DuelConsoleController.teleport_delayed(playerID, spawnPos);
			return;
		}
		//player has not had a spawn set yet.


		//checking that id is part of the groupfight.
		int teamID;
		if (!idToTeam.TryGetValue(playerID,out teamID))
		{
			Debug.Log("Attempted to teleport: " + playerID + " but he is not in this groupfight.");
			return;
		}
		spawnPos = arenaInuse.GetLocation(idToTeam[playerID]).transform.position;
		idToSpawnPoint[playerID] = spawnPos;
		DuelConsoleController.teleport_delayed(playerID, spawnPos);
	}


	public void ApplyTeamKill(int killerId,int victimId)
	{
		idToEnemyKills[killerId] -= TEAM_KILL_AMOUNT;


		Player killerPlayer = DuelController.Instance.idToPlayer[killerId];
		Player victimPlayer = DuelController.Instance.idToPlayer[victimId];

		DuelELOController.CalculateElo(killerPlayer, victimPlayer, 0, TEAM_KILL_AMOUNT);

		idToAlive[victimId] = false;
		teamIDToAlive[idToTeam[victimId]] -= 1;
		RoundEndCheck();
	}

	public void ApplyKill(int killerId, int victimId)
	{
		idToEnemyKills[killerId] += 1;


		Player killerPlayer = DuelController.Instance.idToPlayer[killerId];
		Player victimPlayer = DuelController.Instance.idToPlayer[victimId];

		DuelELOController.CalculateElo(killerPlayer, victimPlayer, 1, 0);

		idToAlive[victimId] = false;
		teamIDToAlive[idToTeam[victimId]] -= 1;
		RoundEndCheck();
	}

	public void RoundEndCheck()
	{
		if (teamIDToAlive[1] <= 0)
		{
			EndDuel(1);
			return;
		}
		if (teamIDToAlive[2] <= 0)
		{
			EndDuel(2);
			return;
		}
	}

	public void EndDuel(int winningTeamID)
	{
		for (int i = 0; i < team1.Count; i++)
		{
			Matchmaking.Instance.IdToGroupfight.Remove(team1[i].ID);
			Matchmaking.Instance.IdToGroupfight.Remove(team2[i].ID);

			team1[i].currentState = PlayerStates.idle;
			team2[i].currentState = PlayerStates.idle;

			if (team1[i].wantsToMatchMake)
			{
				team1[i].AddToMatchMaking();
				team1[i].MakePlayerLost();
			}
			if (team2[i].wantsToMatchMake)
			{
				team2[i].AddToMatchMaking();
				team2[i].MakePlayerLost();
			}

			//winning team gets extra elo added for every person.
			if (winningTeamID == 1)
			{
				//team 1 won
				DuelELOController.CalculateTeamEloChange(team1[i], team2[i], team1ELO, team2ELO, 1,0);
			}
			else
			{
				//team 2 won
				DuelELOController.CalculateTeamEloChange(team1[i], team2[i], team1ELO,team2ELO, 0, 1);
			}
		}


		DuelController.Instance.CurrentDuelArenasInUse.Remove(arenaInuse);
		DuelController.Instance.CurrentDuelArenasIdle.Add(arenaInuse);
	}
}
