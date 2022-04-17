using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App
{
    public class SwitchToken : TokenBase
    {
        public override void OnAwake()
        {
            tokenType = TokenType.Switch;
        }

        public override bool IsSelectable()
        {
            return true;
        }

        public bool CanSwap(TokenBase token)
        {
            switch (token.tokenType)
            {
                case TokenType.Deflector:
                case TokenType.Defender:
                    return true;
                default:
                    return false;
            }
        }

        public override bool IsDead(TokenDir laserDir)
        {
            return false;
        }

        public override TokenDir Reflect(TokenDir laserDir)
        {
            switch (dir)
            {
                case TokenDir.Up:
                    if (laserDir == TokenDir.Up) return TokenDir.Right;
                    if (laserDir == TokenDir.Left) return TokenDir.Down;
                    if (laserDir == TokenDir.Down) return TokenDir.Left;
                    if (laserDir == TokenDir.Right) return TokenDir.Up;
                    break;
                case TokenDir.Right:
                    if (laserDir == TokenDir.Up) return TokenDir.Left;
                    if (laserDir == TokenDir.Left) return TokenDir.Up;
                    if (laserDir == TokenDir.Down) return TokenDir.Right;
                    if (laserDir == TokenDir.Right) return TokenDir.Down;
                    break;
                case TokenDir.Down:
                    if (laserDir == TokenDir.Up) return TokenDir.Right;
                    if (laserDir == TokenDir.Right) return TokenDir.Up;
                    if (laserDir == TokenDir.Down) return TokenDir.Left;
                    if (laserDir == TokenDir.Left) return TokenDir.Down;
                    break;
                case TokenDir.Left:
                    if (laserDir == TokenDir.Up) return TokenDir.Left;
                    if (laserDir == TokenDir.Right) return TokenDir.Down;
                    if (laserDir == TokenDir.Down) return TokenDir.Right;
                    if (laserDir == TokenDir.Left) return TokenDir.Up;
                    break;
            }
            return laserDir;
        }
    }
}
