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

    // AI mạnh
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

        matrix[bestRow, bestCol] = "0";

        // Cập nhật sprite cho ô AI đánh
        foreach (Transform child in Board)
        {
            Cell cell = child.GetComponent<Cell>();
            if (cell.row == bestRow && cell.colum == bestCol)
            {
                cell.ChangeImage("0");
                break;
            }
        }

        if (Check(bestRow, bestCol, "0"))
        {
            Transform canvas = FindAnyObjectByType<Canvas>().transform;
            GameObject window = Instantiate(gameOverPrefab, canvas);
            window.transform.localScale = Vector3.one;
            window.GetComponent<GameOver>().SetName("0");
        }

        currentTurn = "x";
    }

    // -------------------------------
    //      AI ĐÁNH GIÁ MẠNH
    // -------------------------------

    private int EvaluateCellStrong(int row, int col)
    {
        string ai = "0";
        string human = "x";

        // Nếu đánh thắng ngay
        matrix[row, col] = ai;
        if (Check(row, col, ai))
        {
            matrix[row, col] = "";
            return 100000;
        }
        matrix[row, col] = "";

        // Nếu người sắp thắng => chặn ngay
        matrix[row, col] = human;
        if (Check(row, col, human))
        {
            matrix[row, col] = "";
            return 90000;
        }
        matrix[row, col] = "";

        int score = 0;

        score += PatternScore(row, col, ai);     // Tấn công
        score += PatternScore(row, col, human);  // Phòng thủ

        int center = boardSize / 2;
        score += (boardSize - (Mathf.Abs(row - center) + Mathf.Abs(col - center)));

        return score;
    }

    private int PatternScore(int row, int col, string p)
    {
        int score = 0;

        score += EvaluateLine(row, col, 1, 0, p);   // dọc
        score += EvaluateLine(row, col, 0, 1, p);   // ngang
        score += EvaluateLine(row, col, 1, 1, p);   // chéo \
        score += EvaluateLine(row, col, 1, -1, p);  // chéo /

        return score;
    }

    private int EvaluateLine(int row, int col, int dx, int dy, string p)
    {
        int count = 1;
        int openEnds = 0;

        // đi lùi
        int r = row - dx;
        int c = col - dy;
        while (r >= 1 && r <= boardSize && c >= 1 && c <= boardSize && matrix[r, c] == p)
        {
            count++;
            r -= dx; c -= dy;
        }
        if (r >= 1 && r <= boardSize && c >= 1 && c <= boardSize && matrix[r, c] == "") openEnds++;

        // đi tới
        r = row + dx;
        c = col + dy;
        while (r >= 1 && r <= boardSize && c >= 1 && c <= boardSize && matrix[r, c] == p)
        {
            count++;
            r += dx; c += dy;
        }
        if (r >= 1 && r <= boardSize && c >= 1 && c <= boardSize && matrix[r, c] == "") openEnds++;

        // tính điểm theo chuỗi
        if (count >= 5) return 10000;
        if (count == 4 && openEnds == 2) return 5000;
        if (count == 4 && openEnds == 1) return 2000;
        if (count == 3 && openEnds == 2) return 1000;
        if (count == 3 && openEnds == 1) return 300;
        if (count == 2 && openEnds == 2) return 150;
        if (count == 2 && openEnds == 1) return 40;

        return 10;
    }
}
