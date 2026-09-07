using UnityEngine;
using UnityEngine.InputSystem; // used for input system package
using System;

public class Lander : MonoBehaviour
{

    public static Lander Instance{ get; private set;}


    
    public event EventHandler OnUpForce;
    public event EventHandler OnRightForce;
    public event EventHandler OnLeftForce;
    public event EventHandler OnBeforeForce;
    public event EventHandler OnCoinPickup;
    public event EventHandler<OnLandedEventArgs> OnLanded;
    public class OnLandedEventArgs : EventArgs
    {   
        public LandingType landingType;
        public int score;
        public float dotVector;
        public float landingSpeed;
        public float scoreMultiplier;
    }

    public enum LandingType
    {
        Success,
        WrongLangingArea,
        TooSteepAngle,
        TooFastLanding,
    }

    private Rigidbody2D landerRigidbody2D;
    private float fuelAmount;
    private float fuelAmountMax = 10f;


    private void Awake()
    {
        Instance = this;
        
        fuelAmount = fuelAmountMax;

        landerRigidbody2D = GetComponent<Rigidbody2D>();
    }
    
    private void FixedUpdate()
    {
        OnBeforeForce?.Invoke(this, EventArgs.Empty);

        // Debug.Log(fuelAmount);

        if (fuelAmount <= 0f)
        {
            // No Fuel
            return;
        }

        if (Keyboard.current.wKey.isPressed ||
            Keyboard.current.wKey.isPressed ||
            Keyboard.current.wKey.isPressed)
            {
                // Pressing any input
                ConsumeFuel();
            }

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

            OnUpForce?.Invoke(this, EventArgs.Empty);
        }

        if(Keyboard.current.aKey.isPressed)
        {
            float turnSpeed = +80f;
            landerRigidbody2D.AddTorque(turnSpeed * Time.fixedDeltaTime);

            OnLeftForce?.Invoke(this, EventArgs.Empty);

        }

        if(Keyboard.current.dKey.isPressed)
        {
            float turnSpeed = -80f;
            landerRigidbody2D.AddTorque(turnSpeed * Time.fixedDeltaTime);

            OnRightForce?.Invoke(this, EventArgs.Empty);

        }
    }

    private void OnCollisionEnter2D(Collision2D Collision2D)
    {
        if (!Collision2D.gameObject.TryGetComponent(out LandingPad langingPad))
        {
            Debug.Log("Crashed on the Terrain!");
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {   
                landingType = LandingType.WrongLangingArea,
                dotVector = 0f,
                landingSpeed = 0f,
                scoreMultiplier = 0,
                score = 0,
            });
            return;
        }

        float softLandingVelocityMagnitude = 4f;
        float relativeVelocityMagnitude = Collision2D.relativeVelocity.magnitude;
        if (Collision2D.relativeVelocity.magnitude > softLandingVelocityMagnitude)
        {
            // Landed too hard!
            Debug.Log("Landed too hard!");
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {   
                landingType = LandingType.TooFastLanding,
                dotVector = 0f,
                landingSpeed = relativeVelocityMagnitude,
                scoreMultiplier = 0,
                score = 0,
            });
            return;
        }

        float dotVector = Vector2.Dot(Vector2.up, transform.up);
        float minDotVector = .90f;
        if (dotVector < minDotVector)
        {
            // Landed on a too steep angle!
            Debug.Log("Landed on a too steep angle!");
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {   
                landingType = LandingType.TooSteepAngle,
                dotVector = dotVector,
                landingSpeed = relativeVelocityMagnitude,
                scoreMultiplier = 0,
                score = 0,
            });
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

        int score = Mathf.RoundToInt((landingAngleScore + landingSpeedScore) * langingPad.GetScoreMultiplier());

        Debug.Log("Score: " + score);
        OnLanded?.Invoke(this, new OnLandedEventArgs
        {   
            landingType = LandingType.Success,
            dotVector = dotVector,
            landingSpeed = relativeVelocityMagnitude,
            scoreMultiplier = langingPad.GetScoreMultiplier(),
            score = score,
        });
    }
    private void OnTriggerEnter2D(Collider2D collider2D)
        {
            if (collider2D.gameObject.TryGetComponent(out FuelPickup fuelPickup))
            {
                float addFuelAmount = 10f;
                fuelAmount += addFuelAmount;
                if (fuelAmount > fuelAmountMax)
                {
                    fuelAmount = fuelAmountMax;
                }
            
                // Destroy(collider2D.gameObject); can do this but not ideal for clean code
                fuelPickup.DestroySelf(); // calls DestroySelf() function from FuelPickup script
            }

            if (collider2D.gameObject.TryGetComponent(out CoinPickup coinPickup))
            {
                OnCoinPickup?.Invoke(this, EventArgs.Empty);
                coinPickup.DestroySelf();
            }
        }
    private void ConsumeFuel()
    {
        float fuelComsumptionAmount = 1f;
        fuelAmount -= fuelComsumptionAmount * Time.fixedDeltaTime;
    }
    
    public float GetFuel()
    {
        return fuelAmount;
    }

    public float GetFuelAmountNormalized()
    {
        return fuelAmount / fuelAmountMax;
    }

    public float GetSpeedX()
    {
        return landerRigidbody2D.linearVelocityX;
    }

    public float GetSpeedY()
    {
        return landerRigidbody2D.linearVelocityY;
    }
}
