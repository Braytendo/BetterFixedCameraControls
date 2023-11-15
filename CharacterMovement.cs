using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class characterMovement: MonoBehaviour {

  private static Player player;
  private CharacterController MyController;
  private Camera camera;
  private Vector3 moveDirection = Vector3.zero;
  private Vector2 inputDirection2 = Vector2.zero;
  private Plane m_FloorPlane;
  //uses very small number by default so raycast positions are always close to the player
  //use a number like 0.05f if the camera is far away from the player to prevent shaky controls
  private float directionPosMultiplier = 0.0005 f;
  public float speed = 10;
  private Vector3 PlayerPos;
  public static float camChangeInputAngle;
  public static bool checkcamChangeInputAngle = false;
  private bool canChangeRotation = false;
  public static float inputAngle = 0;
  void Awake() {
    //if you don't use the Rewired input plugin from the asset store, you will need to remove or change this code and all references to "player" to match your input methods.
    player = Rewired.ReInput.players.GetPlayer(0);
    m_FloorPlane = new Plane(Vector3.up, Vector3.zero);
  }

  void Start() {

    if (!camera) {
      camera = Camera.main;
    }

    MyController = GetComponent<CharacterController>();
    if (!MyController) {
      return;
    }

    player = Rewired.ReInput.players.GetPlayer(0);
    PlayerPos = transform.position;

  }

  // Update is called once per frame
  void Update() {
    if (!camera.enabled) {
      camera = Camera.main;
    }
    if (!MyController) {
      return;
    }
    if (((transform.position.y - camera.transform.position.y) < 0.025 f && (transform.position.y - camera.transform.position.y) > -0.025 f)) {
      m_FloorPlane.SetNormalAndPosition(Vector3.up, PlayerPos);

    } else {
      PlayerPos = transform.position;
      m_FloorPlane.SetNormalAndPosition(Vector3.up, transform.position);
    }

    RaycastHit hit;

    //Bottom of controller. Slightly above ground so it doesn't bump into slanted platforms. (Adjust to your needs)
    Vector3 p1 = transform.position + Vector3.up * 1 f;
    Vector3 p2 = transform.position;

    float deadzone = 0.25 f;
    //uses character controller for 8 direction movement
    moveDirection = new Vector3(player.GetAxis("Horizontal"), 0, player.GetAxis("Vertical"));
    if (moveDirection.magnitude < deadzone)
      moveDirection = Vector3.zero;
    if (moveDirection != Vector3.zero) {

      //get input from the player

      Vector3 playerPos = camera.WorldToViewportPoint(transform.position);
      Vector3 directionPos;
      Vector2 inputDirection = new Vector2(player.GetAxis("Horizontal"), player.GetAxis("Vertical")).normalized;
      directionPos = new Vector3(playerPos.x + (inputDirection.x * directionPosMultiplier), playerPos.y + (inputDirection.y * directionPosMultiplier), playerPos.z);

      // Cast ray back to floor plane.
      float lDist = 0, lDistTop2 = 0, lDistBottom2 = 0, lDistLeft2 = 0, lDistRight2 = 0;

      Vector3 playerPos3 = camera.WorldToViewportPoint(transform.position);
      Vector3 directionTop = new Vector3(playerPos3.x, playerPos3.y + (1 * directionPosMultiplier), playerPos3.z);
      Vector3 directionBottom = new Vector3(playerPos3.x, playerPos3.y - (1 * directionPosMultiplier), playerPos3.z);
      Vector3 directionLeft = new Vector3(playerPos3.x - (1 * directionPosMultiplier), playerPos3.y, playerPos3.z);
      Vector3 directionRight = new Vector3(playerPos3.x + (1 * directionPosMultiplier), playerPos3.y, playerPos3.z);
      Ray lRayTop = camera.ViewportPointToRay(directionTop);
      Ray lRayBottom = camera.ViewportPointToRay(directionBottom);
      Ray lRayLeft = camera.ViewportPointToRay(directionLeft);
      Ray lRayRight = camera.ViewportPointToRay(directionRight);
      Vector3 lNewPosTop = Vector3.zero;
      if (m_FloorPlane.Raycast(lRayTop, out lDistTop2)) {
        lNewPosTop = lRayTop.origin + (lRayTop.direction * lDistTop2);
      }
      Vector3 lNewPosBottom = Vector3.zero;
      if (m_FloorPlane.Raycast(lRayBottom, out lDistBottom2)) {
        lNewPosBottom = lRayBottom.origin + (lRayBottom.direction * lDistBottom2);
      }
      Vector3 lNewPosLeft = Vector3.zero;
      if (m_FloorPlane.Raycast(lRayLeft, out lDistLeft2)) {
        lNewPosLeft = lRayLeft.origin + (lRayLeft.direction * lDistLeft2);
      }

      Vector3 lNewPosRight = Vector3.zero;
      if (m_FloorPlane.Raycast(lRayRight, out lDistRight2)) {
        lNewPosRight = lRayRight.origin + (lRayRight.direction * lDistRight2);
      }

      Ray lRay = camera.ViewportPointToRay(directionPos);
      if (m_FloorPlane.Raycast(lRay, out lDist)) {

        Vector3 playerAbovePos = Vector3.zero;
        Vector3 lNewPosTop2 = lNewPosTop, lNewPosLeft2 = lNewPosLeft, lNewPosRight2 = lNewPosRight;
        if (camera.transform.rotation.z > 0) {
          lNewPosLeft2 = lNewPosTop - (lNewPosBottom - lNewPosLeft);
          lNewPosRight2 = lNewPosBottom - (lNewPosTop - lNewPosRight);
        } else if (camera.transform.rotation.z < 0) {
          lNewPosLeft2 = lNewPosBottom - (lNewPosTop - lNewPosLeft);
          lNewPosRight2 = lNewPosTop - (lNewPosBottom - lNewPosRight);
        }

        if (!m_FloorPlane.GetSide(lRay.origin)) {
          lNewPosTop2 = lNewPosTop;
          lNewPosTop = lNewPosBottom;
          lNewPosBottom = lNewPosTop2;
          lNewPosRight = lNewPosRight2;
          lNewPosLeft = lNewPosLeft2;

        }

        if (inputDirection != inputDirection2) {
          inputAngle = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg;
        }

        if (checkcamChangeInputAngle) {

          if (inputAngle > camChangeInputAngle - 6 && inputAngle < camChangeInputAngle + 6 && moveDirection != Vector3.zero) {

            canChangeRotation = false;
          } else {
            camChangeInputAngle = 1000 f;
            checkcamChangeInputAngle = false;
          }
        } else {
          canChangeRotation = true;
        }

        Vector3 topDir = lNewPosTop - transform.position;
        Vector3 rightDir = lNewPosRight - transform.position;
        Vector3 bottomDir = lNewPosBottom - transform.position;
        Vector3 leftDir = lNewPosLeft - transform.position;
        float angleTopRight = Vector3.Angle(topDir, rightDir);
        float angleBottomRight = Vector3.Angle(rightDir, bottomDir);
        float angleTopLeft = Vector3.Angle(topDir, leftDir);
        float angleBottomLeft = Vector3.Angle(leftDir, bottomDir);

        Quaternion rotation = Quaternion.Euler(0, 0, 0);
        float inputDegrees = 0;
        if (player.GetAxis("Vertical") >= 0 && player.GetAxis("Horizontal") >= 0) {
          inputDegrees = (inputAngle / 90) * angleTopRight;
          rotation = Quaternion.LookRotation(topDir);

        } else if (player.GetAxis("Vertical") <= 0 && player.GetAxis("Horizontal") >= 0) {
          inputDegrees = ((inputAngle - 90) / 90) * angleBottomRight;
          rotation = Quaternion.LookRotation(rightDir);

        } else if (player.GetAxis("Vertical") >= 0 && player.GetAxis("Horizontal") <= 0) {
          inputDegrees = (inputAngle / 90) * angleTopLeft;
          rotation = Quaternion.LookRotation(topDir);

        } else if (player.GetAxis("Vertical") <= 0 && player.GetAxis("Horizontal") <= 0) {
          inputDegrees = ((inputAngle + 90) / 90) * angleBottomLeft;
          rotation = Quaternion.LookRotation(leftDir);

        }

        rotation *= Quaternion.Euler(0, inputDegrees, 0);
        rotation.x = 0;
        rotation.z = 0;
        if (!((transform.position.y - camera.transform.position.y) < 0.04 f && (transform.position.y - camera.transform.position.y) > -0.04 f)) {
          if (canChangeRotation) {
            if (inputDirection != inputDirection2) {
              transform.rotation = rotation;
            }
          }

        }

      }
      inputDirection2 = inputDirection;
      moveDirection = transform.forward;
      moveDirection.y = 0;
      moveDirection = Vector3.ClampMagnitude(moveDirection, 1 f);

      float inputMultiplier = Mathf.Abs(player.GetAxis("Horizontal")) + Mathf.Abs(player.GetAxis("Vertical"));
      inputMultiplier = Mathf.Clamp(inputMultiplier, 0 f, 1 f);

      moveDirection = moveDirection * speed * inputMultiplier * Time.deltaTime;

    }

    MyController.Move(moveDirection);

  }

  public Vector3 ReflectionOverPlane(Vector3 point, Plane plane) {
    Vector3 N = transform.TransformDirection(plane.normal);
    return point - 2 * N * Vector3.Dot(point, N) / Vector3.Dot(N, N);
  }

}
