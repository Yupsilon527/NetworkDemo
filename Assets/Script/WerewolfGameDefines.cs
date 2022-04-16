using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WerewolfGameDefines
{
    public static string GamePhase = "GamePhase";
    public static string VotedPlayer = "VotedPlayer";
    public static string DayCycle = "DayCycle";
    public static string PhaseCountdown = "PhaseCountdown";

    public static string PlayerClass = "PlayerClass";
    public static string PlayerVote = "VoteTarget";
    public static string PlayerState = "WasKilled";
    public static string PlayerLoading = "FinishedLoading";
    public static string PlayerPortrait = "CustomPortrait";
    public static string PlayerVoteYes = "Y";
    public static string PlayerVoteNo = "N";

    public static float UIAbilityDelayTime = 3;
    public static float UIDeathPanelTime = 3;

    public static float PregameTime = 3;
    public static float PhaseTimeShort = 5;
    public static float DayTimeDuration = 300;
    public static float NightTimeDuration = 60;

    public enum PlayerAliveState
    {
        alive = 0,
        lynched = 1,
        mauled = 2
    }
}
