using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelCommandController : MonoBehaviour
{
	private static DuelCommandController _instance;
	public static DuelCommandController Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType<DuelCommandController>();
			}
			return _instance;
		}
	}

	public Dictionary<string, Action<passedVar>> commands;

	public void Start()
	{
		commands = new Dictionary<string, Action<passedVar>>()
		{
			{"duel",command_duel },
			{"leave",command_leave },
			{"join",command_join },
			{"ELO",command_elo },
			{"elo",command_elo },
			{"scoreboard",command_scoreboard },
			{"status",command_state },
			{"priority",command_priority },

			{"help",command_help }

		};
	}

	private void command_priority(passedVar obj)
	{
		int id = obj.id;
		string[] data = obj.data;

		if (data.Length <= 1)
		{
			getPriority(id);
			return;
		}
		//change priority.
		string[] enumArray = Enum.GetNames(typeof(PlayerPriority));
		if (DuelHelper.CustomContains(enumArray,data[1]))
		{
			Player p;
			if (DuelController.Instance.idToPlayer.TryGetValue(id,out p))
			{
				p.matchMakingPriority = (PlayerPriority)Enum.Parse(typeof(PlayerPriority), data[1]);
				string msg = "Your new priority is: " + p.matchMakingPriority;
				DuelConsoleController.privateMessage(id, msg);
			}
		}
	}
	private void getPriority(int id)
	{
		Player p;
		if (DuelController.Instance.idToPlayer.TryGetValue(id,out p))
		{
			string msg = "Your priority is: " + p.matchMakingPriority;
			DuelConsoleController.privateMessage(id, msg);
		}
	}


	private void command_state(passedVar obj)
	{
		int id = obj.id;

		Player p;
		if (DuelController.Instance.idToPlayer.TryGetValue(id,out p))
		{
			string message = "Your current Status is: " + p.currentState;
			DuelConsoleController.privateMessage(id, message);
			return;
		}
		DuelConsoleController.privateMessage(id, "Error, was not able to find Player instance");
		Debug.Log("player with ID: " + id + " was not found for state output");

	}

	private void command_help(passedVar obj)
	{
		string msg = "Current commands \n";
		foreach(var pair in commands)
		{
			msg += pair.Key + " \n";
		}
		msg += "some commands may not be possible without admin perms";
		DuelConsoleController.privateMessage(obj.id, msg);
	}

	private void command_scoreboard(passedVar obj)
	{
		int id = obj.id;
		Player temp;
		if (DuelController.Instance.idToPlayer.TryGetValue(id,out temp) && temp.isAdmin)
		{
			Matchmaking.Instance.PostTopELOScores();
		}
	}

	private void command_elo(passedVar obj)
	{
		//output elo to the player
		int id = obj.id;

		Player p;
		if (DuelController.Instance.idToPlayer.TryGetValue(id,out p))
		{
			string message = "Your current ELO is: " + p.ELO;
			DuelConsoleController.privateMessage(id, message);
			return;
		}
		DuelConsoleController.privateMessage(id, "Error, was not able to find Player Object");
		Debug.Log("player with ID: " + id + " was not found for ELO output");


	}

	private void command_join(passedVar data)
	{
		int id = data.id;
		string[] input = data.data;

		Player temp;
		if (DuelController.Instance.idToPlayer.TryGetValue(id, out temp))
		{
			temp.AddToMatchMaking();
		}
	}

	private void command_leave(passedVar data)
	{
		int id = data.id;
		string[] input = data.data;

		Player temp;
		if (DuelController.Instance.idToPlayer.TryGetValue(id,out temp))
		{
			temp.RemoveFromMatchMaking();
		}
	}

	public static void command_duel(passedVar data)
	{
		int id = data.id;
		string[] input = data.data;
		string message = "Make sure to use command as /duel [playerID]. You can get the ID from the P menu.";
		if (input.Length == 1) { DuelConsoleController.privateMessage(id, message); return; }
		Player temp;
		if (!DuelController.Instance.idToPlayer.TryGetValue(id,out temp)) { Debug.LogWarning("Did not find: " + id); return; }
		//input must be just a id.
		int challengedID;
		if (int.TryParse(input[1], out challengedID))
		{
			if (DuelController.Instance.idToPlayer.ContainsKey(challengedID))
			{
				//now need to determine the rounds.
				int rounds = 1;//defualt
				if (input.Length >= 3 && int.TryParse(input[2], out rounds) && rounds >= 1)
				{
					temp.command_duel(challengedID,rounds);
					return;
				}
				temp.command_duel(challengedID, rounds);

			}
		}
		else
		{
			DuelConsoleController.privateMessage(id, message);
		}
	}
}

public class SetConsoleCommandVariable
{
    public string Description;
    public Action<object> SetAction;
    public Action<object> GetAction;
}

public struct passedVar
{
	public string[] data;
	public int id;
}