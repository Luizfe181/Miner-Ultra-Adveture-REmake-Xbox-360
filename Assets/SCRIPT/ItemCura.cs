using UnityEngine;

public class ItemCura : MonoBehaviour
{
    public float velocidadeRotacao = 100f;

    void Update()
    {
        // Mantém o item girando no cenário
        transform.Rotate(Vector3.forward * velocidadeRotacao * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        string nomeObjeto = other.gameObject.name.ToLower();

        // 1. Verifica se quem tocou é o jogador ou o chapéu
        if (other.CompareTag("Player") || nomeObjeto.Contains("mineirinho") ||
            other.CompareTag("Chapeu") || nomeObjeto.Contains("chapeu"))
        {
            PlayerVida scriptVida = other.GetComponent<PlayerVida>();

            // Se não achou diretamente, procura no pai (caso o chapéu tenha pego)
            if (scriptVida == null)
            {
                GameObject jogador = GameObject.FindGameObjectWithTag("Player");
                if (jogador != null) scriptVida = jogador.GetComponent<PlayerVida>();
            }

            // 2. Ação única de cura e som
            if (scriptVida != null)
            {
                // Aplica a cura
                scriptVida.RegenerarVidaCompleta();

                // Toca o som de Pão de Queijo
                MinerMovimento somPlayer = scriptVida.GetComponent<MinerMovimento>();
                if (somPlayer != null)
                {
                    somPlayer.AnimarBocaSom(somPlayer.sonsPaoDeQueijo);
                }

                // Destrói o item
                Destroy(gameObject);
            }
        }
    }
}
      