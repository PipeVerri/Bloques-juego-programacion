using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Piece : MonoBehaviour, 
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] List<Sprite> sprites;
    List<int> rotationAngles = new() { 0, 90, 180, 270 };

    // static shares it accross instances, readonly stops me from accidental edits
    List<int[,]> Shapes = new() {
        // I
        new int[,]
        {
            {0,0,0,0},
            {1,1,1,1},
            {0,0,0,0},
            {0,0,0,0}
        },

        // O
        new int[,]
        {
            {0,1,1,0},
            {0,1,1,0},
            {0,0,0,0},
            {0,0,0,0}
        },

        // T
        new int[,]
        {
            {0,1,0,0},
            {1,1,1,0},
            {0,0,0,0},
            {0,0,0,0}
        },

        // L
        new int[,]
        {
            {1,0,0,0},
            {1,0,0,0},
            {1,1,0,0},
            {0,0,0,0}
        },

        // J
        new int[,]
        {
            {0,1,0,0},
            {0,1,0,0},
            {1,1,0,0},
            {0,0,0,0}
        },

        // S
        new int[,]
        {
            {0,1,1,0},
            {1,1,0,0},
            {0,0,0,0},
            {0,0,0,0}
        },

        // Z
        new int[,]
        {
            {1,1,0,0},
            {0,1,1,0},
            {0,0,0,0},
            {0,0,0,0}
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

    void LogShape(int[,] shape)
    {
        string output = "";

        for (int y = 0; y < shape.GetLength(0); y++)
        {
            for (int x = 0; x < shape.GetLength(1); x++)
            {
                output += shape[y, x] + " ";
            }

            output += "\n";
        }

        Debug.Log(output);
    }

    private void ApplyShape(int[,] shape, Sprite sprite)
    {
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                int index = y * 4 + x;
                Transform block = transform.GetChild(index);
                Image image = block.GetComponent<Image>();
                bool occupied = shape[y, x] == 1;
                
                if (occupied)
                {
                    image.sprite = sprite;
                    image.enabled = true;
                }
                else
                {
                    image.enabled = false;
                }
            }
        }
    }
    
    public void SetRandomShape()
    {
        int[,] shape = Shapes[Random.Range(0, Shapes.Count)];
        int random_angle = rotationAngles[Random.Range(0, rotationAngles.Count)];
        shape = Rotate(shape, random_angle);
        ApplyShape(shape, sprites[Random.Range(0, sprites.Count)]);
    }

    Piece dragClone;
    public void OnBeginDrag(PointerEventData eventData)
    {
        GameObject cloneObj =
            Instantiate(gameObject,
                        transform.parent);

        dragClone = cloneObj.GetComponent<Piece>();

        dragClone.transform.position =
            transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragClone != null)
        {
            dragClone.transform.position =
                eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragClone != null)
        {
            Destroy(dragClone.gameObject);
        }
    }
}
