using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ConcreateFactoryCreature : MonoBehaviour
{
    List<Creature> loadedCreatureList;
    float counter = 0;

    void OnEnable()
    {
        loadedCreatureList = new List<Creature>();
    }

    void Start()
    {
        Creature creature = CreateCreature("Abra", new Vector3(-11.6953087f,7f,-5.6152215f));
    }
    

    public Creature CreateRandomCreature(Vector3 position)
    {
        string[] dir = Directory.GetDirectories("Assets/Resources/Creatures/PokemonXY");
        int n = Random.Range(0, dir.Length);
        
        string[] data = dir[n].Split(new char[]{'\\'});

        try
        {
            Creature creature = CreateCreature(data[data.Length-1], new Vector3(-11.6953087f,7f,-5.6152215f));
            return creature;
        }
        catch (System.Exception)
        {
            
        }
        return null;
    }
    public Creature CreateCreature(string name, Vector3 position)
    {
        

        GameObject loadedObject = Resources.Load("Creatures/PokemonXY/"+name+"/"+name) as GameObject;
        
        GameObject o = Instantiate(loadedObject);
        //o.AddComponent<BoxCollider>();
        /*
        Rigidbody rigidbody = o.AddComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rigidbody.drag = 1.5f;
        rigidbody.angularDrag = 1.5f;
        */
        RigBuilder rigBuilder = o.AddComponent<RigBuilder>();
        
        List<IKFootSolver2> solverList = BuildRig(o.transform, rigBuilder);
        BoneRenderer boneRenderer = o.AddComponent<BoneRenderer>();
        Animator animator = o.GetComponent<Animator>();
        RuntimeAnimatorController loadedAnimatorController = Resources.Load("Creatures/PokemonXY/"+name+"/animation/"+name) as RuntimeAnimatorController;
        if(loadedAnimatorController!=null)
        {
            animator.runtimeAnimatorController = loadedAnimatorController;
        }
        Transform innerObj = o.transform.Find(name);

        SkinnedMeshRenderer renderer = innerObj.GetComponent<SkinnedMeshRenderer>();
        Vector3 size = renderer.bounds.size;

        MeshCollider collider = innerObj.gameObject.AddComponent<MeshCollider>();
        Mesh colliderMesh = new Mesh();
        renderer.BakeMesh(colliderMesh);
        collider.sharedMesh = null;
        collider.sharedMesh = colliderMesh;
        collider.convex =true;
        
        
        GameObject loadedHealthbarObject = Resources.Load("Creatures/Healthbar Canvas") as GameObject;
        GameObject healthbarObject = Instantiate(loadedHealthbarObject);
        healthbarObject.transform.localPosition = Vector3.up * size.y;
        HealthBar bar = healthbarObject.transform.GetChild(0).GetComponent<HealthBar>();
        
        Creature defaultCrature = o.AddComponent<Walking>();
        healthbarObject.transform.parent = defaultCrature.transform;
        defaultCrature.healthBar = bar;
        defaultCrature.speed = size.magnitude/4;
        defaultCrature.brothers = solverList;
        defaultCrature.followDistance = size.magnitude/2+1;
        defaultCrature.SetFollowTarget(GameObject.Find("Player").transform);
        defaultCrature.transform.position = position;
        
        foreach (var item in loadedCreatureList)
        {
            Destroy(item.gameObject);
        }
        
        loadedCreatureList.Clear();
        loadedCreatureList.Add(defaultCrature);
        return defaultCrature;
    }

    private List<IKFootSolver2> BuildRig(Transform root, RigBuilder rigBuilder)
    {
        List<IKFootSolver2> solver = new List<IKFootSolver2>();
        GameObject rigGameObject = new GameObject("Rig");
        rigGameObject.transform.parent = root;
        rigGameObject.transform.localPosition = Vector3.zero;
        Rig rig = rigGameObject.AddComponent<Rig>();

        //rarm
        IKFootSolver2 rArmSolver = create2Bone(root, rigGameObject.transform, new string[]{"RArm", "RForeArm", "RHand"});
        IKFootSolver2 lArmSolver = create2Bone(root, rigGameObject.transform, new string[]{"LArm", "LForeArm", "LHand"});
        IKFootSolver2 rLegSolver = create2Bone(root, rigGameObject.transform, new string[]{"RThigh", "RLeg", "RFoot"});
        IKFootSolver2 lLegSolver = create2Bone(root, rigGameObject.transform, new string[]{"LThigh", "LLeg", "LFoot"});

        Transform origin = CustomFindChild("Origin", root);
        MultiPositionConstraint positionConstraint = null;;
        if(origin!=null)
            {
            GameObject originConstraintGameObject = new GameObject("Origin");
            originConstraintGameObject.transform.parent = rigGameObject.transform;
            originConstraintGameObject.transform.localPosition = Vector3.zero;
            positionConstraint = originConstraintGameObject.AddComponent<MultiPositionConstraint>();
            positionConstraint.data.constrainedObject = origin;
            positionConstraint.data.sourceObjects = new WeightedTransformArray(4);
            positionConstraint.weight = 0.2f;
            positionConstraint.data.maintainOffset = true;
            positionConstraint.data.constrainedXAxis = true;
            positionConstraint.data.constrainedYAxis = true;
            positionConstraint.data.constrainedZAxis = true;
         
        }
        WeightedTransformArray arr = new WeightedTransformArray(4);
        if(rArmSolver!=null && lArmSolver!=null){
            rArmSolver.brothers.Add(lArmSolver);
            lArmSolver.brothers.Add(rArmSolver);
            solver.Add(rArmSolver);
            solver.Add(lArmSolver);
            if(origin!=null)
            {
                arr[0] = new WeightedTransform(rArmSolver.transform, 1f);
                arr[1] = new WeightedTransform(lArmSolver.transform, 1f);
                Debug.Log("add");
            }
        }
        if(rLegSolver!=null && lLegSolver!=null){
            rLegSolver.brothers.Add(lLegSolver);
            lLegSolver.brothers.Add(rLegSolver);
            solver.Add(rLegSolver);
            solver.Add(lLegSolver);
            if(origin!=null)
            {
                arr[2] = new WeightedTransform(rLegSolver.transform, 1f);
                arr[3] = new WeightedTransform(lLegSolver.transform, 1f);
            }
        }
        
        if(origin!=null)
        {
            positionConstraint.data.sourceObjects = arr;
        }
        if(rArmSolver!=null && lArmSolver!=null && rLegSolver!=null && lLegSolver!=null){
            rLegSolver.brothers.Add(lArmSolver);
            lLegSolver.brothers.Add(rArmSolver);
            rArmSolver.brothers.Add(lLegSolver);
            lArmSolver.brothers.Add(rLegSolver);
        }

        rigBuilder.layers.Add(new RigLayer(rig, true));
        rigBuilder.enabled = true;
        rigBuilder.Build();
        return solver;
    }

    private IKFootSolver2 create2Bone(Transform root, Transform rigTransform, string[] names)
    {
        Transform rArm = CustomFindChild(names[0], root);
        if(rArm == null)
            return null;
        Transform rForeArm = rArm.Find(names[1]);
        if(rForeArm == null)
            return null;
        Transform rHand = rForeArm.Find(names[2]);
        if(rHand == null)
            return null;
        
        GameObject rArmConstraintGameObject = new GameObject(names[0]);
        rArmConstraintGameObject.transform.parent = rigTransform;
        rArmConstraintGameObject.transform.localPosition = Vector3.zero;
        
        GameObject rHandTargetConstraintGameObject = new GameObject("target");
        rHandTargetConstraintGameObject.transform.parent = rArmConstraintGameObject.transform;
        //rHandTargetConstraintGameObject.transform.localScale *= 0.1f;
        rHandTargetConstraintGameObject.transform.localPosition = rHandTargetConstraintGameObject.transform.localPosition + rArm.position;
                
        GameObject rHandCastConstraintGameObject = new GameObject("cast");
        rHandCastConstraintGameObject.transform.parent = rArmConstraintGameObject.transform;
        //rHandCastConstraintGameObject.transform.localScale *= 0.1f;
        rHandCastConstraintGameObject.transform.localPosition = rHandCastConstraintGameObject.transform.localPosition + rHand.position;
        
        IKFootSolver2 rArmSolver = rHandTargetConstraintGameObject.AddComponent<IKFootSolver2>();
        rArmSolver.body = root;
        rArmSolver.castPosition = rHandCastConstraintGameObject.transform;

        
        float magnitude = (rHand.position - rArm.position).magnitude;
        GameObject rHandHintConstraintGameObject = new GameObject("hint");
        rHandHintConstraintGameObject.transform.parent = rArmConstraintGameObject.transform;
        //rHandHintConstraintGameObject.transform.localScale *= 0.1f;
        rHandHintConstraintGameObject.transform.localPosition+= rHand.position + new Vector3(0,1,1) * magnitude;

        rArmSolver.stepDistance = magnitude*1.5f;
        rArmSolver.speed = 1/magnitude+1f;
        rArmSolver.stepHeight = magnitude*0.75f;

        TwoBoneIKConstraint rhandConstraint = rArmConstraintGameObject.AddComponent<TwoBoneIKConstraint>();
        rhandConstraint.data.root = rArm;
        rhandConstraint.data.mid = rForeArm;
        rhandConstraint.data.tip = rHand;

        rhandConstraint.data.target = rHandTargetConstraintGameObject.transform;
        rhandConstraint.data.hint = rHandHintConstraintGameObject.transform;

        rhandConstraint.data.targetPositionWeight = 1f;
        rhandConstraint.data.hintWeight = 1f;

        return rArmSolver;
    }

    public Creature GetCreature()
    {
        if(loadedCreatureList.Count>0)
            return loadedCreatureList[0];
        return null;
    }
    private Transform CustomFindChild(string key, Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.name == key)
            {
                return child;
            } else
            {
                if (child.childCount > 0)
                {
                    var tmp = CustomFindChild(key, child);
                    if(tmp != null)
                        return tmp;
                }
            }
        }

        return null;
    }
}
