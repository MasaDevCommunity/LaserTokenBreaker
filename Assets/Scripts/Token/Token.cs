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
                if(_TeamType != value)
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

        [SerializeField]
        private TokenType _TokenType;

        [SerializeField]
        private bool _isUserable;

        [SerializeField]
        private TeamMats _TeamMats;

        [SerializeField]
        private MeshRenderer _MeshRenderer;

        void Start()
        {
            _isUserable = false;
            if(_MeshRenderer == null)
            {
                _MeshRenderer = this.GetComponentInChildren<MeshRenderer>();
            }
        }

        void Update()
        {
            if (_isUserable)
            {
                UpdateKeyState();
            }
        }

        void UpdateKeyState()
        {
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                transform.position += Vector3.forward;
                return;
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                transform.position += Vector3.back;
                return;
            }

            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                transform.position += Vector3.left;
                return;
            }

            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                transform.position += Vector3.right;
                return;
            }
        }
    }

}