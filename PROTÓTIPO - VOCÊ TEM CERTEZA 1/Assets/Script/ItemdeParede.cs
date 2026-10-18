using UnityEngine;
using UnityEngine.EventSystems;

// Coloque este script em CADA jornal e CADA post-it pequeno que fica colado na parede da direita.
// Ele guarda: a imagem grande, o texto (só jornal) e em quais dias o item aparece na parede.
public class ItemDeParede : MonoBehaviour, IPointerClickHandler
{
    public enum TipoItem
    {
        Jornal,   // 1º clique: imagem grande | 2º clique: caixa de texto rolável
        PostIt    // 1º clique: imagem grande | clique de novo (ou fora): fecha. Sem texto.
    }

    [Header("Que tipo de item é este?")]
    public TipoItem tipo = TipoItem.Jornal;

    [Header("Imagem GRANDE e legível (aparece no centro da tela)")]
    public Sprite imagemGrande;

    [Header("Texto de leitura (SÓ para Jornal; no Post-it pode deixar vazio)")]
    [TextArea(5, 20)]
    public string texto;

    [Header("Em quais dias este item aparece na parede")]
    [Tooltip("O item aparece a partir deste dia. Exemplo: 1 = já no Dia 1, 2 = só a partir do Dia 2.")]
    public int aparecerNoDia = 1;

    [Tooltip("O item some DEPOIS deste dia. Use 0 para o item NUNCA sumir. Exemplo: 1 = aparece no Dia 1 e some no Dia 2.")]
    public int sumirDepoisDoDia = 0;

    [Header("Evento da janela (marque SÓ no post-it novo do Dia 2)")]
    [Tooltip("Ao abrir este item, o jogo sorteia (50%) se aparece uma figura misteriosa na janela da esquerda")]
    public bool sorteiaEventoDaJanela = false;

    // O LeitorDaParede pergunta isto para decidir se o item deve estar visível
    public bool EstaVisivelNoDia(int dia)
    {
        if (dia < aparecerNoDia)
            return false;

        if (sumirDepoisDoDia > 0 && dia > sumirDepoisDoDia)
            return false;

        return true;
    }

    // Clique do mouse no item pequeno da parede
    public void OnPointerClick(PointerEventData eventData)
    {
        // Só o botão ESQUERDO abre
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        LeitorDaParede leitor = FindAnyObjectByType<LeitorDaParede>();

        if (leitor == null)
        {
            Debug.LogError("ItemDeParede (" + name + "): não achei o LeitorDaParede na cena. Crie o objeto SistemaParede e coloque o script nele.");
            return;
        }

        leitor.Abrir(this);
    }
}