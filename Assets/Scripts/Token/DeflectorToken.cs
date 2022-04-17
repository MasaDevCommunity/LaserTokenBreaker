using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App
{
    public class DeflectorToken : TokenBase
    {
        public override void OnAwake()
        {
            tokenType = TokenType.Deflector;
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
                    if (laserDir == TokenDir.Up) return true;
                    if (laserDir == TokenDir.Left) return true;
                    break;
                case TokenDir.Left:
                    if (laserDir == TokenDir.Down) return true;
                    if (laserDir == TokenDir.Left) return true;
                    break;
                case TokenDir.Down:
                    if (laserDir == TokenDir.Down) return true;
                    if (laserDir == TokenDir.Right) return true;
                    break;
                case TokenDir.Right:
                    if (laserDir == TokenDir.Up) return true;
                    if (laserDir == TokenDir.Right) return true;
                    break;
            }
            return false;
        }

        public override TokenDir Reflect(TokenDir laserDir)
        {
            switch (dir)
            {
                case TokenDir.Up:
                    return laserDir == TokenDir.Down ? TokenDir.Left : TokenDir.Up;
                case TokenDir.Left:
                    return laserDir == TokenDir.Right ? TokenDir.Down : TokenDir.Left;
                case TokenDir.Down:
                    return laserDir == TokenDir.Up ? TokenDir.Right : TokenDir.Down;
                case TokenDir.Right:
                    return laserDir == TokenDir.Left ? TokenDir.Up : TokenDir.Right;
            }
            return laserDir;
        }
    }
}