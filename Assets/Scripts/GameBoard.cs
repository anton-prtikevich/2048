using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using System.Collections.Generic;
using DG.Tweening;

public class GameBoard : MonoBehaviour
{
    [SerializeField] private int boardSize = 4;
    [SerializeField] private float moveTime = 0.15f;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private RectTransform boardRect;
    [SerializeField] private GameObject cellPrefab;
    
    private Tile[,] tiles;
    private bool isAnimating;
    private List<Tile> allTiles = new List<Tile>();
    private bool hasMoved;
    private float tileSize;

    private void Start()
    {
        tiles = new Tile[boardSize, boardSize];
        tileSize = boardRect.rect.width / boardSize;
        InitializeBoard();
        SpawnNewTile();
        SpawnNewTile();
    }

    public int[,] GetBoardState()
    {
        int[,] state = new int[boardSize, boardSize];
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                state[x, y] = tiles[x, y]?.Value ?? 0;
            }
        }
        return state;
    }

    public void LoadBoardState(int[,] state)
    {
        // Очищаем текущую доску
        foreach (var tile in allTiles)
        {
            if (tile != null)
            {
                Destroy(tile.gameObject);
            }
        }
        allTiles.Clear();
        tiles = new Tile[boardSize, boardSize];

        // Восстанавливаем состояние
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (state[x, y] > 0)
                {
                    CreateTile(x, y, state[x, y]);
                }
            }
        }
        // Debug.Log("Игровая доска загружена");
    }

    private void InitializeBoard()
    {
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                Vector2 position = GetTilePosition(x, y);
                GameObject cell = Instantiate(cellPrefab, boardRect);
                cell.GetComponent<RectTransform>().anchoredPosition = position;
                cell.GetComponent<RectTransform>().sizeDelta = new Vector2(tileSize * 0.9f, tileSize * 0.9f);
            }
        }
    }

    private Vector2 GetTilePosition(int x, int y)
    {
        return new Vector2(
            x * tileSize - boardRect.rect.width / 2 + tileSize / 2,
            y * tileSize - boardRect.rect.height / 2 + tileSize / 2
        );
    }

    private void SpawnNewTile()
    {
        List<Vector2Int> emptyPositions = new List<Vector2Int>();
        
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (tiles[x, y] == null)
                {
                    emptyPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        if (emptyPositions.Count > 0)
        {
            Vector2Int pos = emptyPositions[UnityEngine.Random.Range(0, emptyPositions.Count)];
            int value = UnityEngine.Random.value > 0.9f ? 4 : 2;
            CreateTile(pos.x, pos.y, value);
        }
        else if (!CanMove())
        {
            GameManager.Instance.GameOver();
        }
    }

    private bool CanMove()
    {
        // Проверяем наличие пустых клеток
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (tiles[x, y] == null)
                    return true;
            }
        }

        // Проверяем возможность слияния по горизонтали
        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize - 1; x++)
            {
                if (tiles[x, y].Value == tiles[x + 1, y].Value)
                    return true;
            }
        }

        // Проверяем возможность слияния по вертикали
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize - 1; y++)
            {
                if (tiles[x, y].Value == tiles[x, y + 1].Value)
                    return true;
            }
        }

        return false;
    }

    private void CreateTile(int x, int y, int value)
    {
        GameObject tileObj = Instantiate(tilePrefab, boardRect);
        Vector2 position = GetTilePosition(x, y);
        
        tileObj.GetComponent<RectTransform>().anchoredPosition = position;
        tileObj.GetComponent<RectTransform>().sizeDelta = new Vector2(tileSize * 0.9f, tileSize * 0.9f);
        
        Tile tile = tileObj.GetComponent<Tile>();
        tile.SetValue(value);
        tiles[x, y] = tile;
        allTiles.Add(tile);

        tileObj.transform.localScale = Vector3.zero;
        tileObj.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
    }

    private void Update()
    {
        if (isAnimating) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || SwipeDetector.SwipeDirection == SwipeDirection.Left)
        {
            MoveTiles(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || SwipeDetector.SwipeDirection == SwipeDirection.Right)
        {
            MoveTiles(Vector2Int.right);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || SwipeDetector.SwipeDirection == SwipeDirection.Up)
        {
            MoveTiles(Vector2Int.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || SwipeDetector.SwipeDirection == SwipeDirection.Down)
        {
            MoveTiles(Vector2Int.down);
        }
    }

    private void MoveTiles(Vector2Int direction)
    {
        if (isAnimating) return;
        isAnimating = true;
        hasMoved = false;

        // Копируем текущее состояние доски
        Tile[,] oldTiles = new Tile[boardSize, boardSize];
        for (int x = 0; x < boardSize; x++)
            for (int y = 0; y < boardSize; y++)
                oldTiles[x, y] = tiles[x, y];

        // Очищаем текущее состояние
        tiles = new Tile[boardSize, boardSize];

        // Определяем порядок обхода
        int startX = direction.x >= 0 ? boardSize - 1 : 0;
        int endX = direction.x >= 0 ? -1 : boardSize;
        int stepX = direction.x >= 0 ? -1 : 1;

        int startY = direction.y >= 0 ? boardSize - 1 : 0;
        int endY = direction.y >= 0 ? -1 : boardSize;
        int stepY = direction.y >= 0 ? -1 : 1;

        List<Tile> tilesToDestroy = new List<Tile>();
        List<(Tile tile, Vector2 position, bool willDestroy)> movements = new List<(Tile, Vector2, bool)>();

        // Перемещаем тайлы
        for (int y = startY; y != endY; y += stepY)
        {
            for (int x = startX; x != endX; x += stepX)
            {
                if (oldTiles[x, y] == null) continue;

                Tile currentTile = oldTiles[x, y];
                Vector2Int currentPos = new Vector2Int(x, y);
                Vector2Int newPos = currentPos;

                // Находим новую позицию
                while (true)
                {
                    Vector2Int nextPos = newPos + direction;
                    if (!IsValidPosition(nextPos)) break;

                    if (tiles[nextPos.x, nextPos.y] == null)
                    {
                        newPos = nextPos;
                        hasMoved = true;
                    }
                    else if (tiles[nextPos.x, nextPos.y].Value == currentTile.Value && 
                            !tilesToDestroy.Contains(tiles[nextPos.x, nextPos.y]))
                    {
                        // Слияние
                        newPos = nextPos;
                        hasMoved = true;
                        tilesToDestroy.Add(currentTile);
                        movements.Add((currentTile, GetTilePosition(newPos.x, newPos.y), true));
                        
                        // Обновляем значение целевого тайла
                        tiles[nextPos.x, nextPos.y].SetValue(currentTile.Value * 2);
                        GameManager.Instance.AddScore(currentTile.Value * 2);
                        
                        // Воспроизводим звук слияния
                        if (SoundManager.Instance != null)
                        {
                            SoundManager.Instance.PlayMergeSound();
                        }
                        
                        goto NextTile;
                    }
                    else break;
                }

                // Перемещаем тайл на новую позицию
                if (!tilesToDestroy.Contains(currentTile))
                {
                    tiles[newPos.x, newPos.y] = currentTile;
                    if (currentPos != newPos)
                    {
                        movements.Add((currentTile, GetTilePosition(newPos.x, newPos.y), false));
                    }
                }

                NextTile: continue;
            }
        }

        // Анимируем все движения
        if (movements.Count > 0)
        {
            // Воспроизводим звук движения, если были перемещения
            if (hasMoved && SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayMoveSound();
            }

            foreach (var move in movements)
            {
                move.tile.GetComponent<RectTransform>()
                    .DOAnchorPos(move.position, moveTime)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() => {
                        if (move.willDestroy)
                        {
                            allTiles.Remove(move.tile);
                            Destroy(move.tile.gameObject);
                        }
                    });
            }

            DOVirtual.DelayedCall(moveTime + 0.1f, () => {
                isAnimating = false;
                if (hasMoved)
                {
                    SpawnNewTile();
                }
                else if (!CanMove())
                {
                    GameManager.Instance.GameOver();
                }
            });
        }
        else
        {
            isAnimating = false;
            if (!CanMove())
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    private bool IsValidPosition(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < boardSize && pos.y >= 0 && pos.y < boardSize;
    }

    public void RestartGame()
    {
        foreach (var tile in allTiles)
        {
            if (tile != null)
            {
                Destroy(tile.gameObject);
            }
        }
        
        allTiles.Clear();
        tiles = new Tile[boardSize, boardSize];
        
        SpawnNewTile();
        SpawnNewTile();
    }
}
