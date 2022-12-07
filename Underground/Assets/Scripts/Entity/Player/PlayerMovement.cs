using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody m_Rigidbody;
    public float m_Speed = 5f;

	[Range(0.1f, 9f)][SerializeField] float sensitivity = 2f;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; 
        //Fetch the Rigidbody from the GameObject with this script attached
        m_Rigidbody = GetComponent<Rigidbody>();
    }
        
	Vector2 rotation = Vector2.zero;
    const string xAxis = "Mouse X"; 
    void Update(){
        rotation.x += Input.GetAxis(xAxis) * sensitivity;
		var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
		var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

		m_Rigidbody.MoveRotation(xQuat * yQuat); //Quaternions seem to rotate more consistently than EulerAngles. Sensitivity seemed to change slightly at certain degrees using Euler. transform.localEulerAngles = new Vector3(-rotation.y, rotation.x, 0);
        if (Input.GetKeyDown("space")){
            m_Rigidbody.velocity += new Vector3(0,10,0);
        }
    }
    void FixedUpdate()
    {
        //Store user input as a movement vector
        Vector3 m_Input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        m_Input = this.transform.TransformDirection(m_Input);
        //Apply the movement vector to the current position, which is
        //multiplied by deltaTime and speed for a smooth MovePosition
        m_Rigidbody.MovePosition(transform.position +  m_Input * Time.deltaTime * m_Speed);
    }
}