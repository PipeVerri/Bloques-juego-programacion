using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Piece[] pieces;

    void Start()
    {
        foreach (Piece piece in pieces)
        {
            piece.SetRandomShape();
        }
    }

    public void CheckAllPiecesPlaced()
    {
        bool allPlaced = true;
        foreach (Piece piece in pieces)
        {
            if (!piece.isPlaced)
            {
                allPlaced = false;
                break;
            }
        }

        if (allPlaced)
        {
            foreach (Piece piece in pieces)
            {
                piece.SetRandomShape();
            }
        }
    }
}
