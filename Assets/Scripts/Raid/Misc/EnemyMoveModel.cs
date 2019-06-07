using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TaidouCommon.Model;

public class EnemyMoveModel {
    public string Guid { get; set; }
	public Position Position { get; set; }
    public Euler Euler { get; set; }

    public Vector3 GetPosition()
    {
        return new Vector3((float)Position.x, (float)Position.y, (float)Position.z);
    }
    public Vector3 GetEuler()
    {
        return new Vector3((float)Euler.x, (float)Euler.y, (float)Euler.z);
    }
}
