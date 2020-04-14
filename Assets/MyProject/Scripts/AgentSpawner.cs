using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AgentSpawner : MonoBehaviour
{
    [SerializeField]
    private int _numAgents;

    [SerializeField]
    private GameObject _carPrefab;

    [SerializeField]
    private TextMeshPro _lapTimeText;

    private float _bestTime;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < _numAgents; i++)
        {
            Instantiate(_carPrefab, transform.position, Quaternion.identity, transform);
        }
    }

    public void AddNewLapTime(float pTime)
    {
        if(pTime < _bestTime || _bestTime == 0)
        {
            _bestTime = pTime;
            _lapTimeText.text = _bestTime.ToString("0.00");
        }
        else
        {
            Debug.Log("current time = " + _bestTime + ", parameter = " + pTime);
        }
    }
}
