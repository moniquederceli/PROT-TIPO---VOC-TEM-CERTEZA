using UnityEngine;
using UnityEngine.UI;

public class NPCRecusado : MonoBehaviour
{
    [Header("Imagem do NPC")]
    public Image imagemNPC;
    public Sprite imagemBravo;

    [Header("Movimento")]
    public float velocidade = 300f;

    private bool indoEmbora = false;

    public void Recusar()
    {
        // Troca o NPC normal pelo NPC bravo
        if (imagemNPC != null && imagemBravo != null)
        {
            imagemNPC.sprite = imagemBravo;
        }

        // Começa a andar para trás
        indoEmbora = true;
    }

    private void Update()
    {
        if (indoEmbora)
        {
            transform.localPosition += Vector3.left * velocidade * Time.deltaTime;

            if (transform.localPosition.x <= -900f)
            {
                gameObject.SetActive(false);
            }
        }
    }
}