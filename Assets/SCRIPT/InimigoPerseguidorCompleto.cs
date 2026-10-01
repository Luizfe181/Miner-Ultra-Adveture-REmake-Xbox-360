using UnityEngine;
using System.Collections;

public class InimigoPerseguidorCompleto : MonoBehaviour
{
    [Header("Configurações de IA")]
    private Transform alvo;
    public float velocidade = 3.0f;
    public float distanciaDeVisao = 10.0f;
    public float distanciaDeAtaque = 3.0f;
    public float tempoDeSusto = 1.0f;

    private Animator animator;
    private bool foiSurpreso = false;
    private bool estaPerseguindo = false;
    private bool estaAtacando = false;

    [Header("Configurações do Laser")]
    public LineRenderer linhaLaser;
    public Transform origemLaser;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (linhaLaser != null) linhaLaser.enabled = false;

        // BUSCA AUTOMÁTICA PELO JOGADOR
        GameObject jogadorEncontrado = GameObject.FindGameObjectWithTag("Player");
        if (jogadorEncontrado != null)
        {
            alvo = jogadorEncontrado.transform;
        }
        else
        {
            Debug.LogError("Inimigo não encontrou um objeto com a Tag 'Player' na cena!");
        }
    }

    void Update()
    {
        if (alvo == null) return;

        float distancia = Vector3.Distance(transform.position, alvo.position);

        if (distancia < distanciaDeVisao)
        {
            if (!estaAtacando) // Inimigo só olha enquanto não está atacando
                transform.LookAt(new Vector3(alvo.position.x, transform.position.y, alvo.position.z));

            if (!foiSurpreso) StartCoroutine(IniciarSusto());
           

            if (distancia < distanciaDeAtaque && estaPerseguindo && !estaAtacando)
            {
                StartCoroutine(ExecutarAtaque());
            }
            else if (estaPerseguindo && !estaAtacando)
            {
                transform.position = Vector3.MoveTowards(transform.position, alvo.position, velocidade * Time.deltaTime);
                animator.SetBool("Andando", true);
            }
        }
        else
        {
            foiSurpreso = false;
            estaPerseguindo = false;
            animator.SetBool("Andando", false);
        }
    }

    IEnumerator IniciarSusto()
    {
        foiSurpreso = true;

        // Avisa o script de som que começou o combate
        GetComponent<InimigoSom>().estaEmCombate = true;

        GetComponent<InimigoSom>().TocarSom(GetComponent<InimigoSom>().sonsSurpreso);
        animator.SetTrigger("Surpreso");

        yield return new WaitForSeconds(tempoDeSusto);
        estaPerseguindo = true;
    }

    IEnumerator ExecutarAtaque()
    {
        estaAtacando = true;
        animator.SetBool("Andando", false);
        animator.SetTrigger("Atacar");
        yield return new WaitForSeconds(1.5f);
        estaAtacando = false;
    }

    // ESTA FUNÇÃO DEVE SER CHAMADA PELO ANIMATION EVENT NO FRAME 25
    public void AtivarLaser()
    {
        StartCoroutine(SequenciaLaser());
    }

    IEnumerator SequenciaLaser()
    {
        linhaLaser.enabled = true;
        RaycastHit hit;
        Vector3 direcao = origemLaser.forward;
        linhaLaser.SetPosition(0, origemLaser.position);
        linhaLaser.SetPosition(1, origemLaser.position + (direcao * 20f));

        if (Physics.Raycast(origemLaser.position, direcao, out hit, 20f))
        {
            if (hit.collider.CompareTag("Player"))
            {
                // Busca o script de vida
                PlayerVida pv = hit.collider.GetComponent<PlayerVida>();

                if (pv != null)
                {
                    // Calcula a direção: do Inimigo para o Jogador
                    Vector3 direcaoEmpurrao = hit.collider.transform.position - transform.position;

                    // Aplica Dano e Empurrão de uma só vez usando o método do PlayerVida
                    pv.TomarDano(1, direcaoEmpurrao);
                }
            }
        }

        yield return new WaitForSeconds(0.5f);
        linhaLaser.enabled = false;
    }
}