using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    private void GenerateDateSequence(int length) {
        Game.dateSequence.Clear();
        Game.gameLength = length;

        for (int i = 0; i < length; i++) {
            string spriteName = Utils.GetRandomDateSpriteName();
            Debug.Log(spriteName);
            DateObject d = new DateObject(spriteName, length - i);
            Game.dateSequence.Push(d);
        }
    }

    public void StartGame(int length) {
        GenerateDateSequence(length);
        Game.LoadNextDate();
        SceneManager.LoadScene("Battle");
    }

    public void StartSingle() {
        StartGame(1);
    }
}