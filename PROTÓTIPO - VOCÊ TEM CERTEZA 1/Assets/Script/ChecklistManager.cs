using UnityEngine;

public class ChecklistManager : MonoBehaviour
{
    public GameObject checklistInspect;

    public void AbrirChecklist()
    {
        if (checklistInspect != null)
        {
            checklistInspect.SetActive(true);
        }
    }

    public void FecharChecklist()
    {
        if (checklistInspect != null)
        {
            checklistInspect.SetActive(false);
        }
    }
}