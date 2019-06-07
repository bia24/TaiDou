using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TaidouCommon.Model;


public class EnemyCreateModel {
    public string Name { get; set; }
    public string Guid { get; set; }
    public Position Position { get; set; }


    public Vector3 GetPosition()
    {
        Vector3 position = new Vector3((float)Position.x, (float)Position.y, (float)Position.z);
        return position;
    }

}



