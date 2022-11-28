// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:0,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:2,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.65,fgcg:0.9302794,fgcb:0.9779412,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:1012,x:32927,y:32693,varname:node_1012,prsc:2|diff-6826-OUT,spec-1741-OUT,gloss-5343-OUT,normal-5327-RGB,transm-4497-RGB,lwrap-4071-RGB,clip-5017-OUT,voffset-2981-OUT;n:type:ShaderForge.SFN_Tex2d,id:9307,x:31824,y:32117,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_2956,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:5327,x:32035,y:32645,ptovrint:False,ptlb:Normal,ptin:_Normal,varname:node_3796,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_ValueProperty,id:1741,x:32122,y:32513,ptovrint:False,ptlb:Specular,ptin:_Specular,varname:node_8057,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Sin,id:7587,x:31513,y:32841,varname:node_7587,prsc:2|IN-6182-OUT;n:type:ShaderForge.SFN_Multiply,id:2981,x:32256,y:32928,varname:node_2981,prsc:2|A-4427-OUT,B-7930-RGB,C-8227-OUT;n:type:ShaderForge.SFN_Time,id:1380,x:31096,y:32776,varname:node_1380,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:7930,x:31536,y:33207,ptovrint:False,ptlb:WindMask,ptin:_WindMask,varname:node_4787,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:False|UVIN-4244-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:4244,x:31250,y:33262,varname:node_4244,prsc:2,uv:2;n:type:ShaderForge.SFN_Add,id:6182,x:31325,y:32841,varname:node_6182,prsc:2|A-7494-OUT,B-1380-T;n:type:ShaderForge.SFN_Pi,id:7494,x:31129,y:32899,varname:node_7494,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:9608,x:31315,y:32648,prsc:2,pt:False;n:type:ShaderForge.SFN_ValueProperty,id:478,x:31480,y:33008,ptovrint:False,ptlb:Main Wind Str,ptin:_MainWindStr,varname:node_8610,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.3;n:type:ShaderForge.SFN_Color,id:4071,x:32429,y:32503,ptovrint:False,ptlb:Light Wrap,ptin:_LightWrap,varname:_Diffusecolor_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.7270221,c2:0.8308824,c3:0.7499826,c4:1;n:type:ShaderForge.SFN_Panner,id:7737,x:30407,y:32331,varname:node_7737,prsc:2,spu:0.1,spv:0|UVIN-7312-UVOUT;n:type:ShaderForge.SFN_ValueProperty,id:6983,x:30724,y:32586,ptovrint:False,ptlb:Additional wind str,ptin:_Additionalwindstr,varname:_BulgeShape,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.01;n:type:ShaderForge.SFN_Multiply,id:3362,x:31693,y:32841,varname:node_3362,prsc:2|A-7587-OUT,B-478-OUT;n:type:ShaderForge.SFN_Vector1,id:8227,x:31860,y:33334,varname:node_8227,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:8220,x:31697,y:32590,varname:node_8220,prsc:2|A-7277-OUT,B-9608-OUT;n:type:ShaderForge.SFN_Add,id:4427,x:32056,y:32833,varname:node_4427,prsc:2|A-8220-OUT,B-6861-OUT;n:type:ShaderForge.SFN_Tex2d,id:441,x:30628,y:32331,ptovrint:False,ptlb:Additional wind Gradient,ptin:_AdditionalwindGradient,varname:node_7752,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-7737-UVOUT;n:type:ShaderForge.SFN_Multiply,id:7277,x:30987,y:32532,varname:node_7277,prsc:2|A-441-RGB,B-6983-OUT;n:type:ShaderForge.SFN_Multiply,id:6861,x:31876,y:32861,varname:node_6861,prsc:2|A-3362-OUT,B-4443-XYZ;n:type:ShaderForge.SFN_Vector4Property,id:4443,x:31652,y:33054,ptovrint:False,ptlb:Main Wind vector,ptin:_MainWindvector,varname:node_5857,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Desaturate,id:6826,x:32203,y:32256,varname:node_6826,prsc:2|COL-9731-OUT,DES-7915-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7915,x:32030,y:32338,ptovrint:False,ptlb:Desaturation,ptin:_Desaturation,varname:_Specular_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:5017,x:32454,y:32788,varname:node_5017,prsc:2|A-9307-A,B-3658-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3658,x:32268,y:32852,ptovrint:False,ptlb:Alpha Cutoff,ptin:_AlphaCutoff,varname:_Specular_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Color,id:4497,x:32467,y:32161,ptovrint:False,ptlb:Transmission,ptin:_Transmission,varname:node_4452,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_TexCoord,id:7312,x:30208,y:32331,varname:node_7312,prsc:2,uv:0;n:type:ShaderForge.SFN_ValueProperty,id:5343,x:32241,y:32704,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:node_5343,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:9731,x:32050,y:32099,varname:node_9731,prsc:2|A-6345-RGB,B-9307-RGB;n:type:ShaderForge.SFN_Color,id:6345,x:31824,y:31950,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_6345,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;proporder:9307-6345-7915-3658-1741-4071-4497-5327-7930-478-6983-441-4443-5343;pass:END;sub:END;*/

Shader "Custom/Grass_Wind" {
    Properties {
        _MainTex ("MainTex", 2D) = "black" {}
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _Desaturation ("Desaturation", Float ) = 0
        _AlphaCutoff ("Alpha Cutoff", Float ) = 1
        _Specular ("Specular", Float ) = 0
        _LightWrap ("Light Wrap", Color) = (0.7270221,0.8308824,0.7499826,1)
        _Transmission ("Transmission", Color) = (0.5,0.5,0.5,1)
        _Normal ("Normal", 2D) = "bump" {}
        _WindMask ("WindMask", 2D) = "bump" {}
        _MainWindStr ("Main Wind Str", Float ) = 0.3
        _Additionalwindstr ("Additional wind str", Float ) = 0.01
        _AdditionalwindGradient ("Additional wind Gradient", 2D) = "white" {}
        _MainWindvector ("Main Wind vector", Vector) = (0,0,0,0)
        _Gloss ("Gloss", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _Specular;
            uniform sampler2D _WindMask; uniform float4 _WindMask_ST;
            uniform float _MainWindStr;
            uniform float4 _LightWrap;
            uniform float _Additionalwindstr;
            uniform sampler2D _AdditionalwindGradient; uniform float4 _AdditionalwindGradient_ST;
            uniform float4 _MainWindvector;
            uniform float _Desaturation;
            uniform float _AlphaCutoff;
            uniform float4 _Transmission;
            uniform float _Gloss;
            uniform float4 _Color;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                LIGHTING_COORDS(4,5)
                UNITY_FOG_COORDS(6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_5494 = _Time + _TimeEditor;
                float2 node_7737 = (o.uv0+node_5494.g*float2(0.1,0));
                float4 _AdditionalwindGradient_var = tex2Dlod(_AdditionalwindGradient,float4(TRANSFORM_TEX(node_7737, _AdditionalwindGradient),0.0,0));
                float4 node_1380 = _Time + _TimeEditor;
                float4 _WindMask_var = tex2Dlod(_WindMask,float4(TRANSFORM_TEX(o.uv2, _WindMask),0.0,0));
                v.vertex.xyz += ((((_AdditionalwindGradient_var.rgb*_Additionalwindstr)*v.normal)+((sin((3.141592654+node_1380.g))*_MainWindStr)*_MainWindvector.rgb))*_WindMask_var.rgb*1.0);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0, _Normal)));
                float3 normalDirection = _Normal_var.rgb;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                clip((_MainTex_var.a*_AlphaCutoff) - 0.5);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = _Gloss;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_Specular,_Specular,_Specular);
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = dot( normalDirection, lightDirection );
                float3 w = _LightWrap.rgb*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * _Transmission.rgb;
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = (forwardLight+backLight) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 diffuseColor = lerp((_Color.rgb*_MainTex_var.rgb),dot((_Color.rgb*_MainTex_var.rgb),float3(0.3,0.59,0.11)),_Desaturation);
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _Specular;
            uniform sampler2D _WindMask; uniform float4 _WindMask_ST;
            uniform float _MainWindStr;
            uniform float4 _LightWrap;
            uniform float _Additionalwindstr;
            uniform sampler2D _AdditionalwindGradient; uniform float4 _AdditionalwindGradient_ST;
            uniform float4 _MainWindvector;
            uniform float _Desaturation;
            uniform float _AlphaCutoff;
            uniform float4 _Transmission;
            uniform float _Gloss;
            uniform float4 _Color;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                LIGHTING_COORDS(4,5)
                UNITY_FOG_COORDS(6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_8767 = _Time + _TimeEditor;
                float2 node_7737 = (o.uv0+node_8767.g*float2(0.1,0));
                float4 _AdditionalwindGradient_var = tex2Dlod(_AdditionalwindGradient,float4(TRANSFORM_TEX(node_7737, _AdditionalwindGradient),0.0,0));
                float4 node_1380 = _Time + _TimeEditor;
                float4 _WindMask_var = tex2Dlod(_WindMask,float4(TRANSFORM_TEX(o.uv2, _WindMask),0.0,0));
                v.vertex.xyz += ((((_AdditionalwindGradient_var.rgb*_Additionalwindstr)*v.normal)+((sin((3.141592654+node_1380.g))*_MainWindStr)*_MainWindvector.rgb))*_WindMask_var.rgb*1.0);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0, _Normal)));
                float3 normalDirection = _Normal_var.rgb;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                clip((_MainTex_var.a*_AlphaCutoff) - 0.5);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = _Gloss;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_Specular,_Specular,_Specular);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = dot( normalDirection, lightDirection );
                float3 w = _LightWrap.rgb*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * _Transmission.rgb;
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = (forwardLight+backLight) * attenColor;
                float3 diffuseColor = lerp((_Color.rgb*_MainTex_var.rgb),dot((_Color.rgb*_MainTex_var.rgb),float3(0.3,0.59,0.11)),_Desaturation);
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _WindMask; uniform float4 _WindMask_ST;
            uniform float _MainWindStr;
            uniform float _Additionalwindstr;
            uniform sampler2D _AdditionalwindGradient; uniform float4 _AdditionalwindGradient_ST;
            uniform float4 _MainWindvector;
            uniform float _AlphaCutoff;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_7706 = _Time + _TimeEditor;
                float2 node_7737 = (o.uv0+node_7706.g*float2(0.1,0));
                float4 _AdditionalwindGradient_var = tex2Dlod(_AdditionalwindGradient,float4(TRANSFORM_TEX(node_7737, _AdditionalwindGradient),0.0,0));
                float4 node_1380 = _Time + _TimeEditor;
                float4 _WindMask_var = tex2Dlod(_WindMask,float4(TRANSFORM_TEX(o.uv2, _WindMask),0.0,0));
                v.vertex.xyz += ((((_AdditionalwindGradient_var.rgb*_Additionalwindstr)*v.normal)+((sin((3.141592654+node_1380.g))*_MainWindStr)*_MainWindvector.rgb))*_WindMask_var.rgb*1.0);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                clip((_MainTex_var.a*_AlphaCutoff) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
