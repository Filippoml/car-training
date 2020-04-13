using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSpawner : MonoBehaviour
{
    [SerializeField]
    private int _numAgents;

    [SerializeField]
    private GameObject _carPrefab;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < _numAgents; i++)
        {
            Instantiate(_carPrefab, transform.position, Quaternion.identity, transform);
        }
    }
}
