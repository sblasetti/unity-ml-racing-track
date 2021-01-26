using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckpointsContainer : MonoBehaviour
{
    public List<Checkpoint> listOfCheckpoints;

    private void Start()
    {
        listOfCheckpoints = GetComponentsInChildren<Checkpoint>().OrderBy(x => x.Order).ToList();
    }
}
