using UnityEngine;
using Cinemachine;

//Got lazy and used this instead https://sharpcoderblog.com/blog/head-bobbing-effect-in-unity-3d
public class CameraBobbing : MonoBehaviour
{
    public float walkingBobbingSpeed = 14f;
    public float bobbingAmount = 0.05f;
    public CharacterController controller;
    public CinemachineVirtualCamera _vcam;
    private CinemachineTransposer vTransposer;
    public float defaultPosY = 0;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        vTransposer = _vcam.GetCinemachineComponent<CinemachineTransposer>();
        defaultPosY = vTransposer.m_FollowOffset.y;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 inputMovement = InputManager.instance.GetPlayerMovement();
        defaultPosY = PlayerController.ActionTriggered || PlayerController.UnderCeiling ? ChangePosY(-1) : ChangePosY(0);
        walkingBobbingSpeed = PlayerController.ActionTriggered || PlayerController.UnderCeiling ? 3 : 8;
        if (inputMovement.x != 0f || inputMovement.y != 0f)
        {
            //Player is moving
            timer += Time.deltaTime * walkingBobbingSpeed;
            vTransposer.m_FollowOffset.y = defaultPosY + Mathf.Sin(timer) * bobbingAmount;
        }
        else if(PlayerController.ActionTriggered)
        {
            //Idle
            timer = 0;
            vTransposer.m_FollowOffset.y = Mathf.Lerp(vTransposer.m_FollowOffset.y, defaultPosY, Time.deltaTime * walkingBobbingSpeed);
        }
    }

    private float ChangePosY(float N)
    {
        float results = Mathf.Lerp(defaultPosY, N, Time.deltaTime / 0.5f);
        return results;
    }
}