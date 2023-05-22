using System.Collections.Generic;
using UnityEngine;

public class ProtesterVisual : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";

    [Header("Initialization Parameters")]
    [SerializeField] private ProtesterData _protesterData;
    [SerializeField] private ProtesterController _protesterController;
    [SerializeField] private LayerMask _floorMask;
    [SerializeField] private List<SkinSO> _skinSOs;
    [SerializeField] private List<MeshRenderer> _coloredClothes;
    [SerializeField] private List<Transform> _visualComponents;
    [SerializeField] private List<Transform> _attributes;

    private Vector3 _targetDirection;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _protesterController.OnMove.AddListener(ProtesterController_OnMove);

        int randomIndex = Random.Range(0, _skinSOs.Count);

        foreach(MeshRenderer meshRenderer in _coloredClothes)
        {
            meshRenderer.material = _skinSOs[randomIndex].skinMaterial;
        }
        _protesterData.Skin = _skinSOs[randomIndex];

        foreach(Transform attribute in _attributes)
        {
            attribute.gameObject.SetActive(false);
        }
        //draw a random value to see if protester should wear an attribute or no
        int minRange = 1;
        int maxRange = 100;
        int draw = Random.Range(minRange, maxRange);
        int meanValue = Mathf.CeilToInt(((float)maxRange - (float)minRange) / (float)2);
        if(draw > meanValue)
        {
            for (int i = 0; i < _attributes.Count; i++)
            {
                int lowerBound = Mathf.CeilToInt(meanValue * (1 + (float)i / (float)_attributes.Count));
                int higherBound = Mathf.CeilToInt(meanValue * (1 + (float)(1 + i) / (float)_attributes.Count));
                if(draw> lowerBound && draw <= higherBound)
                {
                    _attributes[i].gameObject.SetActive(true);
                    break;
                }
            }
        }
    }

    private void Update()
    {
        float floorDetectionDistance = 2f;
        if(!Physics.Raycast(transform.position, Vector3.down, floorDetectionDistance, _floorMask))
        {
            Hide();
        }
        else
        {
            Show();
        }

        if(_targetDirection !=  Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);
            float turnSpeed = 4f;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
    }
    
    private void ProtesterController_OnMove(Vector3 direction)
    {
        if(direction != Vector3.zero)
        {
            //character visual has a 180° rotation to forward vector
            _targetDirection = -direction;
            _animator.SetBool(IS_WALKING, true);
        }
        else
        {
            _animator.SetBool(IS_WALKING, false);
        }
    }

    public void Show()
    {
        foreach(Transform component in _visualComponents)
        {
            component.gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        foreach(Transform component in _visualComponents)
        {
            component.gameObject.SetActive(false);
        }
    }
}
