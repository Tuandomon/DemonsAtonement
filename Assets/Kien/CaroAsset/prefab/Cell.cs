using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public GameObject gameOver;       // Prefab GameOver
    private Transform canvas;
    public int row;
    public int colum;
    private board Board;
    public Sprite x;
    public Sprite o;

    private Image image;
    private Button button;

    private void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void Start()
    {
        Board = FindAnyObjectByType<board>();
        canvas = FindAnyObjectByType<Canvas>().transform;
    }

    public void ChangeImage(string s)
    {
        if (s == "x")
            image.sprite = x;
        else
            image.sprite = o;
    }

    public void OnClick()
    {
        // Kiểm tra ô trống dựa vào matrix
        if (!string.IsNullOrEmpty(Board.matrix[row, colum]))
            return;

        // Người chơi đánh
        Board.matrix[row, colum] = Board.currentTurn;
        ChangeImage(Board.currentTurn);

        // Kiểm tra thắng
        if (Board.Check(row, colum, Board.currentTurn))
        {
            GameObject window = Instantiate(gameOver, canvas);
            window.GetComponent<GameOver>().SetName(Board.currentTurn);
            return;
        }

        // Chuyển lượt cho AI nếu là X
        if (Board.currentTurn == "x")
        {
            Board.currentTurn = "0";
            StartCoroutine(Board.AIDelay()); // AI đánh sau 0.1s
        }
        else
        {
            Board.currentTurn = "x";
        }
    }
}
