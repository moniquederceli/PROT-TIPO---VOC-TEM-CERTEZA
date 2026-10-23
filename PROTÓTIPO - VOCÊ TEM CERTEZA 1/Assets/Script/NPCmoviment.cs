using UnityEngine;
using System.Collections;

public class NPCMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 300f;

    public float centerPosition = 0f;
    public float rightPosition = 750f;

    [Header("Walking Animation")]
    public float walkHeight = 8f;
    public float walkSpeed = 8f;

    private RectTransform rectTransform;

    private float originalY;

    private bool canDecide = false;
    private bool isMoving = false;

    // Trava de segurança: só o GameManager (que confere o checklist e o carimbo) pode mandar
    // o NPC aceitar ou recusar. Se algum botão ligar direto em Accept/Deny pelo Inspector,
    // o clique é ignorado.
    [HideInInspector] public bool decisaoAutorizada = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void StartNPC()
    {
        // Coloca o NPC na posição inicial
        Vector2 position = rectTransform.anchoredPosition;

        position.x = -750f;

        rectTransform.anchoredPosition = position;

        originalY = position.y;

        canDecide = false;
        isMoving = true;

        // Passos começam junto com a caminhada
        AudioManager.IniciarPassos(gameObject);

        StartCoroutine(EnterCabin());
    }

    // Chamado pelo GameManager quando a porta "mata" este NPC.
    // Tira a permissão de decidir, pra nenhum clique atrasado mexer nele depois.
    public void CancelarPorMorte()
    {
        canDecide = false;
        isMoving = false;

        // Se o NPC morrer, não tem som de passos
        AudioManager.PararPassos(gameObject);

        PararFala();
    }

    // Esconde o balão de fala (se existir um DialogueManager na cena)
    private void PararFala()
    {
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.Parar();
    }

    // Se o NPC for desligado (morte pela porta, por exemplo), os passos dele param
    private void OnDisable()
    {
        AudioManager.PararPassos(gameObject);
    }

    private void Update()
    {
        // Só faz o movimento de caminhada enquanto estiver andando
        if (isMoving)
        {
            Vector2 position = rectTransform.anchoredPosition;

            position.y = originalY + Mathf.Sin(Time.time * walkSpeed) * walkHeight;

            rectTransform.anchoredPosition = position;
        }
    }

    IEnumerator EnterCabin()
    {
        yield return MoveToPosition(centerPosition);

        isMoving = false;

        // Chegou na frente do vidro: os passos param
        AudioManager.PararPassos(gameObject);

        Vector2 position = rectTransform.anchoredPosition;
        position.y = originalY;
        rectTransform.anchoredPosition = position;

        canDecide = true;

        GameManager gameManager = FindAnyObjectByType<GameManager>();

        if (gameManager != null)
        {
            gameManager.ShowDocumentIdentity();
            gameManager.EnableDecisionButtons();
        }

        // O NPC chegou e parou na frente do vidro: começa a falar
        DialogoNPC dialogo = GetComponent<DialogoNPC>();

        if (dialogo != null && DialogueManager.Instance != null)
            DialogueManager.Instance.FalarAbertura(dialogo);
    }

    IEnumerator MoveToPosition(float targetX)
    {
        while (Mathf.Abs(rectTransform.anchoredPosition.x - targetX) > 1f)
        {
            Vector2 position = rectTransform.anchoredPosition;

            position.x = Mathf.MoveTowards(
                position.x,
                targetX,
                moveSpeed * Time.deltaTime
            );

            rectTransform.anchoredPosition = position;

            yield return null;
        }
    }

    // BOTÃO VERDE
    public void Accept()
    {
        bool autorizado = decisaoAutorizada;
        decisaoAutorizada = false;

        if (!autorizado)
            return;

        // Impede aceitar antes de chegar, ou se este NPC está desligado (ex: morto pela porta)
        if (!canDecide || !gameObject.activeInHierarchy)
            return;

        canDecide = false;

        PararFala();

        StartCoroutine(LeaveToRight());
    }

    // BOTÃO VERMELHO
    public void Deny()
    {
        bool autorizado = decisaoAutorizada;
        decisaoAutorizada = false;

        if (!autorizado)
            return;

        // Mesma proteção do Accept
        if (!canDecide || !gameObject.activeInHierarchy)
            return;

        canDecide = false;

        PararFala();

        NPCRecusado recusado = GetComponent<NPCRecusado>();

        if (recusado != null)
        {
            recusado.Recusar();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    IEnumerator LeaveToRight()
    {
        isMoving = true;

        // Ele vai embora: os passos voltam
        AudioManager.IniciarPassos(gameObject);

        yield return MoveToPosition(rightPosition);

        isMoving = false;

        AudioManager.PararPassos(gameObject);

        gameObject.SetActive(false);
    }
}