using UnityEngine;

[System.Serializable]
public class NPCData
{
    public string npcName;

    [TextArea(2, 5)]
    public string dialogue;

    public Sprite npcSprite;

    public Sprite[] documents;

    public bool isDecayed;

    public bool correctDecision;
}

//Aqui estamos dizendo que cada NPC possui:

//nome;
//diálogo;
//imagem;
//documentos;
//se é Decaído;
//qual é a decisão correta.
