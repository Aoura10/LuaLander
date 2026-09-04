using UnityEngine;
using UnityEngine.InputSystem; // used for input system package

public class Lander : MonoBehaviour
{

    private Rigidbody2D landerRigidbody2D;

    private void Awake()
    {
        landerRigidbody2D = GetComponent<Rigidbody2D>();
    }
    
    private void FixedUpdate()
    {
        // for input system package
        if(Keyboard.current.upArrowKey.isPressed){
            landerRigidbody2D.AddForce(transform.up * Time.deltatime);
        }

        if(Keyboard.current.leftArrowKey.isPressed){
            Debug.Log("Left");
        }

        if(Keyboard.current.rightArrowKey.isPressed){
            Debug.Log("Right");
        }

        // for input manager
        // if(Input.GetKey(KeyCode.UpArrow))
        // {
        //     Debug.Log("Up");
        // }
    }
}
