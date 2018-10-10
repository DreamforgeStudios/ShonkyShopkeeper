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
    public int PointsGained;
    public int GoldGained;

    public PostRoundInfo(int playerIdx, int roundNumber, string gameSceneName, int points = 0, int gold = 0) {
        PlayerIndex = playerIdx;
        RoundNumber = roundNumber;
        GameSceneName = gameSceneName;
        PointsGained = points;
        GoldGained = gold;
    }

    public PostRoundInfo(RoundInfo roundInfo, int points = 0, int gold = 0) {
        PlayerIndex = roundInfo.PlayerIndex;
        RoundNumber = roundInfo.RoundNumber;
        GameSceneName = roundInfo.GameSceneName;
        PointsGained = points;
        GoldGained = gold;
    }
}

public class PlayerInfo {
    public int Index;
    public Sprite Avatar;
    public Item.GemType GemType;
    public float Points;
    public int Gold;

    public PlayerInfo(int idx, Sprite avatar, Item.GemType gemType = Item.GemType.Ruby, float startPoints = 0, int startGold = 0) {
        Index = idx;
        Avatar = avatar;
        GemType = gemType;
        Points = startPoints;
        Gold = startGold;
    }
}
