using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MainMenu : MonoBehaviour {
    public AudioClip buttonSound;

    AudioSource src;

    private void Start() {
        src = GetComponent<AudioSource>();
    }

    private void GenerateDateSequence(int length) {
        Game.dateSequence.Clear();
        Game.gameLength = length;

        Game.dateSequence.Push(Game.dateDictionary["Boss"]);
        for (int i = 1; i < length; i++) {
            string spriteName = Utils.GetRandomDateSpriteName();
            Debug.Log(spriteName);
            DateObject d = Game.GetDate("Date_" + i, length - i);
            Game.dateSequence.Push(d);
        }
    }

    public void StartGame(int length) {
        GenerateDateSequence(length);
        Game.LoadNextDate();
        Game.SetLastScene("Menu");
        SceneManager.LoadScene("Battle");
    }

    public void StartSingle() {
        StartGame(1);
    }

    public void PlaySound() {
        src.PlayOneShot(buttonSound);
    }
}