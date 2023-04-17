using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class ProtesterManager : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask floorMask;
    [SerializeField] private float meetingPointReachedDistance = 2f;
    [SerializeField] private float noiseMaxRange = 10f;
    [SerializeField] private float noiseMagnitude = .1f;

    private MeshRenderer meshRenderer;
    private int currentFlowFieldIndex;
    private List<FlowFieldData> flowFieldsData;
    private Transform endOfProtest;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        Hide();
    }

    private void Start()
    {
        ProtestManager.Instance.OnFlowFieldsCreated += ProtestManager_OnFlowFieldsCreated;
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
        float floorDetectionDistance = 2f;
        if(Physics.Raycast(transform.position, Vector3.down, floorDetectionDistance, floorMask))
        {
            Show();
        }
        else
        {
            Hide();
        }

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
        Vector3 moveDirection = new Vector3(nodeBelow.bestDirection.Vector.x, 0, nodeBelow.bestDirection.Vector.y).normalized;
        moveDirection = (moveDirection + MoveDirectionNoise() * noiseMagnitude).normalized;

        Rigidbody protesterRB = GetComponent<Rigidbody>();
        protesterRB.velocity = moveDirection * moveSpeed;
    }

    private void Show()
    {
        meshRenderer.enabled = true;
    }

    private void Hide()
    {
        meshRenderer.enabled = false;
    }

    private Vector3 MoveDirectionNoise()
    {
        float x = UnityEngine.Random.Range(0f, noiseMaxRange);
        float y = UnityEngine.Random.Range(0f, noiseMaxRange);

        return new Vector3(x, 0, y).normalized;
    }

}
