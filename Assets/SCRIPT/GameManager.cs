using UnityEngine;
using UnityEngine.UI; // Importante para o componente Text

public class GameManager : MonoBehaviour
{
    // Instância estática para que qualquer item possa acessar o GameManager facilmente
    public static GameManager instance;

    [Header("Pontuação")]
    public int paesDeQueijoColetados = 0;
    public Text textoContador; // Arraste o seu TextoPaoDeQueijo para cá

    void Awake()
    {
        // Garante que só exista um GameManager na cena
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        AtualizarTextoUI();
    }

    // Função chamada toda vez que o Mineirinho pega um item
    public void AdicionarPaoDeQueijo(int quantidade)
    {
        paesDeQueijoColetados += quantidade;
        Debug.Log("Pão de queijo coletado! Total: " + paesDeQueijoColetados);
        AtualizarTextoUI();
    }

    void AtualizarTextoUI()
    {
        if (textoContador != null)
        {
            textoContador.text = "Paes de Queijo: " + paesDeQueijoColetados;
        }
    }
}