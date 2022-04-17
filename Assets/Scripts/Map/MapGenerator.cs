using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App
{
    /// <summary>
    /// タイルのプレハブ
    /// </summary>
    [Serializable]
    public class TilePrefabs
    {
        public GameObject None;
        public GameObject Red;
        public GameObject Blue;

        public GameObject this[Team type]
        {
            get
            {
                switch (type)
                {
                    case Team.None:
                        return None;
                    case Team.Red:
                        return Red;
                    case Team.Blue:
                        return Blue;
                }
                return null;
            }
        }
    }

    [Serializable]
    public class TokenPrefabs
    {
        public GameObject Laser;
        public GameObject King;
        public GameObject Deflector;
        public GameObject Defender;
        public GameObject Switch;

        public GameObject this[TokenType type]
        {
            get
            {
                switch (type)
                {
                    case TokenType.Laser:
                        return Laser;
                    case TokenType.King:
                        return King;
                    case TokenType.Deflector:
                        return Deflector;
                    case TokenType.Defender:
                        return Defender;
                    case TokenType.Switch:
                        return Switch;
                }
                return null;
            }
        }
    }

    /// <summary>
    /// マップ情報
    /// </summary>
    [Serializable]
    public class TileInfo
    {
        public Team teamType = Team.None;

        // 配置されているピース
        public TokenBase token;
    }

    [Serializable]
    public class BoardMap
    {
        public TileInfo[] tileMap;

        public class LaserTokenHub
        {
            internal LaserToken red;
            internal LaserToken blue;

            public LaserToken this[Team team]
            {
                get
                {
                    switch (team)
                    {
                        case Team.Red:
                            return red;
                        case Team.Blue:
                            return blue;
                    }
                    return null;
                }
            }
        }
        public LaserTokenHub lasers;

        public class KingTokenHub
        {
            internal KingToken red;
            internal KingToken blue;

            public KingToken this[Team team]
            {
                get
                {
                    switch (team)
                    {
                        case Team.Red:
                            return red;
                        case Team.Blue:
                            return blue;
                    }
                    return null;
                }
            }
        }
        public KingTokenHub kings;

        public BoardMap()
        {
            lasers = new LaserTokenHub();
            kings = new KingTokenHub();
        }

        public TileInfo GetTileInfo(int x, int z)
        {
            x = Mathf.Clamp(x, 0, MapTip.MapX);
            z = Mathf.Clamp(z, 0, MapTip.MapZ);
            return tileMap[z * MapTip.MapX + x];
        }

        public bool IsOutOfBoard(int x, int z)
        {
            if (x < 0 || MapTip.MapX <= x)
                return true;
            if (z < 0 || MapTip.MapZ <= z)
                return true;
            return false;
        }
    }

    /// <summary>
    /// マップ生成
    /// </summary>
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField]
        private TilePrefabs tilePrefabs;

        [SerializeField]
        private TokenPrefabs tokenPrefabs;

        public BoardMap Generate()
        {
            BoardMap result = new BoardMap();

            var tileMap = new TileInfo[MapTip.MapX * MapTip.MapZ];
            var TileFolder = new GameObject("Tiles");
            var TokenFolder = new GameObject("Tokens");

            var offset = new Vector3(0, 0, 0);

            for (int iz = 0; iz < MapTip.MapZ; ++iz)
            {
                for (int ix = 0; ix < MapTip.MapX; ++ix)
                {
                    var pos = offset + new Vector3(ix, 0, -iz);
                    int angle = 0;
                    var tile = tileMap[(iz * MapTip.MapX) + ix] = new TileInfo();

                    // 陣地
                    tile.teamType = (Team)MapTip.TileTip[iz, ix];

                    var tileObj = GameObject.Instantiate(tilePrefabs[tile.teamType], TileFolder.transform);
                    tileObj.transform.localPosition = pos;

                    // Red
                    {
                        TokenType tokenType = (TokenType)MapTip.RedTokenTip[0, iz, ix];
                        if (tokenType != TokenType.None)
                        {
                            var gobj = GameObject.Instantiate(tokenPrefabs[tokenType], TokenFolder.transform);
                            var token = gobj.GetComponent<TokenBase>();
                            token.SetTeam(Team.Red);
                            tile.token = token;

                            angle = MapTip.RedTokenTip[1, iz, ix];

                            if(tokenType == TokenType.Laser)
                            {
                                result.lasers.red = token as LaserToken;
                            }
                            else if(tokenType == TokenType.King)
                            {
                                result.kings.red = token as KingToken;
                            }
                        }
                    }

                    // Blue
                    {
                        TokenType tokenType = (TokenType)MapTip.BlueTokenTip[0, iz, ix];
                        if (tokenType != TokenType.None)
                        {
                            var gobj = GameObject.Instantiate(tokenPrefabs[tokenType], TokenFolder.transform);
                            var token = gobj.GetComponent<TokenBase>();
                            token.SetTeam(Team.Blue);
                            //token.tokenType = tokenType;
                            tile.token = token;

                            angle = MapTip.BlueTokenTip[1, iz, ix];

                            if(tokenType == TokenType.Laser)
                            {
                                result.lasers.blue = token as LaserToken;
                            }
                            else if(tokenType == TokenType.King)
                            {
                                result.kings.blue = token as KingToken;
                            }
                        }
                    }

                    // Transform設定
                    if (tile.token != null)
                    {
                        angle = Mathf.Clamp(angle - 1,(int)TokenDir.Up, (int)TokenDir.Right);

                        tile.token.transform.localPosition = pos;
                        tile.token.SetPos(ix,iz);
                        tile.token.SetRot(angle);
                    }
                }
            }

            result.tileMap = tileMap;

            return result;
        }
    }
}