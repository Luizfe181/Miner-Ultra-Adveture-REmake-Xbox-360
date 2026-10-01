using UnityEngine;
using System.Collections;

public class ChapeuProjetil : MonoBehaviour
{
    [Header("Configurações do Bumerangue")]
    public float velocidadeIda = 15.0f;
    public float velocidadeVolta = 18.0f;
    public float tempoIda = 0.6f;

    private Transform jogador;
    private bool voltando = false;
    private float cronometro = 0.0f;
    private Vector3 direcaoIda;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            jogador = playerObj.transform;
        }
        direcaoIda = transform.TransformDirection(Vector3.back);
    }

    void Update()
    {
        if (!voltando)
        {
            transform.position += direcaoIda * velocidadeIda * Time.deltaTime;
            transform.Rotate(Vector3.up * 1000 * Time.deltaTime, Space.Self);
            cronometro += Time.deltaTime;
            if (cronometro >= tempoIda)
            {
                voltando = true;
            }
        }
        else
        {
            if (jogador != null)
            {
                Vector3 posicaoAlvo = jogador.position + new Vector3(0, 1.2f, 0);
                transform.position = Vector3.MoveTowards(transform.position, posicaoAlvo, velocidadeVolta * Time.deltaTime);
                transform.Rotate(Vector3.up * 1000 * Time.deltaTime, Space.Self);
                float distancia = Vector3.Distance(transform.position, posicaoAlvo);
                if (distancia < 0.5f)
                {
                    RecolherChapeu();
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    void RecolherChapeu()
    {
        if (jogador != null)
        {
            jogador.GetComponent<MinerMovimento>().RetornarChapeuParaCabeca();
        }
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Inimigo"))
        {
            voltando = true;
        }
    }
}