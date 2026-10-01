using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class TesteRedeSimples : MonoBehaviour
{
    public string ipDestino = "192.168.0.x"; // IP do PC
    public int porta = 7777;
    private UdpClient udp;

    void Start()
    {
        udp = new UdpClient();
        // Se for o PC, você pode usar udp.Bind para ouvir
    }

    void Update()
    {
        // Aperte o botão 'A' (button 0) para enviar um sinal
        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            byte[] dados = Encoding.UTF8.GetBytes("ConexaoOK");
            udp.Send(dados, dados.Length, ipDestino, porta);
            Debug.Log("Sinal enviado!");
        }
    }
}