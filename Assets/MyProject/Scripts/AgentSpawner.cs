
using Barracuda;
using MLAgents;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


[CustomPropertyDrawer(typeof(AgentSpawnerParameters))]
public class AgentSpawnerDrawer : PropertyDrawer
{
    AgentSpawnerParameters _agentSpawnerParameters;
    float lineHeight = 17;
    private bool toogleValue;


    private void LazyInitializeHub(SerializedProperty property, GUIContent label)
    {
        if (_agentSpawnerParameters != null)
        {
            return;
        }
        var target = property.serializedObject.targetObject;
        _agentSpawnerParameters = fieldInfo.GetValue(target) as AgentSpawnerParameters;
        if (_agentSpawnerParameters == null)
        {
            _agentSpawnerParameters = new AgentSpawnerParameters();
        }
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
        {
            return lineHeight * 15;
        }
        return lineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        LazyInitializeHub(property, label);
        var indent = EditorGUI.indentLevel; 
        position.height = lineHeight;
        EditorGUI.indentLevel = 0;
        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);


        if (property.isExpanded)
        {
            EditorGUI.BeginProperty(position, label, property);
            position.y += lineHeight;
            var widthEighth = position.width / 8;
            EditorGUI.BeginChangeCheck();
            _agentSpawnerParameters.NumAgents = EditorGUI.IntField(position, "Number of clones:", _agentSpawnerParameters.NumAgents);
            if (EditorGUI.EndChangeCheck())
            {
                if(_agentSpawnerParameters.NumAgents > _agentSpawnerParameters.MaterialsList.Count)
                {
                    int elementsDifference = _agentSpawnerParameters.NumAgents - _agentSpawnerParameters.MaterialsList.Count;
                    for (int i = 0; i < elementsDifference; i++)
                    {
                        _agentSpawnerParameters.ModelsList.Add(null);
                        _agentSpawnerParameters.MaterialsList.Add(null);
                    }
                }
                else if(_agentSpawnerParameters.NumAgents < _agentSpawnerParameters.MaterialsList.Count)
                {
                    int elementsDifference = _agentSpawnerParameters.MaterialsList.Count - _agentSpawnerParameters.NumAgents;
                    int listsCount = _agentSpawnerParameters.MaterialsList.Count;
                    for (int i = 1; i <= elementsDifference; i++)
                    {
                        int index = listsCount - i;
                        _agentSpawnerParameters.ModelsList.RemoveAt(index);
                        _agentSpawnerParameters.MaterialsList.RemoveAt(index);
                    }
                }
                MarkSceneAsDirty();
            }
            position.y += lineHeight;
            EditorGUI.BeginChangeCheck();
            _agentSpawnerParameters.UsingModel = GUI.Toggle(new Rect(position.x, position.y, 3 * widthEighth, position.height), _agentSpawnerParameters.UsingModel, "Using Model?");
            if (EditorGUI.EndChangeCheck())
            {
                MarkSceneAsDirty();
            }

            position.y += lineHeight;

            if (_agentSpawnerParameters.UsingModel)
            {
                EditorGUI.BeginChangeCheck();

                for (int i = 0; i < _agentSpawnerParameters.NumAgents; i++)
                {
                    _agentSpawnerParameters.ModelsList[i] = EditorGUI.ObjectField(new Rect(position.x, position.y, 3 * widthEighth, position.height), _agentSpawnerParameters.ModelsList[i], typeof(NNModel), true) as NNModel;
                    _agentSpawnerParameters.MaterialsList[i] = EditorGUI.ObjectField(new Rect(position.x + 3 * widthEighth, position.y, 3 * widthEighth, position.height), _agentSpawnerParameters.MaterialsList[i], typeof(Material), true) as Material;

                    position.y += lineHeight;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    MarkSceneAsDirty();
                }
            }
            EditorGUI.EndProperty();

        }
        EditorGUI.indentLevel = indent;

    }

    private void MarkSceneAsDirty()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}

[System.Serializable]
public class AgentSpawnerParameters
{
    public int NumAgents;

    [SerializeField]
    public bool UsingModel;

    public List<NNModel> ModelsList = new List<NNModel>();
    public List<Material> MaterialsList = new List<Material>();
}

public class AgentSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _carPrefab;

    [SerializeField]
    private TextMeshPro _lapTimeText;

    private float _bestTime;

    [SerializeField]
    private AgentSpawnerParameters _agentSpawnerDrawerManager;


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < _agentSpawnerDrawerManager.NumAgents; i++)
        {
            GameObject carObject = Instantiate(_carPrefab, transform.position, Quaternion.identity, transform);


            Material customMaterial = _agentSpawnerDrawerManager.MaterialsList[i];
            if (customMaterial != null)
            {
                carObject.GetComponent<MeshRenderer>().material = customMaterial;
            }

            CarAgent carAgent = carObject.GetComponent<CarAgent>();
            NNModel customModel = _agentSpawnerDrawerManager.ModelsList[i];
            if (carAgent != null && customModel != null)
            {
                LearningBrain customBrain = new LearningBrain();
                customBrain.brainParameters = carAgent.brain.brainParameters;
                customBrain.model = customModel;
                carAgent.brain = customBrain;
            }
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

