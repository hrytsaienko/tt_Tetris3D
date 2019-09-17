using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Board : MonoBehaviour {

    public static int widthw = 10;
    public static int hight = 20;
    public Transform[,] grid = new Transform[widthw, hight];
    
    public float fallSpeed = 1.0f;
    public static bool gameStarted = false;
    
    private int numberOfClearLines = 0; //number of clear lines

    //Scoring
    //The more lines was cleared at one time, the more points player will get
    [SerializeField]
    private int scoreOneLine = 10;
    [SerializeField]
    private int scoreTwoLine = 30;
    [SerializeField]
    private int scoreThreeLine = 60;
    [SerializeField]
    private int scoreFourLine = 100;

    [SerializeField]
    private Text hud_score;
    [SerializeField]
    private Text hud_level;

    private int numberOfRowsThisTurn = 0;  // number of cleared lines(rows) at one time
    private int currentScore = 0;
    private int currentLevel = 0;

    private Spawner spawner;

    private void Start()
    {
        spawner = GetComponent<Spawner>();
        SpawnNext();
    }

    public void SpawnNext()
    {
        spawner.SpawnNext();
    }

    private void FixedUpdate()
    {
        UpdateScore();
        UpdateUI();
        UpdateLevel();
        UpdateSpeed();
    }

    void UpdateUI()
    {
        hud_score.text = currentScore.ToString();
        hud_level.text = currentLevel.ToString();
    }

    void UpdateLevel()
    {
        currentLevel = numberOfClearLines / 10;     //The level and speed are increased after every ten collected lines
    }

    void UpdateSpeed()
    {
        fallSpeed = 1.0f - ((float)currentLevel * 0.1f);
    }

    public void UpdateScore()
    {
        if (numberOfRowsThisTurn > 0)
        {
            if(numberOfRowsThisTurn == 1)
            {
                ClearedOneLine();
            }
            else if(numberOfRowsThisTurn == 2)
            {
                ClearedTwoLine();
            }
            else if(numberOfRowsThisTurn == 3)
            {
                ClearedThreeLine();
            }
            else if(numberOfRowsThisTurn == 4)
            {
                ClearedFourLine();
            }

            numberOfRowsThisTurn = 0;
        }
    }

    public void ClearedOneLine()
    {
        currentScore += scoreOneLine;
        numberOfClearLines++;
    }

    public void ClearedTwoLine()
    {
        currentScore += scoreTwoLine;
        numberOfClearLines += 2;
    }

    public void ClearedThreeLine()
    {
        currentScore += scoreThreeLine;
        numberOfClearLines += 3;
    }

    public void ClearedFourLine()
    {
        currentScore += scoreFourLine;
        numberOfClearLines += 4;
    }


    public Vector2 RoundVec2(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x),
                           Mathf.Round(v.y));
    }

    public bool InsideBorder(Vector2 pos)
    {
        return ((int)pos.x >= 0 &&
                (int)pos.x < widthw &&
                (int)pos.y >= 0);
    }

    public void DeleteRow(int y)
    {
        for (int x = 0; x < widthw; ++x)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    public void DecreaseRow(int y)
    {
        for (int x = 0; x < widthw; ++x)
        {
            if (grid[x, y] != null)
            {
                // Move one towards bottom
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;

                // Update Block position
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    public void DecreaseRowsAbove(int y)
    {
        for (int i = y; i < hight; ++i)
            DecreaseRow(i);
    }

    public bool IsRowFull(int y)
    {
        for (int x = 0; x < widthw; ++x)
            if (grid[x, y] == null)
                return false;

        numberOfRowsThisTurn++;
        return true;
    }

    public void DeleteFullRows()
    {
        for (int y = 0; y < hight; ++y)
        {
            if (IsRowFull(y))
            {
                DeleteRow(y);
                DecreaseRowsAbove(y + 1);
                --y;
            }
        }
    }

    public bool IsValidGridPos(Transform newTransform)
    {
        foreach (Transform child in newTransform)
        {
            Vector2 vector = RoundVec2(child.position);

            // Not inside Border?
            if (!InsideBorder(vector))
                return false;

            // Block in grid cell (and not part of same group)?
            if (grid[(int)vector.x, (int)vector.y] != null &&
                grid[(int)vector.x, (int)vector.y].parent != newTransform)
                return false;
        }
        return true;
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOverScene");
    }

    private void OnDestroy()
    {
        currentScore = 0;
        gameStarted = false;
        currentScore = 0;
        numberOfClearLines = 0;
    }
}
