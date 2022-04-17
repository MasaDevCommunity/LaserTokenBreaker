using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App
{
    public class KingToken : TokenBase
    {
        public override void OnAwake()
        {
            tokenType = TokenType.King;
        }

        public override bool IsSelectable()
        {
            return true;
        }
    }
}