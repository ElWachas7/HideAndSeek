using UnityEngine;
using UnityEngine.Audio;
public enum LayerType
{
    Additive,
    Single
}

[CreateAssetMenu(menuName = "SoundSystem/Music Event", fileName = "Mus_")]
public class MusicEvent : ScriptableObject
{
    [SerializeField] AudioClip[] _musicLayers;
    [SerializeField] LayerType _layerType = LayerType.Additive;
    [SerializeField] AudioMixerGroup _mixer;

    public AudioClip[] MusicLayers => _musicLayers;
    public LayerType LayerType => _layerType;
    public AudioMixerGroup mixer => _mixer;
}
