using UnityEngine;

public class ChecklistSquare : MonoBehaviour
{
    public GameObject checkMark;
    public GameObject otherCheckMark;

    public void Marcar()
    {
        if (checkMark == null)
            return;

        // Se este ainda não está marcado
        if (!checkMark.activeSelf)
        {
            // Marca este
            checkMark.SetActive(true);

            // Desmarca o outro
            if (otherCheckMark != null)
            {
                otherCheckMark.SetActive(false);
            }
        }
        else
        {
            // Se clicar novamente, desmarca este
            checkMark.SetActive(false);
        }
    }
}