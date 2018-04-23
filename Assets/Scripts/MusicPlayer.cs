using UnityEngine;

public class MusicPlayer : MonoBehaviour {
    public AudioClip bossIntro;
    public AudioClip bossSong;

    AudioSource src;

    private void Start() {
        src = GetComponent<AudioSource>();
        DontDestroyOnLoad(this.gameObject);
    }

    public void PlayBossIntro() {
        src.Stop();
        src.clip = bossIntro;
        src.Play();
    }

    public void PlayBossSong() {
        src.Stop();
        src.clip = bossSong;
        src.Play();
    }
}