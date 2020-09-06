using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileSweeper
{
    public interface IGameplayLogic
    {
        Vector2 FindItemCell(Vector2 position);
        int DistanceToNearestItemCell(CellMono cell);
    }
}
