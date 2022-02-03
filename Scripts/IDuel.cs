using HoldfastSharedMethods;
using UnityEngine;
using UnityEngine.UI;

public class IDuel : IHoldfastSharedMethods
{
    bool isServer = false;
    bool isClient = false;

    public void OnIsServer(bool server)
    {
        isServer = server;

        //Code from Wrex: https://github.com/CM2Walki/HoldfastMods/blob/master/NoShoutsAllowed/NoShoutsAllowed.cs

        //Get all the canvas items in the game
        var canvases = Resources.FindObjectsOfTypeAll<Canvas>();

        for (int i = 0; i < canvases.Length; i++)
        {

            //Find the one that's called "Game Console Panel"
            if (string.Compare(canvases[i].name, "Game Console Panel", true) == 0)
            {
                //Inside this, now we need to find the input field where the player types messages.
                DuelConsoleController.f1MenuInputField = canvases[i].GetComponentInChildren<InputField>(true);
                if (DuelConsoleController.f1MenuInputField != null)
                {
                    Debug.Log("Found the Game Console Panel");
                }
                else
                {
                    Debug.Log("We did Not find Game Console Panel");
                }
                //break;
            }
        }
    }

    public void OnIsClient(bool client, ulong steamId)
    {
        isClient = client;
    }

    public void PassConfigVariables(string[] value)
    {
        if (isServer == false || isClient == true) { return; }
        DuelConfigVariables.PassConfigVariables(value);
        Debug.Log("finished passing config variables");
    }



    public void OnPlayerJoined(int playerId, ulong steamId, string playerName, string regimentTag, bool isBot)
    {
        if (isServer == false || isClient == true) { return; }

        DuelController.Instance.OnPlayerJoined(playerId, steamId, playerName, regimentTag, isBot);
    }

    public void OnPlayerLeft(int playerId)
    {
        if (isServer == false || isClient == true) { return; }
        DuelController.Instance.OnPlayerLeft(playerId);
    }



    public void OnPlayerSpawned(int playerId, int spawnSectionId, FactionCountry playerFaction, PlayerClass playerClass, int uniformId, GameObject playerObject)
    {
        if (isServer == false || isClient == true) { return; }
        DuelController.Instance.OnPlayerSpawned(playerId, spawnSectionId, playerFaction, playerClass, uniformId, playerObject);
    }

    public void OnTextMessage(int playerId, TextChatChannel channel, string text)
    {
        if (isServer == false || isClient == true) { return; }
        DuelController.Instance.OnTextMessage(playerId, channel, text);
    }

    public void OnPlayerHurt(int playerId, byte oldHp, byte newHp, EntityHealthChangedReason reason)
    {
        if (isServer == false || isClient == true) { return; }
        DuelController.Instance.OnPlayerHurt(playerId, newHp);
    }

    public void OnPlayerKilledPlayer(int killerPlayerId, int victimPlayerId, EntityHealthChangedReason reason, string additionalDetails)
    {
        if (isServer == false || isClient == true) { return; }
        DuelController.Instance.OnPlayerKilledPlayer(killerPlayerId, victimPlayerId);
    }

    public void OnUpdateTimeRemaining(float time)
    {
        if (isServer == false || isClient == true) { return; }

        if ((int)time != DuelController.Instance.currentTime)
        {
            DuelController.Instance.currentTime = (int)time;
        }
    }

    public void OnRCLogin(int playerId, string inputPassword, bool isLoggedIn)
    {
        if (isServer == false || isClient == true) { return; }

        if (!isLoggedIn) { return; }
        Player temp;
        if (DuelController.Instance.idToPlayer.TryGetValue(playerId,out temp))
        {
            temp.isAdmin = true;
        }
    }

    public void OnRCCommand(int playerId, string input, string output, bool success)
    {
        if (isServer == false || isClient == true) { return; }

        if (!success) { return; }
        Player temp;
        if (DuelController.Instance.idToPlayer.TryGetValue(playerId, out temp))
        {
            temp.isAdmin = true;
        }
        Debug.LogFormat("OnRCCommand {0} {1} {2} {3}", playerId, input, output, success);

    }


    #region vlaues that wont be used
    public void OnSyncValueState(int value)
    {
        DuelController.Instance.InitializeRandom(value);
    }

    public void OnDamageableObjectDamaged(GameObject damageableObject, int damageableObjectId, int shipId, int oldHp, int newHp)
    {
    }

    #endregion


    public void OnUpdateSyncedTime(double time)
    {
        //Debug.LogWarningFormat("OnUpdateSyncedTime {0}", time);
    }

    public void OnUpdateElapsedTime(float time)
    {
        //Debug.LogWarningFormat("OnUpdateElapsedTime {0}", time);
    }








    public void OnPlayerShoot(int playerId, bool dryShot)
    {
    }






    public void OnScorableAction(int playerId, int score, ScorableActionType reason)
    {
    }

    public void OnRoundDetails(int roundId, string serverName, string mapName, FactionCountry attackingFaction, FactionCountry defendingFaction, GameplayMode gameplayMode, GameType gameType)
    {
    }

    public void OnPlayerBlock(int attackingPlayerId, int defendingPlayerId)
    {
    }

    public void OnPlayerMeleeStartSecondaryAttack(int playerId)
    {
    }

    public void OnPlayerWeaponSwitch(int playerId, string weapon)
    {
    }

    public void OnCapturePointCaptured(int capturePoint)
    {
    }

    public void OnCapturePointOwnerChanged(int capturePoint, FactionCountry factionCountry)
    {
    }

    public void OnCapturePointDataUpdated(int capturePoint, int defendingPlayerCount, int attackingPlayerCount)
    {
    }

    public void OnRoundEndFactionWinner(FactionCountry factionCountry, FactionRoundWinnerReason reason)
    {
    }

    public void OnRoundEndPlayerWinner(int playerId)
    {
    }

    public void OnPlayerStartCarry(int playerId, CarryableObjectType carryableObject)
    {
    }

    public void OnPlayerEndCarry(int playerId)
    {
    }

    public void OnPlayerShout(int playerId, CharacterVoicePhrase voicePhrase)
    {
    }

    public void OnInteractableObjectInteraction(int playerId, int interactableObjectId, GameObject interactableObject, InteractionActivationType interactionActivationType, int nextActivationStateTransitionIndex)
    {
    }



    public void OnEmplacementPlaced(int itemId, GameObject objectBuilt, EmplacementType emplacementType)
    {
    }

    public void OnEmplacementConstructed(int itemId)
    {
    }

    public void OnBuffStart(int playerId, BuffType buff)
    {
    }

    public void OnBuffStop(int playerId, BuffType buff)
    {
    }

    public void OnShotInfo(int playerId, int shotCount, Vector3[][] shotsPointsPositions, float[] trajectileDistances, float[] distanceFromFiringPositions, float[] horizontalDeviationAngles, float[] maxHorizontalDeviationAngles, float[] muzzleVelocities, float[] gravities, float[] damageHitBaseDamages, float[] damageRangeUnitValues, float[] damagePostTraitAndBuffValues, float[] totalDamages, Vector3[] hitPositions, Vector3[] hitDirections, int[] hitPlayerIds, int[] hitDamageableObjectIds, int[] hitShipIds, int[] hitVehicleIds)
    {
    }

    public void OnVehicleSpawned(int vehicleId, FactionCountry vehicleFaction, PlayerClass vehicleClass, GameObject vehicleObject, int ownerPlayerId)
    {
    }

    public void OnVehicleHurt(int vehicleId, byte oldHp, byte newHp, EntityHealthChangedReason reason)
    {
    }

    public void OnPlayerKilledVehicle(int killerPlayerId, int victimVehicleId, EntityHealthChangedReason reason, string details)
    {
    }

    public void OnShipSpawned(int shipId, GameObject shipObject, FactionCountry shipfaction, ShipType shipType, int shipNameId)
    {
    }

    public void OnShipDamaged(int shipId, int oldHp, int newHp)
    {
    }

    public void OnAdminPlayerAction(int playerId, int adminId, ServerAdminAction action, string reason)
    {
    }

    public void OnConsoleCommand(string input, string output, bool success)
    {
        if (isServer == false || isClient == true) { return; }
    }




}