using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class FixerEscalable : MonoBehaviour
{
    // Start is called before the first frame update
    //GameObject[] objectsWithTag;
    GameObject[] cloths;
    public tetraEdroGenerator[] obj_cloths;
    public struct fixable_cloths
    {
        public List<Node> nodes;        // Lista de nodos de cada prenda
        public Vector3[] localPos;      // Coordenadas locales de los vertices respecto al fixer
        public bool[] nodesInside;      // Guarda si el node estaba inicialmente dentro del fixer
    }
    fixable_cloths[] obj_fixable;
    private void Awake()
    {

    }
    void Start()
    {

        //objectsWithTag = GameObject.FindGameObjectsWithTag("Fixer");

        cloths = GameObject.FindGameObjectsWithTag("edro");



        if (cloths != null)
        {
            obj_fixable = new fixable_cloths[cloths.Length];    // Tantos structs como prendas
            obj_cloths = new tetraEdroGenerator[cloths.Length];
            int i = 0;
            foreach (GameObject c in cloths)
            {
                obj_cloths[i] = c.GetComponent<tetraEdroGenerator>();
                i++;
            }

            i = 0;
            foreach (fixable_cloths f in obj_fixable)
            {
                obj_fixable[i].nodes = obj_cloths[i].nodeList;      // Referencia a la lista de nodos de la prenda
                obj_fixable[i].localPos = new Vector3[obj_fixable[i].nodes.Count];  // inicializamos a la cantidad de nodos
                int a = 0;
                foreach (Node n in obj_fixable[i].nodes)
                {
                    obj_fixable[i].localPos[a] = transform.InverseTransformPoint(n.pos); // transformar a coordenadas locales del fixer la coord del vertice
                    a++;
                }
                obj_fixable[i].nodesInside = new bool[obj_fixable[i].nodes.Count];
                i++;
            }

        }

        Bounds bounds = GetComponent<Collider>().bounds;    // Obtener el collider del objeto fixed
        int j = 0;
        foreach (tetraEdroGenerator c in obj_cloths)
        {
            int i = 0;
            foreach (Node n in obj_fixable[j].nodes)
            {
                bool isInside = bounds.Contains(n.pos);     // Comprobar si una posición de vertice en coord globales está dentro del collider
                if (isInside)
                {
                    n._fixed = true;        // Si el vertice está agarrado las fisicas no le afectan

                    Vector3 globalPos = transform.TransformPoint(obj_fixable[j].localPos[i]);    // aplicar la transformación que haya tenido el fixer al vertice
                    //print(n.pos);
                    n.pos = globalPos;                                            // Aplicar las coordenadas globales aplicadas a dicho vertice
                    //print("new " + n.pos);
                    isInside = true;
                    obj_fixable[j].nodesInside[i] = true;
                }
                else
                {
                    n._fixed = false;       // Si el vertice no esta agarrado las fisicas le afectan
                }
                i++;
            }
            j++;
        }


    }

    // Update is called once per frame
    void Update()
    {

        Bounds bounds = GetComponent<Collider>().bounds;    // Obtener el collider del objeto fixer
        int j = 0;
        foreach (tetraEdroGenerator c in obj_cloths)
        {
            int i = 0;
            foreach (Node n in obj_fixable[j].nodes)
            {

                //n._fixed = true;        // Si el vertice está agarrado las fisicas no le afectan
                if (obj_fixable[j].nodesInside[i])
                {
                    Vector3 globalPos = transform.TransformPoint(obj_fixable[j].localPos[i]);    // aplicar la transformación que haya tenido el fixer al vertice
                                                                                                 //print(n.pos);
                    n.pos = globalPos;                                            // Aplicar las coordenadas globales aplicadas a dicho vertice
                                                                                  //print("new " + n.pos);
                                                                                  //obj_fixable[j].nodesInside[i] = true;
                }
                i++;
            }
            j++;
        }
    }
}

