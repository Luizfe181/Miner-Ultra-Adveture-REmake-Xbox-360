using UnityEngine;

public class VerificadorRede : MonoBehaviour
{
    void OnGUI()
    {
        GUI.skin.label.fontSize = 24;

        // No sistema antigo, usamos Network.peerType para verificar a conexão
        if (Network.peerType == NetworkPeerType.Client || Network.peerType == NetworkPeerType.Server)
        {
            GUI.Label(new Rect(20, 20, 500, 100), "STATUS: CONECTADO À REDE");
        }
        else
        {
            GUI.Label(new Rect(20, 20, 500, 100), "STATUS: DESCONECTADO");
        }
    }
}