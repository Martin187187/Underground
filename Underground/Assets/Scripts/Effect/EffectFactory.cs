using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EffectFactory : MonoBehaviour
{
    public List<Particle> projectileList;
    protected Executer exe;

    void Start()
    {
        projectileList = new List<Particle>();
        exe = new Executer(this);
    }

    public List<Particle> GetProjectileList()
    {
        return projectileList;
    }

    public void RemoveParticle(Particle obj)
    {
        projectileList.Remove(obj);
        Destroy(obj.gameObject);
    }

    public void CreateRandomEffect(Vector3 position, Quaternion rotation, Transform parent)
    {        
        string[] dir = Directory.GetFiles("Assets/52SpecialEffectPack/Effect/Effect(Shuriken)");
        int n = Random.Range(0, dir.Length);
        Debug.Log(dir);
        string[] data = dir[n].Split(new char[]{'\\'});
        CreateEffect(data[data.Length-1], position, rotation, parent);
    }
    public void CreateEffect(string name, Vector3 position , Quaternion rotation, Transform parent)
    {
        Object loadedEffect = AssetDatabase.LoadAssetAtPath("Assets/52SpecialEffectPack/Effect/Effect(Shuriken)/"+ name, typeof(Object));
        GameObject obj = Instantiate(loadedEffect, position, rotation) as GameObject;
        var psys = obj.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in psys)
        {
            var main = ps.main;
            main.scalingMode = ParticleSystemScalingMode.Local;
            ps.transform.localScale*=0.05f;
        }
        Particle particle = obj.GetComponent<Particle>();
        particle.parent = parent;
        projectileList.Add(particle);
        exe.DelayExecute(10f, x => {
            Particle o = x[0] as Particle;
            if(o!=null){
                projectileList.Remove(o);
                Destroy(o.gameObject);
            }
        }, particle);
        Debug.Log("spawned effect: " + obj.name);
    }
}
