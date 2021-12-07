using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoCameraTransform : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_CameraEulerRotation = new Vector3();
    public Quaternion GetQuaternion() { return Quaternion.Euler(m_CameraEulerRotation); }
}
