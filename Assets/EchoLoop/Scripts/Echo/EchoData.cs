using System.Collections.Generic;
using UnityEngine;

namespace EchoLoop.Echo
{
    [System.Serializable]
    public class EchoData
    {
        public List<Vector3> recordedPositions = new List<Vector3>();

        public void AddPosition(Vector3 position)
        {
            recordedPositions.Add(position);
        }

        public void Clear()
        {
            recordedPositions.Clear();
        }
    }
}
