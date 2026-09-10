using System.Collections.Generic;
using UnityEngine;

public class TileBuskingBooth : CTile
{
    public TileBuskingBooth()
    {
        this._name = "버스킹 부스";
        this._description = "음악동아리 부원들이 돌아가며 공연을 하고 있습니다!\n연결된 모든 음향중계기 + 1만큼 점수를 얻습니다.";
        this._tileState = ETileState.Built | ETileState.Point;
        this._tileInfo = "";
        this._baseColor = new UnityEngine.Color(1f, 0.5f, 0f);
        this._radius = 1;
        this._tileInCatalog = ETileCatalog.BuskingBooth;
    }

    public override SScoreInfo OnScore()
    {
        int count = GetConnectRelay();
        int score = 1 + count;
        if (_gameManager.GetTech(ETech.Music
            ) >= 5)
        {
            this._description = "음악동아리 부원들이 돌아가며 공연을 하고 있습니다!\n연결된 모든 음향중계기 + 1만큼 점수를 얻습니다.\n추가로 3개마다 1점을 더 얻습니다.";
            score += count / 3;
        }
        return new SScoreInfo(score, _name);
    }

    public override void OnSelected()
    {
        SScoreInfo score = OnScore();
        _tileInfo = $"<sprite=9> {score.Score}";
        _additionalDescription = $"현재 점수는 <b>{score.Score}</b> 입니다.";
        base.OnSelected();
    }

    private int GetConnectRelay()
    {
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
        Queue<Vector3Int> queue = new Queue<Vector3Int>();

        visited.Add(this._tilePosition);
        queue.Enqueue(this._tilePosition);

        int relayCount = 0;

        while (queue.Count > 0)
        {
            Vector3Int currentPos = queue.Dequeue();

            List<CTile> neighborTiles = _gameManager.FindNeighborTiles(currentPos, 1);
            
            for (int i = 0; i < neighborTiles.Count;i++)
            {
                if (visited.Contains(neighborTiles[i].TilePosition)) continue;
                visited.Add(neighborTiles[i].TilePosition);
                if (neighborTiles[i].TileInCatalog == ETileCatalog.SoundRelay)
                {
                    queue.Enqueue(neighborTiles[i].TilePosition);
                    relayCount++;
                }
            }
        }

        return relayCount;
    }
}
