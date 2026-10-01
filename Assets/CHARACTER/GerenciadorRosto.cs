using UnityEngine;

public class GerenciadorRosto : MonoBehaviour
{
    [Header("Configurações de Foco")]
    public string tagInimigo = "Inimigo";
    public float raioDeBusca = 12f;
    public float distanciaSeguranca = 0.8f; // Zona morta para evitar tremor
    public Transform ossoCabeca;
    public Vector3 ajusteOffset = new Vector3(0, 90, 0);

    [Header("Configurações de Piscada")]
    public SkinnedMeshRenderer rend;
    public float velocidadePiscada = 5f;
    public float duracaoPiscada = 0.2f;

    private int blinkIndex;
    private Transform inimigoFocado;
    private bool temAlvo = false;

    void Start()
    {
        if (rend != null)
            blinkIndex = rend.sharedMesh.GetBlendShapeIndex("blink");
    }

    void Update()
    {
        BuscarInimigoMaisProximo();
    }

    void LateUpdate()
    {
        // 1. Lógica da Cabeça
        if (ossoCabeca != null)
        {
            if (temAlvo && inimigoFocado != null)
            {
                float dist = Vector3.Distance(ossoCabeca.position, inimigoFocado.position);

                // Só gira se estiver fora da distância de segurança
                if (dist > distanciaSeguranca)
                {
                    Vector3 dir = (inimigoFocado.position - ossoCabeca.position).normalized;
                    // Definimos a rotação e usamos ela logo abaixo
                    Quaternion rotAlvo = Quaternion.LookRotation(dir) * Quaternion.Euler(ajusteOffset);
                    ossoCabeca.rotation = Quaternion.Slerp(ossoCabeca.rotation, rotAlvo, Time.deltaTime * 4f);
                }
            }
            else
            {
                // Volta para a rotação neutra baseada no corpo
                Quaternion rotNeutra = transform.rotation * Quaternion.Euler(ajusteOffset);
                ossoCabeca.rotation = Quaternion.Slerp(ossoCabeca.rotation, rotNeutra, Time.deltaTime * 5f);
            }
        }

        // 2. Lógica da Piscada
        if (rend != null && blinkIndex != -1)
        {
            float tempo = Mathf.PingPong(Time.time * velocidadePiscada, 1f);
            float peso = (tempo < duracaoPiscada) ? 100f : 0f;
            rend.SetBlendShapeWeight(blinkIndex, peso);
        }
    }

    void BuscarInimigoMaisProximo()
    {
        GameObject[] inimigos = GameObject.FindGameObjectsWithTag(tagInimigo);
        float menorDist = raioDeBusca;
        inimigoFocado = null;

        foreach (var i in inimigos)
        {
            float d = Vector3.Distance(transform.position, i.transform.position);
            if (d < menorDist)
            {
                menorDist = d;
                inimigoFocado = i.transform;
            }
        }
        temAlvo = (inimigoFocado != null);
    }
}