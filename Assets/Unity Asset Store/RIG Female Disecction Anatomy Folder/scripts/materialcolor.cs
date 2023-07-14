using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class materialcolor : MonoBehaviour
{

        public Material[] material;
        public int x;
        Renderer rend;

    void Start()
    {
        x=1;
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = material[x];       
    }



void Update()
    {
            
            rend.sharedMaterial = material[x];
            if(x<1)
            {  
            x=1;
            //Debug.Log(x);
            }
            if(x>4)
            {  
            x=4;
            //Debug.Log(x);
            }
            }

            

    public void hide_leyer()
    {
        if(x<4)
        {  
            x++;
            //Debug.Log(x);
            //print("hide");

        }
        
        else
        {
        if(x>5)
        x=5; 
        //Debug.Log ("topo conteo a CUATRO"); 
        //Debug.Log(x);  
        }
        
    }

        public void show_leyer()
    {
        if(x>1)
        {  
            x=x-1;
            //Debug.Log(x);
            //print("show");
        }
        
        else
        {
        if(x<1)
        x=1; 
        //Debug.Log ("topo conteo a UNO"); 
        //Debug.Log(x);  
        }
        
    }





}
// Debug.Log ("topo conteo");
