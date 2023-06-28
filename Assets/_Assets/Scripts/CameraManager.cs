using System;
using System.Collections;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _vcamMain;
    [SerializeField] private CinemachineVirtualCamera _vcamAttack;
    [SerializeField] private CinemachineVirtualCamera _vcamIDed;

    private WaitForSecondsRealtime _waitTimeCamAttack;
    private WaitForSecondsRealtime _waitTimeCamIDed;
    private bool _isIded = false;

    private void Awake()
    {
        _waitTimeCamAttack = new WaitForSecondsRealtime(.2f);
        _waitTimeCamIDed = new WaitForSecondsRealtime(2f);

    }

    private void Start()
    {
        PlayerController.Instance.OnAttackPerformed.AddListener(PlayerController_OnAttackPerformed);
        PoliceResponseManager.Instance.OnPlayerIdentified.AddListener(PoliceResponseManager_OnPlayerIDed);
        _vcamAttack.gameObject.SetActive(false);
        _vcamIDed.gameObject.SetActive(false);
    }

    private void PoliceResponseManager_OnPlayerIDed()
    {
        _isIded = true;
        StartCoroutine(ZoomOnID());
    }

    private void PlayerController_OnAttackPerformed(Transform breakable)
    {
        StartCoroutine(ZoomOnAttack());
    }

    private IEnumerator ZoomOnAttack()
    {
        if(_isIded)
        {
            yield break;
        }

        _vcamAttack.gameObject.SetActive(true);
        _vcamMain.gameObject.SetActive(false);

        yield return _waitTimeCamAttack;
        
        _vcamAttack.gameObject.SetActive(false);

        if(!_isIded)
        {
            _vcamMain.gameObject.SetActive(true);
        }

        yield return null;
    } 

    private IEnumerator ZoomOnID()
    {
        Time.timeScale = .5f;
        _vcamIDed.gameObject.SetActive(true);
        _vcamMain.gameObject.SetActive(false);
        _vcamAttack.gameObject.SetActive(false);

        yield return _waitTimeCamIDed;
        
        _vcamIDed.gameObject.SetActive(false);
        _vcamMain.gameObject.SetActive(true);

        Time.timeScale = 1f;

        _isIded = false;
        yield return null;
    } 
}
