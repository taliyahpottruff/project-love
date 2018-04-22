using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    public static Stack<DateObject> dateSequence = new Stack<DateObject>();
    public static DateObject currentDate = null;
    public static int gameLength = 0;
    private static string lastScene = "";

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
}

public class DateObject {
    public Sprite sprite;
    public float difficulty;
    
    public DateObject(string spriteName, float difficultyModifier) {
        sprite = Resources.Load<Sprite>("Sprites/Dates/" + spriteName);
        difficulty = difficultyModifier;
    }
}