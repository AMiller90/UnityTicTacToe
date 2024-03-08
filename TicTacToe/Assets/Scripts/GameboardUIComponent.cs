using UnityEngine;
using UnityEngine.UI;

public class GameboardUIComponent : MonoBehaviour
{
    /// <summary>
    /// Reference to the grid layout group for the gameboard
    /// </summary>
    [SerializeField] private GridLayoutGroup _gridLayoutGroup;
    /// <summary>
    /// Function used to initialize the grid layout constraints
    /// </summary>
    /// <param name="maxSize">The max size of the grid</param>
    public void Initialize(int maxSize)
    {
        _gridLayoutGroup.constraintCount = maxSize;
    }
}
