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
            float force = 500f;
            landerRigidbody2D.AddForce(force * transform.up * Time.fixedDeltaTime);
        }

        if(Keyboard.current.aKey.isPressed)
        {
            float turnSpeed = +80f;
            landerRigidbody2D.AddTorque(turnSpeed * Time.fixedDeltaTime);
        }

        if(Keyboard.current.dKey.isPressed)
        {
            float turnSpeed = -80f;
            landerRigidbody2D.AddTorque(turnSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D Collision2D)
    {
        if (!Collision2D.gameObject.TryGetComponent(out LandingPad langingPad))
        {
            Debug.Log("Crashed on the Terrain!");
            return;
        }

        float softLandingVelocityMagnitude = 4f;
        float relativeVelocityMagnitude = Collision2D.relativeVelocity.magnitude;
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

        float maxScoreAmountLandingAngle = 100;
        float scoreDotVectorMultiplier = 10f;
        float landingAngleScore = maxScoreAmountLandingAngle - Mathf.Abs(dotVector - 1f) * scoreDotVectorMultiplier * maxScoreAmountLandingAngle;
        
        float maxScoreAmountLandingSpeed = 100;
        float landingSpeedScore = (softLandingVelocityMagnitude - relativeVelocityMagnitude) * maxScoreAmountLandingSpeed;

        Debug.Log("landingAngleScore: " + landingAngleScore);
        Debug.Log("landingSpeedScore: " + landingSpeedScore);
    }
}
