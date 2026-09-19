using UnityEngine;
using UnityEngine.UI;

public class NPCRecusado : MonoBehaviour
{
    public float velocidade = 300f;
    public float limiteEsquerda = -900f;

    public float distanciaPasso = 10f;
    public float velocidadePasso = 8f;

    [Header("Imagem quando for recusado")]
    public Sprite imagemRecusado;

    private bool indoEmbora = false;
    private float posicaoYInicial;

    private Image imagemNPC;

    private void Start()
    {
        posicaoYInicial = transform.localPosition.y;

        imagemNPC = GetComponent<Image>();
    }

    public void Recusar()
    {
        // Troca a imagem
        if (imagemNPC != null && imagemRecusado != null)
        {
            imagemNPC.sprite = imagemRecusado;
        }

        // Começa a ir embora
        indoEmbora = true;
    }

    private void Update()
    {
        if (!indoEmbora)
            return;

        // Movimento para trás
        transform.localPosition += Vector3.left * velocidade * Time.deltaTime;

        // Movimento para cima e para baixo
        float movimentoY = Mathf.Sin(Time.time * velocidadePasso) * distanciaPasso;

        Vector3 novaPosicao = transform.localPosition;
        novaPosicao.y = posicaoYInicial + movimentoY;

        transform.localPosition = novaPosicao;

        // Saiu da tela
        if (transform.localPosition.x <= limiteEsquerda)
        {
            gameObject.SetActive(false);
        }
    }
}