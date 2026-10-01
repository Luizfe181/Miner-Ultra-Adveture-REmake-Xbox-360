using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerVida : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public float vidaMaxima = 100f;
    public float vidaAtual;

    [Header("Configurações de Feedback")]
    public float forcaEmpurrao = 6f;
    public List<Renderer> partesDoCorpo;

    void Start()
    {
        vidaAtual = vidaMaxima;
    }

    // No PlayerVida.cs, adicione um Debug para testar
    public void TomarDano(float dano, Vector3 direcao)
    {
        vidaAtual -= dano;

        // Se a vida for <= 0, não chame o som de dano!
        if (vidaAtual > 0)
        {
            MinerMovimento movimento = GetComponent<MinerMovimento>();
            if (movimento != null)
            {
                movimento.AnimarBocaSom(movimento.sonsDano);
            }
        }

        
       
        StartCoroutine(AplicarEmpurrao(direcao.normalized));
        StartCoroutine(EfeitoPiscar());

        if (vidaAtual <= 0) Morrer();
    }

    public void RegenerarVidaCompleta()
    {
        vidaAtual = vidaMaxima;
        Debug.Log("Vida regenerada!");
    }

    IEnumerator EfeitoPiscar()
    {
        for (int i = 0; i < 5; i++)
        {
            foreach (Renderer parte in partesDoCorpo)
            {
                if (parte != null) parte.enabled = !parte.enabled;
            }
            yield return new WaitForSeconds(0.1f);
        }

        foreach (Renderer parte in partesDoCorpo)
        {
            if (parte != null) parte.enabled = true;
        }
    }

    IEnumerator AplicarEmpurrao(Vector3 direcao)
    {
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
        {
            float tempo = 0;
            while (tempo < 0.2f)
            {
                cc.Move(direcao * forcaEmpurrao * Time.deltaTime);
                tempo += Time.deltaTime;
                yield return null;
            }
        }
    }

    public void Morrer()
    {
        // Força a vida a zero para garantir que o sistema entenda que ele morreu
        vidaAtual = 0;

        var movimento = GetComponent<MinerMovimento>();
        if (movimento != null)
        {
            movimento.enabled = false;
            movimento.Morrer();
        }

        GetComponent<Animator>().SetTrigger("Morte");

        if (UIManager.Instancia != null)
        {
            UIManager.Instancia.MostrarGameOver();
        }
    }
}