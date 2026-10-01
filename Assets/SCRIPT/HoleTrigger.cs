using UnityEngine;

public class HoleTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Pega o script de vida do jogador
            PlayerVida vida = other.GetComponent<PlayerVida>();

            if (vida != null)
            {
                // Faz o jogador morrer instantaneamente ao cair no buraco
                vida.Morrer();
            }
        }
    }
}