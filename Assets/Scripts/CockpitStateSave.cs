﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace EVRC
{
    public class CockpitStateSave : MonoBehaviour
    {
        public GameObject root;
        public MovableSurface metaPanel;
        public MovableSurface shipThrottle;
        public MovableSurface srvThrottle;
        public MovableSurface sixDofController;
        public MovableSurface shipJoystick;
        public MovableSurface srvJoystick;
        public MovableSurface shipPowerDeliveryPanel;
        public MovableSurface srvPowerDeliveryPanel;
        public MovableSurface mapPlaneController;
        public ControlButtonAssetCatalog controlButtonCatalog;

        [Serializable]
        public struct State
        {
            [Serializable]
            public struct SavedTransform
            {
                public Vector3 pos;
                public Vector3 rot;
            }

            [Serializable]
            public struct StaticLocations
            {
                public SavedTransform metaPanel;
                public SavedTransform shipThrottle;
                public SavedTransform srvThrottle;
                public SavedTransform shipJoystick;
                public SavedTransform srvJoystick;
                public SavedTransform shipPowerDeliveryPanel;
                public SavedTransform srvPowerDeliveryPanel;
                public SavedTransform sixDofController;
                public SavedTransform mapPlaneController;
            }

            [Serializable]
            public struct SavedControlButton
            {
                public string type;
                public SavedTransform loc;
            }

            public int version;
            public StaticLocations staticLocations;
            public SavedControlButton[] controlButtons;
        }

        public static CockpitStateSave _instance;
        public static CockpitStateSave instance
        {
            get
            {
                return OverlayUtils.Singleton(ref _instance, "[CockpitStateSave]", false);
            }
        }

        public static string savedStateFilePath
        {
            get
            {
                return Path.Combine(Application.persistentDataPath, "SavedState.json");
            }
        }

        protected State.SavedTransform SerializeTransform(Transform transform)
        {
            return new State.SavedTransform
            {
                pos = transform.localPosition,
                rot = transform.localEulerAngles,
            };
        }

        public IEnumerable<State.SavedControlButton> ReadControlButtons(IEnumerable<ControlButton> controlButtons)
        {
            foreach (var button in controlButtons)
            {
                yield return new State.SavedControlButton
                {
                    type = button.controlButtonAsset.name,
                    loc = SerializeTransform(button.transform),
                };
            }
        }

        protected void ApplyTransform(Transform transform, State.SavedTransform savedTransform)
        {
            transform.localPosition = savedTransform.pos;
            transform.localRotation = Quaternion.Euler(savedTransform.rot);
        }


        private void AddControlButton(State.SavedControlButton controlButton)
        {
            var controlButtonAsset = controlButtonCatalog.GetByName(controlButton.type);
            ControlButton button;
            if(controlButton.type.StartsWith("SetSpeed")) {
                button = ControlButtonManager.instance.AddControlButton(controlButtonAsset as SpeedControlButtonAsset);
            } else {
                button = ControlButtonManager.instance.AddControlButton(controlButtonAsset);
            }
            ApplyTransform(button.transform, controlButton.loc);
        }

        public State ReadState()
        {
            var state = new State
            {
                version = 3
            };

            state.staticLocations.metaPanel = SerializeTransform(metaPanel.transform);
            state.staticLocations.shipThrottle = SerializeTransform(shipThrottle.transform);
            state.staticLocations.srvThrottle = SerializeTransform(srvThrottle.transform);
            state.staticLocations.shipJoystick = SerializeTransform(shipJoystick.transform);
            state.staticLocations.srvJoystick = SerializeTransform(srvJoystick.transform);
            state.staticLocations.shipPowerDeliveryPanel = SerializeTransform(shipPowerDeliveryPanel.transform);
            state.staticLocations.srvPowerDeliveryPanel = SerializeTransform(srvPowerDeliveryPanel.transform);
            state.staticLocations.sixDofController = SerializeTransform(sixDofController.transform);
            state.staticLocations.mapPlaneController = SerializeTransform(mapPlaneController.transform);

            state.controlButtons = ReadControlButtons(root.GetComponentsInChildren<ControlButton>(true)).ToArray();

            return state;
        }

        public void ApplyState(State state)
        {
            ApplyTransform(metaPanel.transform, state.staticLocations.metaPanel);
            ApplyTransform(shipThrottle.transform, state.staticLocations.shipThrottle);
            ApplyTransform(srvThrottle.transform, state.staticLocations.srvThrottle);
            ApplyTransform(shipJoystick.transform, state.staticLocations.shipJoystick);
            ApplyTransform(srvJoystick.transform, state.staticLocations.srvJoystick);
            if (state.version >= 1)
            {
                ApplyTransform(sixDofController.transform, state.staticLocations.sixDofController);
            }
            if (state.version >= 2)
            {
                ApplyTransform(mapPlaneController.transform, state.staticLocations.mapPlaneController);
            }
            if (state.version >= 3)
            {
                ApplyTransform(shipPowerDeliveryPanel.transform, state.staticLocations.shipPowerDeliveryPanel);
                ApplyTransform(srvPowerDeliveryPanel.transform, state.staticLocations.srvPowerDeliveryPanel);
            }

            foreach (var controlButton in state.controlButtons)
            {
                AddControlButton(controlButton);
            }
        }

        public void Load()
        {
            var path = savedStateFilePath;
            if (File.Exists(path))
            {
                Debug.LogFormat("Loading from {0}", path);
                var state = JsonUtility.FromJson<State>(File.ReadAllText(path));
                ApplyState(state);
            }
        }

        public static void Save()
        {
            var cockpitStateSave = instance;
            if (cockpitStateSave == null)
            {
                Debug.LogWarning("Save failed");
                return;
            }

            var path = savedStateFilePath;
            var state = cockpitStateSave.ReadState();
            File.WriteAllText(path, JsonUtility.ToJson(state));
            Debug.LogFormat("Saved to {0}", path);
        }

        private void Awake()
        {
            StartCoroutine(DelayedLoad());
        }

        private IEnumerator DelayedLoad()
        {
            yield return null;
            Load();
        }
    }
}
