using UnityEngine;
using UnityEngine.InputSystem; // used for input system package
using System;

public class Lander : MonoBehaviour
{

    private const float GRAVITY_NORMAL = 0.7f;

    public static Lander Instance{ get; private set;}

    
    public event EventHandler OnUpForce;
    public event EventHandler OnRightForce;
    public event EventHandler OnLeftForce;
    public event EventHandler OnBeforeForce;
    public event EventHandler OnCoinPickup;

    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

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
        WrongLandingArea,
        TooSteepAngle,
        TooFastLanding,
    }

    public enum State
    {
        WaitingToStart,
        Normal,
        GameOver,
    }


    private Rigidbody2D landerRigidbody2D;
    private float fuelAmount;
    private float fuelAmountMax = 10f;
    private State state;

    private void Awake()
    {
        Instance = this;
        
        fuelAmount = fuelAmountMax;
        state = State.WaitingToStart;

        landerRigidbody2D = GetComponent<Rigidbody2D>();
        landerRigidbody2D.gravityScale = 0f;
    }
    
    private void FixedUpdate()
    {
        OnBeforeForce?.Invoke(this, EventArgs.Empty);

        switch (state)
        {
            default:
            case State.WaitingToStart:
                if (Keyboard.current.wKey.isPressed ||
                    Keyboard.current.wKey.isPressed ||
                    Keyboard.current.wKey.isPressed)
                    {
                        landerRigidbody2D.gravityScale = GRAVITY_NORMAL;
                        SetState(State.Normal);
                    }
                break;
            case State.Normal:
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
                    float force = 600f;
                    landerRigidbody2D.AddForce(force * transform.up * Time.fixedDeltaTime);

                    OnUpForce?.Invoke(this, EventArgs.Empty);
                }

                if(Keyboard.current.aKey.isPressed)
                {
                    float turnSpeed = +85f;
                    landerRigidbody2D.AddTorque(turnSpeed * Time.fixedDeltaTime);

                    OnLeftForce?.Invoke(this, EventArgs.Empty);
                }

                if(Keyboard.current.dKey.isPressed)
                {
                    float turnSpeed = -85f;
                    landerRigidbody2D.AddTorque(turnSpeed * Time.fixedDeltaTime);

                    OnRightForce?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
        }
        
    }

    private void OnCollisionEnter2D(Collision2D Collision2D)
    {
        if (!Collision2D.gameObject.TryGetComponent(out LandingPad langingPad))
        {
            Debug.Log("Crashed on the Terrain!");
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {   
                landingType = LandingType.WrongLandingArea,
                dotVector = 0f,
                landingSpeed = 0f,
                scoreMultiplier = 0,
                score = 0,
            });
            SetState(State.GameOver);
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
            SetState(State.GameOver);
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
        SetState(State.GameOver);
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
    
    private void SetState(State state)
    {
        this.state = state;
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            state = state
        });

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
