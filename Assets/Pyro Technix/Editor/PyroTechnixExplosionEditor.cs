/*
 * Copyright (c) 2014 LowLevelTech
 * Alex Dunn
 * llt@dunnalex.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using PyroTechnix;

[CustomEditor(typeof(PyroTechnixExplosion))]
public class PyroTechnixExplosionEditor : Editor 
{
    SerializedProperty primitiveType;
    SerializedProperty colourGradient;
    SerializedProperty uvScale;
    SerializedProperty uvBias;
    SerializedProperty maxNumSteps;
    SerializedProperty noiseFrequencyFactor;
    SerializedProperty noiseAmplitudeFactor;
    SerializedProperty noiseAnimationSpeed;
    SerializedProperty displacementWS;
    SerializedProperty noiseScale;
    SerializedProperty edgeSoftness;
    SerializedProperty opacity;
    SerializedProperty useLighting;
    SerializedProperty density;
    SerializedProperty directionalLight;
    SerializedProperty isAxisAligned;

    private void OnEnable()
    {
        primitiveType           = serializedObject.FindProperty("primitiveType");
        colourGradient          = serializedObject.FindProperty("colourGradient");
        uvScale                 = serializedObject.FindProperty("uvScale");
        uvBias                  = serializedObject.FindProperty("uvBias");
        maxNumSteps             = serializedObject.FindProperty("maxNumSteps");
        noiseFrequencyFactor    = serializedObject.FindProperty("noiseFrequencyFactor");
        noiseAmplitudeFactor    = serializedObject.FindProperty("noiseAmplitudeFactor");
        noiseAnimationSpeed     = serializedObject.FindProperty("noiseAnimationSpeed");
        displacementWS          = serializedObject.FindProperty("displacementWS");
        noiseScale              = serializedObject.FindProperty("noiseScale");
        edgeSoftness            = serializedObject.FindProperty("edgeSoftness");
        opacity                 = serializedObject.FindProperty("opacity");
        useLighting             = serializedObject.FindProperty("useLighting");
        density                 = serializedObject.FindProperty("density");
        directionalLight        = serializedObject.FindProperty("directionalLight");
        isAxisAligned           = serializedObject.FindProperty("isAxisAligned");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(primitiveType, new GUIContent("Primitive Type: "));
        EditorGUILayout.PropertyField(isAxisAligned, new GUIContent("Axis Aligned: ", "If you are using a primitive which is axis symmetric (sphere), then axis aligned is a minor optimisation."));
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(maxNumSteps, new GUIContent("Render Steps: "));
        EditorGUILayout.PropertyField(edgeSoftness, new GUIContent("Edge Softness: "));
       
        EditorGUILayout.Space();
        
        EditorGUILayout.PropertyField(noiseFrequencyFactor, new GUIContent("Noise Frequency Factor: "));
        EditorGUILayout.PropertyField(noiseAmplitudeFactor, new GUIContent("Noise Amplitude Factor: "));
        EditorGUILayout.PropertyField(noiseAnimationSpeed, new GUIContent("Noise Animation Speed: "));
        EditorGUILayout.PropertyField(displacementWS, new GUIContent("Displacement: "));
        EditorGUILayout.PropertyField(noiseScale, new GUIContent("Noise Scale: "));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(useLighting, new GUIContent("Use Lighting: "));
        if (useLighting.boolValue)
        {
            EditorGUILayout.PropertyField(density, new GUIContent("Gas Density: "));
            EditorGUILayout.PropertyField(directionalLight, new GUIContent("Directional Light: "));
        }

        EditorGUILayout.Space();

        EditorGUILayout.Slider(opacity, 0, 1, new GUIContent("Opacity: "));
        EditorGUILayout.PropertyField(colourGradient, new GUIContent("Gradient: "));

        if( serializedObject.ApplyModifiedProperties() )
        {
            (target as PyroTechnixExplosion).SetGradientDirty();
        }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(uvScale, new GUIContent("UV Scale: "));
        EditorGUILayout.PropertyField(uvBias, new GUIContent("UV Bias: "));

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();
        EditorGUILayout.Space();


        EditorGUILayout.LabelField(new GUIContent("Inner Radius: " + (target as PyroTechnixExplosion).InnerRadiusWS));
        EditorGUILayout.LabelField(new GUIContent("Displacement : " + (target as PyroTechnixExplosion).DisplacementWS));
    }
}
