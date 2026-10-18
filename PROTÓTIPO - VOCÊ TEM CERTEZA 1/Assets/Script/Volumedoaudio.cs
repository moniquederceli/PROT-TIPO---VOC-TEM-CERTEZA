using UnityEngine;

// PARA O FUTURO: quando você tiver sons, coloque este script em cada objeto que tem um
// "Audio Source" (música de fundo, efeito de carimbo, porta...).
// Escolha se ele é Música ou Efeito, e o volume passa a obedecer os sliders do menu.
[RequireComponent(typeof(AudioSource))]
public class VolumeDoAudio : MonoBehaviour
{
    public enum TipoDeAudio
    {
        Musica,
        Efeito
    }

    [Header("Este som é uma Música ou um Efeito?")]
    public TipoDeAudio tipo = TipoDeAudio.Efeito;

    private AudioSource fonte;
    private float volumeOriginal = 1f;

    private void Awake()
    {
        fonte = GetComponent<AudioSource>();
        volumeOriginal = fonte.volume;
    }

    private void OnEnable()
    {
        SettingsManager.AoMudarVolume += Aplicar;
        Aplicar();
    }

    private void OnDisable()
    {
        SettingsManager.AoMudarVolume -= Aplicar;
    }

    private void Aplicar()
    {
        float volumeDoMenu = (tipo == TipoDeAudio.Musica) ? SettingsManager.VolumeMusicas : SettingsManager.VolumeEfeitos;
        fonte.volume = volumeOriginal * volumeDoMenu;
    }
}