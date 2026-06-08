using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Piece[] pieces;
    [SerializeField] GameObject gameOverPanel;
    Board board;

    void Awake()
    {
        board = FindFirstObjectByType<Board>();
    }

    void Start()
    {
        gameOverPanel.SetActive(false);

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

    public void ResetGame()
    {
        gameOverPanel.SetActive(false);
        foreach (Piece piece in pieces)
        {
            piece.SetRandomShape();
        }
        board.ClearBoard();
    }

    public void CheckGameOver()
    {
        // We check if at least one unplaced piece can be placed anywhere
        foreach (Piece piece in pieces)
        {
            if (piece.isPlaced) continue;

            int[,] shape = piece.GetShape();
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (board.IsValidPlacement(shape, x, y))
                    {
                        return; // Found a valid spot, game continues
                    }
                }
            }
        }

        // If we reach here, no piece can be placed
        gameOverPanel.SetActive(true);
    }
}
