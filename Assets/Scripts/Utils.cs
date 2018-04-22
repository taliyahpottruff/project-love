using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour {
    public static string GetRandomDateSpriteName() {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Dates");
        int randomIndex = Random.Range(0, sprites.Length);
        return sprites[randomIndex].name;
    }
}