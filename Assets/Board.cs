using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public enum CellType { Empty, Red, Green, Blue }

    [SerializeField] Sprite redSprite;
    [SerializeField] Sprite greenSprite;
    [SerializeField] Sprite blueSprite;

    private CellType[,] grid = new CellType[8, 8];

    void Start()
    {
        // Example: Fill some cells to test rendering
        grid[0, 0] = CellType.Red;
        grid[1, 1] = CellType.Green;
        grid[2, 2] = CellType.Blue;
        grid[7, 7] = CellType.Red;
        
        Render();
    }

    public void Render()
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                int index = y * 8 + x;
                
                // Safety check to avoid index out of bounds if children are missing
                if (index >= transform.childCount) continue;

                Transform cellTransform = transform.GetChild(index);
                Image image = cellTransform.GetComponent<Image>();
                
                if (image == null) continue;

                CellType cell = grid[y, x];

                if (cell == CellType.Empty)
                {
                    image.enabled = false;
                }
                else
                {
                    image.enabled = true;
                    image.sprite = cell switch
                    {
                        CellType.Red => redSprite,
                        CellType.Green => greenSprite,
                        CellType.Blue => blueSprite,
                        _ => null
                    };
                }
            }
        }
    }
}
