using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelInfomation 
{
	public Player challenger;
	public Player challenged;

	public int roundsToPlay = 0;

	public Vector3 challengerSpawnLocation;
	public Vector3 challengedSpawnLocation;

	public Dictionary<int, int> playerDeaths;

	DuelArena arenaInuse;


	public DuelInfomation(Player p1, Player p2, int rounds, Vector3 spawn1, Vector3 spawn2, DuelArena arena)
	{
		challenger = p1;
		challenged = p2;
		roundsToPlay = rounds;
		challengerSpawnLocation = spawn1;
		challengedSpawnLocation = spawn2;

		playerDeaths = new Dictionary<int, int>();
		playerDeaths.Add(p1.ID, 0);
		playerDeaths.Add(p2.ID, 0);

		arenaInuse = arena;
	}



	public void EndDuel()
	{
		Matchmaking.Instance.IdToDuel.Remove(challenger.ID);
		Matchmaking.Instance.IdToDuel.Remove(challenged.ID);

		DuelController.Instance.CurrentDuelArenasInUse.Remove(arenaInuse);
		DuelController.Instance.CurrentDuelArenasIdle.Add(arenaInuse);

		challenger.currentState = PlayerStates.idle;
		challenged.currentState = PlayerStates.idle;

		DuelELOController.CalculateElo(challenger,challenged,playerDeaths[challenged.ID],playerDeaths[challenger.ID]);


		string finalMessage = string.Format("Duel finished. \n" +
			challenger.Name + ": " + playerDeaths[challenged.ID] + " \n" +
			challenged.Name + ": " + playerDeaths[challenger.ID]);

		DuelConsoleController.privateMessage(challenger.ID, finalMessage);
		DuelConsoleController.privateMessage(challenged.ID, finalMessage);

		if (challenged.wantsToMatchMake)
		{
			challenged.AddToMatchMaking();
			challenged.MakePlayerLost();
		}
		if (challenger.wantsToMatchMake)
		{
			challenger.AddToMatchMaking();
			challenger.MakePlayerLost();
		}

	}
}
