﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC
{
    public class ControlButtonManager : MonoBehaviour
    {
        [Serializable]
        public struct CategoryRootPair
        {
            public ControlButtonAsset.ButtonCategory category;
            public GameObject root;
        }

        public GameObject controlButtonPrefab;
        public CategoryRootPair[] rootMappings;
        protected Dictionary<ControlButtonAsset.ButtonCategory, GameObject> rootMap;

        public static ControlButtonManager _instance;
        public static ControlButtonManager instance
        {
            get
            {
                return OverlayUtils.Singleton(ref _instance, "[ControlButtonManager]", false);
            }
        }

        private void OnEnable()
        {
            rootMap = new Dictionary<ControlButtonAsset.ButtonCategory, GameObject>();
            foreach (var pair in rootMappings)
            {
                rootMap.Add(pair.category, pair.root);
            }
        }

        /**
         * Add a new control button into the scene
         */
        public ControlButton AddControlButton(ControlButtonAsset controlButtonAsset) {
            return AddControlButton<ControlButton, ControlButtonAsset>(controlButtonAsset, controlButtonPrefab);
        }

        protected T AddControlButton<T, U>(U controlButtonAsset, GameObject prefab) where T: ControlButton where U: ControlButtonAsset
        {
            if (!rootMap.ContainsKey(controlButtonAsset.category))
            {
                Debug.LogErrorFormat("ControlButtonManager ({0}) does not contain mapping for the {1} category", name, controlButtonAsset.category.ToString());
                return null;
            }

            prefab.SetActive(false);
            var controlButtonInstance = Instantiate(prefab);
            var controlButton = controlButtonInstance.GetComponent<T>();
            controlButtonInstance.name = controlButtonAsset.name;
            controlButton.controlButtonAsset = controlButtonAsset;
            
            controlButton.transform.SetParent(rootMap[controlButtonAsset.category].transform, false);

            controlButtonInstance.SetActive(true);
            return controlButton;
        }
    }
}
