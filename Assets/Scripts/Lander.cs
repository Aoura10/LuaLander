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
        // for input manager
        // if(Input.GetKey(KeyCode.UpArrow))
        // {
        //     Debug.Log("Up");
        // }

        // for input system package
        if(Keyboard.current.wKey.isPressed)
        {
            float force = 700f;
            landerRigidbody2D.AddForce(force * transform.up * Time.fixedDeltaTime);
        }

        if(Keyboard.current.aKey.isPressed)
        {
            float turnSpeed = +100f;
            landerRigidbody2D.AddTorque(turnSpeed * Time.fixedDeltaTime);
        }

        if(Keyboard.current.dKey.isPressed)
        {
            float turnSpeed = -100f;
            landerRigidbody2D.AddTorque(turnSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D Collision2D)
    {
        float softLandingVelocityMagnitude = 4f;
        if (Collision2D.relativeVelocity.magnitude > softLandingVelocityMagnitude)
        {
            // Landed too hard!
            Debug.Log("Landed too hard!");
            return;
        }

        float dotVector = Vector2.Dot(Vector2.up, transform.up);
        float minDotVector = .90f;
        if (dotVector < minDotVector)
        {
            // Landed on a too steep angle!
            Debug.Log("Landed on a too steep angle!");
            return;
        }
            
        Debug.Log("Successful Landing!");   
    }
}
