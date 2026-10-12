using UnityEngine;

namespace SirikanyaKawilawan
{
    public sealed class AudioService : MonoBehaviour
    {
        public static AudioService Instance { get; private set; }
        public AudioClip musicClip;
        AudioSource music, effects;
        public float MusicVolume { get; private set; }
        public float EffectsVolume { get; private set; }
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                if (musicClip != null) Instance.SetTrack(musicClip);
                Destroy(gameObject); return;
            }
            Instance = this; DontDestroyOnLoad(gameObject);
            music = gameObject.AddComponent<AudioSource>(); music.loop = true; music.playOnAwake = false;
            effects = gameObject.AddComponent<AudioSource>(); effects.playOnAwake = false;
            MusicVolume = Mathf.Clamp01(PlayerPrefs.GetFloat("SkyIslands.MusicVolume", 0.35f));
            EffectsVolume = Mathf.Clamp01(PlayerPrefs.GetFloat("SkyIslands.EffectsVolume", 0.75f));
            music.volume = MusicVolume; effects.volume = EffectsVolume;
            SetTrack(musicClip);
        }
        public void SetTrack(AudioClip track)
        {
            if (track == null || music == null) return;
            if (music.clip == track && music.isPlaying) return;
            music.clip = track; music.Play();
        }
        public void PlayEffect(AudioClip clip) { if (clip != null && effects != null) effects.PlayOneShot(clip); }
        public void SetMusicVolume(float value) { MusicVolume = Mathf.Clamp01(value); music.volume = MusicVolume; PlayerPrefs.SetFloat("SkyIslands.MusicVolume", MusicVolume); PlayerPrefs.Save(); }
        public void SetEffectsVolume(float value) { EffectsVolume = Mathf.Clamp01(value); effects.volume = EffectsVolume; PlayerPrefs.SetFloat("SkyIslands.EffectsVolume", EffectsVolume); PlayerPrefs.Save(); }
        void OnDestroy() { if (Instance == this) Instance = null; }
    }
}
