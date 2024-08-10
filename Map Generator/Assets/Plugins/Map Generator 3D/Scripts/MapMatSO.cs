using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CNB
{
    /// <summary>
    /// These few materials define most of the look of the generated map. Saving this looks in scriptable objects makes it easy to create and backUp new map skins.
    /// Exposes variables to define the TileX variable of every material uses end calculates ( every time e map is loaded) the correct TileY for that TileX to show textures with the right proportions
    /// </summary>
    [CreateAssetMenu(fileName = "NewMapMat", menuName = "ScriptableObjects/MapMatSO", order = 1)]
    public class MapMatSO : ScriptableObject
    {
        public Material _meshMaterial;
        public int _meshMaterialTileX = 20;

        public Material _floorMaterial;
        public int _floorMaterialTileX = 20;

        public Material _wallsMaterial;
        public int _wallsMaterialTileX = 200;

        public Material _waterMaterial;
        public int _waterMaterialTileX = 20;

        public Material _mapLimitsWallsUp;
        public int _mapLimitsWallsUpTileX = 200;

        public Material _mapLimitsTopUp;
        public int _mapLimitsTopUpTileX = 20;

        public Material _mapLimitsWallsDown;
        public int _mapLimitsWallsDownTileX = 200;

        public Material _mapLimitsTopDown;
        public int _mapLimitsTopDownTileX = 20;


    }
}