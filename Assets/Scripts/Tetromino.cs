using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tetromino : MonoBehaviour {

    public bool allowRotation = true;
    public bool limitRotation = false;

    // Time since last gravity tick
    float lastFall = 0;

    private readonly float continousVerticalSpeed = 0.005f;    // The speed in which a tetromino will move when the down botton is heid down
    private readonly float continousHorizontalSpeed = 0.1f;   // The speed in which a tetromino will move when the left or right bottons are heid down
    private readonly float bottonDownWaitMax = 0.2f;          // How long to wait before a tetromino will start to react on botton

    private float verticalTimer = 0;
    private float horizontalTimer = 0;
    private float buttonDownWaitTimer = 0;

    private bool movedImmediateHorizontal = false;
    private bool movedImmediateVertical = false;

    private Board board;
    private SwipeInput swipeInput;
    private Transform newTransform;
    private bool rotatePressed = false;
    private bool isOnBottom = false;

    private Button rotateButton;

    void Start()
    {
        board = GameObject.FindObjectOfType<Board>();
        swipeInput = FindObjectOfType<SwipeInput>();
        rotateButton = GameObject.Find("RotateButton").GetComponent<Button>();
        rotateButton.onClick.AddListener(() => SetRotate());

        // Default position not valid? Then it's game over
        if (!board.IsValidGridPos(transform))
        {
            Destroy(gameObject);
            board.GameOver();
        }
    }
	
	void Update ()
    {
        newTransform = transform;
        
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            movedImmediateHorizontal = false;
            movedImmediateVertical = false;

            horizontalTimer = 0;
            verticalTimer = 0;
            buttonDownWaitTimer = 0;
        }

        GetInput();

        GetSwipeInput();
    }

    private void GetSwipeInput()
    {
        SwipeDirection direction = SwipeDirection.Null;
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                swipeInput.setFingerUpPosition(touch.position);
                swipeInput.setFingerDownPosition(touch.position);
            }

            if (!swipeInput.GetDetectSwipeOnlyAfterRelease() && touch.phase == TouchPhase.Moved)
            {
                swipeInput.setFingerDownPosition(touch.position);
                direction = swipeInput.DetectSwipe();
            }

            if (touch.phase == TouchPhase.Ended)
            {
                swipeInput.setFingerDownPosition(touch.position);
                direction = swipeInput.DetectSwipe();
            }

            if (direction == SwipeDirection.Left)
                MoveLeft();

            if (direction == SwipeDirection.Right)
                MoveRight();

            if (direction == SwipeDirection.Down ||
                 Time.time - lastFall >= board.fallSpeed)
                MoveDown();
        }
    }

    private void FixedUpdate()
    {
        UpdateTetrominoTransform();
    }

    private void UpdateTetrominoTransform()
    {
        if (rotatePressed)        
            Rotate();
        
        if(newTransform != null)
        {
            transform.position = newTransform.position;
            UpdateGrid();
        }         
        
        if(isOnBottom)
        {
            // Clear filled horizontal lines
            board.DeleteFullRows();

            // Spawn next Group
            board.SpawnNext();
            
            enabled = false;
            isOnBottom = false;
        }
        
    }

    private void GetInput()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveLeft();
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveRight();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            rotatePressed = true;
        }

        if (Input.GetKey(KeyCode.DownArrow) ||
                 Time.time - lastFall >= board.fallSpeed)
        {
            MoveDown();
        }
    }

    private void SetRotate()
    {
        rotatePressed = true;
    }

    void UpdateGrid()
    {
        // Remove old children from grid
        for (int y = 0; y < Board.hight; ++y)
            for (int x = 0; x < Board.widthw; ++x)
                if (board.grid[x, y] != null)
                    if (board.grid[x, y].parent == transform)
                        board.grid[x, y] = null;

        // Add new children to grid
        foreach (Transform child in transform)
        {
            Vector2 v = board.RoundVec2(child.position);
            board.grid[(int)v.x, (int)v.y] = child;
        }
    }

    /// <summary>
    /// Moves to the left
    /// </summary>
    void MoveLeft()
    {
        if (movedImmediateHorizontal)
        {
            if (buttonDownWaitTimer < bottonDownWaitMax)
            {
                buttonDownWaitTimer += Time.deltaTime;
                return;
            }

            if (horizontalTimer < continousHorizontalSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }

        if (!movedImmediateHorizontal)
            movedImmediateHorizontal = true;

        horizontalTimer = 0;

        // Modify new position
        newTransform.position += new Vector3(-1, 0, 0);

        // See if valid
        if (!board.IsValidGridPos(newTransform))            
            //It's not valid. revert.
            newTransform.position += new Vector3(1, 0, 0);
    }

    /// <summary>
    /// Moves to the right
    /// </summary>
    void MoveRight()
    {
        if (movedImmediateHorizontal)
        {
            if (buttonDownWaitTimer < bottonDownWaitMax)
            {
                buttonDownWaitTimer += Time.deltaTime;
                return;
            }

            if (horizontalTimer < continousHorizontalSpeed)
            {
                horizontalTimer += Time.deltaTime;
                return;
            }
        }

        if (!movedImmediateHorizontal)
            movedImmediateHorizontal = true;

        horizontalTimer = 0;

        // Modify position
        newTransform.position += new Vector3(1, 0, 0);

        //// See if valid
        if (!board.IsValidGridPos(newTransform))
            newTransform.position += new Vector3(-1, 0, 0);
    }

    /// <summary>
    /// Rotate
    /// </summary>
    void Rotate()
    {
        if (allowRotation)
        {
            if (limitRotation)
            {
                if (transform.rotation.eulerAngles.z >= 90)
                {
                    transform.Rotate(0, 0, -90);
                }
                else
                {
                    transform.Rotate(0, 0, 90);
                }
            }
            else
            {
                transform.Rotate(0, 0, -90);
            }
            // See if valid
            if (board.IsValidGridPos(transform))
            {
                // It's valid. Update grid.
                UpdateGrid();
            }            
            else
            {
                if (limitRotation)
                {
                    if (transform.rotation.eulerAngles.z >= 90)
                    {
                        transform.Rotate(0, 0, -90);
                    }
                    else
                    {
                        transform.Rotate(0, 0, 90);
                    }
                }
                else
                {
                    // It's not valid. revert.
                    transform.Rotate(0, 0, 90);
                }
            }                           
        }
        rotatePressed = false;
    }

    /// <summary>
    /// Moves Downwards and Fall
    /// </summary>
    void MoveDown()
    {
        if (movedImmediateVertical)
        {
            if (buttonDownWaitTimer < bottonDownWaitMax)
            {
                buttonDownWaitTimer += Time.deltaTime;
                return;
            }

            if (verticalTimer < continousVerticalSpeed)
            {
                verticalTimer += Time.deltaTime;
                return;
            }
        }

        if (!movedImmediateVertical)
            movedImmediateVertical = true;


        verticalTimer = 0;

        // Modify position
        newTransform.position += new Vector3(0, -1, 0);

        // See if valid
        if (!board.IsValidGridPos(newTransform))
        {
            newTransform.position += new Vector3(0, 1, 0);
            isOnBottom = true;
        }

        lastFall = Time.time;
    }

    //private void OnDestroy()
    //{
    //    rotateButton.onClick.RemoveListener(() => SetRotate());
    //}
}
