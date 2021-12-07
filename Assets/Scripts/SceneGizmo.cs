using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SceneGizmo : MonoBehaviour
{
    [SerializeField]
    private Camera m_GizmoCamera = null;

    [SerializeField]
    private Transform m_MainVirtualCamaera = null;

    [SerializeField]
    private List<GameObject> m_listTriangleButtons = new List<GameObject>();

    [SerializeField]
    private Transform m_Pivot = null;

    [SerializeField]
    private Material m_SelectedMaterial = null;

    [SerializeField]
    private GameObject m_CurrentHitObject = null;

    [SerializeField]
    private Renderer m_CurrentRenderer = null;

    [SerializeField]
    private float m_ZoomSpeed = 1.0f;

    [SerializeField]
    private Vector2 m_CameraMoveSpeed = new Vector2(0.01f, 0.01f);

    [SerializeField]
    private Vector2 m_CameraRotateSpeed = new Vector2(0.2f, 0.2f);

    [SerializeField]
    private float m_PivotDistance = 10.0f;

    private Vector2 m_LastMousePosition = Vector2.zero;

    private Vector3 m_HomeViewPosition = Vector3.zero;

    private Vector3 m_HomeViewRotation = Vector3.zero;

    [SerializeField]
    private float ShowPivotDuration = 2.0f;

    private WaitForSeconds m_ShowPivotDuration = null;

    private bool m_isShowPivot = false;

    public enum GizmoDirection
    {
        Front = 0,
        Back,
        Right,
        Left,
        Up,
        Down,
        Num
    }

    [SerializeField]
    private List<Transform> m_listGizmoSurface = new List<Transform>(); 

    [SerializeField]
    private bool m_isDragGizmo = false;

    private void Awake()
    {
        m_ShowPivotDuration = new WaitForSeconds(ShowPivotDuration);
    }

    private void Update()
    {
        SelectedChangeColor();
        RotateBySceneGizmo();
        SwitchTriangleButton();
        MoveCameraByMouseWheel();
    }

    IEnumerator ShowPivotEnum()
    {
        m_Pivot.gameObject.SetActive(true);
        yield return m_ShowPivotDuration;
        m_Pivot.gameObject.SetActive(false);
        m_isShowPivot = false;
    }

    private void ShowPivot()
    {
        if (!m_isShowPivot)
        {
            StartCoroutine("ShowPivotEnum");
            m_isShowPivot = true;
        }
        else
        {
            StopCoroutine("ShowPivotEnum");
            StartCoroutine("ShowPivotEnum");
            m_isShowPivot = true;
        }
    }

    private void MoveCameraByMouseWheel()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            if(!m_isShowPivot)
            {
                var position = Input.mousePosition;
                position.z = m_PivotDistance;
                m_Pivot.transform.position = Camera.main.ScreenToWorldPoint(position);
            }

            ShowPivot();
            var scroll = Input.mouseScrollDelta.y;
            m_MainVirtualCamaera.position += Vector3.Normalize(m_Pivot.position - m_MainVirtualCamaera.position) * scroll * m_ZoomSpeed;
        }

        if (Input.GetMouseButtonDown(2))
        {
            m_Pivot.gameObject.SetActive(false);
            m_LastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(2))
        {
            var x = (m_LastMousePosition.x - Input.mousePosition.x) * m_MainVirtualCamaera.right * m_CameraMoveSpeed.x;
            var y = (m_LastMousePosition.y - Input.mousePosition.y) * m_MainVirtualCamaera.up * m_CameraMoveSpeed.y;

            if ((x.sqrMagnitude != 0|| y.sqrMagnitude != 0))
            {
                m_MainVirtualCamaera.position += x + y;
                m_LastMousePosition = Input.mousePosition;
            }
        }
    }

    private void SwitchTriangleButton()
    {
        if (m_GizmoCamera.transform.eulerAngles.x % 90.0f == 0.0f &&
           m_GizmoCamera.transform.eulerAngles.y % 90.0f == 0.0f &&
           m_GizmoCamera.transform.eulerAngles.z % 90.0f == 0.0f)
        {
            foreach (var button in m_listTriangleButtons)
            {
                button.SetActive(true);
            }
        }
        else
        {
            foreach (var button in m_listTriangleButtons)
            {
                button.SetActive(false);
            }
        }
    }

    public void PushHomeViewButton()
    {
        m_MainVirtualCamaera.rotation = Quaternion.Euler(m_HomeViewRotation);
        m_GizmoCamera.transform.rotation = Quaternion.Euler(m_HomeViewRotation);

        m_MainVirtualCamaera.position = m_HomeViewPosition;
        m_GizmoCamera.transform.position = m_GizmoCamera.transform.forward  * -6.0f;
    }

    public void PushTriangleButton(int direction)
    {
        Transform surface = null;

        if (m_GizmoCamera.transform.eulerAngles.x == 270.0f)
        {
            switch ((GizmoDirection)direction)
            {
                case GizmoDirection.Up:
                    surface = m_listGizmoSurface[(int)GizmoDirection.Front];
                    break;
                case GizmoDirection.Down:
                    surface = m_listGizmoSurface[(int)GizmoDirection.Back];
                    break;
                case GizmoDirection.Right:
                    surface = m_listGizmoSurface[(int)GizmoDirection.Right];
                    break;
                case GizmoDirection.Left:
                    surface = m_listGizmoSurface[(int)GizmoDirection.Left];
                    break;
                default:
                    break;
            }
        }
        else if (m_GizmoCamera.transform.eulerAngles.x == 0.0f)
        {
            if (m_GizmoCamera.transform.eulerAngles.y == 270.0f)
            {
                switch ((GizmoDirection)direction)
                {
                    case GizmoDirection.Up:
                        surface = m_listGizmoSurface[(int)GizmoDirection.Up];
                        break;
                    case GizmoDirection.Down:
                        surface = m_listGizmoSurface[(int)GizmoDirection.Down];
                        break;
                    case GizmoDirection.Right:
                        surface = m_listGizmoSurface[(int)GizmoDirection.Back];
                        break;
                    case GizmoDirection.Left:
                        surface = m_listGizmoSurface[(int)GizmoDirection.Front];
                        break;
                    default:
                        break;
                }
            }
            else if (m_GizmoCamera.transform.eulerAngles.y == 0.0f)
            {
                switch ((GizmoDirection)direction)
                {
                    case GizmoDirection.Up:
                        surface = m_listGizmoSurface[(int)GizmoDirection.Up];
                        break;
                    case GizmoDirection.Down:
                        surface = m_listGizmoSurface[(int)GizmoDirection.Down];
                        break;
                    case GizmoDirection.Right:
                        surface = m_listGizmoSurface[(int)GizmoDirection.Right];
                        break;
                    case GizmoDirection.Left:
                        surface = m_listGizmoSurface[(int)GizmoDirection.Left];
                        break;
                    default:
                        break;
                }
            }
            else if (m_GizmoCamera.transform.eulerAngles.y == 90.0f)
            {
                switch ((GizmoDirection)direction)
                {
                    case GizmoDirection.Up:
                        surface = m_listGizmoSurface[(int)GizmoDirection.Up];
                        break;
                    case GizmoDirection.Down:
                        surface = m_listGizmoSurface[(int)GizmoDirection.Down];
                        break;
                    case GizmoDirection.Right:
                        surface = m_listGizmoSurface[(int)GizmoDirection.Front];
                        break;
                    case GizmoDirection.Left:
                        surface = m_listGizmoSurface[(int)GizmoDirection.Back];
                        break;
                    default:
                        break;
                }
            }
            else if (m_GizmoCamera.transform.eulerAngles.y == 180.0f)
            {
                switch ((GizmoDirection)direction)
                {
                    case GizmoDirection.Up:
                        surface = m_listGizmoSurface[(int)GizmoDirection.Up];
                        break;
                    case GizmoDirection.Down:
                        surface = m_listGizmoSurface[(int)GizmoDirection.Down];
                        break;
                    case GizmoDirection.Right:
                        surface = m_listGizmoSurface[(int)GizmoDirection.Left];
                        break;
                    case GizmoDirection.Left:
                        surface = m_listGizmoSurface[(int)GizmoDirection.Right];
                        break;
                    default:
                        break;
                }
            }
        }
        else if(m_GizmoCamera.transform.eulerAngles.x == 90.0f)
        {
            switch ((GizmoDirection)direction)
            {
                case GizmoDirection.Up:
                    surface = m_listGizmoSurface[(int)GizmoDirection.Back];
                    break;
                case GizmoDirection.Down:
                    surface = m_listGizmoSurface[(int)GizmoDirection.Front];
                    break;
                case GizmoDirection.Right:
                    surface = m_listGizmoSurface[(int)GizmoDirection.Right];
                    break;
                case GizmoDirection.Left:
                    surface = m_listGizmoSurface[(int)GizmoDirection.Left];
                    break;
                default:
                    break;
            }
        }
        else
        {
            return;
        }
        Vector3 vec = Vector3.Normalize(transform.position - surface.transform.position);
        m_MainVirtualCamaera.position = -vec * 10.0f;
        m_GizmoCamera.transform.position = -vec * 6.0f;
        m_MainVirtualCamaera.transform.LookAt(transform);
        m_GizmoCamera.transform.LookAt(transform);
    }

    private void RotateBySceneGizmo()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_LastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            var x = Input.mousePosition.y - m_LastMousePosition.y;
            var y = m_LastMousePosition.x - Input.mousePosition.x;

            if ((x != 0.0f || y != 0.0f) && m_CurrentHitObject)
            {
                ShowPivot();

                m_isDragGizmo = true;
                var newAngle = Vector3.zero;
                newAngle.x = x * m_CameraRotateSpeed.x;
                newAngle.y = y * m_CameraRotateSpeed.y;

                if (m_CurrentHitObject.name == "Ring")
                {
                    m_GizmoCamera.transform.RotateAround(transform.position,
                        transform.up,
                        -newAngle.y);

                    m_MainVirtualCamaera.transform.RotateAround(m_Pivot.position,
                        transform.up,
                        -newAngle.y);

                    m_LastMousePosition = Input.mousePosition;
                }
                else
                {
                    m_GizmoCamera.transform.RotateAround(transform.position,
                        -m_GizmoCamera.transform.right,
                        newAngle.x);

                    m_GizmoCamera.transform.RotateAround(transform.position,
                        -m_GizmoCamera.transform.up,
                        newAngle.y);

                    m_MainVirtualCamaera.transform.RotateAround(m_Pivot.position,
                        -m_MainVirtualCamaera.transform.right,
                        newAngle.x);

                    m_MainVirtualCamaera.transform.RotateAround(m_Pivot.position,
                        -m_MainVirtualCamaera.transform.up,
                        newAngle.y);

                    m_LastMousePosition = Input.mousePosition;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (m_isDragGizmo)
            {
                m_isDragGizmo = false;
            }
            else if (m_CurrentHitObject && m_CurrentHitObject.name != "Ring")
            {
                m_Pivot.position = Vector3.zero;

                Vector3 direction = Vector3.Normalize(m_CurrentHitObject.transform.root.position - m_CurrentHitObject.transform.position);
                m_MainVirtualCamaera.position = -direction * 10.0f;
                m_GizmoCamera.transform.position = -direction * 6.0f;
                m_MainVirtualCamaera.transform.LookAt(m_CurrentHitObject.transform.root);
                m_GizmoCamera.transform.LookAt(m_CurrentHitObject.transform.root);
            }
        }
        else
        {
            m_CurrentHitObject = null;
        }
    }

    private void SelectedChangeColor()
    {
        Ray ray = m_GizmoCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, LayerMask.NameToLayer("SceneGizmo")))
        {
            if (hit.collider.gameObject.layer != LayerMask.NameToLayer("SceneGizmo") ||
                EventSystem.current.IsPointerOverGameObject())
                return;

            if (m_CurrentRenderer != null)
            {
                var currentMats = m_CurrentRenderer.materials;
                currentMats[1] = null;
                m_CurrentRenderer.materials = currentMats;
            }

            if (!m_isDragGizmo)
            {
                m_CurrentHitObject = hit.collider.gameObject;
            }

            m_CurrentRenderer = m_CurrentHitObject.GetComponent<Renderer>();
            var mats = m_CurrentRenderer.materials;
            mats[1] = m_SelectedMaterial;
            m_CurrentRenderer.materials = mats;
        }
        else if (m_CurrentRenderer != null)
        {
            var currentMats = m_CurrentRenderer.materials;
            currentMats[1] = null;
            m_CurrentRenderer.materials = currentMats;
            m_CurrentRenderer = null;
        }
    }
}
