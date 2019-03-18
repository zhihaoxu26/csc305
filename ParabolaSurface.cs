/*
UVic CSC 305, 2019 Spring
Helping lab for assignment02
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolaSurface : MonoBehaviour
{

    public Material simpleMaterial;
    public GameObject Church;
    //public GameObject player;
    //private Vector3 offset;

    // Use this for initialization
    void Start()
    {
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        renderer.material = simpleMaterial;
        GenerateParabolaSurface();
        Church = GameObject.FindWithTag("Church");//.transform.position;
        //offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        //transform.position = player.transform.position + offset;
    }

    void GenerateParabolaSurface()
    {
        Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        int[] indices = mesh.triangles;
        Vector2[] uvs = mesh.uv;

        mesh.Clear();

        //subdivision = how many squares per row/col
        int subdivision = 250;
        //number of vertex per row/col
        int stride = subdivision + 1;
        //number of total vertexs
        int num_vert = stride * stride;
        //number of total triangles
        int num_tri = subdivision * subdivision * 2;

        indices = new int[num_tri * 3];
        int index_ptr = 0;
        for (int i = 0; i < subdivision; i++)
        {
            for (int j = 0; j < subdivision; j++)
            {
                int quad_corner = i * stride + j;
                indices[index_ptr] = quad_corner;
                indices[index_ptr + 1] = quad_corner + stride;
                indices[index_ptr + 2] = quad_corner + stride + 1;
                indices[index_ptr + 3] = quad_corner;
                indices[index_ptr + 4] = quad_corner + stride + 1;
                indices[index_ptr + 5] = quad_corner + 1;
                index_ptr += 6;
            }
        }

        Debug.Assert(index_ptr == indices.Length);

        const float xz_start = -5;
        const float xz_end = 5;
        //10/250
        float step = (xz_end - xz_start) / (float)(subdivision);
        vertices = new Vector3[num_vert];
        uvs = new Vector2[num_vert];
        float cur_y = 0;
        Vector2[] gradients = getGradients(121);
        int flag = 1;
        //stride = 251
        //step =10/250
        int index = 0;
        for (int i = 0; i < stride; i++)
        {
            //int index = 0;
            for (int j = 0; j < stride; j++)
            {
                // notice the bahavior here
                bool show_backface = false;
                float cur_x;
                float cur_z;
                //i don't know how this happened(showing back faces)
                if (show_backface)
                {
                    cur_x = xz_start + i * step;
                    cur_z = xz_start + j * step;
                }
                else
                {
                    cur_x = xz_start + j * step;
                    cur_z = xz_start + i * step;
                }
                //float cur_y = (-(cur_x * cur_x + cur_z * cur_z) / (float)10.0) + 5;


                ///???
                /// 
                Vector2 gradients1 = new Vector2(0, 0);
                Vector2 gradients2 = new Vector2(0, 0);
                Vector2 gradients3 = new Vector2(0, 0);
                Vector2 gradients4 = new Vector2(0, 0);


                if(index+1%11!= 0 && index <109)
                {
                    gradients1 = gradients[index];
                    gradients2 = gradients[index+11];
                    gradients3 = gradients[index + 1];
                    gradients4 = gradients[index + 12];
                }
                else if(index == 0){
                    gradients1 = gradients[index];
                    gradients2 = gradients[index + 11];
                    gradients3 = gradients[index + 1];
                    gradients4 = gradients[index + 12];
                }
                else if(index+1%11 == 0 && index<=109)
                {
                    gradients1 = gradients[index - 1];
                    gradients2 = gradients[index +10];
                    gradients3 = gradients[index];
                    gradients4 = gradients[index+11];
                }
                else if(index+1%11 != 0 && index >109)
                {
                    gradients1 = gradients[index-11];
                    gradients2 = gradients[index];
                    gradients3 = gradients[index-10];
                    gradients4 = gradients[index + 1];
                }
                else
                {
                    gradients1 = gradients[index - 12];
                    gradients2 = gradients[index-1];
                    gradients3 = gradients[index - 11];
                    gradients4 = gradients[index];
                }


                //My PerlinNoise Function
                cur_y = perlin(cur_x, cur_z, gradients1, gradients2, gradients3, gradients4);


                //Call Unity's PerlinNoise Function
                //cur_y = Mathf.PerlinNoise(cur_x,cur_z);


                //q6
                if (cur_y > 0.7 && flag == 1 && cur_x > -5 && cur_z > -5 && j > 100 && i > 100)
                {
                    Vector3 normal = new Vector3((Mathf.PerlinNoise(cur_x + 1, cur_z) - Mathf.PerlinNoise(cur_x - 1, cur_z)) / 2, 1, (Mathf.PerlinNoise(cur_x, cur_z + 1) - Mathf.PerlinNoise(cur_x, cur_z - 1)) / 2);
                    Church.transform.Translate(cur_x, cur_y, cur_z);
                    Church.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
                    flag = 0;
                }

                vertices[i * stride + j] = new Vector3(cur_x, cur_y, cur_z);
                if (j != 0 && j % 24 == 0)
                {
                    index++;
                }
            }
            if (i != 0 && i % 24 == 0)
            {
                index++;
            }
            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = indices;
            mesh.RecalculateNormals();
        }
    }


    Vector2[] getGradients(int n)
    {
        Vector2[] return_gradients = new Vector2[n];
        for (int i = 0; i < n; ++i)
        {
            Vector2 rand_vector = new Vector2(Random.value * 2 - 1, Random.value * 2 - 1);
            return_gradients[i] = rand_vector.normalized;
        }
        return return_gradients;
    }
    float perlin(float x, float y, Vector2 gradients1, Vector2 gradients2, Vector2 gradients3, Vector2 gradients4)
    {

        // Determine grid cell coordinates
        float x0 = Mathf.Floor(x);//x;
        float x1 = x0+1;//x0 + (float)1/25;
        float y0 = Mathf.Floor(y);//y;
        float y1 = y0+1;//y0 + (float)1/25;

        //Assign 4 vertices in cell
        Vector2 v1 = new Vector2(x0, y0);
        Vector2 v2 = new Vector2(x0, y1);
        Vector2 v3 = new Vector2(x1, y0);
        Vector2 v4 = new Vector2(x1, y1);

        Vector2 p = new Vector2(x, y);

        Vector2 a = p - v1;
        Vector2 b = p - v2; 
        Vector2 c = p - v3; 
        Vector2 d = p - v4;

        float s = Vector2.Dot(gradients1.normalized, a.normalized);
        float t = Vector2.Dot(gradients2.normalized, b.normalized);
        float u = Vector2.Dot(gradients3.normalized, c.normalized);
        float v = Vector2.Dot(gradients4.normalized, d.normalized);



        float st = mix(s, t, interpolation(p.x-x0));
        float uv = mix(u, v, interpolation(p.x-x0));
        float noise = mix(st, uv, interpolation(p.y-y0));

        return noise;
    }
    float interpolation(float t)
    {
        float alpha = (float)6 * Mathf.Pow(t, 5) - (float)15 * Mathf.Pow(t, 4) + (float)10 * Mathf.Pow(t, 3);
        return alpha;
    }
    float mix(float x, float y, float z)
    {
        float value = ((float)1 - z) * x + z * y;
        return value;
    }

}