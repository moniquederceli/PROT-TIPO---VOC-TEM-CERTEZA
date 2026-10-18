using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Coloque este script no objeto "SistemaMenu" (o mesmo do MainMenuManager).
//
// Cuida de:
//  - Modo de tela: Tela Cheia <-> Modo Janela
//  - Volume de Efeitos e de Músicas (já pronto para quando você tiver sons)
// Tudo fica salvo: quando o jogador abrir o jogo de novo, as escolhas continuam.
public class SettingsManager : MonoBehaviour
{
    // ==========================================
    // VOLUMES (qualquer script de som pode consultar)
    // ==========================================
    public static event Action AoMudarVolume;

    private static bool carregado = false;
    private static float volumeEfeitos = 0.5f;
    private static float volumeMusicas = 0.5f;

    public static float VolumeEfeitos
    {
        get { Carregar(); return volumeEfeitos; }
    }

    public static float VolumeMusicas
    {
        get { Carregar(); return volumeMusicas; }
    }

    private static void Carregar()
    {
        if (carregado)
            return;

        carregado = true;
        volumeEfeitos = PlayerPrefs.GetFloat("volumeEfeitos", 0.5f);
        volumeMusicas = PlayerPrefs.GetFloat("volumeMusicas", 0.5f);
    }

    // ==========================================
    // CAMPOS DO INSPECTOR
    // ==========================================
    [Header("MODO DE TELA")]
    [Tooltip("A Image que mostra o texto do modo atual ('Tela Cheia' ou 'Modo Janela')")]
    public Image imagemModoDeTela;

    [Tooltip("Imagem do texto 'Tela Cheia'")]
    public Sprite spriteTelaCheia;

    [Tooltip("Imagem do texto 'Modo Janela'")]
    public Sprite spriteJanela;

    [Tooltip("Ligado: a imagem muda de tamanho junto com o texto (Tela Cheia / Janela) e nunca fica esticada")]
    public bool ajustarTamanhoAoTrocar = true;

    [Tooltip("Tamanho da janela quando estiver em Modo Janela")]
    public int larguraDaJanela = 1280;
    public int alturaDaJanela = 720;

    [Header("ÁUDIO: Efeitos")]
    public Slider sliderEfeitos;
    public TMP_Text textoEfeitos;

    [Header("ÁUDIO: Músicas")]
    public Slider sliderMusicas;
    public TMP_Text textoMusicas;

    private bool telaCheia = true;
    private float escalaDaImagem = 1f;

    private void Awake()
    {
        Carregar();

        // Descobre em que escala você deixou a imagem no editor (pra manter a mesma proporção depois)
        if (imagemModoDeTela != null && imagemModoDeTela.sprite != null && imagemModoDeTela.sprite.rect.width > 0f)
            escalaDaImagem = imagemModoDeTela.rectTransform.rect.width / imagemModoDeTela.sprite.rect.width;

        // Modo de tela: se o jogador já escolheu antes, aplica a escolha dele
        if (PlayerPrefs.HasKey("telaCheia"))
        {
            telaCheia = PlayerPrefs.GetInt("telaCheia") == 1;
            AplicarTela();
        }
        else
        {
            telaCheia = Screen.fullScreen;
        }

        AtualizarImagemDaTela();

        // Sliders: mostram o volume salvo e avisam este script quando mudam
        if (sliderEfeitos != null)
        {
            sliderEfeitos.minValue = 0f;
            sliderEfeitos.maxValue = 1f;
            sliderEfeitos.SetValueWithoutNotify(volumeEfeitos);
            sliderEfeitos.onValueChanged.AddListener(MudouVolumeEfeitos);
        }

        if (sliderMusicas != null)
        {
            sliderMusicas.minValue = 0f;
            sliderMusicas.maxValue = 1f;
            sliderMusicas.SetValueWithoutNotify(volumeMusicas);
            sliderMusicas.onValueChanged.AddListener(MudouVolumeMusicas);
        }

        AtualizarTextosDeVolume();
    }

    private void OnDestroy()
    {
        if (sliderEfeitos != null)
            sliderEfeitos.onValueChanged.RemoveListener(MudouVolumeEfeitos);

        if (sliderMusicas != null)
            sliderMusicas.onValueChanged.RemoveListener(MudouVolumeMusicas);
    }

    // ==========================================
    // MODO DE TELA (ligue as DUAS setinhas neste método)
    // ==========================================
    public void AlternarModoDeTela()
    {
        telaCheia = !telaCheia;

        AplicarTela();
        AtualizarImagemDaTela();

        PlayerPrefs.SetInt("telaCheia", telaCheia ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void AplicarTela()
    {
        if (telaCheia)
        {
            // Usa o tamanho real do monitor
            Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, FullScreenMode.FullScreenWindow);
        }
        else
        {
            Screen.SetResolution(larguraDaJanela, alturaDaJanela, FullScreenMode.Windowed);
        }
    }

    private void AtualizarImagemDaTela()
    {
        if (imagemModoDeTela == null)
            return;

        Sprite sprite = telaCheia ? spriteTelaCheia : spriteJanela;

        if (sprite == null)
            return;

        imagemModoDeTela.sprite = sprite;

        if (ajustarTamanhoAoTrocar)
        {
            // Cada texto tem um tamanho: ajusta a caixa para o tamanho certo daquela imagem
            imagemModoDeTela.preserveAspect = true;

            RectTransform caixa = imagemModoDeTela.rectTransform;
            caixa.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sprite.rect.width * escalaDaImagem);
            caixa.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sprite.rect.height * escalaDaImagem);
        }
    }

    // ==========================================
    // VOLUMES
    // ==========================================
    private void MudouVolumeEfeitos(float valor)
    {
        volumeEfeitos = valor;
        PlayerPrefs.SetFloat("volumeEfeitos", valor);

        AtualizarTextosDeVolume();

        if (AoMudarVolume != null)
            AoMudarVolume();
    }

    private void MudouVolumeMusicas(float valor)
    {
        volumeMusicas = valor;
        PlayerPrefs.SetFloat("volumeMusicas", valor);

        AtualizarTextosDeVolume();

        if (AoMudarVolume != null)
            AoMudarVolume();
    }

    private void AtualizarTextosDeVolume()
    {
        if (textoEfeitos != null)
            textoEfeitos.text = Mathf.RoundToInt(volumeEfeitos * 100f) + "%";

        if (textoMusicas != null)
            textoMusicas.text = Mathf.RoundToInt(volumeMusicas * 100f) + "%";
    }
}