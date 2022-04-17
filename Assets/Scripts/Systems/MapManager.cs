using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App
{
    public class StageMap
    {
        private TileInfo[] _TileMap;

        public TileInfo GetTileInfo(int x, int z)
        {
            x = Mathf.Clamp(x, 0, MapTip.MapX);
            z = Mathf.Clamp(z, 0, MapTip.MapZ);
            return _TileMap[z * MapTip.MapX + x];
        }
    }
}