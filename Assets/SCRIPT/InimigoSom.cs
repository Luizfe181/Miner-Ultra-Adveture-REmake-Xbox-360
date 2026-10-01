using UnityEngine;
public class InimigoSom : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] sonsIdle;
    public AudioClip[] sonsSurpreso;
    public AudioClip[] sonsMorte;

    public float tempoMinimoIdle = 5f;
    public float tempoMaximoIdle = 12f;
    private float timerIdle;

    // VARIÁVEL DE CONTROLE
    public bool estaEmCombate = false;

    void Start()
    {
        timerIdle = Random.Range(tempoMinimoIdle, tempoMaximoIdle);
    }

    void Update()
    {
        // SE ESTIVER EM COMBATE, NÃO TOCA IDLE!
        if (estaEmCombate) return;

        timerIdle -= Time.deltaTime;

        if (timerIdle <= 0)
        {
            TocarSom(sonsIdle);
            timerIdle = Random.Range(tempoMinimoIdle, tempoMaximoIdle);
        }
    }

    public void TocarSom(AudioClip[] listaSons)
    {
        if (listaSons != null && listaSons.Length > 0 && audioSource != null)
        {
            AudioClip clip = listaSons[Random.Range(0, listaSons.Length)];
            audioSource.PlayOneShot(clip);
        }
    }
}