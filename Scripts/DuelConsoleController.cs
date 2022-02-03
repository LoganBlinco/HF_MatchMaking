using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DuelConsoleController : MonoBehaviour
{
	public static InputField f1MenuInputField;

	public static void broadcast(string message)
	{
		if (f1MenuInputField == null) { return; }

		var rcCommand = string.Format("broadcast {0}", message);
		f1MenuInputField.onEndEdit.Invoke(rcCommand);
		//Debug.Log("running command: " + rcCommand);
	}


	public static void privateMessage(int id, string message)
	{
		if (f1MenuInputField == null) { return; }

		if (DuelController.Instance.idToPlayer[id].isBot) { return; }

		var rcCommand = string.Format("serverAdmin privateMessage {0} {1}", id, message);
		f1MenuInputField.onEndEdit.Invoke(rcCommand);
		//Debug.Log("running command: " + rcCommand);
	}

	public static void privateMessage_delayed(int id, string message)
	{
		float DELAY = 1f;

		if (f1MenuInputField == null) { return; }

		if (DuelController.Instance.idToPlayer[id].isBot) { return; }


		var rcCommand = string.Format("delayed {2} serverAdmin privateMessage {0} {1}", id, message, DuelController.Instance.currentTime - DELAY);
		f1MenuInputField.onEndEdit.Invoke(rcCommand);
		//Debug.Log("running command: " + rcCommand);
	}


	public static void slap(int id, int amount)
	{
		if (f1MenuInputField == null) { return; }

		var rcCommand = string.Format("serverAdmin slap {0} {1}", id, amount);
		f1MenuInputField.onEndEdit.Invoke(rcCommand);
		//Debug.Log("running command: " + rcCommand);
	}
	public static void slap_delayed(int id, int amount)
	{
		float DELAY = 1f;

		if (f1MenuInputField == null) { return; }

		var rcCommand = string.Format("delayed {2} serverAdmin slap {0} {1}", id, amount, DuelController.Instance.currentTime - DELAY);
		f1MenuInputField.onEndEdit.Invoke(rcCommand);
		//Debug.Log("running command: " + rcCommand);
	}

	public static void teleport(int iD, Vector3 spawn1)
	{
		float DELAY = 1f;

		if (f1MenuInputField == null) { return; }

		var rcCommand = string.Format("delayed {4} teleport {0} {1},{2},{3}", iD, spawn1.x,spawn1.y,spawn1.z,DuelController.Instance.currentTime - DELAY);
		f1MenuInputField.onEndEdit.Invoke(rcCommand);
		//Debug.Log("running command: " + rcCommand);
	}
	public static void teleport_delayed(int iD, Vector3 spawn1)
	{
		float DELAY = 1f;

		if (f1MenuInputField == null) { return; }

		var rcCommand = string.Format("delayed {4} teleport {0} {1},{2},{3}", iD, spawn1.x, spawn1.y, spawn1.z, DuelController.Instance.currentTime - DELAY);
		f1MenuInputField.onEndEdit.Invoke(rcCommand);
		//Debug.Log("running command: " + rcCommand);
	}
}
