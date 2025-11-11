using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyHandController : MonoBehaviour
{
    [SerializeField] InputActionReference actionGrip;
    [SerializeField] InputActionReference actionTrigger;
    private Animator handAnimator;
    // Start is called before the first frame update
    void Start()
    {
        actionGrip.action.performed += TriggerPress;
        actionTrigger.action.performed += GripPress;
        handAnimator = GetComponent<Animator>();
    }
    private void GripPress(InputAction.CallbackContext obj)
    {
        handAnimator.SetFloat("Grip", obj.ReadValue<float>());
    }
    private void TriggerPress(InputAction.CallbackContext obj)
    {
        handAnimator.SetFloat("Trigger", obj.ReadValue<float>());
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
