using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Piece : MonoBehaviour, 
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] List<Sprite> sprites;
    List<int> rotationAngles = new() { 0, 90, 180, 270 };

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

    int[,] Rotate(int[,] shape, int degrees)
    {
        degrees = ((degrees % 360) + 360) % 360;

        return degrees switch
        {
            0 => (int[,])shape.Clone(),
            90 => Rotate90(shape),
            180 => Rotate90(Rotate90(shape)),
            270 => Rotate90(Rotate90(Rotate90(shape))),
            _ => throw new System.ArgumentException("Degrees must be a multiple of 90")
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

    private void ApplyShape(int[,] shape, Sprite sprite)
    {
        currentShape = shape;
        currentSprite = sprite;

        if (sprite.name.Contains("red")) currentType = Board.CellType.Red;
        else if (sprite.name.Contains("green")) currentType = Board.CellType.Green;
        else if (sprite.name.Contains("blue")) currentType = Board.CellType.Blue;
        else currentType = Board.CellType.Empty;

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
        ApplyShape(shape, sprites[Random.Range(0, sprites.Count)]);
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

            if (board != null && board.GetGridPosition(eventData.position, out int gx, out int gy))
            {
                if (board.IsValidPlacement(currentShape, gx, gy))
                {
                    board.ShowPreview(currentShape, gx, gy, currentType);
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
            if (board != null && board.GetGridPosition(eventData.position, out int gx, out int gy))
            {
                if (board.IsValidPlacement(currentShape, gx, gy))
                {
                    board.PlacePiece(currentShape, gx, gy, currentType);
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
