    Shader "Mobile/Tex&Env"
    {
        Properties
        {
            _Color ("Main Color", Color) = (1,1,1,1)
            _MainTex ("Texture", 2D) = "white" { }
            _EnvTex ("Reflection", 2D) = "black" { TexGen SphereMap}
        }
       
        Category
        {
            Lighting Off ZWrite On
               
            SubShader
            {
                Pass
                {
                   
                    SetTexture [_EnvTex]
                    {
                        combine texture //double
                    }
                    SetTexture [_MainTex]
                    {
                        combine texture lerp(texture) previous
                           
                    }
                }
            }
        }
    }