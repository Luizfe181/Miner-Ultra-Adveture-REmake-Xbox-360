using UnityEngine;

public class ItemColetavel : MonoBehaviour
{
    [Header("Configurações do Item")]
    public int valorDoItem = 1;
    public float velocidadeRotacao = 100f;

    void Update()
    {
        // Faz o pão de queijo ficar girando no próprio eixo, estilo jogo clássico
        transform.Rotate(Vector3.back * velocidadeRotacao * Time.deltaTime);
    }

    // Detecta quando o Mineirinho encosta no item
    private void OnTriggerEnter(Collider other)
    {
        string nomeObjeto = other.gameObject.name.ToLower();

        if (other.CompareTag("Player") || nomeObjeto.Contains("mineirinho") ||
            other.CompareTag("Chapeu") || nomeObjeto.Contains("chapeu"))
        {
            // 1. Somar o ponto no GameManager
            if (GameManager.instance != null)
            {
                GameManager.instance.AdicionarPaoDeQueijo(valorDoItem);
            }

            // 2. Tocar o som de Pão de Queijo
            // Se quem colidiu foi o jogador, usamos ele mesmo. 
            // Se foi o chapéu, buscamos o "Player" na cena.
            GameObject jogador = other.CompareTag("Player") ? other.gameObject : GameObject.FindGameObjectWithTag("Player");

            if (jogador != null)
            {
                MinerMovimento somPlayer = jogador.GetComponent<MinerMovimento>();
                if (somPlayer != null)
                {
                    somPlayer.AnimarBocaSom(somPlayer.sonsPaoDeQueijo);
                }
            }

            // 3. Destrói o pão de queijo do cenário
            Destroy(gameObject);
        }
    }
}
