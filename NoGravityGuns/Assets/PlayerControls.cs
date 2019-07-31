// GENERATED AUTOMATICALLY FROM 'Assets/PlayerControls.inputactions'

using System.Collections;
using System.Collections.Generic;
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
            ""id"": ""57123031-efeb-4e38-aac4-52ced4d5d9d8"",
            ""actions"": [
                {
                    ""name"": ""Aim"",
                    ""type"": ""Value"",
                    ""id"": ""a85544d0-1ed3-47e9-a38a-3fd3193768ee"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Join"",
                    ""type"": ""Button"",
                    ""id"": ""3bdb88f1-8e76-400d-af5c-bac9d088e9b7"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Drop"",
                    ""type"": ""Button"",
                    ""id"": ""fd86a820-85ca-415a-9822-f867eb48dac8"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Reload"",
                    ""type"": ""Button"",
                    ""id"": ""4d9d6c8e-f362-4384-ab86-050ff307917a"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Value"",
                    ""id"": ""4ff36964-69db-4311-8422-3eb7f3e0de8d"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Start"",
                    ""type"": ""Button"",
                    ""id"": ""2697c853-3556-433d-af32-191d58eb8bef"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""4c77a4be-5cd7-4fb0-bd41-12726f84712a"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Controller"",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7d71589f-44ac-4324-88e4-a9f27c8c21d9"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": "";Controller"",
                    ""action"": ""Join"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2f13da6c-08d3-4743-8545-4c2953ea577c"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": "";Controller"",
                    ""action"": ""Drop"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""96ec35ab-17cf-4209-bc46-e3efe70fbd22"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": "";Controller"",
                    ""action"": ""Reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""adec4b8c-6975-45ac-ad4d-11704c7173af"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": "";Controller"",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fa2bf4ef-a1b4-4333-9e75-e840746c7f20"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": "";Controller"",
                    ""action"": ""Start"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Controller"",
            ""basedOn"": """",
            ""bindingGroup"": ""Controller"",
            ""devices"": []
        }
    ]
}");
        // Gameplay
        m_Gameplay = asset.GetActionMap("Gameplay");
        m_Gameplay_Aim = m_Gameplay.GetAction("Aim");
        m_Gameplay_Join = m_Gameplay.GetAction("Join");
        m_Gameplay_Drop = m_Gameplay.GetAction("Drop");
        m_Gameplay_Reload = m_Gameplay.GetAction("Reload");
        m_Gameplay_Shoot = m_Gameplay.GetAction("Shoot");
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

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_Aim;
    private readonly InputAction m_Gameplay_Join;
    private readonly InputAction m_Gameplay_Drop;
    private readonly InputAction m_Gameplay_Reload;
    private readonly InputAction m_Gameplay_Shoot;
    private readonly InputAction m_Gameplay_Start;
    public struct GameplayActions
    {
        private PlayerControls m_Wrapper;
        public GameplayActions(PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Aim => m_Wrapper.m_Gameplay_Aim;
        public InputAction @Join => m_Wrapper.m_Gameplay_Join;
        public InputAction @Drop => m_Wrapper.m_Gameplay_Drop;
        public InputAction @Reload => m_Wrapper.m_Gameplay_Reload;
        public InputAction @Shoot => m_Wrapper.m_Gameplay_Shoot;
        public InputAction @Start => m_Wrapper.m_Gameplay_Start;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                Aim.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAim;
                Aim.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAim;
                Aim.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAim;
                Join.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJoin;
                Join.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJoin;
                Join.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJoin;
                Drop.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDrop;
                Drop.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDrop;
                Drop.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDrop;
                Reload.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnReload;
                Reload.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnReload;
                Reload.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnReload;
                Shoot.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnShoot;
                Shoot.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnShoot;
                Shoot.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnShoot;
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
                Join.started += instance.OnJoin;
                Join.performed += instance.OnJoin;
                Join.canceled += instance.OnJoin;
                Drop.started += instance.OnDrop;
                Drop.performed += instance.OnDrop;
                Drop.canceled += instance.OnDrop;
                Reload.started += instance.OnReload;
                Reload.performed += instance.OnReload;
                Reload.canceled += instance.OnReload;
                Shoot.started += instance.OnShoot;
                Shoot.performed += instance.OnShoot;
                Shoot.canceled += instance.OnShoot;
                Start.started += instance.OnStart;
                Start.performed += instance.OnStart;
                Start.canceled += instance.OnStart;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);
    private int m_ControllerSchemeIndex = -1;
    public InputControlScheme ControllerScheme
    {
        get
        {
            if (m_ControllerSchemeIndex == -1) m_ControllerSchemeIndex = asset.GetControlSchemeIndex("Controller");
            return asset.controlSchemes[m_ControllerSchemeIndex];
        }
    }
    public interface IGameplayActions
    {
        void OnAim(InputAction.CallbackContext context);
        void OnJoin(InputAction.CallbackContext context);
        void OnDrop(InputAction.CallbackContext context);
        void OnReload(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
        void OnStart(InputAction.CallbackContext context);
    }
}
