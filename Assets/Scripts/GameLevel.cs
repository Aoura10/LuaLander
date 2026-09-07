using UnityEngine;

public class GameLevel : MonoBehaviour
{
    [SerializeField] private int levelNumber;
    [SerializeField] private Transform landerStartPositionTransform;

    public int GetLevelNumber()
    {
        return levelNumber;
    }

    public Vector3 GetLanderStartPosition()
    {
        if (landerStartPositionTransform == null)
        {
            Debug.LogError("landerStartPositionTransform is NULL!");
            return Vector3.zero;
        }

        return landerStartPositionTransform.position;
    }
}
