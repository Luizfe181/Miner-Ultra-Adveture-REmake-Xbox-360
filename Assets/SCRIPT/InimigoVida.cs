using UnityEngine;

public class InimigoVida : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int vidaMaxima = 1;
    private int vidaAtual;

    [Header("Configurações de Ataque")]
    public int danoNoJogador = 1;

    void Start()
    {
        vidaAtual = vidaMaxima;
    }

    private void OnTriggerEnter(Collider other)
    {
        string nomeObjeto = other.gameObject.name.ToLower();

        // 1. SE FOR O JOGADOR ENCOSTANDO NO BLOCO:
        if (other.CompareTag("Player") || nomeObjeto.Contains("mineirinho"))
        {
            // CORREÇÃO: Usamos 'other' (o collider que entrou) e não 'hit.collider'
            PlayerVida pv = other.GetComponent<PlayerVida>();

            if (pv != null)
            {
                // Passamos o dano configurado e a direção neutra
                pv.TomarDano(danoNoJogador, Vector3.zero);
            }
        }
        // 2. SE FOR O CHAPÉU ENCOSTANDO NO BLOCO:
        else if (other.CompareTag("Chapeu") || nomeObjeto.Contains("chapeu"))
        {
            TomarDano(1);
        }
    }

    public void TomarDano(int quantidadeDano)
    {
        vidaAtual -= quantidadeDano;

        if (vidaAtual <= 0)
        {
            DerrotarInimigo();
        }
    }

    void DerrotarInimigo()
    {
        Destroy(gameObject);
    }
}