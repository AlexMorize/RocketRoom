using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public List<AudioClip> Musiques;

    AudioSource MusicPlayer;
    int indexMusique;

    // Start is called before the first frame update
    void Start()
    {

        MusicPlayer = gameObject.AddComponent<AudioSource>();
        MusicPlayer.volume = .5f;
        PlayRandom();

    }

    private void PlayRandom()
    {
        indexMusique = Random.Range(0, Musiques.Count);
        MusicPlayer.clip = Musiques[indexMusique];
        MusicPlayer.Play();
        Invoke("PlayRandom", MusicPlayer.clip.length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
