using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Piece[] pieces;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Piece piece in pieces)
        {
            piece.SetRandomShape();
        }
    }
}
