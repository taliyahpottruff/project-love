using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour {
    public static string GetRandomDateSpriteName() {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Dates");
        int randomIndex = Random.Range(0, sprites.Length);
        return sprites[randomIndex].name;
    }

    public static AttackType GetRandomAttackType() {
        int index = Random.Range(0, 4);

        switch(index) {
            case 0:
                return AttackType.FireAtPlayer;
            case 1:
                return AttackType.FireAtPlayerRandom;
            case 2:
                return AttackType.FireHorizontally;
            case 3:
                return AttackType.AlternatingCone;
            default:
                return AttackType.FireAtPlayer;
        }
    }

    public static Vector2 Rotate(Vector2 v, float degrees) {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static string Scramble(string input) {
        string chars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&";
        string newString = "";
        char[] charArray = input.ToCharArray();

        for (int i = 0; i < charArray.Length; i++) {
            bool b = char.IsLetterOrDigit(charArray[i]);
            int index = Random.Range(0, chars.Length);

            if (b) newString += chars[index];
            else newString += charArray[i];
        }

        return newString;
    }
}