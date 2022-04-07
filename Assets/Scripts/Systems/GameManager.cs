using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace App
{
    /// <summary>
    /// GameManager
    /// </summary>
    public class GameManager : Common.Singleton<GameManager>
    {
        // 盤面
        private const uint BoardX = 10;
        private const uint BoardZ = 8;

        /// <summary>
        /// 0 : None
        /// 1 : Red
        /// 2 : Blue
        /// </summary>
        private int[,] TileTip = new int[8, 10]
        {
            {1,2,0,0,0,0,0,0,1,2},
            {1,0,0,0,0,0,0,0,0,2},
            {1,0,0,0,0,0,0,0,0,2},
            {1,0,0,0,0,0,0,0,0,2},
            {1,0,0,0,0,0,0,0,0,2},
            {1,0,0,0,0,0,0,0,0,2},
            {1,0,0,0,0,0,0,0,0,2},
            {1,2,0,0,0,0,0,0,1,2}
        };

        

        /// <summary>
        /// 赤 Red
        /// </summary>
        private readonly int[,,] RedTokenTip = new int[2, 8, 10]
        {
            /// <summary>
            /// チェスの種類
            /// 0 : None
            /// 1 : Laser
            /// 2 : King
            /// 3 : Deflector
            /// 4 : Defender
            /// 5 : Switch
            /// </summary>
            {
                {1,0,0,0,4,2,4,3,0,0},
                {0,0,3,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {3,0,0,0,5,5,0,3,0,0},
                {3,0,0,0,0,0,0,3,0,0},
                {0,0,0,0,0,0,3,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0}
            },
            /// <summary>
            /// チェスの向き Forward
            /// 0 : Default(0)
            /// 1 : 0       ↑
            /// 2 : 90      ←
            /// 3 : 180     ↓
            /// 4 : 270     →
            /// </summary>
            {
                {3,0,0,0,3,1,3,3,0,0},
                {0,0,2,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {4,0,0,0,2,3,0,3,0,0},
                {3,0,0,0,0,0,0,4,0,0},
                {0,0,0,0,0,0,3,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0}
            }
        };

        /// <summary>
        /// 青 Blue
        /// </summary>
        private readonly int[,,] BlueTokenTip = new int[2, 8, 10]
        {
            /// <summary>
            /// チェスの種類
            /// 0 : None
            /// 1 : Laser
            /// 2 : King
            /// 3 : Deflector
            /// 4 : Defender
            /// 5 : Switch
            /// </summary>
            {
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,3,0,0,0,0,0,0},
                {0,0,3,0,0,0,0,0,0,3},
                {0,0,3,0,5,5,0,0,0,3},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,3,0,0},
                {0,0,3,4,2,4,0,0,0,1}
            },

            /// <summary>
            /// チェスの向き Forward
            /// 0 : Default
            /// 1 : 0       ↑
            /// 2 : 90      ←
            /// 3 : 180     ↓
            /// 4 : 270     →
            /// </summary>
            {
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,1,0,0,0,0,0,0},
                {0,0,2,0,0,0,0,0,0,1},
                {0,0,1,0,3,2,0,0,0,2},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,4,0,0},
                {0,0,1,1,1,1,0,0,0,1}
            }
        };

        /// <summary>
        /// マップ情報
        /// </summary>
        [Serializable]
        public class TileInfo
        {
            public TeamType teamType = TeamType.None;

            // 配置されているピース
            public Token token;
        }
        private TileInfo[] TileMap;

        /// <summary>
        /// タイルのプレハブ
        /// </summary>
        [Serializable]
        public class TilePrefabs
        {
            public GameObject None;
            public GameObject Red;
            public GameObject Blue;

            public GameObject this[TeamType type]
            {
                get
                {
                    switch (type)
                    {
                        case TeamType.None:
                            return None;
                        case TeamType.Red:
                            return Red;
                        case TeamType.Blue:
                            return Blue;
                    }
                    return null;
                }
            }
        }
        [SerializeField]
        private TilePrefabs tilePrefabs;

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
        [SerializeField]
        private TokenPrefabs tokenPrefabs;


        void Start()
        {
            TileMap = new TileInfo[BoardX * BoardZ];
            var TileFolder = new GameObject("Tiles");
            var TokenFolder = new GameObject("Tokens");

            var offset = new Vector3(0, 0, 0);

            for(uint iz = 0; iz < BoardZ; ++iz)
            {
                for(uint ix = 0; ix < BoardX; ++ix)
                {
                    var pos = offset + new Vector3(ix,0,-iz);
                    int angle = 1;
                    var tile = TileMap[(iz * BoardX) + ix] = new TileInfo();

                    // 陣地
                    tile.teamType = (TeamType)TileTip[iz, ix];

                    var tileObj = GameObject.Instantiate(tilePrefabs[tile.teamType],TileFolder.transform);
                    tileObj.transform.localPosition = pos; 

                    // Red
                    {
                        TokenType tokenType = (TokenType)RedTokenTip[0, iz, ix];
                        if (tokenType != TokenType.None)
                        {
                            var gobj = GameObject.Instantiate(tokenPrefabs[tokenType],TokenFolder.transform);
                            var token = gobj.GetComponent<Token>();
                            token.teamType = TeamType.Red;
                            tile.token = token;

                            angle = RedTokenTip[1, iz, ix];
                        }
                    }
                    // Blue
                    {
                        TokenType tokenType = (TokenType)BlueTokenTip[0, iz, ix];
                        if (tokenType != TokenType.None)
                        {
                            var gobj = GameObject.Instantiate(tokenPrefabs[tokenType],TokenFolder.transform);
                            var token = gobj.GetComponent<Token>();
                            token.teamType = TeamType.Blue;
                            tile.token = token;

                            angle = BlueTokenTip[1, iz, ix];
                        }
                    }

                    // Transform設定
                    if(tile.token != null)
                    {
                        tile.token.transform.localPosition = pos;

                        angle = Mathf.Clamp(angle - 1,0,4);
                        tile.token.transform.localRotation = Quaternion.AngleAxis(-90f * angle, Vector3.up);
                    }
                }
            }
        }

#if UNITY_EDITOR

        [ContextMenu("Make mesh")]
        public void MakeMesh()
        {
            var mesh = GetComponent<MeshFilter>();
            AssetDatabase.CreateAsset(mesh.mesh, "Assets/map.asset");
            AssetDatabase.SaveAssets();
        }

#endif
    }
}

