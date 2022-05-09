using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound {
    [Tooltip("Name for this sound (must be unique)")] public string name;
    [Tooltip("Audio clip for this sound")] public AudioClip clip;
    [Range(0f, 1f), Tooltip("Volume to play this sound (0-1)")] public float volume;
    [Range(0.1f, 3f), Tooltip("Pitch to play this sound (0.1-3)")] public float pitch;
    [HideInInspector] public AudioSource source;
}
