using UnityEngine;

public class FocoGeral : MonoBehaviour
{
    [Header("Configurações de Foco")]
    // Agora você pode adicionar quantas tags quiser no Inspector
    public string[] tagsParaFocar = { "Inimigo", "ObjetoInteresse" };
    public float raioDeBusca = 12f;

    [Header("Osso da Cabeça (Blender)")]
    public Transform ossoCabeca;

    [Header("Ajuste de Rotação")]
    public Vector3 ajusteOffset = new Vector3(0, 90, 0);
    public float anguloMaximoOlhar = 70f;

    [Header("Configurações de Piscada")]
    public SkinnedMeshRenderer rend;
    public float velocidadePiscada = 5f;
    public float duracaoPiscada = 0.2f;

    private Transform inimigoMaisProximo;
    private bool temAlvo = false;
    private int blinkIndex;

    void Start()
    {
        if (rend != null)
            blinkIndex = rend.sharedMesh.GetBlendShapeIndex("blink");
    }

    void Update()
    {
        BuscarAlvoMaisProximo();
        GerenciarPiscada();
    }

    void GerenciarPiscada()
    {
        if (rend != null && blinkIndex != -1)
        {
            float tempo = Mathf.PingPong(Time.time * velocidadePiscada, 1f);
            float peso = (tempo < duracaoPiscada) ? 100f : 0f;
            rend.SetBlendShapeWeight(blinkIndex, peso);
        }
    }

    void BuscarAlvoMaisProximo()
    {
        GameObject alvoPerto = null;
        float menorDistancia = raioDeBusca;

        // Loop através de todas as tags na sua lista
        foreach (string tag in tagsParaFocar)
        {
            GameObject[] alvos = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject alvo in alvos)
            {
                float distancia = Vector3.Distance(transform.position, alvo.transform.position);
                if (distancia < menorDistancia)
                {
                    menorDistancia = distancia;
                    alvoPerto = alvo;
                }
            }
        }

        if (alvoPerto != null)
        {
            inimigoMaisProximo = alvoPerto.transform;
            Vector3 direcaoParaAlvo = (inimigoMaisProximo.position - transform.position).normalized;
            float angulo = Vector3.Angle(transform.forward, direcaoParaAlvo);
            temAlvo = angulo <= anguloMaximoOlhar;
        }
        else
        {
            inimigoMaisProximo = null;
            temAlvo = false;
        }
    }

    void LateUpdate()
    {
        if (ossoCabeca == null) return;

        if (temAlvo && inimigoMaisProximo != null)
        {
            Vector3 direcaoMundo = (inimigoMaisProximo.position - ossoCabeca.position).normalized;

            if (direcaoMundo != Vector3.zero)
            {
                Vector3 direcaoLocal = transform.InverseTransformDirection(direcaoMundo);
                Quaternion rotationLocalAlvo = Quaternion.LookRotation(direcaoLocal);
                ossoCabeca.localRotation = rotationLocalAlvo * Quaternion.Euler(ajusteOffset);
            }
        }
    }

    public Transform ObterInimigoFocado()
    {
        return temAlvo ? inimigoMaisProximo : null;
    }
}