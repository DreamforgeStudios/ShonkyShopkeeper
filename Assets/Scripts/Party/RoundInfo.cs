using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundInfo {
    public int PlayerIndex;
    public int RoundNumber;
    public string GameSceneName;

    public RoundInfo(int playerIdx, int roundNumber, string gameSceneName) {
        PlayerIndex = playerIdx;
        RoundNumber = roundNumber;
        GameSceneName = gameSceneName;
    }
}

public class PostRoundInfo {
    public int PlayerIndex;
    public int RoundNumber;
    public string GameSceneName;
    public float PointsGained;
    public int GoldGained;

    public PostRoundInfo(int playerIdx, int roundNumber, string gameSceneName, float points = 0, int gold = 0) {
        PlayerIndex = playerIdx;
        RoundNumber = roundNumber;
        GameSceneName = gameSceneName;
        PointsGained = points;
        GoldGained = gold;
    }

    public PostRoundInfo(RoundInfo roundInfo, float points = 0, int gold = 0) {
        PlayerIndex = roundInfo.PlayerIndex;
        RoundNumber = roundInfo.RoundNumber;
        GameSceneName = roundInfo.GameSceneName;
        PointsGained = points;
        GoldGained = gold;
    }
}


public class PlayerInfo {
    public int Index;
    public Avatar Avatar;
    public float Points;
    public int Gold;

    private const int GOLD_MULTIPLIER = 20;

    public float AggregatePoints {
        get { return Points + Gold * GOLD_MULTIPLIER; }
    }

    public PlayerInfo(int idx, Avatar avatar, float startPoints = 0, int startGold = 0) {
        Index = idx;
        Avatar = avatar;
        Points = startPoints;
        Gold = startGold;
    }
}
