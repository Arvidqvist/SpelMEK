using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public CameraSettings cameraSettings;
    [System.Serializable]
    public class CameraSettings
    {
        [Tooltip("The senisitivity of your mouse")]
        [Range(0,1)]
        public float MouseSensitivity = 1f;
        [Range(0,100)]
        [Tooltip("Camera distance to the player")]
        public float cameraDistance = 10f;
    }


    public PlayerSettings playerSettings;
    [System.Serializable]
    public class PlayerSettings
    {
        [Header("Speed Settings")]
        [Tooltip("How fast this transform accelerates")]
        public float acceleration = 2;
        [Tooltip("How fast this transform decelerates")]
        public float deceleration = 1;
        [Tooltip("The maximum speed this transform can achieve")]
        public float maxSpeed = 3;
        [Tooltip("The lowest possible speed this transform can achieve before it gets set to zero")]
        public float minimumSpeedCutoff = 0.1f;

        [Header("Friction Settings")]
        [Tooltip("Weird friction 1")]           //TODO: Understand what these frictions/resistances are
        public float staticFriction = 0.45f;
        [Tooltip("Weird friction 2")]           //TODO: Understand what these frictions/resistances are
        public float dynamicFriction = 0.25f;
        [Tooltip("Weird friction 3")]           //TODO: Understand what these frictions/resistances are
        public float airResistance = 0.25f;

        [Header("Jump & Collision Settings")]
        [Tooltip("How much this transform is affected by gravity")]
        public float gravityModifier = 1;
        [Tooltip("How close this transform can get near other transforms")]
        public float skinWidth = 0.03f;
        [Tooltip("How near this transform needs to be to the ground to be able to jump")]
        public float groundCheckDistance = 0.1f;
        [Tooltip("How high this transform can jump")]
        [Range(0, 25)]
        public float jumpHeight = 10f;
        [Tooltip("how much power your dash has")]
        [Range(0,25)]
        public float dashPower = 5;
        [Tooltip("The layer that this transform will collide with")]
        public LayerMask collisionLayer = new LayerMask();
        [Tooltip("The layer that this transform will be able to gravity flip with")]
        public LayerMask cubeLayer = new LayerMask();
        
    }
}
