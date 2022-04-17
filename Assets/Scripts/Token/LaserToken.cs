using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App
{
    public class LaserToken : TokenBase
    {
        private LineRenderer _LineRenderer;

        public float laserHeight;
        public float laserWidth;
        public Material laserMat;

        public override void OnAwake()
        {
            tokenType = TokenType.Laser;
        }

        public override void OnStart()
        {
            _LineRenderer = this.GetComponent<LineRenderer>();
            _LineRenderer.material = laserMat;
            _LineRenderer.startWidth = laserWidth;
            _LineRenderer.endWidth = laserWidth;
        }

        public override bool IsDead(TokenDir laserDir)
        {
            return false;
        }

        public void LaserReset()
        {
            var pos = this.transform.position;
            pos.y += laserHeight;
            _LineRenderer.positionCount = 1;
            _LineRenderer.SetPosition(0,pos);
        }

        public void AddPoint(Vector3 pos)
        {
            var index = _LineRenderer.positionCount;
            _LineRenderer.positionCount = index + 1;
            pos.y += laserHeight;
            _LineRenderer.SetPosition(index,pos);
        }
    }
}