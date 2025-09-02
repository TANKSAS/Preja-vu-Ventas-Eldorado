﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;  // Necesario para crear el botón en el editor

public class QuadScript : MonoBehaviour
{
    Material mMaterial;
    public Material mMaterialOriginal;
    MeshRenderer mMeshRenderer;
    public float[] mPoints;
    public int mHitCount;

    void Awake()
    {
        mMeshRenderer = GetComponent<MeshRenderer>();
        mMaterialOriginal = mMeshRenderer.material;
        AssingNewMaterial();
    }

    public void AssingNewMaterial()
    {
        mMaterial = new Material(mMaterialOriginal);
        mMeshRenderer.material = mMaterial;
    }

    public void SetSize(int size) // mirar como acomodar eso con el shader
    {
        mHitCount = 0;
        mPoints = null;
        mPoints = new float[500 * 6]; // 32 point array
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (mPoints.Length < 0)
            return;

        foreach (ContactPoint cp in collision.contacts)
        {
            Vector3 startOfRay = cp.point - cp.normal;
            Vector3 rayDir = cp.normal;

            Ray ray = new Ray(startOfRay, rayDir);
            RaycastHit hit;

            bool hitit = Physics.Raycast(ray, out hit, 10f, LayerMask.GetMask("HeatMapLayer"));

            if (hitit)
            {
                Debug.Log("Hit");
                AddHitPoint(hit.textureCoord.x * 4 - 2, hit.textureCoord.y * 4 - 2);
            }

            cp.otherCollider.gameObject.SetActive(false);
        }
    }

    public void AddHitPoint(float xp, float yp)
    {
        mPoints[mHitCount * 3] = xp;
        mPoints[mHitCount * 3 + 1] = yp;
        mPoints[mHitCount * 3 + 2] = Random.Range(1f, 3f);

        mHitCount++;
        mHitCount %= mPoints.Length;

        mMaterial.SetFloatArray("_Hits", mPoints);
        mMaterial.SetInt("_HitCount", mHitCount);
    }
}
