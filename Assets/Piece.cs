using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Piece : MonoBehaviour, 
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [System.Serializable]
    public struct PieceData
    {
        public Sprite sprite;
        public Board.CellType type;
    }

    [SerializeField] List<PieceData> pieceDataList;
    List<int> rotationAngles = new() { 0, 1, 2, 3 };

    int[,] currentShape;
    Sprite currentSprite;
    Board.CellType currentType;
    Board board;

    public bool isPlaced;
    GameManager gameManager;

    void Awake()
    {
        board = FindFirstObjectByType<Board>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    List<int[,]> Shapes = new() {
        // I
        new int[,] { {1,1,1,1} },
        // O
        new int[,]
        {
            {1,1},
            {1,1}
        },
        // T
        new int[,]
        {
            {0,1,0},
            {1,1,1}
        },
        // L
        new int[,]
        {
            {1,0},
            {1,0},
            {1,1}
        },
        // J
        new int[,]
        {
            {0,1},
            {0,1},
            {1,1}
        },
        // S
        new int[,]
        {
            {0,1,1},
            {1,1,0}
        },
        // Z
        new int[,]
        {
            {1,1,0},
            {0,1,1}
        }
    };

    int[,] Rotate(int[,] shape, int rotation)
    {
        return (rotation % 4) switch
        {
            0 => (int[,])shape.Clone(),
            1 => Rotate90(shape),
            2 => Rotate90(Rotate90(shape)),
            3 => Rotate90(Rotate90(Rotate90(shape))),
            _ => (int[,])shape.Clone()
        };
    }

    private static int[,] Rotate90(int[,] shape)
    {
        int rows = shape.GetLength(0);
        int cols = shape.GetLength(1);

        int[,] rotated = new int[cols, rows];

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                rotated[x, rows - 1 - y] = shape[y, x];
            }
        }

        return rotated;
    }

    private void ApplyShape(int[,] shape, Sprite sprite, Board.CellType type)
    {
        currentShape = shape;
        currentSprite = sprite;
        currentType = type;

        int rows = shape.GetLength(0);
        int cols = shape.GetLength(1);

        int startX = (4 - cols) / 2;
        int startY = (4 - rows) / 2;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Image>().enabled = false;
        }

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (shape[y, x] == 1)
                {
                    int visualX = startX + x;
                    int visualY = startY + y;
                    int index = visualY * 4 + visualX;

                    if (index >= 0 && index < transform.childCount)
                    {
                        Image image = transform.GetChild(index).GetComponent<Image>();
                        image.sprite = sprite;
                        image.enabled = true;
                    }
                }
            }
        }
    }
    
    public int[,] GetShape() => currentShape;

    public void SetRandomShape()
    {
        isPlaced = false;
        gameObject.SetActive(true);
        GetComponent<CanvasGroup>().alpha = 1f;

        int[,] shape = Shapes[Random.Range(0, Shapes.Count)];
        int random_angle = rotationAngles[Random.Range(0, rotationAngles.Count)];
        shape = Rotate(shape, random_angle);

        PieceData data = pieceDataList[Random.Range(0, pieceDataList.Count)];
        ApplyShape(shape, data.sprite, data.type);
    }

    Piece dragClone;
    public void OnBeginDrag(PointerEventData eventData)
    {
        GameObject cloneObj = Instantiate(gameObject, transform.parent);

        dragClone = cloneObj.GetComponent<Piece>();
        dragClone.currentShape = currentShape;
        dragClone.currentSprite = currentSprite;
        dragClone.currentType = currentType;
        dragClone.board = board;

        dragClone.transform.position = transform.position;
        GetComponent<CanvasGroup>().alpha = 0f; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragClone != null)
        {
            dragClone.transform.position = eventData.position;

            Vector2Int pos = board != null ? board.GetGridPosition(eventData.position) : new Vector2Int(-1, -1);
            if (pos.x != -1)
            {
                if (board.IsValidPlacement(currentShape, pos.x, pos.y))
                {
                    board.ShowPreview(currentShape, pos.x, pos.y, currentType);
                    dragClone.GetComponent<CanvasGroup>().alpha = 0.5f;
                }
                else
                {
                    board.ClearPreview();
                    board.Render();
                    dragClone.GetComponent<CanvasGroup>().alpha = 1f;
                }
            }
            else
            {
                if (board != null)
                {
                    board.ClearPreview();
                    board.Render();
                }
                dragClone.GetComponent<CanvasGroup>().alpha = 1f;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        bool placed = false;
        if (dragClone != null)
        {
            Vector2Int pos = board != null ? board.GetGridPosition(eventData.position) : new Vector2Int(-1, -1);
            if (pos.x != -1)
            {
                if (board.IsValidPlacement(currentShape, pos.x, pos.y))
                {
                    board.PlacePiece(currentShape, pos.x, pos.y, currentType);
                    placed = true;
                }
            }
            Destroy(dragClone.gameObject);
        }

        if (placed)
        {
            isPlaced = true;
            gameObject.SetActive(false);
            if (gameManager != null) gameManager.CheckAllPiecesPlaced();
        }
        else
        {
            GetComponent<CanvasGroup>().alpha = 1f;
            if (board != null)
            {
                board.ClearPreview();
                board.Render();
            }
        }
    }
}
