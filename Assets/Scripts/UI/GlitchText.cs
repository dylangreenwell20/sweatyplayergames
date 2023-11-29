using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GlitchText : MonoBehaviour
{
    TMP_Text textMesh;

    Mesh mesh;

    Vector3[] vertices;

    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        textMesh.ForceMeshUpdate();
        mesh = textMesh.mesh;
        vertices = mesh.vertices;
    }


}
