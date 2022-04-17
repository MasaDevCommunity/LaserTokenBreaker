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
            Start,      // ゲーム開始
            Idle,       // ゲーム中
            End         // ゲーム終了
        }
        private GameState gameState;

        public enum TurnState
        {
            Start,          // ターン   開始

            TokenSelect,    // トークン 選択
            TokenMove,      // トークン 移動
            TokenSet,       // トークン 決定

            LaserMove,      // レーザー 移動
            LaserSet,       // レーザー 決定
            LaserOn,        // レーザー 発射
            LaserEnd,       // レーザー 終了

            End,            // ターン   終了
        }
        private TurnState turnState;

        [SerializeField]
        private Team turnTeam;

        private BoardMap boardMap;

        // カーソル位置
        [SerializeField]
        private int CursorX;
        [SerializeField]
        private int CursorZ;

        // Tokenの選択開始位置
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
                    Debug.LogError("予測していない状態");
                    break;
            }
        }

        #region GameState

        private void GameStart()
        {
            // 開始チーム
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
                    Debug.LogError("予測していない状態");
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
                Debug.Log("そのトークンは選択できません。");
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

            // 回転
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

            // 決定
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
                    // トークンが破壊
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
                    // キングに触れた
                    if (token.tokenType == TokenType.King)
                    {
                        if (token.team != Team.None)
                        {
                            // 倒れたキングの逆値
                            var winner = token.team == Team.Red ? Team.Blue : Team.Red;
                            Debug.Log($"Winner : { winner }");
                        }
                        // ゲーム終了
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
            // カーソル移動
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

            // 選択リクエスト
            if (Input.GetKeyUp(KeyCode.Return))
            {
                // トークン選択
                var tile = boardMap.GetTileInfo(CursorX, CursorZ);
                if (tile.token == null)
                    return false;

                // トークン選択 選択判定
                if (tile.token.IsSelectable(pickTeam))
                {
                    // トークン選択
                    selectedToken = tile.token;
                    tile.token = null;

                    // リセット用
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
                // 回転
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

            // カーソル位置のタイル
            var tile = boardMap.GetTileInfo(CursorX, CursorZ);

            int posX = CursorX;
            int posZ = CursorZ;

            // カーソル位置のタイルが有効
            if (tile.teamType != Team.None && tile.teamType != token.team)
            {
                posX = OriginX;
                posZ = OriginZ;
            }

            // トークンがある
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
            // 設置
            if (Input.GetKeyUp(KeyCode.Return))
            {
                swapToken = null;
                // 同じ位置の場合リセット扱い
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

            // リセット
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

