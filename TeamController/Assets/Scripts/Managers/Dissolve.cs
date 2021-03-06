using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    [SerializeField] private Color originalColor;
    [SerializeField] private Material deathMat;
    private Material materialInstance;
    [SerializeField] private MeshRenderer thisMesh;
    [SerializeField] private Material thisMaterial;

    private bool startDissolve;
    [SerializeField] private float dissolveTime;
    [SerializeField] private float value;

    private void Start()
    {
        materialInstance = new Material(deathMat);
        value = materialInstance.GetFloat("_DissolveControl");

        originalColor = thisMesh.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (startDissolve)
        {
            dissolveMaterials();
        }
    }

    public void swapMaterials()
    {
        thisMesh.material = Instantiate(materialInstance); //Creates instance of material, stops others being affected;

        thisMaterial = thisMesh.material;
        thisMaterial.SetColor("_BaseColor", originalColor);

        startDissolve = true;
    }

    private void dissolveMaterials()
    {
        value += dissolveTime * Time.deltaTime;

        thisMaterial.SetFloat("_DissolveControl", value);
    }
}
