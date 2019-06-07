using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmModel  {
    public string playerName { get; set; }
    public string transationName { get; set; }
	public TransationType type { get; set; }
    public bool state { get; set; }
}

public enum TransationType
{
    Bool,
    Trigger
}

