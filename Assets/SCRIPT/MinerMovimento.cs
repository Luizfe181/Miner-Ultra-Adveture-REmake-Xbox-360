using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class MinerMovimento : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;
    private FocoGeral scriptFoco;
    

    [Header("Configuração do Modelo")]
    public SkinnedMeshRenderer corpoMineirinho; // Arraste o corpo do Mineirinho aqui no Inspector
    public string nomeBlendShapeBoca = "BocaAberta"; // O nome exato lá no Blender/Maya

    [Header("Configuração de Câmera")]
    public Transform cameraTransform;

    [Header("Configurações de Movimento")]
    public float velocidadeAndar = 4.0f;
    public float velocidadeCorrer = 7.5f;
    public float forcaPulo = 8.0f;
    public float gravidade = 20.0f;
    public float velocidadeRotacao = 5.0f;

    [Header("Mecânica de Chapéu")]
    public GameObject prefabChapeu;
    public Transform pontoDeDisparo;
    public GameObject chapeuDaCabeca;

    private Vector3 direcaoMovimento = Vector3.zero;
    private float velocidadeAtualH;
    private float velocidadeRef;
    private bool jaLancouChapeu = false;
    private int pulosRealizados = 0;
    [Header("Scripts Adicionais")]
    public TrocaBandana scriptTrocaBandana;

    [Header("Variações de Áudio")]
    public AudioSource audioSource;
    public AudioClip[] sonsDano;        // Lista de audios de dano
    public AudioClip[] sonsPaoDeQueijo; // Lista de audios de pao de queijo
    public AudioClip[] sonsCantoria;    // Lista de audios de cantoria

    private float tempoSemMover = 0f;
    private float tempoParaCantar = 30f;

    private float tempoUltimoSomDano = 0f;
    private float tempoUltimoSomPao = 0f;
    private float tempoUltimoSomCantoria = 0f; // Adicione esta linha aqui
    private float intervaloMinimoSom = 1.5f;
    private bool estaVivo = true;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        scriptFoco = GetComponent<FocoGeral>();
    }

    void Update()
    {
        // Lógica de Cantoria (Idle)
        Vector3 velocidadeHorizontal = new Vector3(direcaoMovimento.x, 0, direcaoMovimento.z);
        if (velocidadeHorizontal.magnitude < 0.1f)
        {
            tempoSemMover += Time.deltaTime;
            if (tempoSemMover >= tempoParaCantar && !audioSource.isPlaying)
            {
                TocarSomAleatorio(sonsCantoria, "Cantar");
                tempoSemMover = 0;
            }
        }
        else
        {
            tempoSemMover = 0;
        }
        if (cameraTransform == null) return;

        // --- MOVIMENTAÇÃO ---
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 frente = cameraTransform.forward;
        Vector3 direita = cameraTransform.right;
        frente.y = 0; direita.y = 0;
        Vector3 direcaoInput = (frente * v + direita * h).normalized;

        if (controller.isGrounded)
        {
            pulosRealizados = 0;
            bool correndo = Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Fire1");
            float alvo = (direcaoInput.magnitude > 0.1f) ? (correndo ? velocidadeCorrer : velocidadeAndar) : 0f;
            velocidadeAtualH = Mathf.SmoothDamp(velocidadeAtualH, alvo, ref velocidadeRef, 0.1f);

            float yAtual = direcaoMovimento.y;
            direcaoMovimento = direcaoInput * velocidadeAtualH;
            direcaoMovimento.y = yAtual;

            animator.SetFloat("Speed", direcaoInput.magnitude > 0.1f ? (correndo ? 2f : 1f) : 0f);

            if (direcaoInput.magnitude > 0.25f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direcaoInput), Time.deltaTime * velocidadeRotacao);

           // --- BLOCO DO PULO ---
            if (Input.GetButtonDown("Jump")) 
            { 
                direcaoMovimento.y = forcaPulo; 
                pulosRealizados = 1; 
                animator.SetTrigger("JumpTrigger"); 
            }
        } // <--- Fechamento do if (controller.isGrounded)
        else
        {
            // --- LÓGICA DO PULO DUPLO ---
            if (Input.GetButtonDown("Jump") && pulosRealizados < 2)
            {
                direcaoMovimento.y = forcaPulo; 
                pulosRealizados = 2; 
                animator.SetTrigger("JumpTrigger");
            }
        
        }
        if (Input.GetButtonDown("trocar"))// Ou o botão que preferir
    {
        if (scriptTrocaBandana != null)
        {
            scriptTrocaBandana.AlternarBandana();
        }
        else
        {
            Debug.LogError("Script TrocaBandana não está atribuído no Inspector!");
        }
    }

        direcaoMovimento.y -= gravidade * Time.deltaTime;
        controller.Move(direcaoMovimento * Time.deltaTime);

        // --- GATILHO DO CHAPÉU ---
        if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetMouseButtonDown(1))
        {
            if (!jaLancouChapeu && prefabChapeu != null && pontoDeDisparo != null && chapeuDaCabeca != null)
            {
                jaLancouChapeu = true;
                animator.SetTrigger("AtacarTrigger");
            }
        }
        
    }

    public void DispararChapeuObjeto()
    {
        if (prefabChapeu && pontoDeDisparo)
        {
            Quaternion rot = transform.rotation;
            if (scriptFoco && scriptFoco.ObterInimigoFocado())
                rot = Quaternion.LookRotation(scriptFoco.ObterInimigoFocado().position - pontoDeDisparo.position);

            Instantiate(prefabChapeu, pontoDeDisparo.position, rot);
            if (chapeuDaCabeca) chapeuDaCabeca.SetActive(false);
        }
    }

    public void RetornarChapeuParaCabeca()
    {
        if (chapeuDaCabeca)
        {
            chapeuDaCabeca.SetActive(true);
            jaLancouChapeu = false;
        }
    }
    public void TocarSomAleatorio(AudioClip[] listaSons, string tipoSom)
    {
        if (!estaVivo) return;
        // Lógica com trava de tempo
        if (tipoSom == "Dano" && Time.time < tempoUltimoSomDano + intervaloMinimoSom) return;
        if (tipoSom == "Pao" && Time.time < tempoUltimoSomPao + intervaloMinimoSom) return;
        if (tipoSom == "Cantar" && Time.time < tempoUltimoSomCantoria + intervaloMinimoSom) return; // Se tiver cantoria

        if (listaSons != null && listaSons.Length > 0 && audioSource != null)
        {
            int indice = Random.Range(0, listaSons.Length);
            audioSource.PlayOneShot(listaSons[indice]);

            if (tipoSom == "Dano") tempoUltimoSomDano = Time.time;
            if (tipoSom == "Pao") tempoUltimoSomPao = Time.time;
            if (tipoSom == "Cantar") tempoUltimoSomCantoria = Time.time;
        }
    }
    // Chama isso junto com TocarSomAleatorio(sonsDano) ou sonsCantoria
    // Método ÚNICO para tocar som e animar a boca
    public void AnimarBocaSom(AudioClip[] listaSons)
    {
        if (!estaVivo) return;
        // 1. Trava de tempo (Cooldown) para não repetir sons muito rápido
        if (Time.time < tempoUltimoSomPao + intervaloMinimoSom) return;

        if (corpoMineirinho == null || audioSource == null) return;
        if (listaSons == null || listaSons.Length == 0) return;

        // 2. Escolhe o som
        AudioClip somEscolhido = listaSons[Random.Range(0, listaSons.Length)];
        audioSource.PlayOneShot(somEscolhido);

        // 3. Inicia a sincronia visual
        StartCoroutine(SincronizarBocaComAudio(somEscolhido.length));

        // 4. Marca o tempo do último som (ajuste o nome da variável se precisar)
        tempoUltimoSomPao = Time.time;
    }

    private IEnumerator SincronizarBocaComAudio(float duracao)
    {
        int blendShapeIndex = corpoMineirinho.sharedMesh.GetBlendShapeIndex(nomeBlendShapeBoca);
        if (blendShapeIndex == -1) yield break;

        float tempo = 0f;
        // Precisamos criar um buffer para ler os dados do áudio
        float[] buffer = new float[1024];

        while (tempo < duracao)
        {
            // Pega os dados da forma de onda atual (Waveform)
            audioSource.GetOutputData(buffer, 0);

            // Calcula a média da amplitude (o volume real do áudio agora)
            float amplitude = 0f;
            foreach (float s in buffer) amplitude += Mathf.Abs(s);
            amplitude /= 1024;

            // Multiplique por um valor alto (ex: 50 a 200) para ver a boca mexer
            // Se ainda não mexer, aumente esse 100f
            float peso = Mathf.Clamp(amplitude * 100f, 0f, 100f);

            corpoMineirinho.SetBlendShapeWeight(blendShapeIndex, peso);

            tempo += Time.deltaTime;
            yield return null;
        }

        // Garante que fecha a boca no final
        corpoMineirinho.SetBlendShapeWeight(blendShapeIndex, 0f);
    }
    public void Morrer()
    {
        estaVivo = false;
        StopAllCoroutines(); // Para a boca de mexer

        if (audioSource != null)
        {
            audioSource.Stop(); // <--- ISSO PARA O SOM IMEDIATAMENTE
        }

        if (corpoMineirinho != null)
        {
            int index = corpoMineirinho.sharedMesh.GetBlendShapeIndex(nomeBlendShapeBoca);
            if (index != -1) corpoMineirinho.SetBlendShapeWeight(index, 0f); // Fecha a boca
        }
    }
}