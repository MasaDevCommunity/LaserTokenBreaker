using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App
{
    public class DefenderToken : TokenBase
    {
        public override void OnAwake()
        {
            tokenType = TokenType.Defender;
        }

        public override bool IsSelectable()
        {
            return true;
        }

        public override bool IsDead(TokenDir laserDir)
        {
            switch (dir)
            {
                case TokenDir.Up:
                    return laserDir != TokenDir.Down;
                case TokenDir.Right:
                    return laserDir != TokenDir.Left;
                case TokenDir.Down:
                    return laserDir != TokenDir.Up;
                case TokenDir.Left:
                    return laserDir != TokenDir.Right;
            }
            return base.IsDead(laserDir);
        }
    }
}