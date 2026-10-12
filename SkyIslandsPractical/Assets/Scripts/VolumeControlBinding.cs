using UnityEngine;
using UnityEngine.UI;

namespace SirikanyaKawilawan
{
    public sealed class VolumeControlBinding : MonoBehaviour
    {
        public Slider slider;
        public Text percentage;
        public bool music;
        void Start()
        {
            var service=AudioService.Instance;if(service==null)return;
            float value=music?service.MusicVolume:service.EffectsVolume;
            slider.SetValueWithoutNotify(value);percentage.text=Mathf.RoundToInt(value*100)+"%";
        }
        public void Apply(float value)
        {
            var service=AudioService.Instance;
            if(service!=null){if(music)service.SetMusicVolume(value);else service.SetEffectsVolume(value);}
            if(percentage!=null)percentage.text=Mathf.RoundToInt(value*100)+"%";
        }
    }
}
