using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public enum Difficulty
    {
        EASY = 3,
        MEDIUM = 5,
        HARD = 7
    }

    public Difficulty difficulty = Difficulty.HARD;

    PlayerCore playerCore;
    BoardManager manager;
    Dictionary<string, (bool, int)> tranposeTable = new Dictionary<string, (bool, int)>();

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<BoardManager>();
        playerCore = GetComponent<PlayerCore>();

        difficulty = PhotonNetwork.CurrentRoom.CustomProperties["difficulty"] != null ? (Difficulty)PhotonNetwork.CurrentRoom.CustomProperties["difficulty"] : Difficulty.EASY;
    }

    private float moveDelay = 1.0f;
    private float lastMoveTime = Time.time;

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastMoveTime < moveDelay)
        {
            return;
        }
        lastMoveTime = Time.time;

        TurnManager turnManager = FindAnyObjectByType<TurnManager>();

        if (turnManager == null) return;

        if (playerCore.turnID == turnManager.turnID)
        {
            Move();
        }
    }

    private void Move()
    {
        int[,] board = new int[6, 7];
        Dictionary<Vector2Int, Slot> slotDictionary = manager.slotDictionary;

        if (slotDictionary == null || slotDictionary.Count < 6 * 7) return;

        // Convert slotDictionary to int[,] board
        foreach (var kvp in slotDictionary)
        {
            Vector2Int pos = kvp.Key;
            Slot slot = kvp.Value;
            board[pos.y + 3, pos.x + 3] = slot.occupyingPlayer != -1 ? slot.occupyingPlayer : 0;
        }

        Vector2Int bestMove = GetBestMove(board, playerCore.turnID, difficulty);
        if (bestMove != null)
        {
            Slot slot = manager.GetSlotAt(new Vector2Int(bestMove.x - 3, bestMove.y - 3));
            playerCore.Interact(slot);
        }
    }

    private Vector2Int GetBestMove(int[,] board, int playerID, Difficulty difficulty)
    {
        int bestScore = int.MinValue;
        Vector2Int bestMove = Vector2Int.zero;

        for (int x = 0; x < 7; x++)
        {
            int y = GetLowestEmptyRow(board, x);
            if (y != -1)
            {
                board[y, x] = playerID;
                int score = Minimax(board, 0, false, int.MinValue, int.MaxValue, playerID, difficulty);
                board[y, x] = 0;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = new Vector2Int(x, y);
                }
            }
        }

        return bestMove;
    }

    private int GetLowestEmptyRow(int[,] board, int column)
    {
        for (int y = 0; y < 6; y++)
        {
            if (board[y, column] == 0)
            {
                return y;
            }
        }
        return -1;
    }

    private int Minimax(int[,] board, int depth, bool isMaximizing, int alpha, int beta, int playerID, Difficulty difficulty)
    {
        //if (depth >= 3) {
        //    return EvaluateBoard(board, playerID);
        //}

        string boardHash = GetBoardHash(board);
        if (tranposeTable.ContainsKey(boardHash))
        {
            (bool isMax, int score) = tranposeTable[boardHash];
            if (isMax == isMaximizing)
            {
                return score;
            }
        }

        if (depth >= (int)difficulty || IsGameOver(board))
        {
            return EvaluateBoard(board, playerID);
        }

        if (isMaximizing)
        {
            int maxEval = int.MinValue;
            for (int x = 0; x < 7; x++)
            {
                int y = GetLowestEmptyRow(board, x);
                if (y != -1)
                {
                    board[y, x] = 2; // AI ID is 2
                    int eval = Minimax(board, depth + 1, false, alpha, beta, playerID, difficulty);
                    board[y, x] = 0;
                    maxEval = Mathf.Max(maxEval, eval);
                    alpha = Mathf.Max(alpha, eval);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            for (int x = 0; x < 7; x++)
            {
                int y = GetLowestEmptyRow(board, x);
                if (y != -1)
                {
                    board[y, x] = 1; // Player ID is 1
                    int eval = Minimax(board, depth + 1, true, alpha, beta, playerID, difficulty);
                    board[y, x] = 0;
                    minEval = Mathf.Min(minEval, eval);
                    beta = Mathf.Min(beta, eval);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
            }
            return minEval;
        }
    }

    private int EvaluateBoard(int[,] board, int playerID)
    {
        int score = 0;

        // Define the scoring criteria
        int twoInARowScore = 10;
        int threeInARowScore = 500;
        int fourInARowScore = 10000;
        int losingScore = -1000000; // Large negative score for losing

        // Check if the other player has won
        int opponentID = (playerID == 1) ? 2 : 1;
        if (IsGameOverForPlayer(board, opponentID))
        {
            return losingScore;
        }

        // Check all rows, columns, and diagonals
        for (int y = 0; y < 6; y++)
        {
            for (int x = 0; x < 7; x++)
            {
                // Check horizontal lines
                if (x <= 3)
                {
                    score += EvaluateLine(board, playerID, new Vector2Int(x, y), new Vector2Int(1, 0), twoInARowScore, threeInARowScore, fourInARowScore);
                }

                // Check vertical lines
                if (y <= 2)
                {
                    score += EvaluateLine(board, playerID, new Vector2Int(x, y), new Vector2Int(0, 1), twoInARowScore, threeInARowScore, fourInARowScore);
                }

                // Check diagonal lines (bottom-left to top-right)
                if (x <= 3 && y <= 2)
                {
                    score += EvaluateLine(board, playerID, new Vector2Int(x, y), new Vector2Int(1, 1), twoInARowScore, threeInARowScore, fourInARowScore);
                }

                // Check diagonal lines (top-left to bottom-right)
                if (x <= 3 && y >= 3)
                {
                    score += EvaluateLine(board, playerID, new Vector2Int(x, y), new Vector2Int(1, -1), twoInARowScore, threeInARowScore, fourInARowScore);
                }
            }
        }

        return score;
    }

    private int EvaluateLine(int[,] board, int playerID, Vector2Int start, Vector2Int direction, int twoInARowScore, int threeInARowScore, int fourInARowScore)
    {
        int playerCount = 0;
        int opponentCount = 0;

        int opponentID = (playerID == 1) ? 2 : 1;

        for (int i = 0; i < 4; i++)
        {
            Vector2Int pos = start + direction * i;
            if (pos.y >= 0 && pos.y < 6 && pos.x >= 0 && pos.x < 7)
            {
                if (board[pos.y, pos.x] == playerID)
                {
                    playerCount++;
                }
                else if (board[pos.y, pos.x] == opponentID)
                {
                    opponentCount++;
                }
            }
        }

        if (playerCount > 0 && opponentCount > 0)
        {
            return 0; // Blocked line
        }

        if (playerCount == 4)
        {
            return fourInARowScore;
        }
        else if (playerCount == 3)
        {
            return threeInARowScore;
        }
        else if (playerCount == 2)
        {
            return twoInARowScore;
        }

        return 0;
    }

    private bool IsGameOver(int[,] board)
    {
        // Check all rows, columns, and diagonals for a winning condition
        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 6; y++)
            {
                // Check horizontal lines
                if (x <= 3 && CheckLine(board, new Vector2Int(x, y), new Vector2Int(1, 0)))
                {
                    return true;
                }

                // Check vertical lines
                if (y <= 2 && CheckLine(board, new Vector2Int(x, y), new Vector2Int(0, 1)))
                {
                    return true;
                }

                // Check diagonal lines (bottom-left to top-right)
                if (x <= 3 && y <= 2 && CheckLine(board, new Vector2Int(x, y), new Vector2Int(1, 1)))
                {
                    return true;
                }

                // Check diagonal lines (top-left to bottom-right)
                if (x <= 3 && y >= 3 && CheckLine(board, new Vector2Int(x, y), new Vector2Int(1, -1)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool CheckLine(int[,] board, Vector2Int start, Vector2Int direction)
    {
        int playerID = -1;
        int count = 0;

        for (int i = 0; i < 4; i++)
        {
            Vector2Int pos = start + direction * i;
            if (pos.y >= 0 && pos.y < 6 && pos.x >= 0 && pos.x < 7)
            {
                if (board[pos.y, pos.x] != 0)
                {
                    if (playerID == -1)
                    {
                        playerID = board[pos.y, pos.x];
                        count = 1;
                    }
                    else if (board[pos.y, pos.x] == playerID)
                    {
                        count++;
                        if (count == 4)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        return false;
    }
    private bool IsGameOverForPlayer(int[,] board, int playerID)
    {
        // Check all rows, columns, and diagonals for a winning condition for the specified player
        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 6; y++)
            {
                // Check horizontal lines
                if (x <= 3 && CheckLineForPlayer(board, new Vector2Int(x, y), new Vector2Int(1, 0), playerID))
                {
                    return true;
                }

                // Check vertical lines
                if (y <= 2 && CheckLineForPlayer(board, new Vector2Int(x, y), new Vector2Int(0, 1), playerID))
                {
                    return true;
                }

                // Check diagonal lines (bottom-left to top-right)
                if (x <= 3 && y <= 2 && CheckLineForPlayer(board, new Vector2Int(x, y), new Vector2Int(1, 1), playerID))
                {
                    return true;
                }

                // Check diagonal lines (top-left to bottom-right)
                if (x <= 3 && y >= 3 && CheckLineForPlayer(board, new Vector2Int(x, y), new Vector2Int(1, -1), playerID))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool CheckLineForPlayer(int[,] board, Vector2Int start, Vector2Int direction, int playerID)
    {
        int count = 0;

        for (int i = 0; i < 4; i++)
        {
            Vector2Int pos = start + direction * i;
            if (pos.y >= 0 && pos.y < 6 && pos.x >= 0 && pos.x < 7)
            {
                if (board[pos.y, pos.x] == playerID)
                {
                    count++;
                    if (count == 4)
                    {
                        return true;
                    }
                }
                else
                {
                    break;
                }
            }
        }
        return false;
    }

    private string GetBoardHash(int[,] board)
    {
        string hash = "";
        for (int y = 0; y < 6; y++)
        {
            for (int x = 0; x < 7; x++)
            {
                hash += board[y, x].ToString();
            }
        }
        return hash;
    }
}

