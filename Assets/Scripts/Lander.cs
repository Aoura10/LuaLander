using UnityEngine;
using UnityEngine.InputSystem; // used for input system package

public class Lander : MonoBehaviour
{
    private void Update()
    {
        // for input manager
        // if(Input.GetKey(KeyCode.UpArrow))
        // {
        //     Debug.Log("Up");
        // }
    
        // for input system package
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
