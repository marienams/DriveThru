using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static bool GameOverState 
   {
    get {return isGameOver;}
    set {isGameOver=value;}
    }
    
    [SerializeField]float speed = 5.0f;
    [SerializeField]float turnSpeed;
    [SerializeField]float maxTiltAngle;
    [SerializeField] Camera mainCamera;
    [SerializeField] Camera hoodCamera;
    [SerializeField] KeyCode switchCameraKey;
    [SerializeField] string inputID; //for local multiplayer
    [SerializeField] int delayBeforeReset;
    [SerializeField] float targetYPosition = -5.0f; // The Y position to check for
    Quaternion originalRotation;
    float horizontalInput;
    
    float forwardInput;
    bool IsToppledOver;
    static bool isGameOver;
   
    // Start is called before the first frame update
    void Start()
    {
        originalRotation = transform.rotation;
        isGameOver = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isGameOver){
            PlayerMovement();
            PlayerOffTrack();
            if(IsPlayerTilted() && !IsToppledOver){
                IsToppledOver = true;
                StartCoroutine(SnapBackToOriginalRotation());
            }
        }
        else {
            StartCoroutine(GameOver());
        }
        
    }

    
    void PlayerMovement()
    {

        //we will move forward our vehicle
        //horizontalInput = Input.GetAxis("Horizontal");
        //forwardInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("horizontal");
        forwardInput = Input.GetAxis("vertical");
        transform.Translate(Vector3.forward * Time.deltaTime * speed *forwardInput);
        //transform.Rotate(Vector3.forward * Time.deltaTime * turnSpeed *horizontalInput);
        transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);

        if (Input.GetKeyDown(switchCameraKey))                        // condition to switch camera through key press
        {
            mainCamera.enabled = !mainCamera.enabled;
            hoodCamera.enabled = !hoodCamera.enabled;
        }
    }
    void PlayerOffTrack(){
        if(transform.position.y < targetYPosition){
            //Game ends if player falls off track
            isGameOver = true;
            Debug.Log("Player fell over");
            StartCoroutine(GameOver());
        }
    }

    bool IsPlayerTilted()
    {
        if(isGameOver){
            //returning false means snap back will not work
            Debug.Log($"Game Over: {isGameOver}No snap back in place");
            return false;
        }
        float angleFromForward = Vector3.Angle(transform.up, Vector3.up);
        return angleFromForward > originalRotation.z;
    }
    IEnumerator SnapBackToOriginalRotation(){
        
        yield return new WaitForSeconds(delayBeforeReset);
         // Smoothly rotate back to original rotation over 0.5 seconds
        float elapsedTime = 0f;
        float duration = 0.5f;  // Duration of the smooth rotation
        Quaternion startingRotation = transform.rotation;

        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Slerp(startingRotation, originalRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the rotation is set exactly to the original
        transform.rotation = originalRotation;

        IsToppledOver = false;
    }

    IEnumerator GameOver(){
        yield return new WaitForSeconds(delayBeforeReset);
        SceneManager.LoadScene(2);
    }

}

