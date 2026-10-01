using UnityEngine;

public class TestadorDeBotoes : MonoBehaviour
{
    void Update()
    {
        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKeyDown("joystick button " + i))
            {
                Debug.Log("O botão que você apertou no Unity é: joystick button " + i);
            }
        }
    }
}