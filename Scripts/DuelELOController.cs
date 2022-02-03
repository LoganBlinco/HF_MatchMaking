using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelELOController
{
	public static float DEFAULT_ELO = 1000;

    private static float mod = 400;
    //Higher K means less volatile
    private static float K = 48;
    private static float pointsPerWin = 1.0f;
    private static float pointsPerDraw = 0.0f;
    private static float pointsPerLoss = 0.0f;


    public static void CalculateElo(Player A, Player B, int AKills, int BKills)
    {

        float expectedScoreA = 1 / (1 + Mathf.Pow(10, (B.ELO - A.ELO) / mod));
        float expectedScoreB = 1 / (1 + Mathf.Pow(10, (A.ELO - B.ELO) / mod));

        float pointsA = AKills / (AKills + BKills);
        float pointsB = BKills / (AKills + BKills);

        A.SetElo(A.ELO + Change(pointsA, expectedScoreA));
        B.SetElo(B.ELO + Change(pointsB, expectedScoreB));
    }

    public static void CalculateTeamEloChange(Player toAdd, Player toSubtract, float teamAElo, float teamBElo, float teamAScore, float teamBScore)
    {

        float expectedScoreA = 1 / (1 + Mathf.Pow(10, (teamBElo - teamAElo) / mod));
        float expectedScoreB = 1 / (1 + Mathf.Pow(10, (teamAElo - teamBElo) / mod));

        float pointsA = teamAScore / (teamAScore + teamBScore);
        float pointsB = teamBScore / (teamAScore + teamBScore);

        toAdd.SetElo(toAdd.ELO + Change(teamAScore, expectedScoreA));
        toSubtract.SetElo(toSubtract.ELO + Change(teamBScore, expectedScoreB));
    }

    public static float Change(float points, float expectedScore)
    {
        return K * (points - expectedScore);
    }
}
