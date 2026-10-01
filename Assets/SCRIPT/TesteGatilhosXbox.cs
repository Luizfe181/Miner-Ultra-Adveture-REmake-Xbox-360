using UnityEngine;
using System.Collections.Generic;

public class TesteGatilhosXbox : MonoBehaviour
{
    private List<string> eixosValidos = new List<string>();

    void Start()
    {
        // Cria uma lista de eixos possíveis para testar
        for (int i = 1; i <= 20; i++)
        {
            eixosValidos.Add("Joy1 Axis " + i);
        }
    }

    void Update()
    {
        foreach (string nomeEixo in eixosValidos)
        {
            if (EixoExiste(nomeEixo))
            {
                float valor = Input.GetAxisRaw(nomeEixo);
                if (Mathf.Abs(valor) > 0.1f)
                {
                    Debug.Log("Movimento no " + nomeEixo + ": " + valor);
                }
            }
        }
    }

    // Função que verifica se o eixo está definido no Input Manager
    bool EixoExiste(string nome)
    {
        try
        {
            Input.GetAxis(nome);
            return true;
        }
        catch
        {
            return false;
        }
    }
}