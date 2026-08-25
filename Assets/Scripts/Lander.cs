using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour
{
    private void Update()
    {
        // for input manager
        // if(Input.GetKey(KeyCode.UpArrow))
        // {
        //     Debug.Log("Up");
        // }
    
        // for input system 
        if(Keyboard.current.upArrowKey.isPressed){
            Debug.Log("Up");
        }

        if(Keyboard.current.leftArrowKey.isPressed){
            Debug.Log("Left");
        }

        if(Keyboard.current.rightArrowKey.isPressed){
            Debug.Log("Right");
        }

        if(Keyboard.current.downArrowKey.isPressed){
            Debug.Log("Down");
        }
    }
}
