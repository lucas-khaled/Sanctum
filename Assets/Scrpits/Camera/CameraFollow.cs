using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float cameraMoveSpeed = 120.0f; // Velocidade de movimento da camera
    public GameObject cameraFollowObject; // Objeto que a câmera segue
    public float clampAngle = 80.0f; // Angulo Maximo da câmera (evita que o player fique dando 360º na camera envolta do personagem)
    public float inputSensitivity = 250.0f; //Sensibilidade do mouse com a câmera (pode ser configurado)
    public GameObject cameraObject; // Objeto camera
    public GameObject playerObject; // Objeto Player
    public float smoothX; // Suavização da câmera em X
    public float smoothY; // Suavização da câmera em Y
    private float rotY = 0.0f; 
    private float rotX = 0.0f;


    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles; // Não entendi a parada do angulo euler
        rotY = rot.y;
        rotX = rot.x;

        //Loca o cursor e some com ele
        //IMPORTANTE: No pause tem que voltar com ele**
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
       // Olhar imagem dos inputs
       float inputX = Input.GetAxis("RightStickHorizontal"); // Pega um input criado por mim
       float inputZ = Input.GetAxis("RightStickVertical"); // Pega um input criado por mim

       rotY += inputX + Input.GetAxis("Mouse X") * inputSensitivity * Time.deltaTime; //Lincado ao euler que não entendi
       rotX += inputZ + Input.GetAxis("Mouse Y") * inputSensitivity * Time.deltaTime; //Lincado ao euler que não entendi

       rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle); // o clampAngle é o ângulo que limita o jogador ficar dando 360º no player

       Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f); //Lincado ao euler que não entendi
       transform.rotation = localRotation; // muda a rotação
    }

    void LateUpdate()
    {
        CameraUpdater(); //fazer a Camera seguir o player
    }

    void CameraUpdater()
    {
         Transform target = cameraFollowObject.transform;

         float step = cameraMoveSpeed * Time.deltaTime;
         transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }
}
