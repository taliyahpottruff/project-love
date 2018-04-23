using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    public static Stack<DateObject> dateSequence = new Stack<DateObject>();
    public static DateObject currentDate = null;
    public static int gameLength = 0;
    private static string lastScene = "";

    public static Dictionary<string, DateObject> dateDictionary = new Dictionary<string, DateObject>() {
        {"Boss", new DateObject("Boss", 10, 1, 1, 1) },
        {"Date_1", new DateObject("Date_4" , 1, 0, 0, 0)},
        {"Date_2", new DateObject("Date_3" , 1, 0, 0, 0)},
        {"Date_4", new DateObject("Date_1" , 1, 0, 0, 0)},
        {"Date_3", new DateObject("Date_2" , 1, 0, 0, 0)}
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
    public float hatesFlirting;
    public float hatesJokes;
    public float hatesStory;
    
    public DateObject(string spriteName, float difficultyModifier, float hatesFlirting, float hatesJokes, float hatesStory) {
        sprite = Resources.Load<Sprite>("Sprites/Dates/" + spriteName);
        difficulty = difficultyModifier;
        this.hatesFlirting = hatesFlirting;
        this.hatesJokes = hatesJokes;
        this.hatesStory = hatesStory;
    }
}