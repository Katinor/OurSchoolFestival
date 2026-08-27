using UnityEngine;
using UnityEngine.Tilemaps;

public partial class Test_TilemapSelector
{
    private void DrawMapTiles()
    {
        if (_tilemap == null)
        {
            CPrint.Error("타일맵 찾을 수 없음.");
        }
        if (_tileBases[GetIndex(ETempTileCatalog.RoadBase)] == null)
        {
            CPrint.Error("길 찾을 수 없음.");
            return;
        }
        if (_tileBases[GetIndex(ETempTileCatalog.Basement)] == null)
        {
            CPrint.Error("땅 찾을 수 없음.");
            return;
        }
        TileBase baseRoad = _tileBases[GetIndex(ETempTileCatalog.RoadBase)];
        TileBase baseLand = _tileBases[GetIndex(ETempTileCatalog.Basement)];


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
    
    public static int GetIndex(ETempTileCatalog tileCatalog)
    {
        return (int)tileCatalog;
    }
}