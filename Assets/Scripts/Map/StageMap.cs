using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App
{
    /// <summary>
    /// �Ֆʃf�[�^
    /// </summary>
    public static class MapTip
    {
        // �}�b�v�^�C����
        public const int MapX = 10;
        public const int MapZ = 8;

        /// <summary>
        /// �Ֆʂ̐F
        /// 0 : None
        /// 1 : Red
        /// 2 : Blue
        /// </summary>
        public static readonly int[,] TileTip = new int[MapZ, MapX]
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
        public static readonly int[,,] RedTokenTip = new int[2, MapZ, MapX]
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
        public static readonly int[,,] BlueTokenTip = new int[2, MapZ, MapX]
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
            /// 0 : Default(1)
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
    }

}