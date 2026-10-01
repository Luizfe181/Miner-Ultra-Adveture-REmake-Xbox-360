using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instancia;

    [Header("Configurações de UI")]
    public GameObject painelGameOver;
    public GameObject botaoReiniciar;
    public GameObject painelPausa;
    public GameObject botaoContinuar;

    void Awake()
    {
        // Singleton simples e seguro
        if (Instancia == null)
        {
            Instancia = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (painelGameOver) painelGameOver.SetActive(false);
        if (botaoReiniciar) botaoReiniciar.SetActive(false);
        if (painelPausa) painelPausa.SetActive(false);
    }

    void Update()
    {
        // Detecta o botão de pausa
        if (Input.GetButtonDown("Pausa"))
        {
            if (Time.timeScale == 1f) PausarJogo();
            else ContinuarJogo();
        }

        // Evita erros se o EventSystem não estiver na cena ainda
        if (EventSystem.current == null) return;

        // SE O MENU DE PAUSA ESTIVER ATIVO: Garante que o controle não perca o foco
        if (painelPausa != null && painelPausa.activeSelf)
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(botaoContinuar);
            }
        }

        // SE O GAME OVER ESTIVER ATIVO: Garante que o controle também não perca o foco aqui!
        if (painelGameOver != null && painelGameOver.activeSelf)
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(botaoReiniciar);
            }
        }
    }

    public void PausarJogo()
    {
        if (painelPausa) painelPausa.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Força a seleção do botão ao abrir o menu de forma segura
        if (botaoContinuar) StartCoroutine(ForcarFoco(botaoContinuar));
    }

    public void ContinuarJogo()
    {
        if (painelPausa) painelPausa.SetActive(false);
        Time.timeScale = 1f;
        
        // Opcional: Esconder o cursor novamente ao voltar pro jogo
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void MostrarGameOver()
    {
        StartCoroutine(AtrasarPausa());
    }

    IEnumerator AtrasarPausa()
    {
        yield return new WaitForSecondsRealtime(4.5f);

        if (painelGameOver) painelGameOver.SetActive(true);
        if (botaoReiniciar) botaoReiniciar.SetActive(true);

        if (botaoReiniciar) StartCoroutine(ForcarFoco(botaoReiniciar));
        Time.timeScale = 0f;
    }

    public void ReiniciarJogo()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SairDoJogo()
    {
        Debug.Log("Saindo do jogo...");
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Coroutine robusta para garantir o foco no controle
    IEnumerator ForcarFoco(GameObject targetButton)
    {
        if (EventSystem.current == null) yield break;

        // Limpa a seleção atual
        EventSystem.current.SetSelectedGameObject(null);
        
        // Espera até que o frame termine e a UI física seja renderizada e ativada
        yield return new WaitForEndOfFrame();
        
        // Define o novo botão focado
        EventSystem.current.SetSelectedGameObject(targetButton);
    }
}