using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public enum CellType { Empty, Red, Green, Blue }

    [SerializeField] Sprite redSprite;
    [SerializeField] Sprite greenSprite;
    [SerializeField] Sprite blueSprite;

    private CellType[,] grid = new CellType[8, 8];
    private CellType[,] previewGrid = new CellType[8, 8];
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        ClearBoard();
        Render();
    }

    public void ClearBoard()
    {
        for (int y = 0; y < 8; y++)
            for (int x = 0; x < 8; x++)
                grid[y, x] = CellType.Empty;
        
        ClearPreview();
    }

    public void ClearPreview()
    {
        for (int y = 0; y < 8; y++)
            for (int x = 0; x < 8; x++)
                previewGrid[y, x] = CellType.Empty;
    }

    public bool GetGridPosition(Vector2 screenPoint, out int gridX, out int gridY)
    {
        gridX = -1;
        gridY = -1;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, null, out Vector2 localPoint))
            return false;

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        // Calculate normalized position (0 to 1) relative to the bottom-left corner
        // rectTransform.rect.min is the bottom-left corner in local coordinates
        float nx = (localPoint.x - rectTransform.rect.min.x) / width;
        float ny = (localPoint.y - rectTransform.rect.min.y) / height;

        if (nx < 0 || nx >= 1 || ny < 0 || ny >= 1)
            return false;

        gridX = Mathf.FloorToInt(nx * 8);
        // If your children are ordered top-to-bottom in the grid:
        // (0,0) is top-left, so we invert Y
        gridY = 7 - Mathf.FloorToInt(ny * 8); 

        return true;
    }

    public bool IsValidPlacement(int[,] shape, int startX, int startY)
    {
        int rows = shape.GetLength(0);
        int cols = shape.GetLength(1);

        // Center the piece matrix on the mouse position
        startX -= cols / 2;
        startY -= rows / 2;

        for (int py = 0; py < rows; py++)
        {
            for (int px = 0; px < cols; px++)
            {
                if (shape[py, px] == 0) continue;

                int gx = startX + px;
                int gy = startY + py;

                if (gx < 0 || gx >= 8 || gy < 0 || gy >= 8) return false;
                if (grid[gy, gx] != CellType.Empty) return false;
            }
        }
        return true;
    }

    public void ShowPreview(int[,] shape, int startX, int startY, CellType type)
    {
        ClearPreview();
        
        int rows = shape.GetLength(0);
        int cols = shape.GetLength(1);

        startX -= cols / 2;
        startY -= rows / 2;

        for (int py = 0; py < rows; py++)
        {
            for (int px = 0; px < cols; px++)
            {
                if (shape[py, px] == 0) continue;

                int gx = startX + px;
                int gy = startY + py;

                if (gx >= 0 && gx < 8 && gy >= 0 && gy < 8)
                {
                    previewGrid[gy, gx] = type;
                }
            }
        }
        Render();
    }

    public void PlacePiece(int[,] shape, int startX, int startY, CellType type)
    {
        int rows = shape.GetLength(0);
        int cols = shape.GetLength(1);

        startX -= cols / 2;
        startY -= rows / 2;

        for (int py = 0; py < rows; py++)
        {
            for (int px = 0; px < cols; px++)
            {
                if (shape[py, px] == 0) continue;

                int gx = startX + px;
                int gy = startY + py;

                if (gx >= 0 && gx < 8 && gy >= 0 && gy < 8)
                {
                    grid[gy, gx] = type;
                }
            }
        }
        ClearPreview();
        Render();
    }

    public void Render()
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                int index = y * 8 + x;
                if (index >= transform.childCount) continue;

                Transform cellTransform = transform.GetChild(index);
                Image image = cellTransform.GetComponent<Image>();
                if (image == null) continue;

                // Priority to permanent grid, then preview
                CellType cell = grid[y, x] != CellType.Empty ? grid[y, x] : previewGrid[y, x];

                if (cell == CellType.Empty)
                {
                    image.enabled = false;
                }
                else
                {
                    image.enabled = true;
                    // Add transparency to preview
                    Color c = Color.white;
                    if (grid[y, x] == CellType.Empty && previewGrid[y, x] != CellType.Empty)
                        c.a = 0.5f;
                    image.color = c;

                    image.sprite = GetSprite(cell);
                }
            }
        }
    }

    private Sprite GetSprite(CellType type)
    {
        return type switch
        {
            CellType.Red => redSprite,
            CellType.Green => greenSprite,
            CellType.Blue => blueSprite,
            _ => null
        };
    }
}
