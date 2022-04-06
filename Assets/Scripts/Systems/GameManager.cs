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
        //void TestB()
        //{
        //    var filter = gameObject.GetComponent<MeshFilter>();

        //    var mesh = new Mesh();

        //    var cellScale = new Vector3(1, 0, 1);

        //    var offset = new Vector3(
        //        cellScale.x * BoardX,
        //        0,
        //        cellScale.z * BoardZ
        //    );
        //    offset *= -0.5f;

        //    var vertices = new List<Vector3>();
        //    var triangles = new List<int>();
        //    var normals = new List<Vector3>();
        //    var UVs = new List<Vector2>();

        //    var colors = new List<Color>();

        //    int count = 0;
        //    for (int ix = 0; ix < BoardX; ++ix)
        //    {
        //        for (int iz = 0; iz < BoardZ; ++iz)
        //        {
        //            var info = _boardMap[(ix * BoardZ) + iz];

        //            var vertex = new Vector3[4];
        //            {
        //                vertex[0].x = ix * cellScale.x;
        //                vertex[0].y = 0;
        //                vertex[0].z = iz * cellScale.z;
        //                vertex[0] += offset;
        //                colors.Add(Color.black);

        //                vertex[1].x = (ix + 1) * cellScale.x;
        //                vertex[1].y = 0;
        //                vertex[1].z = iz * cellScale.z;
        //                vertex[1] += offset;
        //                colors.Add(Color.black);

        //                vertex[2].x = ix * cellScale.x;
        //                vertex[2].y = 0;
        //                vertex[2].z = (iz + 1) * cellScale.z;
        //                vertex[2] += offset;
        //                colors.Add(Color.black);

        //                vertex[3].x = (ix + 1) * cellScale.x;
        //                vertex[3].y = 0;
        //                vertex[3].z = (iz + 1) * cellScale.z;
        //                vertex[3] += offset;
        //                colors.Add(Color.black);
        //            }
        //            vertices.AddRange(vertex);

        //            var indices = new int[6];
        //            {
        //                indices[0] = count + 2;
        //                indices[1] = count + 3;
        //                indices[2] = count + 0;

        //                indices[3] = count + 3;
        //                indices[4] = count + 1;
        //                indices[5] = count + 0;

        //                count += 4;
        //            }
        //            triangles.AddRange(indices);

        //            var normal = new Vector3[4];
        //            {
        //                normal[0] = Vector3.up;
        //                normal[1] = Vector3.up;
        //                normal[2] = Vector3.up;
        //                normal[3] = Vector3.up;
        //            }
        //            normals.AddRange(normal);

        //            // 床のタイプ毎にUV設定する
        //            switch (info.cellType)
        //            {
        //                case CellType.None:
        //                    UVs.AddRange(new Vector2[] {
        //                        new Vector2(0,0),
        //                        new Vector2(0.25f,0),
        //                        new Vector2(0,0.25f),
        //                        new Vector2(0.25f,0.25f)
        //                    });
        //                    break;
        //                case CellType.Red:
        //                    UVs.AddRange(new Vector2[] {
        //                        new Vector2(0.25f,0),
        //                        new Vector2(0.5f,0),
        //                        new Vector2(0,0.25f),
        //                        new Vector2(0.5f,0.25f)
        //                    });
        //                    break;
        //                case CellType.Bule:
        //                    UVs.AddRange(new Vector2[] {
        //                        new Vector2(0.5f,0),
        //                        new Vector2(0.75f,0),
        //                        new Vector2(0,0.25f),
        //                        new Vector2(0.75f,0.25f)
        //                    });
        //                    break;
        //                default:
        //                    UVs.AddRange(new Vector2[] {
        //                        new Vector2(0,0),
        //                        new Vector2(0.25f,0),
        //                        new Vector2(0,0.25f),
        //                        new Vector2(0.25f,0.25f)
        //                    });
        //                    break;
        //            }
        //        }
        //    }
        //    mesh.Clear();
        //    mesh.SetVertices(vertices);
        //    mesh.SetTriangles(triangles, 0);
        //    mesh.SetNormals(normals);
        //    mesh.SetUVs(0, UVs);
        //    mesh.SetColors(colors);

        //    filter.mesh = mesh;
        //}

        //void TestA()
        //{
        //    var boardMap = new GameObject("BoardMap");
        //    var pieces = new GameObject("Pieces");

        //    float cell_scale = 1.0f;

        //    Vector3 offset = Vector3.zero;
        //    offset.x = -(cell_scale * BoardY) * 0.5f;
        //    offset.z = (cell_scale * BoardX) * 0.5f;

        //    Vector3 s = offset;
        //    float cell_scale2 = cell_scale * 0.5f;
        //    uint count = 0;
        //    for (uint iy = 0; iy < BoardY; iy++)
        //    {
        //        for (uint ix = 0; ix < BoardX; ix++)
        //        {
        //            CellInfo cell = _boardMap[count];
        //            if (cell.cellType < CellType.Max)
        //            {
        //                GameObject gobj = GameObject.Instantiate(cellPrefabs[(int)cell.cellType].cellObject, boardMap.transform);
        //                gobj.transform.position = offset;
        //            }

        //            if (cell.pieceType < Piece.Type.Max)
        //            {
        //                GameObject gobj = GameObject.Instantiate(piecePrefabs[(int)cell.pieceType].pieceObject, pieces.transform);
        //                gobj.transform.position = offset;
        //            }


        //            offset.x += cell_scale;
        //            count++;
        //        }
        //        offset.x = s.x;
        //        offset.z -= cell_scale;
        //    }
        //}
    }
}

