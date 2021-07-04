using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraShift : MonoBehaviour
{
    [SerializeField, Range(5, 30), Tooltip("���� ������ � ��������� �� ����, �������������� ��� ������ ������")] 
    private float areaShiftSizeSensitivity;

    [SerializeField, Range(1, 10), Tooltip("�������� �������� ������")]
    private float cameraShiftDistance;

    [SerializeField, Tooltip("�������� �������� ������")]
    private float cameraShiftSpeed;

    private CinemachineVirtualCamera cineCam;
    private CinemachineTransposer cineCamTransposer;

    private float screenWidth; //������� ������ ������
    private float screenHeight; //������� ������ ������

    private Vector3 startZeroShift; //��������� ��������� ������ ������������ ������
    

    void Awake()
    {
        cineCam = GetComponent<CinemachineVirtualCamera>();
        cineCamTransposer = cineCam.GetCinemachineComponent<CinemachineTransposer>();
    }

    private void Start()
    {
        startZeroShift = cineCamTransposer.m_FollowOffset; //���������� ��������� ������� ������ ��� ����������� �������� � ���
    }


    private void FixedUpdate()
    {
        screenWidth = Camera.main.pixelWidth; //��������� ����������� ������� ������ �� ������ ��������� ������� ���� � �������� ����
        screenHeight = Camera.main.pixelHeight;

        MousePositionControl();
    }

    /// <summary>
    /// ������������ ��������� ����. � ������ ������ ������� � ���� areaShiftSizeSensitivity � ��������� �� ���� ������, 
    /// ����������� ����� ������ ������, � ���� ������, ������ ������������ � ��������� ���������
    /// </summary>
    private void MousePositionControl()
    {
        Vector3 mousePosition = Input.mousePosition;
        
        //��� � ������
        if (mousePosition.x > screenWidth * (1 - areaShiftSizeSensitivity / 100))
        {
            CameraShifter(new Vector3(-cameraShiftDistance, 0, 0) + startZeroShift);            
        }
        else if (mousePosition.x < screenWidth * areaShiftSizeSensitivity / 100)
        {
            CameraShifter(new Vector3(cameraShiftDistance, 0, 0) + startZeroShift);
        }
        else
        {
            CameraAtStartPosition();
        }

        //��� � ������
        if (mousePosition.y > screenHeight * (1 - areaShiftSizeSensitivity / 100)) 
        {
            CameraShifter(new Vector3(0, 0, -cameraShiftDistance) + startZeroShift);
        }
        else if (mousePosition.y < screenHeight * areaShiftSizeSensitivity / 100) 
        {
            CameraShifter(new Vector3(0, 0, cameraShiftDistance) + startZeroShift);
        }
        else
        {
            CameraAtStartPosition();
        }
    }

    /// <summary>
    /// ������� ������ ������ � �������� �������
    /// </summary>
    private void CameraAtStartPosition()
    {
        cineCamTransposer.m_FollowOffset = Vector3.Lerp(cineCamTransposer.m_FollowOffset, startZeroShift, cameraShiftSpeed / 100);
    }

    /// <summary>
    /// ������� ������ ������ � ������� ��������� �������
    /// </summary>
    /// <param name="shiftVector">������� ������ ��������</param>
    private void CameraShifter(Vector3 shiftVector)
    {
        cineCamTransposer.m_FollowOffset = Vector3.Lerp(cineCamTransposer.m_FollowOffset, shiftVector, cameraShiftSpeed / 100);
    }
}
