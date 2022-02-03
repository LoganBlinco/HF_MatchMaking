using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Matchmaking : MonoBehaviour
{
	public static float MatchMakeFreq = 15;
	public static int MatchMakeDefaultRounds = 1;
	public static int amountOfScoresToPost = 5;
	public static int ScorePostFrequency = 4 * 60;//5 minutes


	private static Matchmaking _instance;
	public static Matchmaking Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType<Matchmaking>();
			}
			return _instance;
		}
	}

	public List<int> playersWaitingForDuels;
	public List<int> playersWaitingForGroupfight;
	public List<int> playersLost;

	public Dictionary<int, DuelInfomation> IdToDuel;
	public Dictionary<int, GroupfightInfo> IdToGroupfight;

	List<int> sortedELOID;
	List<float> sortedELOValue;

	public bool MatchMakingEnabled = true;

	public MoshPit moshPit;


	public void Start()
	{
		moshPit = GameObject.FindObjectOfType<MoshPit>();
		playersLost = new List<int>();
		playersWaitingForDuels = new List<int>();
		playersWaitingForGroupfight = new List<int>();
		IdToDuel = new Dictionary<int, DuelInfomation>();
		IdToGroupfight = new Dictionary<int, GroupfightInfo>();

		sortedELOID = new List<int>();
		sortedELOValue = new List<float>();

		float MatchMakingStartDelay = 8;

		float PostScoreDelay = 4 * 60;


		if(MatchMakingEnabled)
		{
			//InvokeRepeating("checkForMatchMaking", MatchMakingStartDelay, MatchMakeFreq);
			InvokeRepeating("GeneralMatchmakingCheck", MatchMakingStartDelay, MatchMakeFreq);
			InvokeRepeating("PostTopELOScores", PostScoreDelay, ScorePostFrequency);
		}
	}

	public void PostTopELOScores()
	{
		string initialMessage = "Top 5 ELO Scores are currently: ";
		string finalMsg = initialMessage;
		for (int i =0;i<amountOfScoresToPost;i++)
		{
			if (i >= sortedELOID.Count || i >= sortedELOValue.Count) { break; }
			int id = sortedELOID[i];
			float elo = sortedELOValue[i];
			string name = DuelController.Instance.idToPlayer[id].Name;
			finalMsg += "\n " +
				(i+1).ToString() + ". " + name + " - " + elo.ToString();
		}
		DuelConsoleController.broadcast(finalMsg);
	}




	public void PlayerDied(int killerID, int victimID)
	{
		if (PlayerDiedInDuel(killerID, victimID)) { return; }

		if (PlayerDiedInGroupFight(killerID, victimID)) { return; }

		Debug.Log("not from duel or groupfight2");
	}

	private bool PlayerDiedInGroupFight(int killerID, int victimID)
	{
		//check if both of these are in a duel.
		GroupfightInfo temp;
		if (!IdToGroupfight.TryGetValue(killerID, out temp))
		{
			return false; //
		}
		GroupfightInfo temp2;
		if (!IdToGroupfight.TryGetValue(victimID, out temp2))
		{
			return false; //

		}

		//check if player is already dead
		if (!temp.idToAlive[victimID]) { return false; }

		//check if team kill or not.
		if (temp.idToTeam[victimID] == temp.idToTeam[killerID] )
		{
			//TEAM KILL!
			temp.ApplyTeamKill(killerID, victimID);
			return true;
		}
		else
		{
			//not a team kill.
			temp.ApplyKill(killerID, victimID);
			return true;
		}

	}

	public bool PlayerDiedInDuel(int killerID, int victimID)
	{
		//check if both of these are in a duel.
		DuelInfomation temp;
		if (!IdToDuel.TryGetValue(killerID,out temp))
		{
			return false; //
		}
		DuelInfomation temp2;
		if (!IdToDuel.TryGetValue(victimID, out temp2))
		{
			return false; //

		}
		temp.playerDeaths[victimID] += 1;
		if (temp.playerDeaths[victimID] >= temp.roundsToPlay)
		{
			temp.EndDuel();
			//we can end the fight.
		}
		return true;
	}


	internal void AddToMatchMaking(Player player)
	{
		switch(player.matchMakingPriority)
		{
			case PlayerPriority.Both:
				playersWaitingForDuels.Add(player.ID);
				playersWaitingForGroupfight.Add(player.ID);
				break;
			case PlayerPriority.Duel:
				playersWaitingForDuels.Add(player.ID);
				break;
			case PlayerPriority.Groupfight:
				playersWaitingForGroupfight.Add(player.ID);
				break;
		}
	}
	public void RemoveFromMatchMakingPool(int id)
	{
		Player player;
		if (!DuelController.Instance.idToPlayer.TryGetValue(id, out player))
		{
			return;
		}

			
		switch (player.matchMakingPriority)
		{
			case PlayerPriority.Both:
				playersWaitingForDuels.Remove(id);
				playersWaitingForGroupfight.Remove(id);
				playersLost.Remove(id);
				break;
			case PlayerPriority.Duel:
				playersWaitingForDuels.Remove(id);
				playersLost.Remove(id);
				break;
			case PlayerPriority.Groupfight:
				playersWaitingForGroupfight.Remove(id);
				playersLost.Remove(id);
				break;
		}

		DuelInfomation duelInfo;
		if (IdToDuel.TryGetValue(id,out duelInfo))
		{
			duelInfo.EndDuel();
		}

		GroupfightInfo groupfightINfo;
		if (IdToGroupfight.TryGetValue(id, out groupfightINfo))
		{
			groupfightINfo.EndDuel(1);
		}
	}

	private void GeneralMatchmakingCheck()
	{
		if (!MatchMakingEnabled) { return; }


		if (playersWaitingForGroupfight.Count >= playersWaitingForDuels.Count)
		{
			//gropfight then duels
			GroupFightMatchMaking();
			DuelMatchMaking();
			DuelMatchMaking();
		}
		else
		{
			//duels then groupfights.
			DuelMatchMaking();
			DuelMatchMaking();
			GroupFightMatchMaking();
		}
		if (playersWaitingForDuels.Count >= 3 || playersWaitingForGroupfight.Count >= 4)
		{
			GeneralMatchmakingCheck();
		}

		//check players that are lost.
		
		//perform secondary check.
		for (int i =0;i<playersLost.Count;i++)
		{
			int _id = playersLost[i];
			Vector3 pos = moshPit.GetLocation().transform.position;
			DuelConsoleController.teleport_delayed(_id, pos);
		}
		playersLost = new List<int>();
	}


	private void GroupFightMatchMaking()
	{
		int MAX_GROUPFIGHT_SIZE = 5;

		if (playersWaitingForGroupfight.Count < 4) { return; }
		int ceilTeamSize = Mathf.FloorToInt(playersWaitingForGroupfight.Count / 2);

		int teamSize = Random.Range(2,Mathf.Clamp(ceilTeamSize,2,MAX_GROUPFIGHT_SIZE)+1);

		List<int> team1 = new List<int>();
		List<int> team2 = new List<int>();
		List<Player> team1Player = new List<Player>();
		List<Player> team2Player = new List<Player>();

		List<int> temp = new List<int>(playersWaitingForGroupfight);

		for (int i = 0; i < teamSize; i++)
		{
			team1.Add(temp[2 * i]);
			team1Player.Add(DuelController.Instance.idToPlayer[temp[2 * i]]);

			team2.Add(temp[2 * i + 1]);
			team2Player.Add(DuelController.Instance.idToPlayer[temp[2 * i+1]]);

		}
		for (int i =0;i<teamSize*2;i++)
		{
			RemoveFromMatchMakingPool(playersWaitingForGroupfight[0]);
		}

		BeginGroupFight(team1, team2, 1, team1Player, team2Player);

	}

	private void DuelMatchMaking()
	{
		if (!MatchMakingEnabled) { return; }

		//need at least 2 people for it.
		if (playersWaitingForDuels.Count < 2) { return; }

		//put first person up again a random player.
		int challenger = playersWaitingForDuels[0];
		int challenged = playersWaitingForDuels[Random.Range(1, playersWaitingForDuels.Count)];


		//check that they are both idle.
		if (DuelController.Instance.idToPlayer[challenger].currentState != PlayerStates.idle)
		{
			//remove from pool
			RemoveFromMatchMakingPool(challenger);
			return;
		}
		if (DuelController.Instance.idToPlayer[challenged].currentState != PlayerStates.idle)
		{
			//remove from pool
			RemoveFromMatchMakingPool(challenged);
			return;
		}

		//remove them from pool of matchmaking.
		RemoveFromMatchMakingPool(challenger);
		RemoveFromMatchMakingPool(challenged);
		passedVar data = new passedVar()
		{
			id = challenger,
			data = new string[] { "duel", challenged.ToString(), MatchMakeDefaultRounds.ToString() }
		};
		DuelCommandController.command_duel(data);
	}

	private void checkForMatchMaking()
	{
		DuelMatchMaking();
		if (playersWaitingForDuels.Count >= 4)
		{
			checkForMatchMaking();
		}
	}

	internal void ELOChanged(int iD, float eLO)
	{
		int index = sortedELOID.FindIndex(a => a == iD);
		//does that ID have a value?
		if (index >= 0)
		{
			sortedELOID.RemoveAt(index);
			sortedELOValue.RemoveAt(index);
		}
		//now we need to insert.
		//start at 0, see if elo is bigger and if so insert.
		for(int i =0;i<sortedELOValue.Count;i++)
		{
			//biggest at [0]
			if (eLO >= sortedELOValue[i])
			{
				sortedELOValue.Insert(i, eLO);
				sortedELOID.Insert(i, iD);
				return;
			}
		}
		//gone through the list and my ELO is not bigger thany any so is smallest
		sortedELOID.Add(iD);
		sortedELOValue.Add(eLO);
	}

	private bool BeginGroupFight(List<int> ID, List<int> opponentID, int rounds, List<Player> challenger, List<Player> challenged)
	{
		if (rounds != 1) { Debug.Log("we do not support more than 1 round groupfights");return false; }

		if (DuelController.Instance.CurrentDuelArenasIdle.Count == 0) { return false; }

		if (ID.Count != opponentID.Count) { return false; }


		//we have a open arena. Now lets deal with it.
		DuelArena arenaToUse = DuelController.Instance.CurrentDuelArenasIdle[0];
		DuelController.Instance.CurrentDuelArenasIdle.RemoveAt(0);
		DuelController.Instance.CurrentDuelArenasInUse.Add(arenaToUse);

		

		//initialize variables.
		GroupfightInfo groupfightInfo = new GroupfightInfo(challenger,challenged,rounds, arenaToUse);
		//int teamSize = challenger.Count;
		//string messageBegin = "Begin: "+teamSize+"v"+teamSize+" - Qapla!'";

		
		for (int i =0;i<ID.Count;i++)
		{
			IdToGroupfight[ID[i]] = groupfightInfo;
			IdToGroupfight[opponentID[i]] = groupfightInfo;

			/*
			challenger[i].HealMe();
			challenged[i].HealMe();

			challenger[i].currentState = PlayerStates.groupfighting;
			challenged[i].currentState = PlayerStates.groupfighting;

			groupfightInfo.TeleportPlayer(ID[i]);
			groupfightInfo.TeleportPlayer(opponentID[i]);


			DuelConsoleController.privateMessage_delayed(ID[i], messageBegin);
			DuelConsoleController.privateMessage_delayed(opponentID[i], messageBegin);
			*/
		}
		
		return true;
	}

	internal void BeginDuel(int ID, int opponentID, int rounds, Player challenger, Player challenged)
	{
		if (DuelController.Instance.CurrentDuelArenasIdle.Count == 0)
		{
			DuelConsoleController.privateMessage(ID, "All arenas are currently full. Try again later");
			return;
		}

		//we have a open arena. Now lets deal with it.
		DuelArena arenaToUse = DuelController.Instance.CurrentDuelArenasIdle[0];
		DuelController.Instance.CurrentDuelArenasIdle.RemoveAt(0);
		DuelController.Instance.CurrentDuelArenasInUse.Add(arenaToUse);



		Vector3 spawn1 = arenaToUse.GetLocation(1).transform.position;
		Vector3 spawn2 = arenaToUse.GetLocation(2).transform.position;
		//initialize variables.
		DuelInfomation duelInfo = new DuelInfomation(challenger,challenged,rounds,spawn1,spawn2, arenaToUse);
		//heal player
		challenger.HealMe();
		challenged.HealMe();

		challenger.currentState = PlayerStates.dueling;
		challenged.currentState = PlayerStates.dueling;

		IdToDuel[ID] = duelInfo;
		IdToDuel[opponentID] = duelInfo;

		//teleport players
		DuelConsoleController.teleport(ID, spawn1);
		DuelConsoleController.teleport(opponentID, spawn2);


		//pm players
		int teamSize = 1;
		string messageBegin = "Begin: " + teamSize + "v" + teamSize + " - Qapla!'";

		DuelConsoleController.privateMessage_delayed(ID, messageBegin);
		DuelConsoleController.privateMessage_delayed(opponentID, messageBegin);
	}
}
