using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StayingMythical
{
    public static playerController player {get{ return Object.FindObjectOfType<playerController>(); } }
    public static WorldGenerator WorldGenerator { get { return Object.FindObjectOfType<WorldGenerator>();  } }
    public static ExplorerManager ExplorerController { get { return Object.FindObjectOfType<ExplorerManager>(); } }
}
