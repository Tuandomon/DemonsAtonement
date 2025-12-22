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

    // AI RẤT DỄ - Đánh ngẫu nhiên đơn giản
    public void AIMove()
    {
        // Ưu tiên chặn nếu người chơi sắp thắng (4 nước liên tiếp)
        if (TryBlockImmediateWin())
        {
            currentTurn = "x";
            return;
        }

        // Nếu không có nước nào cần chặn gấp, đánh ngẫu nhiên
        List<Vector2Int> emptyCells = new List<Vector2Int>();

        // Tìm tất cả ô trống
        for (int i = 1; i <= boardSize; i++)
        {
            for (int j = 1; j <= boardSize; j++)
            {
                if (matrix[i, j] == "")
                {
                    emptyCells.Add(new Vector2Int(i, j));
                }
            }
        }

        // Nếu không còn ô trống
        if (emptyCells.Count == 0) return;

        // Đánh ngẫu nhiên
        int randomIndex = Random.Range(0, emptyCells.Count);
        Vector2Int selectedCell = emptyCells[randomIndex];

        int row = selectedCell.x;
        int col = selectedCell.y;

        matrix[row, col] = "0";

        // Cập nhật sprite cho ô AI đánh
        foreach (Transform child in Board)
        {
            Cell cell = child.GetComponent<Cell>();
            if (cell.row == row && cell.colum == col)
            {
                cell.ChangeImage("0");
                break;
            }
        }

        if (Check(row, col, "0"))
        {
            Transform canvas = FindAnyObjectByType<Canvas>().transform;
            GameObject window = Instantiate(gameOverPrefab, canvas);
            window.transform.localScale = Vector3.one;
            window.GetComponent<GameOver>().SetName("0");
        }

        currentTurn = "x";
    }

    // Kiểm tra và chặn nếu người chơi sắp thắng (4 nước liên tiếp)
    private bool TryBlockImmediateWin()
    {
        string human = "x";

        // Duyệt qua tất cả ô trống để tìm nước chặn
        for (int i = 1; i <= boardSize; i++)
        {
            for (int j = 1; j <= boardSize; j++)
            {
                if (matrix[i, j] != "") continue;

                // Kiểm tra nếu người chơi đặt ở đây sẽ thắng
                matrix[i, j] = human;
                if (Check(i, j, human))
                {
                    // Chặn nước này
                    matrix[i, j] = "0";

                    // Cập nhật sprite
                    foreach (Transform child in Board)
                    {
                        Cell cell = child.GetComponent<Cell>();
                        if (cell.row == i && cell.colum == j)
                        {
                            cell.ChangeImage("0");
                            break;
                        }
                    }

                    // Kiểm tra AI có thắng không (hiếm khi xảy ra với AI dễ)
                    if (Check(i, j, "0"))
                    {
                        Transform canvas = FindAnyObjectByType<Canvas>().transform;
                        GameObject window = Instantiate(gameOverPrefab, canvas);
                        window.transform.localScale = Vector3.one;
                        window.GetComponent<GameOver>().SetName("0");
                    }

                    return true;
                }
                matrix[i, j] = "";
            }
        }

        return false;
    }

    // Đơn giản hóa: AI dễ không cần đánh giá phức tạp
    // Xóa các hàm EvaluateCellStrong, PatternScore, EvaluateLine không cần thiết
}