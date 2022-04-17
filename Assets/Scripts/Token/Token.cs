using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App
{
    // 駒タイプ
    public enum TokenType
    {
        None = 0,
        Laser,
        King,
        Deflector,
        Defender,
        Switch
    }

    // チーム
    public enum Team
    {
        None = 0,
        Red,
        Blue
    }

    public enum TokenDir
    {
        Up,         // ↑
        Left,       // ←
        Down,       // ↓
        Right,      // →
        Max
    }

    [Serializable]
    public class BoardPos
    {
        public int x;
        public int z;

        public BoardPos(int x, int z)
        {
            this.x = x;
            this.z = z;
        }
    }

    

    public abstract class TokenBase : MonoBehaviour
    {
        private TokenMatAppler _MatAppler;

        #region プロパティ

        public BoardPos pos
        {
            get => _Pos;
            set
            {
                if (_Pos != value)
                {
                    _Pos = value;
                    transform.localPosition = new Vector3(_Pos.x, 0, -_Pos.z);
                }
            }
        }
        private BoardPos _Pos;

        public TokenDir dir
        {
            get => _Dir;
            set
            {
                if(_Dir != value)
                {
                    _Dir = value;
                    transform.localRotation = Quaternion.AngleAxis(-90f * (int)_Dir, Vector3.up);
                }
            }
        }
        private TokenDir _Dir;

        public Team team
        {
            get => _Team;
            private set => _Team = value;
        }
        private Team _Team;

        public TokenType tokenType
        {
            get => _TokenType;
            protected set => _TokenType = value;
        }
        private TokenType _TokenType;

        #endregion

        private void Awake()
        {
            _Pos = new BoardPos(0, 0);
            _Team = Team.None;

            this.OnAwake();
        }

        public virtual void OnAwake()
        {
            
        }

        private void Start()
        {
            _MatAppler = this.GetComponent<TokenMatAppler>();

            this.OnStart();
        }

        public virtual void OnStart()
        {

        }

        public virtual bool IsDead(TokenDir laserDir)
        {
            return true;
        }

        public virtual TokenDir Reflect(TokenDir tokenDir)
        {
            return tokenDir;
        }

        public void SetPos(int x, int z)
        {
            pos = new BoardPos(x, z);
        }

        public void SetRot(int dir)
        {
            this.dir = (TokenDir)(dir % (int)TokenDir.Max);
        }

        public void SetTeam(Team team)
        {
            this.team = team;
            _MatAppler?.SetTeamMat();
        }

        public bool IsSelectable(Team team)
        {
            return IsSelectable() && EqualTeam(team);
        }

        public virtual bool IsSelectable()
        {
            return false;
        }


        public bool EqualTeam(Team team)
        {
            return this.team == team;
        }
    }
}