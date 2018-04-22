using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    public static Stack<DateObject> dateSequence = new Stack<DateObject>();
    public static DateObject currentDate = null;
    public static int gameLength = 0;

    public static void LoadNextDate() {
        currentDate = dateSequence.Pop();
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