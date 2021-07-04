using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraShift : MonoBehaviour
{
    [SerializeField, Range(5, 30), Tooltip("Зона экрана в процентах от края, чувствительная для сдвига камеры")] 
    private float areaShiftSizeSensitivity;

    [SerializeField, Range(1, 10), Tooltip("Величина смещения камеры")]
    private float cameraShiftDistance;

    [SerializeField, Tooltip("Скорость смещения камеры")]
    private float cameraShiftSpeed;

    private CinemachineVirtualCamera cineCam;
    private CinemachineTransposer cineCamTransposer;

    private float screenWidth; //текущая ширина экрана
    private float screenHeight; //текущая высота экрана

    private Vector3 startZeroShift; //стартовое положение камеры относительно игрока
    

    void Awake()
    {
        cineCam = GetComponent<CinemachineVirtualCamera>();
        cineCamTransposer = cineCam.GetCinemachineComponent<CinemachineTransposer>();
    }

    private void Start()
    {
        startZeroShift = cineCamTransposer.m_FollowOffset; //записываем стартовую позицию камеры для возможности возврата в нее
    }


    private void FixedUpdate()
    {
        screenWidth = Camera.main.pixelWidth; //постоянно отслеживаем размеры экрана на случай изменения размера окна в процессе игры
        screenHeight = Camera.main.pixelHeight;

        MousePositionControl();
    }

    /// <summary>
    /// Отслеживание положения мыши. В случае выхода курсора в зону areaShiftSizeSensitivity в процентах от края экрана, 
    /// применяется метод сдвига камеры, в ином случае, камера возвращается в стартовое положение
    /// </summary>
    private void MousePositionControl()
    {
        Vector3 mousePosition = Input.mousePosition;
        
        //Ось Х экрана
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

        //Ось У экрана
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
    /// Смещает камеру плавно в исходную позицию
    /// </summary>
    private void CameraAtStartPosition()
    {
        cineCamTransposer.m_FollowOffset = Vector3.Lerp(cineCamTransposer.m_FollowOffset, startZeroShift, cameraShiftSpeed / 100);
    }

    /// <summary>
    /// Смещает камеру плавно в сторону заданного вектора
    /// </summary>
    /// <param name="shiftVector">Целевой вектор смещения</param>
    private void CameraShifter(Vector3 shiftVector)
    {
        cineCamTransposer.m_FollowOffset = Vector3.Lerp(cineCamTransposer.m_FollowOffset, shiftVector, cameraShiftSpeed / 100);
    }
}
