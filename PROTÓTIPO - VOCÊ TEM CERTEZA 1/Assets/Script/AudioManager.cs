using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Coloque este script UMA VEZ SÓ em um objeto vazio chamado "AudioManager",
// que precisa estar SOLTO na raiz do Hierarchy (não pode ficar dentro de outro objeto).
//
// Ele cuida de TODO o som do jogo:
//  - Música do Menu e Música do Jogo (canal "Música" do Audio Mixer)
//  - Cliques, sirene, portas e som de digitação (canal "Efeitos" do Audio Mixer)
//
// Ele NÃO é destruído ao recarregar a cena (botão Sair do pause), então a música
// nunca reinicia à toa. Se aparecer uma cópia dele numa cena nova, a cópia some sozinha.
//
// Os AudioSources (os "alto-falantes") são criados automaticamente por este script.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private enum EstadoDaMusica
    {
        Nenhuma,
        Menu,
        Jogo
    }

    // ==========================================
    // CAMPOS DO INSPECTOR
    // ==========================================
    [Header("AUDIO MIXER")]
    [Tooltip("Arraste aqui o Audio Mixer que você criou")]
    public AudioMixer mixer;

    [Tooltip("Arraste aqui o grupo 'Musica' do mixer")]
    public AudioMixerGroup grupoMusica;

    [Tooltip("Arraste aqui o grupo 'Efeitos' do mixer")]
    public AudioMixerGroup grupoEfeitos;

    [Header("Nomes dos parâmetros expostos no mixer (escreva IGUAL ao que você renomeou)")]
    public string nomeVolumeMusica = "VolumeMusica";
    public string nomeVolumeEfeitos = "VolumeEfeitos";

    [Header("MÚSICAS")]
    public AudioClip musicaDoMenu;
    public AudioClip musicaDoJogo;

    [Range(0f, 1f)] public float volumeMusicaDoMenu = 1f;
    [Range(0f, 1f)] public float volumeMusicaDoJogo = 1f;

    [Tooltip("Segundos que a música leva para trocar (a de antes some e a nova aparece). 0 = troca seca")]
    public float duracaoDoFade = 1f;

    [Header("EFEITOS SONOROS")]
    [Tooltip("Som de clique / navegação do menu")]
    public AudioClip somDeClique;
    [Range(0f, 1f)] public float volumeDoClique = 1f;

    [Tooltip("Velocidade do som de clique. 2 = dobro da velocidade")]
    public float velocidadeDoClique = 2f;

    [Tooltip("Ligado: TODO clique do mouse (esquerdo ou direito) toca o som de clique")]
    public bool cliqueEmTodoClique = true;

    [Tooltip("Sirene vermelha (toca enquanto a luz vermelha pisca)")]
    public AudioClip somDaSirene;
    [Range(0f, 1f)] public float volumeDaSirene = 1f;

    [Tooltip("Som mecânico das portas (entrada e abate)")]
    public AudioClip somDePorta;
    [Range(0f, 1f)] public float volumeDaPorta = 1f;

    [Tooltip("Máquina de escrever (toca em repetição enquanto o NPC fala)")]
    public AudioClip somDeDigitacao;
    [Range(0f, 1f)] public float volumeDaDigitacao = 1f;

    [Tooltip("Passos do NPC andando (toca em repetição enquanto ele anda)")]
    public AudioClip somDePasso;
    [Range(0f, 1f)] public float volumeDosPassos = 1f;

    [Header("EVENTO DA JANELA (Dia 2)")]
    [Tooltip("Toc toc, como se alguém batesse no vidro")]
    public AudioClip somDeToc;
    [Range(0f, 1f)] public float volumeDoToc = 1f;

    [Tooltip("Susto: toca junto com a tremida da tela quando o monstro some")]
    public AudioClip somDeJumpscare;
    [Range(0f, 1f)] public float volumeDoJumpscare = 1f;

    [Header("Mostra mensagens no Console")]
    public bool mostrarMensagens = true;

    // Os alto-falantes (criados sozinhos)
    private AudioSource fonteMenu;
    private AudioSource fonteJogo;
    private AudioSource fonteEfeitos;
    private AudioSource fonteClique;
    private AudioSource fonteSirene;
    private AudioSource fonteDigitacao;
    private AudioSource fontePasso;
    private Object donoDosPassos;

    private EstadoDaMusica estadoDaMusica = EstadoDaMusica.Nenhuma;
    private Coroutine transicao;

    // ==========================================
    // ATALHOS (qualquer script chama assim: AudioManager.SomDePorta();)
    // Se não existir AudioManager na cena, nada acontece e nada dá erro.
    // ==========================================
    public static void MusicaDoMenu()        { if (Instance != null) Instance.TocarMusica(EstadoDaMusica.Menu); }
    public static void MusicaDoJogo()        { if (Instance != null) Instance.TocarMusica(EstadoDaMusica.Jogo); }
    public static void SomDeClique()         { if (Instance != null) Instance.TocarClique(); }
    public static void SomDePorta()          { if (Instance != null) Instance.TocarPorta(); }
    public static void SomDeToc()            { if (Instance != null) Instance.TocarEfeito(Instance.somDeToc, Instance.volumeDoToc); }
    public static void SomDeJumpscare()      { if (Instance != null) Instance.TocarEfeito(Instance.somDeJumpscare, Instance.volumeDoJumpscare); }
    public static void IniciarSirene()       { if (Instance != null) Instance.LigarLoop(Instance.fonteSirene, Instance.somDaSirene, Instance.volumeDaSirene); }
    public static void PararSirene()         { if (Instance != null) Instance.DesligarLoop(Instance.fonteSirene); }
    public static void IniciarPassos(Object dono)  { if (Instance != null) Instance.LigarPassos(dono); }
    public static void PararPassos(Object dono)    { if (Instance != null) Instance.DesligarPassos(dono); }
    public static void IniciarDigitacao()    { if (Instance != null) Instance.LigarLoop(Instance.fonteDigitacao, Instance.somDeDigitacao, Instance.volumeDaDigitacao); }
    public static void PararDigitacao()      { if (Instance != null) Instance.DesligarLoop(Instance.fonteDigitacao); }

    // ==========================================
    // INÍCIO
    // ==========================================
    private void Awake()
    {
        // Só pode existir UM AudioManager. Cópias de cenas novas se destroem.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        fonteMenu = CriarFonte(grupoMusica, true);
        fonteJogo = CriarFonte(grupoMusica, true);
        fonteEfeitos = CriarFonte(grupoEfeitos, false);
        fonteClique = CriarFonte(grupoEfeitos, false);
        fonteSirene = CriarFonte(grupoEfeitos, true);
        fonteDigitacao = CriarFonte(grupoEfeitos, true);
        fontePasso = CriarFonte(grupoEfeitos, true);

        fonteMenu.volume = 0f;
        fonteJogo.volume = 0f;

        SettingsManager.AoMudarVolume += AplicarVolumes;
        SceneManager.sceneLoaded += AoCarregarCena;
    }

    private void Start()
    {
        if (Instance != this)
            return;

        StartCoroutine(AplicarVolumesNoComeco());
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;

        SettingsManager.AoMudarVolume -= AplicarVolumes;
        SceneManager.sceneLoaded -= AoCarregarCena;
        Instance = null;
    }

    private AudioSource CriarFonte(AudioMixerGroup grupo, bool repetir)
    {
        AudioSource fonte = gameObject.AddComponent<AudioSource>();
        fonte.playOnAwake = false;
        fonte.loop = repetir;
        fonte.spatialBlend = 0f;   // som 2D (igual em qualquer lugar da tela)
        fonte.outputAudioMixerGroup = grupo;
        return fonte;
    }

    // O mixer às vezes ignora ordens no primeiro instante: aplicamos agora e de novo no frame seguinte
    private IEnumerator AplicarVolumesNoComeco()
    {
        AplicarVolumes();
        yield return null;
        AplicarVolumes();
    }

    // Quando a cena é recarregada (botão Sair), sons em repetição não podem ficar presos
    private void AoCarregarCena(Scene cena, LoadSceneMode modo)
    {
        DesligarLoop(fonteDigitacao);
        DesligarLoop(fonteSirene);
        DesligarLoop(fontePasso);
        donoDosPassos = null;
    }

    // ==========================================
    // VOLUMES (ligados aos sliders das Configurações)
    // ==========================================
    private void AplicarVolumes()
    {
        DefinirVolumeNoMixer(nomeVolumeMusica, SettingsManager.VolumeMusicas);
        DefinirVolumeNoMixer(nomeVolumeEfeitos, SettingsManager.VolumeEfeitos);
    }

    // O slider vai de 0 a 1, mas o mixer usa decibéis (-80 = mudo, 0 = volume cheio)
    private void DefinirVolumeNoMixer(string parametro, float valor01)
    {
        if (mixer == null || string.IsNullOrEmpty(parametro))
            return;

        float decibeis = (valor01 <= 0.0001f) ? -80f : Mathf.Log10(valor01) * 20f;

        bool deuCerto = mixer.SetFloat(parametro, decibeis);

        if (!deuCerto && mostrarMensagens)
            Debug.LogWarning("AudioManager: o mixer não tem o parâmetro '" + parametro + "'. Confira se você expôs o Volume e se o nome está IGUAL.");
    }

    // ==========================================
    // MÚSICA
    // ==========================================
    private void TocarMusica(EstadoDaMusica novoEstado)
    {
        if (estadoDaMusica == novoEstado)
            return;

        AudioSource entra = (novoEstado == EstadoDaMusica.Menu) ? fonteMenu : fonteJogo;
        AudioSource sai = (novoEstado == EstadoDaMusica.Menu) ? fonteJogo : fonteMenu;
        AudioClip clip = (novoEstado == EstadoDaMusica.Menu) ? musicaDoMenu : musicaDoJogo;
        float alvo = (novoEstado == EstadoDaMusica.Menu) ? volumeMusicaDoMenu : volumeMusicaDoJogo;

        estadoDaMusica = novoEstado;

        if (mostrarMensagens)
            Debug.Log("AudioManager: música -> " + novoEstado + (clip == null ? " (SEM ARQUIVO DE ÁUDIO!)" : ""));

        if (transicao != null)
            StopCoroutine(transicao);

        entra.clip = clip;
        transicao = StartCoroutine(Transicao(entra, sai, alvo));
    }

    private IEnumerator Transicao(AudioSource entra, AudioSource sai, float alvo)
    {
        if (entra.clip != null && !entra.isPlaying)
        {
            entra.volume = 0f;
            entra.Play();
        }

        float inicioEntra = entra.volume;
        float inicioSai = sai.volume;
        float tempo = 0f;

        while (tempo < duracaoDoFade)
        {
            // unscaledDeltaTime: funciona mesmo com o jogo pausado
            tempo += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(tempo / duracaoDoFade);

            entra.volume = Mathf.Lerp(inicioEntra, alvo, k);
            sai.volume = Mathf.Lerp(inicioSai, 0f, k);

            yield return null;
        }

        entra.volume = alvo;
        sai.volume = 0f;
        sai.Stop();

        transicao = null;
    }

    // ==========================================
    // EFEITOS
    // ==========================================
    private void TocarClique()
    {
        if (somDeClique == null)
            return;

        // O pitch fica no "alto-falante" do clique: 2 = o dobro da velocidade
        fonteClique.pitch = velocidadeDoClique;
        fonteClique.PlayOneShot(somDeClique, volumeDoClique);
    }

    private void TocarPorta()
    {
        if (somDePorta == null)
            return;

        fonteEfeitos.PlayOneShot(somDePorta, volumeDaPorta);
    }

    // Toca um efeito simples (uma vez) no canal de Efeitos
    private void TocarEfeito(AudioClip clip, float volume)
    {
        if (clip == null)
            return;

        fonteEfeitos.PlayOneShot(clip, volume);
    }

    // Passos: só quem LIGOU (o NPC) consegue desligar. Assim, um NPC que some
    // não corta os passos do NPC seguinte.
    private void LigarPassos(Object dono)
    {
        donoDosPassos = dono;
        LigarLoop(fontePasso, somDePasso, volumeDosPassos);
    }

    private void DesligarPassos(Object dono)
    {
        if (donoDosPassos != dono)
            return;

        donoDosPassos = null;
        DesligarLoop(fontePasso);
    }

    private void LigarLoop(AudioSource fonte, AudioClip clip, float volume)
    {
        if (fonte == null || clip == null)
            return;

        // Já está tocando? Deixa continuar (não reinicia)
        if (fonte.isPlaying && fonte.clip == clip)
            return;

        fonte.clip = clip;
        fonte.volume = volume;
        fonte.Play();
    }

    private void DesligarLoop(AudioSource fonte)
    {
        if (fonte != null)
            fonte.Stop();
    }

    // ==========================================
    // TODO FRAME
    // ==========================================
    private void Update()
    {
        // Clique em qualquer lugar da tela
        if (cliqueEmTodoClique && Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
                TocarClique();
        }

        // Com o jogo pausado (ESC), digitação e sirene congelam junto. A música continua.
        bool pausado = Time.timeScale == 0f;

        if (pausado)
        {
            fonteDigitacao.Pause();
            fonteSirene.Pause();
            fontePasso.Pause();
        }
        else
        {
            fonteDigitacao.UnPause();
            fonteSirene.UnPause();
            fontePasso.UnPause();
        }
    }
}