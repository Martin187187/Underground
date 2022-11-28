using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    public MeshGenerator generator;
    public Vector3 firstPosition, secondPosition, thirdPosition, selectedPosition;
    public GameObject markerBox;
    public Transform viewer;
    public MarkerMode mode = MarkerMode.None;
    private Axe axe;

    private float step;
    void OnEnable(){
        step = generator.getStep();
    }
    void Update() {


        switch(mode){
            case MarkerMode.None:
                markerBox.transform.position = selectedPosition;
                markerBox.transform.localScale = Vector3.one*step;
                break;
            case MarkerMode.One:
                Vector3 direction = viewer.transform.TransformDirection(Vector3.forward);
                Vector3 positionDirection = firstPosition - viewer.position;
                
                
                float angleY = Vector3.Angle(direction, Vector3.up);
                float angleX = Vector3.Angle(direction, Vector3.right);
                float angleZ = Vector3.Angle(direction, Vector3.forward);
                float angleYR = Vector3.Angle(direction, Vector3.down);
                float angleXR = Vector3.Angle(direction, Vector3.left);
                float angleZR = Vector3.Angle(direction, Vector3.back);
                
                Debug.DrawRay(firstPosition, Vector3.up*10, Color.red);
                // xz area with x lock
                if((angleX <= angleZ || angleXR <= angleZR) && Mathf.Abs(direction.x) >= Mathf.Abs(direction.z)&&
                    (angleX <= angleY || angleXR <= angleYR) && Mathf.Abs(direction.x) >= Mathf.Abs(direction.y)){

                    float xZero = positionDirection.x/direction.magnitude;
                    Vector3 dis = generator.getIndexFromPositionRasterd(angleX <= angleY ? viewer.position + xZero * direction.normalized : viewer.position - xZero * direction.normalized);
                    //hit poinz in xz area
                    Vector3 hitpoint = new Vector3( firstPosition.x, dis.y, dis.z);
                    secondPosition = generator.getIndexFromPositionRasterd(hitpoint);
                    markerBox.transform.localScale = new Vector3(step, Mathf.Abs(dis.y- firstPosition.y)+step, Mathf.Abs(dis.z- firstPosition.z)+step);
                    axe = Axe.X;
                    
                }

                if((angleZ <= angleX || angleZR <= angleXR) && Mathf.Abs(direction.z) >= Mathf.Abs(direction.x)&&
                    (angleZ <= angleY || angleZR <= angleYR) && Mathf.Abs(direction.z) >= Mathf.Abs(direction.y)){

                    float xZero = positionDirection.z/direction.magnitude;
                    Vector3 dis = generator.getIndexFromPositionRasterd(angleZ <= angleY ? viewer.position + xZero * direction.normalized : viewer.position - xZero * direction.normalized);
                    //hit poinz in xz area
                    Vector3 hitpoint = new Vector3( dis.x, dis.y, firstPosition.z);
                    secondPosition = generator.getIndexFromPositionRasterd(hitpoint);
                    markerBox.transform.localScale = new Vector3(Mathf.Abs(dis.x- firstPosition.x)+step, Mathf.Abs(dis.y- firstPosition.y)+step, step);
                    axe = Axe.Z;
                    
                } 
                if((angleY <= angleX || angleYR <= angleXR) && Mathf.Abs(direction.y) >= Mathf.Abs(direction.x)&&
                    (angleY <= angleZ || angleYR <= angleZR) && Mathf.Abs(direction.y) >= Mathf.Abs(direction.z)){

                    float xZero = positionDirection.y/direction.magnitude;
                    Vector3 dis = generator.getIndexFromPositionRasterd(angleY <= angleZ ? viewer.position + xZero * direction.normalized : viewer.position - xZero * direction.normalized);
                    //hit poinz in xz area
                    Vector3 hitpoint = new Vector3( dis.x, firstPosition.y, dis.z);
                    secondPosition = generator.getIndexFromPositionRasterd(hitpoint);
                    markerBox.transform.localScale = new Vector3(Mathf.Abs(dis.x- firstPosition.x)+step, step, Mathf.Abs(dis.z- firstPosition.z)+step);
                    axe = Axe.Y;
                    
                } 

                
                markerBox.transform.position = (firstPosition + secondPosition)/2;
                
                break;
                
            case MarkerMode.Two:
                
                Debug.DrawRay(firstPosition, Vector3.up*10, Color.red);
                Debug.DrawRay(secondPosition, Vector3.up*10, Color.blue);
                direction = viewer.transform.TransformDirection(Vector3.forward);
                positionDirection = secondPosition - viewer.position;
                
                
                angleY = Vector3.Angle(direction, Vector3.up);
                angleX = Vector3.Angle(direction, Vector3.right);
                angleZ = Vector3.Angle(direction, Vector3.forward);
                angleYR = Vector3.Angle(direction, Vector3.down);
                angleXR = Vector3.Angle(direction, Vector3.left);
                angleZR = Vector3.Angle(direction, Vector3.back);
                if(axe == Axe.X){
                    float xZero = positionDirection.z/direction.magnitude;
                    Vector3 dis = generator.getIndexFromPositionRasterd(angleZ <= angleY ? viewer.position + xZero * direction.normalized : viewer.position - xZero * direction.normalized);
                    //hit poinz in xz area
                    Vector3 hitpoint = new Vector3(dis.x, secondPosition.y, secondPosition.z);
                    thirdPosition = generator.getIndexFromPositionRasterd(hitpoint);
                    markerBox.transform.localScale = new Vector3(Mathf.Abs(dis.x- secondPosition.x)+step, markerBox.transform.localScale.y, markerBox.transform.localScale.z);
                    
                    Vector3 help = new Vector3(markerBox.transform.position.x, markerBox.transform.position.y, thirdPosition.z);
                    markerBox.transform.position = (firstPosition + thirdPosition)/2;
                }
                if(axe == Axe.Z){
                    float xZero = positionDirection.x/direction.magnitude;
                    Vector3 dis = generator.getIndexFromPositionRasterd(angleX <= angleY ? viewer.position + xZero * direction.normalized : viewer.position - xZero * direction.normalized);
                    //hit poinz in xz area
                    Vector3 hitpoint = new Vector3(secondPosition.x, secondPosition.y, dis.z);
                    markerBox.transform.localScale = new Vector3(markerBox.transform.localScale.x, markerBox.transform.localScale.y, Mathf.Abs(dis.z- secondPosition.z)+step);
                    thirdPosition = generator.getIndexFromPositionRasterd(hitpoint);

                    Vector3 help = new Vector3(thirdPosition.x, markerBox.transform.position.y, markerBox.transform.position.z);
                    markerBox.transform.position = (firstPosition + thirdPosition)/2;
                }
                if(axe == Axe.Y){
                    float xZero = positionDirection.y/direction.magnitude;
                    Vector3 dis = generator.getIndexFromPositionRasterd(viewer.position - xZero * direction.normalized);
                    //hit poinz in xz area
                    Vector3 hitpoint = new Vector3(secondPosition.x, dis.y, secondPosition.z);

                    markerBox.transform.localScale = new Vector3(markerBox.transform.localScale.x, Mathf.Abs(dis.y- secondPosition.y)+step,markerBox.transform.localScale.z);
                    thirdPosition = generator.getIndexFromPositionRasterd(hitpoint);

                    Vector3 help = new Vector3(markerBox.transform.position.x, thirdPosition.y, markerBox.transform.position.z);
                    markerBox.transform.position = (firstPosition + thirdPosition)/2;
                }
                break;
        }
    }

    public void SetActiveBox(bool b){
        markerBox.gameObject.SetActive(b);
    }
    public bool isInBox(Vector3 pos){
        Vector3 start = new Vector3(Mathf.Min(firstPosition.x, Mathf.Min(secondPosition.x, thirdPosition.x)),
            Mathf.Min(firstPosition.y, Mathf.Min(secondPosition.y, thirdPosition.y)),
            Mathf.Min(firstPosition.z, Mathf.Min(secondPosition.z, thirdPosition.z)));
        Vector3 end = new Vector3(Mathf.Max(firstPosition.x, Mathf.Max(secondPosition.x, thirdPosition.x)),
            Mathf.Max(firstPosition.y, Mathf.Max(secondPosition.y, thirdPosition.y)),
            Mathf.Max(firstPosition.z, Mathf.Max(secondPosition.z, thirdPosition.z)));
        return start.x <= pos.x && start.y <= pos.y && start.z <= pos.z && end.x >= pos.x && end.y >= pos.y && end.z >= pos.z; 
    }

    public bool isAtEdge(Vector3 pos){
        Vector3 start = new Vector3(Mathf.Min(firstPosition.x, Mathf.Min(secondPosition.x, thirdPosition.x)),
            Mathf.Min(firstPosition.y, Mathf.Min(secondPosition.y, thirdPosition.y)),
            Mathf.Min(firstPosition.z, Mathf.Min(secondPosition.z, thirdPosition.z)));
        Vector3 end = new Vector3(Mathf.Max(firstPosition.x, Mathf.Max(secondPosition.x, thirdPosition.x)),
            Mathf.Max(firstPosition.y, Mathf.Max(secondPosition.y, thirdPosition.y)),
            Mathf.Max(firstPosition.z, Mathf.Max(secondPosition.z, thirdPosition.z)));
        return isInBox(pos) && (Mathf.Abs(start.x - pos.x+step)<step*0.5f || Mathf.Abs(start.y - pos.y)<step*0.5f || Mathf.Abs(start.z - pos.z+step)<step*0.5f || 
        Mathf.Abs(end.x - pos.x)<step*0.5f || Mathf.Abs(end.y - pos.y-step)<step*0.5f || Mathf.Abs(end.z - pos.z)<step*0.5f);
    }

    public bool needSelected(){
        return mode == MarkerMode.None;
    }

    public bool isMarked(){
        return mode == MarkerMode.Three;
    }

    public void AddPosition(Vector3 pos){
        switch(mode){
            case MarkerMode.None:
                mode = MarkerMode.One;
                firstPosition = pos;
                break;
            case MarkerMode.One:
                mode = MarkerMode.Two;
                break;
            case MarkerMode.Two:
                mode = MarkerMode.Three;
                break;


        }
    }


    public void Reset(){
        mode = MarkerMode.None;
    }
    
    public enum MarkerMode {
        None, One, Two, Three
    }

    public enum Axe {
        X, Y, Z
    }
}
