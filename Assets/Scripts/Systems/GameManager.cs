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
        public enum GameState
        {
            Start,      // �Q�[���J�n
            Idle,       // �Q�[����
            End         // �Q�[���I��
        }
        private GameState gameState;

        public enum TurnState
        {
            Start,          // �^�[��   �J�n

            TokenSelect,    // �g�[�N�� �I��
            TokenMove,      // �g�[�N�� �ړ�
            TokenSet,       // �g�[�N�� ����

            LaserMove,      // ���[�U�[ �ړ�
            LaserSet,       // ���[�U�[ ����
            LaserOn,        // ���[�U�[ ����
            LaserEnd,       // ���[�U�[ �I��

            End,            // �^�[��   �I��
        }
        private TurnState turnState;

        [SerializeField]
        private Team turnTeam;

        private BoardMap boardMap;

        // �J�[�\���ʒu
        [SerializeField]
        private int CursorX;
        [SerializeField]
        private int CursorZ;

        // Token�̑I���J�n�ʒu
        private int OriginX;
        private int OriginZ;

        private int OriginDir;

        private TokenBase selectedToken;
        private TokenBase swapToken;

        public GameObject cursorObj;



        private void Start()
        {
            CursorX = 0;
            CursorZ = 0;

            var generator = this.GetComponent<MapGenerator>();
            boardMap = generator.Generate();
            gameState = GameState.Start;
        }

        private void Update()
        {
            switch (gameState)
            {
                case GameState.Start:
                    GameStart();
                    break;
                case GameState.Idle:
                    GameIdle();
                    break;
                case GameState.End:
                    GameEnd();
                    break;
                default:
                    Debug.LogError("�\�����Ă��Ȃ����");
                    break;
            }
        }

        #region GameState

        private void GameStart()
        {
            // �J�n�`�[��
            turnTeam = (Team)Random.Range(1, 3);

            turnState = TurnState.Start;
            gameState = GameState.Idle;
            Debug.Log("---- GameStart ----");
        }

        private void GameIdle()
        {
            switch (turnState)
            {
                case TurnState.Start:
                    TurnStart();
                    break;
                case TurnState.TokenSelect:
                    TurnTokenSelect();
                    break;
                case TurnState.TokenMove:
                    TurnTokenMove();
                    break;
                case TurnState.TokenSet:
                    TurnTokenSet();
                    break;
                case TurnState.LaserMove:
                    TurnLaserMove();
                    break;
                case TurnState.LaserSet:
                    TurnLaserSet();
                    break;
                case TurnState.LaserOn:
                    TurnLaserOn();
                    break;
                case TurnState.LaserEnd:
                    TurnLaserEnd();
                    break;
                case TurnState.End:
                    TurnEnd();
                    break;
                default:
                    Debug.LogError("�\�����Ă��Ȃ����");
                    break;
            }
        }

        private void GameEnd()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                gameState = GameState.Start;
            }
        }

        #endregion


        private void TurnStart()
        {
            turnState = TurnState.TokenSelect;

            var token = boardMap.lasers[turnTeam];
            CursorX = token.pos.x;
            CursorZ = token.pos.z;
            SyncCursor();

            Debug.Log($"--- Turn Start : { turnTeam } ---");
        }

        private void TurnTokenSelect()
        {
            if (TokenSelect(turnTeam))
            {
                if (selectedToken != null)
                {
                    turnState = TurnState.TokenMove;
                }
            }
            else
            {
                Debug.Log("���̃g�[�N���͑I���ł��܂���B");
            }

            SyncCursor();
        }

        private void TurnTokenMove()
        {
            TokenMove();

            SyncCursor();

            if (turnState != TurnState.TokenMove)
            {
                selectedToken = null;
            }
        }

        private void TurnTokenSet()
        {
            turnState = TurnState.LaserMove;

            var token = boardMap.lasers[turnTeam];
            CursorX = token.pos.x;
            CursorZ = token.pos.z;

            SyncCursor();
        }

        private void TurnLaserMove()
        {
            var token = boardMap.lasers[turnTeam];
            var tokenDir = token.dir;

            // ��]
            if (Input.GetKeyUp(KeyCode.Q))
            {
                tokenDir = (TokenDir)(((int)tokenDir - 1 + (int)TokenDir.Max) % (int)TokenDir.Max);
            }
            if (Input.GetKeyUp(KeyCode.E))
            {
                tokenDir = (TokenDir)(((int)tokenDir + 1) % ((int)TokenDir.Max));
            }

            bool outRange = false;
            switch (tokenDir)
            {
                case TokenDir.Up:
                    outRange = boardMap.IsOutOfBoard(token.pos.x, token.pos.z - 1);
                    break;
                case TokenDir.Left:
                    outRange = boardMap.IsOutOfBoard(token.pos.x - 1, token.pos.z);
                    break;
                case TokenDir.Right:
                    outRange = boardMap.IsOutOfBoard(token.pos.x + 1, token.pos.z);
                    break;
                case TokenDir.Down:
                    outRange = boardMap.IsOutOfBoard(token.pos.x, token.pos.z + 1);
                    break;
            }

            if (outRange)
            {
                tokenDir = token.dir;
            }

            token.dir = tokenDir;

            // ����
            if (Input.GetKeyUp(KeyCode.Return))
            {
                turnState = TurnState.LaserSet;
            }
        }

        private void TurnLaserSet()
        {
            turnState = TurnState.LaserOn;

            breakToken = null;
            boardMap.lasers[turnTeam].LaserReset();
        }

        private TokenBase breakToken = null;

        private void TurnLaserOn()
        {
            var laser = boardMap.lasers[turnTeam];

            var laserX = laser.pos.x;
            var laserZ = laser.pos.z;

            var laserDir = laser.dir;
            while (true)
            {
                switch (laserDir)
                {
                    case TokenDir.Up:
                        laserZ--;
                        break;
                    case TokenDir.Left:
                        laserX--;
                        break;
                    case TokenDir.Right:
                        laserX++;
                        break;
                    case TokenDir.Down:
                        laserZ++;
                        break;
                }

                laser.AddPoint(new Vector3(laserX, laser.transform.position.y, -laserZ));

                if (boardMap.IsOutOfBoard(laserX, laserZ))
                {
                    break;
                }

                var tile = boardMap.GetTileInfo(laserX, laserZ);
                if (tile.token != null)
                {
                    // �g�[�N�����j��
                    if (tile.token.IsDead(laserDir))
                    {
                        laser.AddPoint(tile.token.transform.position);

                        breakToken = tile.token;
                        tile.token = null;
                        break;
                    }

                    laserDir = tile.token.Reflect(laserDir);
                }
            }

            turnState = TurnState.LaserEnd;
        }

        private void TurnLaserEnd()
        {
            if (Input.GetKeyUp(KeyCode.Return))
            {
                var token = breakToken;
                if (token != null)
                {
                    // �L���O�ɐG�ꂽ
                    if (token.tokenType == TokenType.King)
                    {
                        if (token.team != Team.None)
                        {
                            // �|�ꂽ�L���O�̋t�l
                            var winner = token.team == Team.Red ? Team.Blue : Team.Red;
                            Debug.Log($"Winner : { winner }");
                        }
                        // �Q�[���I��
                        gameState = GameState.End;
                        Debug.Log("---- Game End ----");
                    }
                    GameObject.Destroy(token.gameObject);
                }

                boardMap.lasers[turnTeam].LaserReset();
                turnState = TurnState.End;
            }
        }

        private void TurnEnd()
        {
            if (turnTeam == Team.Red)
            {
                turnTeam = Team.Blue;
            }
            else if (turnTeam == Team.Blue)
            {
                turnTeam = Team.Red;
            }

            turnState = TurnState.Start;
            Debug.Log("--- Turn End ---");
        }

        private void SyncCursor()
        {
            if (cursorObj != null)
            {
                cursorObj.transform.position = new Vector3(CursorX, 0, -CursorZ);
            }
        }



        private bool TokenSelect(Team pickTeam)
        {
            // �J�[�\���ړ�
            if (Input.GetKeyUp(KeyCode.S))
            {
                CursorZ = Mathf.Min(CursorZ + 1, MapTip.MapZ - 1);
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                CursorZ = Mathf.Max(CursorZ - 1, 0);
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                CursorX = Mathf.Min(CursorX + 1, MapTip.MapX - 1);
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                CursorX = Mathf.Max(CursorX - 1, 0);
            }

            // �I�����N�G�X�g
            if (Input.GetKeyUp(KeyCode.Return))
            {
                // �g�[�N���I��
                var tile = boardMap.GetTileInfo(CursorX, CursorZ);
                if (tile.token == null)
                    return false;

                // �g�[�N���I�� �I�𔻒�
                if (tile.token.IsSelectable(pickTeam))
                {
                    // �g�[�N���I��
                    selectedToken = tile.token;
                    tile.token = null;

                    // ���Z�b�g�p
                    OriginX = CursorX;
                    OriginZ = CursorZ;
                    OriginDir = (int)selectedToken.dir;
                    MoveRot = 0;
                }
            }

            return true;
        }

        int MoveRot;

        private void TokenMove()
        {
            if (swapToken)
            {
                swapToken.transform.localPosition = new Vector3(swapToken.pos.x, 0, -swapToken.pos.z);
                swapToken = null;
            }

            var token = selectedToken;
            if (OriginX == CursorX && OriginZ == CursorZ)
            {
                // ��]
                if (Input.GetKeyUp(KeyCode.Q))
                {
                    MoveRot++;
                }
                if (Input.GetKeyUp(KeyCode.E))
                {
                    MoveRot--;
                }

                MoveRot = Mathf.Clamp(MoveRot, -1, 1);
                token.SetRot(OriginDir + MoveRot);
            }
            else
            {
                token.SetRot(OriginDir);
                MoveRot = 0;
            }

            if (Input.GetKeyUp(KeyCode.S))
            {
                CursorZ += 1;
                if (CursorZ >= MapTip.MapZ) CursorZ = MapTip.MapZ - 1;
                if (CursorZ > OriginZ + 1) CursorZ = OriginZ + 1;
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                CursorZ -= 1;
                if (CursorZ < 0) CursorZ = 0;
                if (CursorZ < OriginZ - 1) CursorZ = OriginZ - 1;
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                CursorX += 1;
                if (CursorX >= MapTip.MapX) CursorX = MapTip.MapX - 1;
                if (CursorX > OriginX + 1) CursorX = OriginX + 1;
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                CursorX -= 1;
                if (CursorX < 0) CursorX = 0;
                if (CursorX < OriginX - 1) CursorX = OriginX - 1;
            }

            // �J�[�\���ʒu�̃^�C��
            var tile = boardMap.GetTileInfo(CursorX, CursorZ);

            int posX = CursorX;
            int posZ = CursorZ;

            // �J�[�\���ʒu�̃^�C�����L��
            if (tile.teamType != Team.None && tile.teamType != token.team)
            {
                posX = OriginX;
                posZ = OriginZ;
            }

            // �g�[�N��������
            if (tile.token != null)
            {
                if (token is SwitchToken switcher)
                {
                    if (switcher.CanSwap(tile.token))
                    {
                        swapToken = tile.token;
                    }
                    else
                    {
                        posX = OriginX;
                        posZ = OriginZ;
                        swapToken = null;
                    }
                }
                else
                {
                    posX = OriginX;
                    posZ = OriginZ;
                }
            }

            // Preview
            {
                if (swapToken)
                {
                    swapToken.transform.localPosition = new Vector3(OriginX, 0, -OriginZ);
                }

                token.transform.localPosition = new Vector3(posX, 0, -posZ);
            }

            bool reset = false;
            // �ݒu
            if (Input.GetKeyUp(KeyCode.Return))
            {
                swapToken = null;
                // �����ʒu�̏ꍇ���Z�b�g����
                if (OriginX == posX && OriginZ == posZ && MoveRot == 0)
                {
                    reset = true;
                }
                else
                {
                    token.SetPos(posX, posZ);
                    token.SetRot(OriginDir + MoveRot);

                    if (tile.token == null)
                    {
                        tile.token = token;
                    }
                    else
                    {
                        var swapTile = boardMap.GetTileInfo(OriginX, OriginZ);
                        swapTile.token = tile.token;
                        swapTile.token.SetPos(OriginX, OriginZ);

                        tile.token = token;
                    }
                    MoveRot = 0;
                    turnState = TurnState.TokenSet;
                }
            }

            // ���Z�b�g
            if (reset || Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.R))
            {
                if (swapToken)
                {
                    swapToken.transform.localPosition = new Vector3(swapToken.pos.x, 0, -swapToken.pos.z);
                }
                swapToken = null;

                token.SetPos(OriginX, OriginZ);
                token.SetRot(OriginDir);

                tile = boardMap.GetTileInfo(OriginX, OriginZ);
                tile.token = token;

                turnState = TurnState.TokenSelect;
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

