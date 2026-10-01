using UnityEngine;

public class Piscada : MonoBehaviour
{
    public SkinnedMeshRenderer rend;
    private int blinkIndex;

    [Header("Configurações da Piscada")]
    [Tooltip("Velocidade da animação (quanto maior, mais rápido)")]
    public float velocidade = 5f;
    [Tooltip("Duração da piscada")]
    public float duracao = 0.5f;

    void Start()
    {
        blinkIndex = rend.sharedMesh.GetBlendShapeIndex("blink");
    }

    void LateUpdate()
    {
        // Math.PingPong cria um movimento de vai-e-vem.
        // Multiplicamos o tempo pela variável 'velocidade' para acelerar ou diminuir o ritmo.
        float tempo = Mathf.PingPong(Time.time * velocidade, 1f);

        // Se o valor estiver dentro do limite de duração, fecha o olho (100), senão abre (0)
        float peso = (tempo < duracao) ? 100f : 0f;

        rend.SetBlendShapeWeight(blinkIndex, peso);
    }
}