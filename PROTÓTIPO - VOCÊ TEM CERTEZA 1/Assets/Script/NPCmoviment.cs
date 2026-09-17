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

        StartCoroutine(EnterCabin());
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

    Vector2 position = rectTransform.anchoredPosition;
    position.y = originalY;
    rectTransform.anchoredPosition = position;

    canDecide = true;

    GameManager gameManager = FindAnyObjectByType<GameManager>();

    if (gameManager != null)
    {
        gameManager.EnableDecisionButtons();
    }
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
        // Impede aceitar antes de chegar
        if (!canDecide)
            return;

        canDecide = false;

        StartCoroutine(LeaveToRight());
    }

    // BOTÃO VERMELHO
    public void Deny()
    {
        // Impede recusar antes de chegar
        if (!canDecide)
            return;

        canDecide = false;

        gameObject.SetActive(false);
    }

    IEnumerator LeaveToRight()
    {
        isMoving = true;

        yield return MoveToPosition(rightPosition);

        isMoving = false;

        gameObject.SetActive(false);
    }
}
