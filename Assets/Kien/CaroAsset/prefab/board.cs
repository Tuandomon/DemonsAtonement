using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class board : MonoBehaviour
{
    public GameObject cellPrefab;
    public Transform Board;
    public GridLayoutGroup GridLayout;
    public GameObject gameOverPrefab;
    public int boardSize = 10;

    public string currentTurn = "x";
    public string[,] matrix;

    private void Start()
    {
        matrix = new string[boardSize + 1, boardSize + 1];
        GridLayout.constraintCount = boardSize;
        CreateBoard();
    }

    private void CreateBoard()
    {
        for (int i = 1; i <= boardSize; i++)
        {
            for (int j = 1; j <= boardSize; j++)
            {
                GameObject cellObj = Instantiate(cellPrefab, Board);
                Cell cell = cellObj.GetComponent<Cell>();
                cell.row = i;
                cell.colum = j;
                matrix[i, j] = "";
            }
        }
    }

    public bool Check(int row, int colum, string player = null)
    {
        string turn = player ?? currentTurn;
        matrix[row, colum] = turn;
        int count;

        // Hàng dọc
        count = 0;
        for (int i = row - 1; i >= 1; i--) if (matrix[i, colum] == turn) count++; else break;
        for (int i = row + 1; i <= boardSize; i++) if (matrix[i, colum] == turn) count++; else break;
        if (count + 1 >= 5) return true;

        // Hàng ngang
        count = 0;
        for (int i = colum - 1; i >= 1; i--) if (matrix[row, i] == turn) count++; else break;
        for (int i = colum + 1; i <= boardSize; i++) if (matrix[row, i] == turn) count++; else break;
        if (count + 1 >= 5) return true;

        // Chéo \
        count = 0;
        for (int i = 1; ; i++) if (row - i >= 1 && colum - i >= 1 && matrix[row - i, colum - i] == turn) count++; else break;
        for (int i = 1; ; i++) if (row + i <= boardSize && colum + i <= boardSize && matrix[row + i, colum + i] == turn) count++; else break;
        if (count + 1 >= 5) return true;

        // Chéo /
        count = 0;
        for (int i = 1; ; i++) if (row - i >= 1 && colum + i <= boardSize && matrix[row - i, colum + i] == turn) count++; else break;
        for (int i = 1; ; i++) if (row + i <= boardSize && colum - i >= 1 && matrix[row + i, colum - i] == turn) count++; else break;
        if (count + 1 >= 5) return true;

        return false;
    }

    public IEnumerator AIDelay()
    {
        yield return new WaitForSeconds(0.1f);
        AIMove();
    }

    // Hàm đánh AI cực mạnh
    public void AIMove()
    {
        int bestRow = -1;
        int bestCol = -1;
        int maxScore = -1;

        for (int i = 1; i <= boardSize; i++)
        {
            for (int j = 1; j <= boardSize; j++)
            {
                if (matrix[i, j] != "") continue;

                int score = EvaluateCellStrong(i, j);

                if (score > maxScore)
                {
                    maxScore = score;
                    bestRow = i;
                    bestCol = j;
                }
            }
        }

        if (bestRow == -1 || bestCol == -1) return;

        // AI đánh
        matrix[bestRow, bestCol] = "0";

        // Cập nhật sprite
        foreach (Transform child in Board)
        {
            Cell cell = child.GetComponent<Cell>();
            if (cell.row == bestRow && cell.colum == bestCol)
            {
                cell.ChangeImage("0");
                break;
            }
        }

        // Kiểm tra thắng AI
        // Kiểm tra thắng AI
        if (Check(bestRow, bestCol, "0"))
        {
            Transform canvas = FindAnyObjectByType<Canvas>().transform;
            GameObject window = Instantiate(gameOverPrefab, canvas);
            window.transform.localScale = Vector3.one; // đảm bảo scale chuẩn
            window.GetComponent<GameOver>().SetName("0");
        }


        currentTurn = "x"; // Trả lượt người chơi
    }

    // Tính điểm mạnh cho 1 ô
    private int EvaluateCellStrong(int row, int col)
    {
        int score = 0;
        string ai = "0";
        string player = "x";

        // Nếu đánh thắng ngay => điểm cực cao
        matrix[row, col] = ai;
        if (Check(row, col, ai))
        {
            matrix[row, col] = "";
            return 10000;
        }
        matrix[row, col] = "";

        // Nếu người chơi sắp thắng => chặn ngay
        matrix[row, col] = player;
        if (Check(row, col, player))
        {
            matrix[row, col] = "";
            return 9000;
        }
        matrix[row, col] = "";

        // Tính điểm theo chuỗi xung quanh
        score += CountDirectionScore(row, col, ai) * 10;      // Tấn công
        score += CountDirectionScore(row, col, player) * 8;   // Chặn người chơi

        // Ưu tiên trung tâm
        int center = boardSize / 2;
        score += (boardSize - Mathf.Abs(row - center) - Mathf.Abs(col - center));

        return score;
    }

    // Đếm chuỗi liên tiếp theo 4 hướng
    private int CountDirectionScore(int row, int col, string player)
    {
        int maxCount = 0;
        int count;

        // Dọc
        count = 0;
        for (int i = row - 1; i >= 1; i--) if (matrix[i, col] == player) count++; else break;
        for (int i = row + 1; i <= boardSize; i++) if (matrix[i, col] == player) count++; else break;
        maxCount = Mathf.Max(maxCount, count);

        // Ngang
        count = 0;
        for (int i = col - 1; i >= 1; i--) if (matrix[row, i] == player) count++; else break;
        for (int i = col + 1; i <= boardSize; i++) if (matrix[row, i] == player) count++; else break;
        maxCount = Mathf.Max(maxCount, count);

        // Chéo \
        count = 0;
        for (int i = 1; ; i++) if (row - i >= 1 && col - i >= 1 && matrix[row - i, col - i] == player) count++; else break;
        for (int i = 1; ; i++) if (row + i <= boardSize && col + i <= boardSize && matrix[row + i, col + i] == player) count++; else break;
        maxCount = Mathf.Max(maxCount, count);

        // Chéo /
        count = 0;
        for (int i = 1; ; i++) if (row - i >= 1 && col + i <= boardSize && matrix[row - i, col + i] == player) count++; else break;
        for (int i = 1; ; i++) if (row + i <= boardSize && col - i >= 1 && matrix[row + i, col - i] == player) count++; else break;
        maxCount = Mathf.Max(maxCount, count);

        return maxCount;
    }
}
