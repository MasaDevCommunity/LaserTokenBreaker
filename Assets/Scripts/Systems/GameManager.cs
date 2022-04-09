using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace App
{
    [Serializable]
    public class TilePos
    {
        public int X;
        public int Z;
    }

    /// <summary>
    /// GameManager
    /// </summary>
    public class GameManager : Common.Singleton<GameManager>
    {
        // �Ֆ�
        public const int BoardX = 10;
        public const int BoardZ = 8;

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
        /// �� Red
        /// </summary>
        private readonly int[,,] RedTokenTip = new int[2, 8, 10]
        {
            /// <summary>
            /// �`�F�X�̎��
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
            /// �`�F�X�̌��� Forward
            /// 0 : Default(0)
            /// 1 : 0       ��
            /// 2 : 90      ��
            /// 3 : 180     ��
            /// 4 : 270     ��
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
        /// �� Blue
        /// </summary>
        private readonly int[,,] BlueTokenTip = new int[2, 8, 10]
        {
            /// <summary>
            /// �`�F�X�̎��
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
            /// �`�F�X�̌��� Forward
            /// 0 : Default
            /// 1 : 0       ��
            /// 2 : 90      ��
            /// 3 : 180     ��
            /// 4 : 270     ��
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
        /// �}�b�v���
        /// </summary>
        [Serializable]
        public class TileInfo
        {
            public TeamType teamType = TeamType.None;

            // �z�u����Ă���s�[�X
            public Token token;
        }
        private TileInfo[] TileMap;

        /// <summary>
        /// �^�C���̃v���n�u
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

        // �J�[�\���ʒu
        [SerializeField]
        private int CursorX;
        [SerializeField]
        private int CursorZ;

        // Token�̑I���J�n�ʒu
        private int OriginX;
        private int OriginZ;

        private Quaternion OriginRot;

        private Token selectedToken;

        public GameObject cursorObj;

        private void Start()
        {
            CursorX = 0;
            CursorZ = 0;

            MapSet();
        }

        private bool TokenSelect()
        {
            // �J�[�\���ړ�
            if (Input.GetKeyUp(KeyCode.S))
            {
                CursorZ = Mathf.Min(CursorZ + 1, BoardZ - 1);
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                CursorZ = Mathf.Max(CursorZ - 1, 0);
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                CursorX = Mathf.Min(CursorX + 1, BoardX - 1);
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                CursorX = Mathf.Max(CursorX - 1, 0);
            }

            // �I�����N�G�X�g
            if (Input.GetKeyUp(KeyCode.Return))
            {
                // �g�[�N���I��
                var tile = GetTileInfo(CursorX, CursorZ);
                if (tile.token == null)
                    return false;

                // �g�[�N���I�� �I�𔻒�
                switch (tile.token.tokenType)
                {
                    case TokenType.King:
                        // Keing�͑I���ł��Ȃ�
                        return false;
                    default:
                        break;
                }

                // �g�[�N���I�� �m��
                selectedToken = tile.token;
                tile.token = null;

                OriginX = CursorX;
                OriginZ = CursorZ;

                OriginRot = selectedToken.transform.localRotation;
            }
            return true;
        }

        private void TokenMove()
        {
            int oldZ = CursorZ;
            int oldX = CursorX;

            var token = selectedToken;
            {
                if (token.IsMove)
                {
                    if (Input.GetKeyUp(KeyCode.S))
                    {
                        CursorZ += 1;
                        if (CursorZ >= BoardZ)
                        {
                            CursorZ = BoardZ - 1;
                        }
                        if (CursorZ > token.PosZ + 1)
                        {
                            CursorZ = token.PosZ + 1;
                        }
                    }
                    if (Input.GetKeyUp(KeyCode.W))
                    {
                        CursorZ -= 1;
                        if (CursorZ < 0)
                        {
                            CursorZ = 0;
                        }
                        if (CursorZ < token.PosZ - 1)
                        {
                            CursorZ = token.PosZ - 1;
                        }
                    }
                    if (Input.GetKeyUp(KeyCode.D))
                    {
                        CursorX += 1;
                        if (CursorX >= BoardX)
                        {
                            CursorX = BoardX - 1;
                        }
                        if (CursorX > token.PosX + 1)
                        {
                            CursorX = token.PosX + 1;
                        }
                    }
                    if (Input.GetKeyUp(KeyCode.A))
                    {
                        CursorX -= 1;
                        if (CursorX < 0)
                        {
                            CursorX = 0;
                        }
                        if (CursorX < token.PosX - 1)
                        {
                            CursorX = token.PosX - 1;
                        }
                    }
                }

                if (Input.GetKeyUp(KeyCode.Q))
                {
                    token.transform.localRotation *= Quaternion.AngleAxis(-90f, Vector3.up);
                }
                if (Input.GetKeyUp(KeyCode.E))
                {
                    token.transform.localRotation *= Quaternion.AngleAxis(90f, Vector3.up);
                }
            }

            var tile = GetTileInfo(CursorX, CursorZ);
            // �^�C���F ����
            if (tile.teamType != TeamType.None && tile.teamType != token.teamType)
            {
                CursorX = oldX;
                CursorZ = oldZ;
            }

            if (tile.token != null)
            {
                switch (token.tokenType)
                {
                    case TokenType.Switch:
                        if (tile.token.tokenType == TokenType.Switch)
                        {
                            CursorX = oldX;
                            CursorZ = oldZ;
                        }
                        break;
                    default:
                        if (tile.token)
                        {
                            CursorX = oldX;
                            CursorZ = oldZ;
                        }
                        // �`�[������
                        if (tile.token.teamType != token.teamType)
                        {
                            CursorX = oldX;
                            CursorZ = oldZ;
                        }
                        break;
                }
            }

            token.transform.localPosition = new Vector3(CursorX, 0, -CursorZ);

            #region �ݒu/���Z�b�g

            // �ݒu
            if (Input.GetKeyUp(KeyCode.Return))
            {
                token.PosX = CursorX;
                token.PosZ = CursorZ;

                if (tile.token == null)
                {
                    tile.token = token;
                }
                else
                {
                    var swapTile = GetTileInfo(OriginX, OriginZ);
                    swapTile.token = tile.token;
                    swapTile.token.PosX = OriginX;
                    swapTile.token.PosZ = OriginZ;
                    swapTile.token.transform.localPosition = new Vector3(OriginX, 0, -OriginZ);

                    tile.token = token;
                }
                selectedToken = null;
            }

            // ���Z�b�g
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                token.transform.localPosition = new Vector3(OriginX, 0, -OriginZ);
                token.PosX = OriginX;
                token.PosZ = OriginZ;
                token.transform.localRotation = OriginRot;

                tile = GetTileInfo(OriginX, OriginZ);
                tile.token = token;

                selectedToken = null;
            }

            #endregion
        }

        private void Update()
        {
            if (selectedToken == null)
            {
                if (!TokenSelect())
                {
                    Debug.Log("���̃g�[�N���͑I���ł��܂���B");
                }
            }
            else
            {
                TokenMove();
            }

            if (cursorObj != null)
            {
                cursorObj.transform.position = GetTilePos(CursorX, CursorZ);
            }
        }

        private void MapSet()
        {
            TileMap = new TileInfo[BoardX * BoardZ];
            var TileFolder = new GameObject("Tiles");
            var TokenFolder = new GameObject("Tokens");

            var offset = new Vector3(0, 0, 0);

            for (int iz = 0; iz < BoardZ; ++iz)
            {
                for (int ix = 0; ix < BoardX; ++ix)
                {
                    var pos = offset + new Vector3(ix, 0, -iz);
                    int angle = 1;
                    var tile = TileMap[(iz * BoardX) + ix] = new TileInfo();

                    // �w�n
                    tile.teamType = (TeamType)TileTip[iz, ix];

                    var tileObj = GameObject.Instantiate(tilePrefabs[tile.teamType], TileFolder.transform);
                    tileObj.transform.localPosition = pos;

                    // Red
                    {
                        TokenType tokenType = (TokenType)RedTokenTip[0, iz, ix];
                        if (tokenType != TokenType.None)
                        {
                            var gobj = GameObject.Instantiate(tokenPrefabs[tokenType], TokenFolder.transform);
                            var token = gobj.GetComponent<Token>();
                            token.teamType = TeamType.Red;
                            token.tokenType = tokenType;
                            tile.token = token;

                            angle = RedTokenTip[1, iz, ix];
                        }
                    }

                    // Blue
                    {
                        TokenType tokenType = (TokenType)BlueTokenTip[0, iz, ix];
                        if (tokenType != TokenType.None)
                        {
                            var gobj = GameObject.Instantiate(tokenPrefabs[tokenType], TokenFolder.transform);
                            var token = gobj.GetComponent<Token>();
                            token.teamType = TeamType.Blue;
                            token.tokenType = tokenType;
                            tile.token = token;

                            angle = BlueTokenTip[1, iz, ix];
                        }
                    }

                    // Transform�ݒ�
                    if (tile.token != null)
                    {
                        tile.token.PosX = ix;
                        tile.token.PosZ = iz;

                        tile.token.transform.localPosition = pos;

                        angle = Mathf.Clamp(angle - 1, 0, 4);
                        tile.token.transform.localRotation = Quaternion.AngleAxis(-90f * angle, Vector3.up);

                        if (tile.token.tokenType == TokenType.Laser)
                        {
                            tile.token.IsMove = false;
                        }
                        else
                        {
                            tile.token.IsMove = true;
                        }
                    }
                }
            }
        }

        public TileInfo GetTileInfo(int x, int z)
        {
            x = Mathf.Clamp(x, 0, BoardX);
            z = Mathf.Clamp(z, 0, BoardZ);
            return TileMap[z * BoardX + x];
        }

        public Vector3 GetTilePos(int x, int z)
        {
            return new Vector3(x, 0, -z);
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

