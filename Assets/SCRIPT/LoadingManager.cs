using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public GameObject painelLoading; // O painel que mostra o texto "Carregando..."
    public Slider barraProgresso;   // Opcional: Arraste um Slider aqui se quiser uma barra de carga

    public void CarregarJogo(string nomeCena)
    {
        StartCoroutine(ProcessoDeCarga(nomeCena));
    }

    IEnumerator ProcessoDeCarga(string nomeCena)
    {
        painelLoading.SetActive(true);

        AsyncOperation operacao = SceneManager.LoadSceneAsync(nomeCena);

        // Enquanto a cena não termina de carregar
        while (!operacao.isDone)
        {
            // O progresso vai de 0 a 0.9 (0.9 é o limite do Unity antes de trocar de cena)
            float progresso = Mathf.Clamp01(operacao.progress / 0.9f);

            if (barraProgresso != null)
            {
                barraProgresso.value = progresso;
            }

            yield return null;
        }
    }
}