﻿using System.Collections;
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

public class PlayerInfo {
    public int Index;
    public Sprite Avatar;
    public Item.GemType GemType;
    public float Points;
    public int Gold;

    public PlayerInfo(int idx, Sprite avatar, Item.GemType gemType = Item.GemType.Ruby, float startPoints = 0, int startGold = 0) {
        Index = idx;
        Avatar = avatar;
        Points = startPoints;
        Gold = startGold;
    }
}
