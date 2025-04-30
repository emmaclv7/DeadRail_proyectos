using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/*
 ############################################################################################################################
 * Estimado programador valiente que osas aventurarte en estas l�neas de c�digo:
 
 * Cuando escrib� este c�digo, s�lo dos entidades en este vasto universo comprend�an su prop�sito y funcionamiento:
   Dios� y yo.
 
 * Hoy, s�lo Dios conserva ese conocimiento.
 
 * Esta funci�n, este fragmento de l�gica arcana, es el vestigio de una batalla olvidada entre hombre y m�quina.
 * Fue escrita en medio del caos, entre caf�s fr�os, noches sin luna y commits sin sentido.
 * Su estructura desaf�a la raz�n, su l�gica se burla de toda convenci�n.
 * Sin embargo, funcionaba. Y eso bastaba.
 
 * Si, por alguna extra�a raz�n, has decidido optimizar esto (o peor a�n, comprenderlo) te ruego: ten piedad de ti mismo.
 * Si fallas (y es muy probable que lo hagas), no te sientas derrotado.
 * No eres el primero, ni ser�s el �ltimo en caer ante esta bestia indomable.
 
 * Simplemente, suma tu sacrificio a los que vinieron antes. Deja una advertencia. A�ade tu nombre al pante�n de los ca�dos.
 
 * total_horas_perdidas_aqui = 12; // Aumenta este n�mero con orgullo si has sufrido.
 ############################################################################################################################
 */


public class FieldOfView : MonoBehaviour
{
    public float viewAngle = 90f; //El angulo de vision (en cono) por grados
    public float viewDistance = 8f; //Distancia de la vision
    public int rayCount = 60; //N�mero de rallos lanzados para formar lo que se ve en el campo de vision
    public LayerMask obstacleMask; //M�scara de capas
    public LayerMask groundMask; //M�scara de capas

    private Mesh viewMesh; //Malla que se encarga de representar el campo de visi�n visualmente
    private Vector3 origin; //Posici�n donde se va a mover el rango de visi�n
    private float startingAngle; //Angulo incial que empieza el rango de vision en un inicio

    void Start()
    {
        viewMesh = new Mesh(); //Se crea una nueva malla
        GetComponent<MeshFilter>().mesh = viewMesh; //Se asigna la nueva malla al componente MeshFilter
    }

    void Update()
    {
        DrawFieldOfView(); //Se actualiza la malla del FOV en cada frame
    }

    //M�todo donde se actualiza la Posici�n de donde esta el rango de visi�on
    public void SetOrigin(Vector3 newOrigin)
    {
        origin = newOrigin;
    }

    //M�todo donde se acttualiza la direcci�n de donde se mira
    public void SetAimDirection(Vector3 direction)
    {
        direction.Normalize(); //Normalizamos el vector de direcci�n
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; //Form�la donde se convierte la direcci�n del �ngulo en grados
        startingAngle = angle + (viewAngle / 2f);  //C�lculo de los �ngulos desde que se inicializan los rayos
    }

    //M�todo donde se genera la visualizaci�n de la malla en el campo de visi�n
    void DrawFieldOfView()
    {
        float angleStep = viewAngle / rayCount; //C�lculo el paso del �ngulo por cada rayo que hay
        Vector3[] vertices = new Vector3[rayCount + 1]; //Creamos una array de v�rtices d�nde el primero es el origen y todo lo demas son las puntas de cada rayo
        int[] triangles = new int[rayCount * 3]; //Creamos un array de tri�ngulos para dibujar el cono que formar� el rango de visi�n 

        vertices[0] = origin; //V�rtice inicial donde es el origen del field of view

        int vertexIndex = 1;
        int triangleIndex = 0;

        for (int i = 0; i < rayCount; i++) //Se lanzan cada uno de los rayos
        {
            float angle = startingAngle - angleStep * i; //Se calculo el �ngulo del rayo
            Vector3 dir = GetDirectionFromAngle(angle); //Ese �ngulo se convierte en una direcci�n
            Vector3 vertex; //el punto final del rayo

            RaycastHit2D hit = Physics2D.Raycast(origin, dir, viewDistance, obstacleMask | groundMask); //Lanzamos un raycast por si se choca con algo
            vertex = (hit.collider != null) ? hit.point : origin + dir * viewDistance; // si colisiona usamos el punto de impacto y sino el punto m�s lejano de la direcci�n

            vertices[vertexIndex] = vertex; //Guardamos el v�rtice en el array

            if (i > 0) //Empezamos a formar tr�angulos
            {
                triangles[triangleIndex++] = 0;
                triangles[triangleIndex++] = vertexIndex - 1;
                triangles[triangleIndex++] = vertexIndex;
            }

            vertexIndex++;
        }

        //conectamos el anterior triangulo con el primer v�rtice para cerrar el rango de visi�n
        triangles[triangleIndex++] = 0;
        triangles[triangleIndex++] = rayCount;
        triangles[triangleIndex++] = 1;

        //Limpiamos y actualizamos la malla con los nuevos tri�ngulos y vertices generados
        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    //M�todo donde convierte el �ngulo en grados hasta un vector de direcci�n
    Vector3 GetDirectionFromAngle(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}