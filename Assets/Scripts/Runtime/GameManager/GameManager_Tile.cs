using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.WSA;

public partial class GameManager
{
    private void DrawMapTiles()
    {
        if (_tilemap == null)
        {
            Logger.Error("타일맵 찾을 수 없음.");
        }
        if (_tileBases[GetIndex(ETileCatalog.RoadBase)] == null)
        {
            Logger.Error("길 찾을 수 없음.");
            return;
        }
        if (_tileBases[GetIndex(ETileCatalog.Basement)] == null)
        {
            Logger.Error("땅 찾을 수 없음.");
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
        List<CTile> tiles = GetAllTiles();
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].IsFirst = true;
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

    private bool findTileByPosition(Vector3Int? posInCell, out CTile tile)
    {
        tile = null;
        if (!posInCell.HasValue)
        {
            return false;
        }
        else
        {
            if (Mathf.Abs(posInCell.Value.x) > _gridSizeXRight || Mathf.Abs(posInCell.Value.y) > _gridSizeYUpper)
            {
                tile = null;
                return false;
            }
            GameObject go = _tilemap.GetInstantiatedObject(posInCell.Value);
            tile = go.GetComponent<CTile>();
            if (tile == null)
            {
                return false;
            }
            return true;
        }
    }

    private void OnClickTile(GameObject go, Vector3Int posInCell)
    {
        _rightUIOn = true;
        CTile targetTile = null;
        CTile tempTile = null;
        List<CTile> tileList = null;
        _camera.StartFocusing(posInCell);
        if (_lastSelectedPosition != null)
        {
            if (findTileByPosition(_lastSelectedPosition, out tempTile))
            {
                tempTile.OnResetShown();
            }

            if (_lastNearObject != null)
            {
                for (int i = 0; i < _lastNearObject.Count; i++)
                {
                    if (findTileByPosition(_lastNearObject[i], out tempTile))
                    {
                        tempTile.OnResetShown();
                    }
                }
            }
        }
        if (go.TryGetComponent<CTile>(out targetTile))
        {
            targetTile.OnSelected();
            if (targetTile.Radius > 0)
            {
                tileList = FindNeighborTiles(posInCell, targetTile.Radius);
                for (int i = 0; i < tileList.Count; i++)
                {
                    tileList[i].OnSelectedByOthers();
                }
            }
            _tileName.text = targetTile.Name;
            _tileDescription.text = targetTile.GetDescription();
            Logger.Log($"{targetTile.TileState}");
            if ((targetTile.TileState & ETileState.Upgradable) != ETileState.None)
            {
                _upgradeButton.gameObject.SetActive(true);
                _tileUpgradeCost.text = UpgradeCostParser(targetTile.UpgradeCost);
                Logger.Log($"{CheckResource(targetTile.UpgradeCost)}");
                if (CheckResource(targetTile.UpgradeCost))
                {
                    _upgradeButton.interactable = true;
                }
                else
                {
                    _upgradeButton.interactable = false;
                }
            }
            else
            {
                _upgradeButton.gameObject.SetActive(false);
            }
            if ((targetTile.TileState & ETileState.Action) != ETileState.None)
            {
                _tileActionButton.gameObject.SetActive(true);
                _tileActionMessage.gameObject.SetActive(true);
                _tileActionCost.text = ActionCostParser(targetTile.ActionName, targetTile.ActionCost);
                if (targetTile.ActionUsed) _tileActionMessage.text = "(이미 사용함)";
                else if (!targetTile.ActionEnabled) _tileActionMessage.text = "(사용조건 불만족)";
                else if (!CheckResource(targetTile.ActionCost)) _tileActionMessage.text = "(액션 비용 부족)";
                else _tileActionMessage.text = "";

                if (!targetTile.ActionUsed && targetTile.ActionEnabled && CheckResource(targetTile.ActionCost))
                {
                    _tileActionButton.interactable = true;
                }
                else
                {
                    _tileActionButton.interactable = false;
                }
            }
            else
            {
                _tileActionButton.gameObject.SetActive(false);
                _tileActionMessage.gameObject.SetActive(false);
            }
                targetTile.PlaySound();
        }
        else
        {
            ChangeBottomText($"({posInCell.x}, {posInCell.y}) : CTile 없는 객체");
        }
        _lastNearObject = FindNeighborPosition(posInCell, targetTile.Radius);
        _lastSelectedPosition = posInCell;
    }

    private void OnPointTile(GameObject go, Vector3Int posInCell)
    {
        CTile targetTile = null;
        CTile tempTile = null;
        List<CTile> tileList = null;
        if (_lastSelectedPosition != null)
        {
            if (findTileByPosition(_lastSelectedPosition, out tempTile))
            {
                tempTile.OnResetShown();
            }

            if (_lastNearObject != null)
            {
                for (int i = 0; i < _lastNearObject.Count; i++)
                {
                    if (findTileByPosition(_lastNearObject[i], out tempTile))
                    {
                        tempTile.OnResetShown();
                    }
                }
            }
        }
        if (go.TryGetComponent<CTile>(out targetTile))
        {
            targetTile.OnPoint(_questionTileRadius);
            if (_questionTileRadius > 0)
            {
                tileList = FindNeighborTiles(posInCell, _questionTileRadius);
                for (int i = 0; i < tileList.Count; i++)
                {
                    tileList[i].OnPointByOthers();
                }
            }
        }
        else
        {
            Logger.Error("CTile 찾기 실패");
        }
        _lastNearObject = FindNeighborPosition(posInCell, _questionTileRadius);
        _lastSelectedPosition = posInCell;
    }

    private void OnClickElse()
    {
        _rightUIOn = false;
        _leftUIOn = false;
        _soundUIOn = false;
        CTile tempTile = null;
        if (_lastSelectedPosition.HasValue)
        {
            if (_questionAction == null && _questionCard == null)
            {
                _camera.StopFocusing();
                if (findTileByPosition(_lastSelectedPosition, out tempTile))
                {
                    tempTile.OnResetShown();
                    tempTile.TextObject.SetActive(false);
                }

                if (_lastNearObject != null)
                {
                    for (int i = 0; i < _lastNearObject.Count; i++)
                    {
                        if (findTileByPosition(_lastNearObject[i], out tempTile))
                        {
                            tempTile.OnResetShown();
                        }
                    }
                }
                _lastSelectedPosition = null;
                _lastNearObject = null;
            }
            else
            {

                if (findTileByPosition(_lastSelectedPosition, out tempTile))
                {
                    tempTile.OnResetShown();
                    tempTile.OnSelected();
                }

                if (_lastNearObject != null)
                {
                    for (int i = 0; i < _lastNearObject.Count; i++)
                    {
                        if (findTileByPosition(_lastNearObject[i], out tempTile))
                        {
                            tempTile.OnResetShown();
                        }
                    }
                }
                _lastNearObject = null;

            }
        }
        ChangeBottomText("");
    }

    private string UpgradeCostParser(SCost cost)
    {
        return $"업그레이드 ({cost.toCostString()})";
    }

    private string ActionCostParser(string actionName, SCost cost)
    {
        return $"{actionName} ({cost.toCostString()})";
    }

    public static int GetIndex(ETileCatalog tileCatalog)
    {
        return (int)tileCatalog;
    }

    public TileBase GetTileBase(ETileCatalog tile)
    {
        return _tileBases[GetIndex(tile)];
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
                Logger.Error("CTile 찾기 실패");
            }
        }
    }

    public List<CTile> GetAllTiles()
    {
        List<CTile> tempTileList = new List<CTile>();
        for (int i = (-1) * _gridSizeYUpper; i <= _gridSizeYUpper; i++)
        {
            for (int j = (-1) * _gridSizeXRight; j <= _gridSizeXRight; j++)
            {
                Vector3Int pos = new Vector3Int(j, i, 0);
                if (findTileByPosition(pos, out CTile tempClass))
                {
                    tempTileList.Add(tempClass);
                }
                else
                {
                    Logger.Error("CTile 찾기 실패");
                    tempTileList.Add(new TileBasement());
                }
            }
        }
        Logger.Log($"GetAllTiles : {tempTileList.Count}개");
        return tempTileList;
    }

    public (List<int> tileId, List<int> tilePoint) GetAllTilesByInt()
    {
        List<CTile> tempTileList = GetAllTiles();
        List<int> tileIdList = new List<int>();
        List<int> tilePointList = new List<int>();
        for (int i = 0; i < tempTileList.Count; i++)
        {
            tileIdList.Add((int)tempTileList[i].TileInCatalog);
            tilePointList.Add(tempTileList[i].Points);
        }
        return (tileIdList, tilePointList);
    }

    private void LoadTilesByInt(List<int> tileIdList, List<int> tilePointList)
    {
        int index = 0;
        for (int i = (-1) * _gridSizeYUpper; i <= _gridSizeYUpper; i++)
        {
            for (int j = (-1) * _gridSizeXRight; j <= _gridSizeXRight; j++)
            {
                Vector3Int pos = new Vector3Int(j, i, 0);
                BuildTile((ETileCatalog)tileIdList[index], pos);
                if (findTileByPosition(pos, out CTile tempClass))
                {
                    tempClass.IsFirst = true;
                    tempClass.Points = tilePointList[index];
                }
                else
                {
                    Logger.Error("CTile 건설 후 찾기 실패");
                }
                index++;
            }
        }
    }

    public List<CTile> FindNeighborTiles(Vector3Int pos, int radius = 1)
    {
        List<CTile> tempTileList = new List<CTile>();
        List<Vector3Int> tempPosList = FindNeighborPosition(pos, radius);
        for (int i = 0; i < tempPosList.Count; i++)
        {
            if (findTileByPosition(tempPosList[i], out CTile tempClass))
            {
                tempTileList.Add(tempClass);
            }
            else
            {
                Logger.Error("CTile 찾기 실패");
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