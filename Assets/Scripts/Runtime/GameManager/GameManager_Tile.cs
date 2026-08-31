using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public partial class GameManager
{
    private void DrawMapTiles()
    {
        if (_tilemap == null)
        {
            CPrint.Error("타일맵 찾을 수 없음.");
        }
        if (_tileBases[GetIndex(ETileCatalog.RoadBase)] == null)
        {
            CPrint.Error("길 찾을 수 없음.");
            return;
        }
        if (_tileBases[GetIndex(ETileCatalog.Basement)] == null)
        {
            CPrint.Error("땅 찾을 수 없음.");
            return;
        }
        TileBase baseRoad = _tileBases[GetIndex(ETileCatalog.RoadBase)];
        TileBase baseLand = _tileBases[GetIndex(ETileCatalog.Basement)];


        _tilemap.ClearAllTiles();
        for (int i = (-1) * _gridSizeYUpper; i <= _gridSizeYUpper; i++)
        {
            for (int j = (-1) * _gridSizeXRight; j <= _gridSizeXRight; j++)
            {
                if (i == 0 && j >= (_gridSizeXRight + 1 - _roadSize))
                {
                    _tilemap.SetTile(new Vector3Int(j, i, 0), baseRoad);
                }
                else
                {
                    _tilemap.SetTile(new Vector3Int(j, i, 0), baseLand);
                }
            }
        }
    }

    private bool findObjectByTile(Vector3 hitpoint, out Vector3Int posInCell, out GameObject go)
    {
        posInCell = _tilemap.WorldToCell(hitpoint);
        if (Mathf.Abs(posInCell.x) > _gridSizeXRight || Mathf.Abs(posInCell.y) > _gridSizeYUpper)
        {
            go = null;
            return false;
        }
        go = _tilemap.GetInstantiatedObject(posInCell);
        if (go == null)
        {
            return false;
        }
        return true;
    }

    private bool findObjectByPosition(Vector3Int posInCell, out GameObject go)
    {
        if (Mathf.Abs(posInCell.x) > _gridSizeXRight || Mathf.Abs(posInCell.y) > _gridSizeYUpper)
        {
            go = null;
            return false;
        }
        go = _tilemap.GetInstantiatedObject(posInCell);
        if (go == null)
        {
            return false;
        }
        return true;
    }

    private bool findTileByPosition(Vector3Int posInCell, out CTile tile)
    {
        if (Mathf.Abs(posInCell.x) > _gridSizeXRight || Mathf.Abs(posInCell.y) > _gridSizeYUpper)
        {
            tile = null;
            return false;
        }
        GameObject go = _tilemap.GetInstantiatedObject(posInCell);
        tile = go.GetComponent<CTile>();
        if (tile == null)
        {
            return false;
        }
        return true;
    }

    private void OnClickTile(GameObject go, Vector3Int posInCell)
    {
        _rightUIOn = true;
        CTile tempClass = null;
        if (_lastObject != null)
        {
            if (_lastObject.TryGetComponent<CTile>(out tempClass))
            {
                tempClass.Highlights(_lastColor);
            }
        }
        if (go.TryGetComponent<CTile>(out tempClass))
        {
            _lastColor = tempClass.getColor();
            tempClass.Highlights(Color.yellow);
            _tileName.text = tempClass.Name;
            _tileDescription.text = tempClass.Description;
            CPrint.Log($"{tempClass.TileState}");
            if ((tempClass.TileState & ETileState.Upgradable) != ETileState.None)
            {
                _upgradeButton.gameObject.SetActive(true);
                _tileUpgradeCost.text = UpgradeCostParser(tempClass.Cost);
            }
            else
            {
                _upgradeButton.gameObject.SetActive(false);
            }
        }
        else
        {
            ChangeBottomText($"({posInCell.x}, {posInCell.y}) : CTile 없는 객체");
        }
        _lastObject = go;
        _lastSelectedObject = go;
        _lastSelectedPosition = posInCell;

        tempClass.PlaySound();
    }

    private void OnClickElse()
    {
        _rightUIOn = false;
        _leftUIOn = false;
        CTile tempClass = null;
        if (_lastObject != null)
        {
            if (_lastObject.TryGetComponent<CTile>(out tempClass))
            {
                tempClass.Highlights(_lastColor);
            }
        }
        _lastObject = null;
        ChangeBottomText("");
    }

    private string UpgradeCostParser(SCost cost)
    {
        return $"업그레이드 ({cost.toCostString()})";
    }
    
    public static int GetIndex(ETileCatalog tileCatalog)
    {
        return (int)tileCatalog;
    }

    public void BuildTile(ETileCatalog tile, Vector3Int pos, string buildInfo = "")
    {
        _tilemap.SetTile(pos, _tileBases[GetIndex(tile)]);
        if (!string.IsNullOrEmpty(buildInfo))
        {
            if (findTileByPosition(pos, out CTile tempClass))
            {
                tempClass.TileInfo = buildInfo;
            }
            else
            {
                CPrint.Error("CTile 찾기 실패");
            }
        }
    }

    public List<CTile> FindNeighborTiles(Vector3Int pos, int radius = 1)
    {
        List<CTile> tempTileList = new List<CTile>();
        List<Vector3Int> tempPosList = FindNeighborPosition(pos, radius);
        for (int i = 0; i < tempPosList.Count; i++)
        {
            if (findTileByPosition(pos, out CTile tempClass))
            {
                tempTileList.Add(tempClass);
            }
            else
            {
                CPrint.Error("CTile 찾기 실패");
            }
        }

        return tempTileList;
    }

    public List<Vector3Int> FindNeighborPosition(Vector3Int pos, int radius = 1)
    {
        List<Vector3Int> tempList = new List<Vector3Int>();
        // 큐브 좌표계로 만들기
        Vector3Int centerAsCube = new Vector3Int
            (
                // 전부 정수라 나머지는 자동으로 버려짐!
                pos.x - ((pos.y - (Mathf.Abs(pos.y) % 2)) / 2),
                pos.y,
                (-1) * (pos.x - (int)((pos.y - (Mathf.Abs(pos.y) % 2)) / 2f)) - pos.y
            );

        for (int q = (-1) * radius; q <= radius; q++)
        {
            // s = -q -r이므로 -rad <= -q -r <= rad 를 만족하도록 범위 제한
            int rMin = Mathf.Max(-radius, -q - radius);
            int rMax = Mathf.Min(radius, -q + radius);

            for (int r = rMin; r <= rMax; r++)
            {
                int s = (-1) * (q + r);
                if (q == 0 && r == 0 && s == 0) continue;
                Vector3Int targetAsCube = centerAsCube + new Vector3Int(q, r, s);
                Vector3Int targetAsPos = new Vector3Int
                    (
                        targetAsCube.x + ((targetAsCube.y - (Mathf.Abs(targetAsCube.y) % 2)) / 2),
                        targetAsCube.y,
                        0
                    );
                if (InGrid(targetAsPos)) tempList.Add(targetAsPos);

            } 
        }
        return tempList;
    }

    private bool InGrid(Vector3Int pos)
    {
        if (Mathf.Abs(pos.x) > _gridSizeXRight) return false;
        if (Mathf.Abs(pos.y) > _gridSizeYUpper) return false;
        return true;
    }

}