using UnityEngine;

public class CameraSeguidor : MonoBehaviour
{
    [Header("Alvos")]
    public Transform alvo; // O Mineirinho
    public string tagInimigo = "Inimigo"; // Tag dos inimigos/boss

    [Header("Configurações de Rotação")]
    public float sensibilidadeX = 250.0f;
    public float sensibilidadeY = 120.0f;
    public float velocidadeDeMirar = 5.0f; // Velocidade da transição para o inimigo

    [Header("Limites e Distância")]
    public float distanciaDoAlvo = 5.0f;
    public float limiteVerticalMin = -10.0f;
    public float limiteVerticalMax = 60.0f;

    private float rotacaoX = 0.0f;
    private float rotacaoY = 0.0f;
    private Transform inimigoAlvo;

    void Start()
    {
        Vector3 angulos = transform.eulerAngles;
        rotacaoX = angulos.y;
        rotacaoY = angulos.x;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (alvo == null) return;

        // 1. Lógica do botão de mira (ex: Botão direito do rato ou gatilho L2/LT)
        if (Input.GetAxisRaw("LTXbox") > 0.5f) // O Fire2 costuma ser o botão direito do rato ou LT do comando
        {
            BuscarInimigoMaisProximo();
        }
        else
        {
            inimigoAlvo = null;
        }

        // 2. Calcular rotação
        if (inimigoAlvo != null)
        {
            // Mira no inimigo
            Vector3 direcao = (inimigoAlvo.position - transform.position).normalized;
            Quaternion rotacaoDesejada = Quaternion.LookRotation(direcao);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacaoDesejada, velocidadeDeMirar * Time.deltaTime);

            // Atualiza os ângulos internos para quando soltar o botão, não haver saltos bruscos
            rotacaoX = transform.eulerAngles.y;
            rotacaoY = transform.eulerAngles.x;
        }
        else
        {
            // Mira livre (seu código original)
            rotacaoX += Input.GetAxis("Mouse X") * sensibilidadeX * Time.deltaTime;
            rotacaoY -= Input.GetAxis("Mouse Y") * sensibilidadeY * Time.deltaTime;
            rotacaoY = Mathf.Clamp(rotacaoY, limiteVerticalMin, limiteVerticalMax);
            transform.rotation = Quaternion.Euler(rotacaoY, rotacaoX, 0);
        }

        // 3. Posição da câmara
        Vector3 posicaoFinal = transform.rotation * new Vector3(0.0f, 0.0f, -distanciaDoAlvo) + alvo.position + new Vector3(0, 1.5f, 0);
        transform.position = posicaoFinal;
    }

    void BuscarInimigoMaisProximo()
    {
        GameObject[] inimigos = GameObject.FindGameObjectsWithTag(tagInimigo);
        float distanciaMinima = 20.0f; // Raio máximo de mira
        inimigoAlvo = null;

        foreach (GameObject inimigo in inimigos)
        {
            float d = Vector3.Distance(alvo.position, inimigo.transform.position);
            if (d < distanciaMinima)
            {
                distanciaMinima = d;
                inimigoAlvo = inimigo.transform;
            }
        }
    }
}