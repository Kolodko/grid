using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class GeometryMovement : MonoBehaviour
{
    [Header("Configuration")] 
    [SerializeField] private string textFileName = "data.txt";
    [SerializeField] private float spacing = 1.1f;

    private InputAction _moveUp;
    private InputAction _moveDown;
    private InputAction _moveLeft;
    private InputAction _moveRight;

    private List<string> _lines = new List<string>();
    private int _currentRow;
    private int _currentCol;
    private GameObject[,] _cubeInstances = new GameObject[3, 3];

    private readonly Dictionary<char, Color> colourMap = new Dictionary<char, Color>
    {
        { '1', Color.red },
        { '2', Color.yellow },
        { '3', Color.blue },
        { '4', new Color(0.5f, 0f, 0.5f) }
    };

    private void Awake()
    {
        _moveUp = new InputAction("Move Up", InputActionType.Button, "keyboard/w");
        _moveDown = new InputAction("Move Down", InputActionType.Button, "keyboard/s");
        _moveLeft = new InputAction("Move Left", InputActionType.Button, "keyboard/a");
        _moveRight = new InputAction("Move Right", InputActionType.Button, "keyboard/d");

        _moveUp.performed += ctx => Move(-1, 0);
        _moveDown.performed += ctx => Move(1, 0);
        _moveLeft.performed += ctx => Move(0, -1);
        _moveRight.performed += ctx => Move(0, 1);
    }

    private void OnEnable()
    {
        _moveUp.Enable();
        _moveDown.Enable();
        _moveLeft.Enable();
        _moveRight.Enable();
    }

    private void OnDisable()
    {
        _moveUp.Disable();
        _moveDown.Disable();
        _moveLeft.Disable();
        _moveRight.Disable();
    }

    private void Start()
    {
        LoadData();
        InitializeRandomPosition();
        CreateInitialCubes();
    }

    private void LoadData()
    {
        string path = Path.Combine(Application.streamingAssetsPath, textFileName);

        try
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();

                    if (!string.IsNullOrEmpty(line))
                    {
                        _lines.Add(line);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to read file at {path}: {ex.Message}");
        }

        if (_lines.Count > 0)
        {
            int width = _lines[0].Length;

            foreach (var l in _lines)
            {
                if (l.Length != width)
                {
                    break;
                }
            }
        }
    }

    private void InitializeRandomPosition()
    {
        if (_lines.Count == 0)
            return;

        _currentRow = Random.Range(0, _lines.Count);
        _currentCol = Random.Range(0, _lines[0].Length);
    }

    private void CreateInitialCubes()
    {
        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.SetParent(transform, false);
                cube.transform.localPosition = new Vector3((c - 1) * spacing, 0, (1 - r) * spacing);
                _cubeInstances[r, c] = cube;
            }
        }

        UpdateCubes();
    }

    private void UpdateCubes()
    {
        if (_lines.Count == 0)
            return;

        int height = _lines.Count;
        int width = _lines[0].Length;

        for (int r = -1; r <= 1; r++)
        {
            for (int c = -1; c <= 1; c++)
            {
                int rowIndex = (_currentRow + r + height) % height;
                int colIndex = (_currentCol + c + width) % width;

                char ch = _lines[rowIndex][colIndex];
                Color colour = colourMap.ContainsKey(ch) ? colourMap[ch] : Color.white;
                GameObject cube = _cubeInstances[r + 1, c + 1];
                Renderer rend = cube.GetComponent<Renderer>();
                rend.material.color = colour;
            }
        }
    }

    private void Move(int rowOffset, int colOffset)
    {
        if (_lines.Count == 0)
            return;

        int height = _lines.Count;
        int width = _lines[0].Length;
        _currentRow = (_currentRow + rowOffset + height) % height;
        _currentCol = (_currentCol + colOffset + width) % width;
        UpdateCubes();
    }
}