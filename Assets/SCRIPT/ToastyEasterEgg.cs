using System.Collections;
using UnityEngine;

public class ToastyEasterEgg : MonoBehaviour
{
    [Header("Componentes de UI e Áudio")]
    [SerializeField] private RectTransform toastyImageRect; // Arraste a Image da UI aqui
    [SerializeField] private AudioSource audioSource;       // Arraste o AudioSource aqui
    [SerializeField] private AudioClip toastyClip;          // Arraste o áudio do "Toasty" aqui

    [Header("Configurações da Animação")]
    [SerializeField] private Vector2 posicaoEscondido = new Vector2(200f, -200f); 
    [SerializeField] private Vector2 posicaoAparecendo = new Vector2(0f, 0f);     
    [SerializeField] private float tempoTransicao = 0.15f;
    [SerializeField] private float tempoExibicao = 0.8f;

    // Definição dos passos do Konami Code em texto para facilitar a leitura
    private enum InputPasso { Cima, Baixo, Esquerda, Direita, B, A }

    private readonly InputPasso[] konamiCode = new InputPasso[]
    {
        InputPasso.Cima, InputPasso.Cima,
        InputPasso.Baixo, InputPasso.Baixo,
        InputPasso.Esquerda, InputPasso.Direita,
        InputPasso.Esquerda, InputPasso.Direita,
        InputPasso.B, InputPasso.A
    };

    private int indiceAtual = 0;
    private bool estaAtivo = false;
    private bool dpadEmUso = false; // Evita que segurar o D-Pad conte como múltiplos cliques

    void Start()
    {
        if (toastyImageRect != null)
        {
            toastyImageRect.anchoredPosition = posicaoEscondido;
        }
    }

    void Update()
    {
        DetectarInputControle();
    }

    private void DetectarInputControle()
    {
        InputPasso? inputDetectado = ObterDirecaoDpadOuBotao();

        if (inputDetectado != null)
        {
            // Se o botão pressionado for o correto na sequência
            if (inputDetectado == konamiCode[indiceAtual])
            {
                indiceAtual++;

                if (indiceAtual >= konamiCode.Length)
                {
                    TriggerToasty();
                    indiceAtual = 0;
                }
            }
            else
            {
                // Se errar o botão, reseta (mas se o erro foi "Cima", já inicia a nova tentativa no passo 1)
                indiceAtual = (inputDetectado == InputPasso.Cima) ? 1 : 0;
            }
        }
    }

    // Lê os eixos do D-Pad (controle) e botões
    private InputPasso? ObterDirecaoDpadOuBotao()
    {
        // 1. Verificar botões de ação (Mapeamento padrão da Unity para controle)
        // No Xbox: "joystick button 0" é o A, "joystick button 1" é o B.
        // No PlayStation: "joystick button 0" é o X (equivalente ao A), "joystick button 1" é o Círculo (B).
        if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.B)) 
            return InputPasso.B;
        
        if (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.A)) 
            return InputPasso.A;

        // 2. Verificar Teclado (Setas) para continuar funcionando no PC se quiser
        if (Input.GetKeyDown(KeyCode.UpArrow)) return InputPasso.Cima;
        if (Input.GetKeyDown(KeyCode.DownArrow)) return InputPasso.Baixo;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) return InputPasso.Esquerda;
        if (Input.GetKeyDown(KeyCode.RightArrow)) return InputPasso.Direita;

        // 3. Verificar D-Pad do Controle (Eixos virtuais da Unity)
        // Por padrão, a Unity lê o D-Pad horizontal no "POVX" / "7th Axis" e o vertical no "POVY" / "8th Axis"
        // Para simplificar sem exigir que você configure a Input Manager, usamos os eixos "Horizontal" e "Vertical" 
        // ou os botões de D-Pad comuns se mapeados. 
        // Abaixo está a leitura padrão dos eixos do D-Pad:
        float dpadX = Input.GetAxisRaw("Horizontal"); // Funciona com o analógico e D-Pad na maioria dos setups
        float dpadY = Input.GetAxisRaw("Vertical");

        if (!dpadEmUso)
        {
            if (dpadY > 0.5f) { dpadEmUso = true; return InputPasso.Cima; }
            if (dpadY < -0.5f) { dpadEmUso = true; return InputPasso.Baixo; }
            if (dpadX < -0.5f) { dpadEmUso = true; return InputPasso.Esquerda; }
            if (dpadX > 0.5f) { dpadEmUso = true; return InputPasso.Direita; }
        }
        else
        {
            // Só libera para um novo clique do D-Pad quando o jogador soltar o botão (voltar ao centro)
            if (Mathf.Abs(dpadX) < 0.2f && Mathf.Abs(dpadY) < 0.2f)
            {
                dpadEmUso = false;
            }
        }

        return null; // Nenhum comando válido neste frame
    }

    public void TriggerToasty()
    {
        if (!estaAtivo)
        {
            StartCoroutine(ToastySequence());
        }
    }

    private IEnumerator ToastySequence()
    {
        estaAtivo = true;

        if (audioSource != null && toastyClip != null)
        {
            audioSource.PlayOneShot(toastyClip);
        }

        // Desliza para dentro da tela
        float tempoAtivo = 0f;
        while (tempoAtivo < tempoTransicao)
        {
            tempoAtivo += Time.deltaTime;
            toastyImageRect.anchoredPosition = Vector2.Lerp(posicaoEscondido, posicaoAparecendo, tempoAtivo / tempoTransicao);
            yield return null;
        }
        toastyImageRect.anchoredPosition = posicaoAparecendo;

        yield return new WaitForSeconds(tempoExibicao);

        // Desliza para fora da tela
        tempoAtivo = 0f;
        while (tempoAtivo < tempoTransicao)
        {
            tempoAtivo += Time.deltaTime;
            toastyImageRect.anchoredPosition = Vector2.Lerp(posicaoAparecendo, posicaoEscondido, tempoAtivo / tempoTransicao);
            yield return null;
        }
        toastyImageRect.anchoredPosition = posicaoEscondido;

        estaAtivo = false;
    }
}