using UnityEngine;
using UnityEngine.UI;

public class PlayVideo : MonoBehaviour {

	// Use this for initialization
	void Start () {

        // Get the movie texture
        MovieTexture introMovie = GetComponentInChildren<RawImage>().texture as MovieTexture;

        // Set it looping
        introMovie.loop = true;

        // Play the video
        introMovie.Play();
    }
}
