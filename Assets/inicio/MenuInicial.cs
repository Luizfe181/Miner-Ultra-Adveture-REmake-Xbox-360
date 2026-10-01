using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MenuInicial : MonoBehaviour
{
    [Header("Configurações de UI")]
    public GameObject canvasMenu;
    public Image painelPreto;
    public GameObject botaoIniciar;

    [Header("Configurações de Loading")]
    public GameObject painelLoading; // Opcional: crie um painel com um texto "Carregando..."
    public Slider barraProgresso;    // Opcional: seu slider

    [Header("Configurações de Cena")]
    public string nomeCenaJogo = "CenaPrincipal";

    private bool transicaoIniciada = false;
    [Header("Configurações de Introdução")]
    public Animation cameraAnimation; // Agora você arrasta o componente Animation da sua câmera aqui
    public string nomeDaAnimacao = "CameraIntro"; // Nome exato que você deu na janela Animation
    public GameObject painelBotoes;

    void Start()
    {
        // Garante que os botões comecem invisíveis
        if (painelBotoes != null) painelBotoes.SetActive(false);

        // ADICIONE ESTA LINHA AQUI: Garante que o loading comece desligado
        if (painelLoading != null) painelLoading.SetActive(false);

        // Reseta o painel preto de fade
        if (painelPreto != null) painelPreto.color = new Color(0, 0, 0, 0);

        if (cameraAnimation != null)
        {
            cameraAnimation.Play(nomeDaAnimacao);
            StartCoroutine(EsperarIntro(cameraAnimation[nomeDaAnimacao].length));
        }
    }

    IEnumerator EsperarIntro(float duracao)
    {
        yield return new WaitForSeconds(duracao);

        // Mostra os botões após a animação terminar
        if (painelBotoes != null) painelBotoes.SetActive(true);

        // Foca no botão
        if (botaoIniciar != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(botaoIniciar);
        }
    }
    void Update()
    {
        // Verifica se o botão "Submit" (A do Xbox) foi pressionado
        if (Input.GetButtonDown("Submit"))
        {
            // Se houver um botão selecionado, dispara a função manualmente
            GameObject selecionado = EventSystem.current.currentSelectedGameObject;
            if (selecionado == botaoIniciar)
            {
                IniciarJogo();
            }
        }
    }

    public void IniciarJogo()
    {
        if (transicaoIniciada) return;
        transicaoIniciada = true;

        StartCoroutine(ProcessoDeCarga());
    }

    IEnumerator ProcessoDeCarga()
    {
        // O Fade ocorre agora, logo após o clique
        float tempo = 0;
        while (tempo < 0.5f) // Fade mais curto, apenas 0.5 segundos
        {
            tempo += Time.deltaTime;
            painelPreto.color = new Color(0, 0, 0, tempo / 0.5f);
            yield return null;
        }

        // Ativa o ícone de Loading
        if (painelLoading != null) painelLoading.SetActive(true);

        // Carrega a cena instantaneamente
        AsyncOperation operacao = SceneManager.LoadSceneAsync(nomeCenaJogo);

        // Não precisa de loop de barra de progresso se você quer rapidez,
        // apenas espera a operação terminar
        yield return new WaitUntil(() => operacao.isDone);
    }
}