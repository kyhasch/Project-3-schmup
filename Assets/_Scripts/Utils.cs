using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public enum EasingType{
        linear,
        easeIn,
        easeOut,
        easeInOut,
        sin,
        sinV2,
        sinIn, 
        sinOut
    }

    
    static public float Ease (float u, EasingType eType, float eMod = 2 ){
        float u2 = u;

        switch(eType){
            case EasingType.linear:
                u2 = u;
                break;
            
            case EasingType.easeIn:
                u2 = Mathf.Pow(u, eMod);
                break;
            
            case EasingType.easeOut:
                u2 = 1 - Mathf.Pow(1 - u, eMod);
                break;

            case EasingType.easeInOut:
                if(u <= 0.5) u2 = 0.5f * Mathf.Pow(u * 2, eMod);
                else u2 = 0.5f + 0.5f * Mathf.Pow(1 - (2 * (u - 0.5f)), eMod);
                break;

            case EasingType.sin:
                u2 = u + eMod * Mathf.Sin(2 * Mathf.PI * u);
                break;
            
            case EasingType.sinV2:
                u2 = u - eMod * Mathf.Sin(2 * Mathf.PI * u);
                break;
            
            case EasingType.sinIn:
                u2 = 1 - Mathf.Cos(u * Mathf.PI * 0.5f);
                break;

            case EasingType.sinOut:
                u2 = Mathf.Sin(u * Mathf.PI * 0.5f);
                break;
        }

        return u2;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    static public Material[] GetAllMaterials(GameObject go){
        Renderer[] rends = go.GetComponentsInChildren<Renderer>();

        List<Material> mats = new List<Material>();
        foreach (Renderer rend in rends)
            mats.Add(rend.material);

        return mats.ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="u"></param>
    /// <param name="vecs"></param>
    /// <returns></returns>
    static public Vector3 Bezier(float u, params Vector3[] vecs){
        return Bezier(u, new List<Vector3>(vecs));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="u"></param>
    /// <param name="pts"></param>
    /// <param name="iL"></param>
    /// <param name="iR"></param>
    /// <returns></returns>
    static public Vector3 Bezier(float u, List<Vector3> pts, int iL=0, int iR=-1){
        if(iR == -1) iR = pts.Count-1;

        if(iL == iR) return pts[iL];

        Vector3 lV3 = Bezier(u, pts, iL, iR-1);
        Vector3 rV3 = Bezier(u, pts, iL+1, iR);

        Vector3 res = Vector3.LerpUnclamped(lV3, rV3, u);
        return res;
    }

    
}