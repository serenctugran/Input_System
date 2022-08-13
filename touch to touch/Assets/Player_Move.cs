using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; 

   

public class Player_Move : MonoBehaviour
{
   [SerializeField]
   private InputAction mouseClick;
   [SerializeField]
   private float mouseDragPhysicsSpeed = 10;
   [SerializeField]
   private float mouseDragSpeed = .1f;

   private Camera mainCamera;
   private Vector3 velocity = Vector3.zero;
   private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

   private void Awake() {
      mainCamera = Camera.main;
   }
   private void OnEnable() {
      mouseClick.Enable();
      mouseClick.performed += MousePres;
   }
   private void OnDisable() {
      mouseClick.performed -= MousePres;
      mouseClick.Disable();
   }
   private void MousePres(InputAction.CallbackContext context){
      Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
      RaycastHit hit;
      if (Physics.Raycast(ray, out hit)){
         if (hit.collider != null)
         {
            StartCoroutine(DragUpdate(hit.collider.gameObject));
         }
      }
   }
   private IEnumerator DragUpdate(GameObject clickObject){
      float initialDistance = Vector3.Distance(clickObject.transform.position, mainCamera.transform.position);
      clickObject.TryGetComponent<Rigidbody>(out var rb);
         while (mouseClick.ReadValue<float>() != 0){
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (rb != null){
               Vector3 direction = ray.GetPoint(initialDistance) - clickObject.transform.position;
               rb.velocity = direction * mouseDragPhysicsSpeed;
               yield return waitForFixedUpdate;
            }else{
               clickObject.transform.position = Vector3.SmoothDamp(clickObject.transform.position, ray.GetPoint(initialDistance), ref velocity, mouseDragSpeed);
               yield return null;
            }
         }
   }
}
