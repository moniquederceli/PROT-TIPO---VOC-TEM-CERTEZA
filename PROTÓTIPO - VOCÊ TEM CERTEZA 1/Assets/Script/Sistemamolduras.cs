using System.Collections.Generic;
using UnityEngine;

// Coloque este script UMA VEZ SÓ em um objeto vazio chamado "SistemaMolduras"
// (um objeto que fica SEMPRE LIGADO).
//
// No começo do jogo ele coloca a moldurinha branca (hover) nos itens clicáveis:
//   - Botões Aceitar e Recusar (achados sozinhos pelo GameManager)
//   - Todos os itens da parede (jornais e post-its com o script ItemDeParede)
//   - Os itens que você arrastar para a lista "Itens Extras" (botão amarelo, envelope,
//     diário, carimbos, ficha...)
public class SistemaMolduras : MonoBehaviour
{
    [Header("Quem recebe moldura automaticamente")]
    [Tooltip("Botões Aceitar e Recusar (pega do GameManager)")]
    public bool incluirBotoesAceitarERecusar = true;

    [Tooltip("Todos os jornais e post-its da parede (os objetos com o script ItemDeParede)")]
    public bool incluirItensDeParede = true;

    [Header("Itens extras: arraste aqui os outros itens clicáveis")]
    [Tooltip("Botão amarelo, envelope, diário, carimbo verde, carimbo vermelho, ficha de especificação...")]
    public GameObject[] itensExtras;

    [Header("Aparência (vale para todos)")]
    public Color cor = Color.white;
    public float espessura = 3f;
    public float margem = 3f;

    [Header("Mostra mensagens no Console")]
    public bool mostrarMensagens = true;

    private void Start()
    {
        List<GameObject> alvos = new List<GameObject>();

        if (incluirBotoesAceitarERecusar)
        {
            GameManager gm = FindAnyObjectByType<GameManager>();

            if (gm != null)
            {
                if (gm.acceptButton != null) alvos.Add(gm.acceptButton.gameObject);
                if (gm.denyButton != null) alvos.Add(gm.denyButton.gameObject);
            }
        }

        if (incluirItensDeParede)
        {
            ItemDeParede[] itens = FindObjectsByType<ItemDeParede>(FindObjectsInactive.Include);

            foreach (ItemDeParede item in itens)
            {
                if (item != null)
                    alvos.Add(item.gameObject);
            }
        }

        if (itensExtras != null)
        {
            foreach (GameObject extra in itensExtras)
            {
                if (extra != null)
                    alvos.Add(extra);
            }
        }

        int total = 0;

        foreach (GameObject alvo in alvos)
        {
            // Não coloca duas molduras no mesmo item
            if (alvo.GetComponent<MolduraNoHover>() != null)
                continue;

            MolduraNoHover moldura = alvo.AddComponent<MolduraNoHover>();
            moldura.cor = cor;
            moldura.espessura = espessura;
            moldura.margem = margem;
            total++;
        }

        if (mostrarMensagens)
            Debug.Log("SistemaMolduras: coloquei moldura em " + total + " item(ns).");
    }
}