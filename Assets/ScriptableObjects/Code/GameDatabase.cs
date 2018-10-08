using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Game {
    public string Name;
    public string Description;
    public string SceneName;
    public Sprite Screenshot;
}

[CreateAssetMenu(menuName = "GameDatabase", fileName = "GameDatabase.asset")]
public class GameDatabase : ScriptableObject {
    public List<Game> Games;

    public int GameCount {
        get { return Games.Count; }
    }
}
