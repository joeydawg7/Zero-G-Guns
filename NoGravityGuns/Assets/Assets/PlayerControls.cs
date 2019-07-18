// GENERATED AUTOMATICALLY FROM 'Assets/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PlayerControls : IInputActionCollection
{
    private InputActionAsset asset;
    public PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""b6159740-ee60-4415-8288-7fa1b3cd948e"",
            ""actions"": [
                {
                    ""name"": ""Aim"",
                    ""id"": ""7740cb5e-a6af-43b6-bd60-181d80b175ea"",
                    ""expectedControlLayout"": """",
                    ""continuous"": true,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""Reload"",
                    ""id"": ""1e7a5aed-fdcb-4089-a5d1-e75b27defca4"",
                    ""expectedControlLayout"": """",
                    ""continuous"": true,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""Drop"",
                    ""id"": ""587a58e4-1f17-410c-ae39-99f1c644cc31"",
                    ""expectedControlLayout"": """",
                    ""continuous"": true,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": ""Tap"",
                    ""bindings"": []
                },
                {
                    ""name"": ""Shoot"",
                    ""id"": ""4c9da691-babc-43a6-b821-dba6c7a81c06"",
                    ""expectedControlLayout"": """",
                    ""continuous"": true,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""Join"",
                    ""id"": ""83c5554b-5ffa-4476-8bb9-57fc0f58727f"",
                    ""expectedControlLayout"": """",
                    ""continuous"": true,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": ""Tap"",
                    ""bindings"": []
                },
                {
                    ""name"": ""Start"",
                    ""id"": ""f67e1fee-b9b4-4094-9b93-10eb00686057"",
                    ""expectedControlLayout"": """",
                    ""continuous"": true,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": ""Tap"",
                    ""bindings"": []
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""56667341-3cee-4436-8b51-26dd3fc50222"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""cc3142b9-f5a6-4246-8238-69108e856d74"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""29d390c5-c105-4d87-bd7d-5d9baa64a5b4"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Drop"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""329547e2-4010-4ec6-b102-1aea403d7964"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""a5f3ee28-5c8f-4423-81f7-7efd08362c62"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Join"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""7a0b9264-1bef-4b87-85f7-411466513b25"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Start"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.GetActionMap("Gameplay");
        m_Gameplay_Aim = m_Gameplay.GetAction("Aim");
        m_Gameplay_Reload = m_Gameplay.GetAction("Reload");
        m_Gameplay_Drop = m_Gameplay.GetAction("Drop");
        m_Gameplay_Shoot = m_Gameplay.GetAction("Shoot");
        m_Gameplay_Join = m_Gameplay.GetAction("Join");
        m_Gameplay_Start = m_Gameplay.GetAction("Start");
    }

    ~PlayerControls()
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

    public ReadOnlyArray<InputControlScheme> controlSchemes
    {
        get => asset.controlSchemes;
    }

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

    // Gameplay
    private InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private InputAction m_Gameplay_Aim;
    private InputAction m_Gameplay_Reload;
    private InputAction m_Gameplay_Drop;
    private InputAction m_Gameplay_Shoot;
    private InputAction m_Gameplay_Join;
    private InputAction m_Gameplay_Start;
    public struct GameplayActions
    {
        private PlayerControls m_Wrapper;
        public GameplayActions(PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Aim { get { return m_Wrapper.m_Gameplay_Aim; } }
        public InputAction @Reload { get { return m_Wrapper.m_Gameplay_Reload; } }
        public InputAction @Drop { get { return m_Wrapper.m_Gameplay_Drop; } }
        public InputAction @Shoot { get { return m_Wrapper.m_Gameplay_Shoot; } }
        public InputAction @Join { get { return m_Wrapper.m_Gameplay_Join; } }
        public InputAction @Start { get { return m_Wrapper.m_Gameplay_Start; } }
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                Aim.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAim;
                Aim.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAim;
                Aim.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAim;
                Reload.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnReload;
                Reload.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnReload;
                Reload.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnReload;
                Drop.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDrop;
                Drop.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDrop;
                Drop.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDrop;
                Shoot.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnShoot;
                Shoot.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnShoot;
                Shoot.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnShoot;
                Join.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJoin;
                Join.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJoin;
                Join.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJoin;
                Start.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnStart;
                Start.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnStart;
                Start.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnStart;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                Aim.started += instance.OnAim;
                Aim.performed += instance.OnAim;
                Aim.canceled += instance.OnAim;
                Reload.started += instance.OnReload;
                Reload.performed += instance.OnReload;
                Reload.canceled += instance.OnReload;
                Drop.started += instance.OnDrop;
                Drop.performed += instance.OnDrop;
                Drop.canceled += instance.OnDrop;
                Shoot.started += instance.OnShoot;
                Shoot.performed += instance.OnShoot;
                Shoot.canceled += instance.OnShoot;
                Join.started += instance.OnJoin;
                Join.performed += instance.OnJoin;
                Join.canceled += instance.OnJoin;
                Start.started += instance.OnStart;
                Start.performed += instance.OnStart;
                Start.canceled += instance.OnStart;
            }
        }
    }
    public GameplayActions @Gameplay
    {
        get
        {
            return new GameplayActions(this);
        }
    }
    public interface IGameplayActions
    {
        void OnAim(InputAction.CallbackContext context);
        void OnReload(InputAction.CallbackContext context);
        void OnDrop(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
        void OnJoin(InputAction.CallbackContext context);
        void OnStart(InputAction.CallbackContext context);
    }
}
