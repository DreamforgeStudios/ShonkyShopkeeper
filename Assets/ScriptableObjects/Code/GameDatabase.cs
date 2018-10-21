using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Game {
    public string Name;
    public string Description;
    // Annoyingly, Unity won't let you load a scene by reference, so we have to use name.
    // This could cause some problems when scenes are renamed...
    public string SceneName;
    public Sprite Screenshot;
}

[CreateAssetMenu(menuName = "GameDatabase", fileName = "GameDatabase.asset")]
public class GameDatabase : ScriptableObject {
    public List<Game> Games;

    public int GameCount {
        get { return Games.Count; }
    }

    public Game GetGameBySceneName(string name) {
        return Games.Find(x => x.SceneName == name);
    }
}
