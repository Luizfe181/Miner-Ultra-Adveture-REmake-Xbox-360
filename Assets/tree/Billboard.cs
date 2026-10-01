using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        // Pega a posição da câmera
        Vector3 camPos = Camera.main.transform.position;

        // Mantém a árvore alinhada apenas no eixo Y (para não girar de cabeça para baixo)
        Vector3 targetPos = new Vector3(camPos.x, transform.position.y, camPos.z);

        // Faz a árvore olhar para a câmera
        transform.LookAt(targetPos);
    }
}