using System;
using UnityEngine;

public class DBGrid<T> {
    private int _width;
    private int _height;
    private float _cellSize;
    private Vector3 _originPos;
    private T[,] _data;

    public DBGrid(int width, int height, float cellSize, Vector3 originPos, Func<T> createGridObject) {
        _width = width;
        _height = height;
        _cellSize = cellSize;
        _originPos = originPos;

        _data = new T[width, height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                _data[x, y] = createGridObject();
            }
        }

        bool showDebug = true;
        if (showDebug) {
            TextMesh[,] debugTextArray = new TextMesh[width, height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    /* debugTextArray[x, y] = Utils.CreateWorldText(_data[x,y]?.ToString(), null, 
                        GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * 0.5f, 12, Color.white);*/

                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, height), GetWorldPosition(width, 0), Color.white, 100f);
        }
    }

    public T GetValue(int x, int y) {
        if (x >= 0 && x < _width && y >= 0 && y < _height) {
            return _data[x, y];
        } else {
            return default(T);
        }
    }

    public T GetValue(Vector3 worldPos) {
        int x, y;
        GetXY(worldPos, out x, out y);
        return GetValue(x, y);
    }

    public void SetValue(int x, int y, T value) {
        if (x >= 0 && x < _width && y >= 0 && y < _height) {
            _data[x, y] = value;
        }
        // ignore if (x,y) out of bounds
    }

    public void SetValue(Vector3 worldPos, T value) {
        int x, y;
        GetXY(worldPos, out x, out y);
        SetValue(x, y, value);  
    }

    public Vector3 GetWorldPosition(int x, int y) {
        return new Vector3(x, y) * _cellSize + _originPos;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - _originPos).x / _cellSize);
        y = Mathf.FloorToInt((worldPosition - _originPos).y / _cellSize);
    }

    public Vector3 SnapToGrid(Vector3 worldPos) {
        // "snaps" the specified Vector3 to the grid square
        int x, y;
        GetXY(worldPos, out x, out y);
        return GetWorldPosition(x, y);
    }

    public Vector3 SnapToGridMidPoint(Vector3 worldPos) {
        Vector3 snapped = SnapToGrid(worldPos);   // this is the bottom-left corner of the cell
        return snapped + new Vector3(_cellSize / 2, _cellSize / 2);
    }
}
