using UnityEngine;

public class AbrirImagem : MonoBehaviour
{
    public GameObject imagemGrande;

    public void Abrir()
    {
        imagemGrande.SetActive(true);
    }
}