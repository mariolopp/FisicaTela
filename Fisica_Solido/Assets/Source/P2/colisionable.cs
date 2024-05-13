using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Colisionable : MonoBehaviour
{
    // Start is called before the first frame update
    //GameObject[] objectsWithTag;
    GameObject[] cloths;
    public tetraEdroGenerator cloth;
    List<Node> nodes;
    Vector3[] localPos;     // Coordenadas locales de los vertices respecto al fixer
    bool[] nodesInside;        // Guarda si el node estaba inicialmente dentro del fixer
    public float coef_sphere = 1.4f;
    private void Awake()
    {

    }
    void Start()
    {

        cloths = GameObject.FindGameObjectsWithTag("Cloth");
        if (cloths != null)
        {
            cloth = cloths[0].GetComponent<tetraEdroGenerator>();
            nodes = cloth.nodeList;      // Referencia a la lista de nodos de la prenda
        }
        nodesInside = new bool[nodes.Count];       
        Bounds bounds = GetComponent<Collider>().bounds;    // Obtener el collider del objeto fixed
        
        
        int i = 0;
        foreach (Node n in nodes)
        {
            bool isInside = bounds.Contains(n.pos);     // Comprobar si una posición de vertice en coord globales está dentro del collider
            if (isInside)
            {
                n.g_enabled = false;    // Si colisiona seteamos a 0 su fuerza hacia abajo
            }
            else
            {
                n.g_enabled = true;       // Si el vertice no esta colisionando recupera la fuerza hacia abajo
            }
            i++;
        }

    }

    // Update is called once per frame
    void Update()
    {

        Bounds bounds = GetComponent<Collider>().bounds;    // Obtener el collider del objeto fixed

        int i = 0;
        foreach (Node n in nodes)
        {
            bool isInside = bounds.Contains(n.pos-(transform.localScale*0.01f));     // Comprobar si una posición de vertice en coord globales está cerca del collider
            if (isInside)
            {
                //n.g_enabled = false;    // Si colisiona seteamos a 0 su fuerza hacia abajo
                n.coef = transform.localScale.x / ((n.pos - transform.position).magnitude*coef_sphere);  // A mas cerca del centro mas alto el coeficiente
                
            }
            else
            {
                //n.g_enabled = false;       // Si el vertice no esta colisionando recupera la fuerza hacia abajo
                n.coef = 0;
            }
            i++;
        }
    }
}

