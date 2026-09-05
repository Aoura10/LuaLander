using UnityEngine;

public class LandingPad : MonoBehaviour
{
    [SerializeField] private int scoreMultiplier; //SerializeField is used to show any private variable in the editor


    public int GetScoreMultiplier()
    {
        return scoreMultiplier;
    }
}
