using UnityEngine;

public class TrocaBandana : MonoBehaviour
{
    [Header("Componentes de Renderização")]
    public SkinnedMeshRenderer meshRosto;   // Arraste o SkinnedMeshRenderer do rosto aqui
    public SkinnedMeshRenderer meshPescoco; // Arraste o SkinnedMeshRenderer do pescoço aqui

    public void AlternarBandana()
    {
        // Verifica qual está ativo e inverte o estado do componente 'enabled'
        if (meshRosto.enabled)
        {
            meshRosto.enabled = false;
            meshPescoco.enabled = true;
        }
        else
        {
            meshRosto.enabled = true;
            meshPescoco.enabled = false;
        }
    }
}