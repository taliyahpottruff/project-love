using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    public static Stack<DateObject> dateSequence = new Stack<DateObject>();
    public static DateObject currentDate = null;
    public static int gameLength = 0;
    private static string lastScene = "";

    public static Dictionary<string, DateObject> dateDictionary = new Dictionary<string, DateObject>() {
        {"Boss", new DateObject("pixel", 10) },
        {"Brunnette_1", new DateObject("Brunnette_1" , 1)},
        {"Blonde_1", new DateObject("Blonde_1" , 1)},
        {"Asian_1", new DateObject("Asian_1" , 1)},
        {"Black_1", new DateObject("Black_1" , 1)}
    };

    public static void LoadNextDate() {
        currentDate = dateSequence.Pop();
    }

    public static void SetLastScene(string scene) {
        lastScene = scene;
    }

    /// <summary>
    /// Checks to see if the last scene is the one specified. If true, the last scene is cleared.
    /// </summary>
    /// <param name="scene">The scene to check for</param>
    /// <returns>Whether or not the last scene is the one specified</returns>
    public static bool IsLastScene(string scene) {
        if (lastScene.Equals(scene)) {
            lastScene = "";
            return true;
        }

        return false;
    }

    public static DateObject GetDate(string id, float modifier) {
        DateObject d = dateDictionary[id];
        d.difficulty = modifier;
        return d;
    }
}

public class DateObject {
    public Sprite sprite;
    public float difficulty;
    
    public DateObject(string spriteName, float difficultyModifier) {
        sprite = Resources.Load<Sprite>("Sprites/Dates/" + spriteName);
        difficulty = difficultyModifier;
    }
}