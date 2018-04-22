using UnityEngine;

public class MusicPlayer : MonoBehaviour {
    private void Start() {
        DontDestroyOnLoad(this.gameObject);
    }
}