using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;

public class ProtesterManager : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float meetingPointReachedDistance = 2f;
    [SerializeField] private bool activateSeekBehaviour;
    [SerializeField] private npcAI npcai;
    //[SerializeField] private float noiseMaxRange = 10f;
    //[SerializeField] private float noiseMagnitude = .1f;

    private int currentFlowFieldIndex;
    private List<FlowFieldData> flowFieldsData;
    private Transform endOfProtest;
    private Vector3 moveDirectionSteering;

    private void Awake()
    {
        moveDirectionSteering = Vector3.zero;
    }
    private void Start()
    {
        ProtestManager.Instance.OnFlowFieldsCreated += ProtestManager_OnFlowFieldsCreated;
        npcai.OnMoveDirectionInput.AddListener(UpdateMoveDirectionSteering);
    }

    private void OnDisable()
    {
        ProtestManager.Instance.OnFlowFieldsCreated -= ProtestManager_OnFlowFieldsCreated;
    }

    private void ProtestManager_OnFlowFieldsCreated(object sender, EventArgs e)
    {
        flowFieldsData = ProtestManager.Instance.GetFlowFields();
        currentFlowFieldIndex = flowFieldsData.IndexOf(flowFieldsData.First(flowfield => flowfield.index == 0));
        endOfProtest = ProtestManager.Instance.GetEndOfProtest();
    }

    private void Update()
    {
        float destructionDistance = 1f;
        if(Vector3.Distance(endOfProtest.position, transform.position) < destructionDistance)
        {
            Destroy(gameObject);
        }
                
        if(Vector3.Distance(flowFieldsData[currentFlowFieldIndex].target, transform.position) < meetingPointReachedDistance && currentFlowFieldIndex < flowFieldsData.Count - 1)
        {
            currentFlowFieldIndex = flowFieldsData.IndexOf(flowFieldsData.First(flowfield => flowfield.index == currentFlowFieldIndex + 1));
        }

    }

    private void FixedUpdate()
    {
        if(flowFieldsData.Count == 0) return;

        Node nodeBelow = flowFieldsData[currentFlowFieldIndex].flowField.GetNodeFromWorldPoint(transform.position);
       //moveDirection = (moveDirection + MoveDirectionNoise() * noiseMagnitude).normalized;

        Rigidbody protesterRB = GetComponent<Rigidbody>();
        Vector3 moveDirectionFlowField = new Vector3(nodeBelow.bestDirection.Vector.x, 0, nodeBelow.bestDirection.Vector.y).normalized;

        if(!activateSeekBehaviour)
        {
            protesterRB.velocity = moveDirectionFlowField * moveSpeed;
        }
        else
        {

            protesterRB.velocity = moveDirectionSteering * moveSpeed;
        }
    }

    private void UpdateMoveDirectionSteering(Vector3 direction)
    {
        moveDirectionSteering = direction;
    }

    

/*
    private Vector3 MoveDirectionNoise()
    {
        float x = UnityEngine.Random.Range(0f, noiseMaxRange);
        float y = UnityEngine.Random.Range(0f, noiseMaxRange);

        return new Vector3(x, 0, y).normalized;
    }
    */

}
