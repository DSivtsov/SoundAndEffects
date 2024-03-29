//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/Controls/MyControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @MyControls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @MyControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""MyControls"",
    ""maps"": [
        {
            ""name"": ""Move"",
            ""id"": ""e9349057-67df-4d78-8965-34c07bdecfd2"",
            ""actions"": [
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""5de68073-e13b-4d42-acc1-e096cb136d5f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Run"",
                    ""type"": ""Button"",
                    ""id"": ""a632680b-840f-4dae-b691-0682835d2902"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PressAnyKey"",
                    ""type"": ""Button"",
                    ""id"": ""b7edee45-5c4e-4837-b6ef-fcfdea35ca97"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""e854ddcc-b7bd-457c-85cc-d4d5496ebd1d"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""87291057-2171-4ee7-8b8c-5469d00a674c"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2053c390-ed8b-4554-a5ed-7fdf1e021ccc"",
                    ""path"": ""<Keyboard>/anyKey"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PressAnyKey"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Move
        m_Move = asset.FindActionMap("Move", throwIfNotFound: true);
        m_Move_Jump = m_Move.FindAction("Jump", throwIfNotFound: true);
        m_Move_Run = m_Move.FindAction("Run", throwIfNotFound: true);
        m_Move_PressAnyKey = m_Move.FindAction("PressAnyKey", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Move
    private readonly InputActionMap m_Move;
    private IMoveActions m_MoveActionsCallbackInterface;
    private readonly InputAction m_Move_Jump;
    private readonly InputAction m_Move_Run;
    private readonly InputAction m_Move_PressAnyKey;
    public struct MoveActions
    {
        private @MyControls m_Wrapper;
        public MoveActions(@MyControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Jump => m_Wrapper.m_Move_Jump;
        public InputAction @Run => m_Wrapper.m_Move_Run;
        public InputAction @PressAnyKey => m_Wrapper.m_Move_PressAnyKey;
        public InputActionMap Get() { return m_Wrapper.m_Move; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MoveActions set) { return set.Get(); }
        public void SetCallbacks(IMoveActions instance)
        {
            if (m_Wrapper.m_MoveActionsCallbackInterface != null)
            {
                @Jump.started -= m_Wrapper.m_MoveActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_MoveActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_MoveActionsCallbackInterface.OnJump;
                @Run.started -= m_Wrapper.m_MoveActionsCallbackInterface.OnRun;
                @Run.performed -= m_Wrapper.m_MoveActionsCallbackInterface.OnRun;
                @Run.canceled -= m_Wrapper.m_MoveActionsCallbackInterface.OnRun;
                @PressAnyKey.started -= m_Wrapper.m_MoveActionsCallbackInterface.OnPressAnyKey;
                @PressAnyKey.performed -= m_Wrapper.m_MoveActionsCallbackInterface.OnPressAnyKey;
                @PressAnyKey.canceled -= m_Wrapper.m_MoveActionsCallbackInterface.OnPressAnyKey;
            }
            m_Wrapper.m_MoveActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Run.started += instance.OnRun;
                @Run.performed += instance.OnRun;
                @Run.canceled += instance.OnRun;
                @PressAnyKey.started += instance.OnPressAnyKey;
                @PressAnyKey.performed += instance.OnPressAnyKey;
                @PressAnyKey.canceled += instance.OnPressAnyKey;
            }
        }
    }
    public MoveActions @Move => new MoveActions(this);
    public interface IMoveActions
    {
        void OnJump(InputAction.CallbackContext context);
        void OnRun(InputAction.CallbackContext context);
        void OnPressAnyKey(InputAction.CallbackContext context);
    }
}
