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
    public enum TeamType
    {
        None = 0,
        Red,
        Blue
    }

    /// <summary>
    /// Token
    /// </summary>
    public class Token : MonoBehaviour
    {
        public int PosX;
        public int PosZ;

        [Serializable]
        public class TeamMats
        {
            public Material RedMat;
            public Material BlueMat;

            public Material this[TeamType type]
            {
                get
                {
                    switch (type)
                    {
                        case TeamType.Red:
                            return RedMat;
                        case TeamType.Blue:
                            return BlueMat;
                    }
                    return null;
                }
            }
        }

        public TeamType teamType
        {
            get => _TeamType;
            set
            {
                if (_TeamType != value)
                {
                    _TeamType = value;
                    if (_MeshRenderer != null)
                    {
                        var mat = _TeamMats[teamType];
                        if (mat != null)
                        {
                            _MeshRenderer.material.color = mat.color;
                        }
                    }
                }
            }
        }
        private TeamType _TeamType;

        public TokenType tokenType
        {
            get => _TokenType;
            set => _TokenType = value;
        }
        [SerializeField]
        private TokenType _TokenType;
        [SerializeField]
        private TeamMats _TeamMats;

        //　移動可能
        public bool IsMove;

        [SerializeField]
        private MeshRenderer _MeshRenderer;

        void Start()
        {
            if (_MeshRenderer == null)
            {
                _MeshRenderer = this.GetComponentInChildren<MeshRenderer>();
            }
        }

        void Update()
        {

        }
    }

}