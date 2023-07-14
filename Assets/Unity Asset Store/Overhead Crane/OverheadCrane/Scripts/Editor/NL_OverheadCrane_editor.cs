namespace NOT_Lonely
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(NL_OverheadCrane))]
    public class NL_OverheadCrane_editor : Editor
    {
        private NL_OverheadCrane instance;

        SerializedProperty allColumns;
        SerializedProperty useColumns;
        SerializedProperty craneHeight;
        SerializedProperty craneWidth;
        SerializedProperty craneLength;
        //SerializedProperty curve;
        //SerializedProperty cablesMainCurve;
        SerializedProperty initialBeamPos;
        SerializedProperty initialTelpherPos;
        SerializedProperty hookHeight;
        SerializedProperty useMainBeamDecal;
        SerializedProperty useColumnDecal;
        SerializedProperty isCombined;
        SerializedProperty remoteHeight;
        SerializedProperty keyboardControls;
        SerializedProperty beamSpeed;
        SerializedProperty telpherSpeed;
        SerializedProperty hookSpeed;
        SerializedProperty cablesSwayAmplitude;
        SerializedProperty useCableSwaying;

        private Color iconsColorDisableDark = new Color(0.7f, 0.7f, 0.7f, 1);
        private Color iconsColorEnableDark = new Color(0, 0.656f, 0.9f, 1);

        private Color iconsColorDisableLight = new Color(0.5f, 0.5f, 0.5f, 1);
        private Color iconsColorEnableLight = new Color(0, 0.628f, 0.882f, 1);

        protected virtual void OnEnable()
        {
            instance = (NL_OverheadCrane)target;

            allColumns = serializedObject.FindProperty("allColumns");
            useColumns = serializedObject.FindProperty("useColumns");
            craneHeight = serializedObject.FindProperty("craneHeight");
            craneWidth = serializedObject.FindProperty("craneWidth");
            craneLength = serializedObject.FindProperty("craneLength");
            //curve = serializedObject.FindProperty("curve");
            //cablesMainCurve = serializedObject.FindProperty("cablesMainCurve");
            initialBeamPos = serializedObject.FindProperty("initialBeamPos");
            initialTelpherPos = serializedObject.FindProperty("initialTelpherPos");
            hookHeight = serializedObject.FindProperty("hookHeight");
            useMainBeamDecal = serializedObject.FindProperty("useMainBeamDecal");
            useColumnDecal = serializedObject.FindProperty("useColumnDecal");
            isCombined = serializedObject.FindProperty("isCombined");
            beamSpeed = serializedObject.FindProperty("beamSpeed");
            telpherSpeed = serializedObject.FindProperty("telpherSpeed");
            hookSpeed = serializedObject.FindProperty("hookSpeed");
            cablesSwayAmplitude = serializedObject.FindProperty("cablesSwayAmplitude");
            useCableSwaying = serializedObject.FindProperty("useCableSwaying");

            remoteHeight = serializedObject.FindProperty("remoteHeight");
            keyboardControls = serializedObject.FindProperty("keyboardControls");

            Undo.undoRedoPerformed += UndoRedoCallback;
        }

        protected virtual void OnDisable()
        {
            Undo.undoRedoPerformed -= UndoRedoCallback;
        }

        void UndoRedoCallback()
        {
            if (!instance.isCombined)
            {
                instance.SwitchColumns(instance.useColumns);
                instance.CraneHeightChange();
                instance.CraneWidthChange();
                instance.CraneLengthChange();
                instance.SwitchMainBeamDecal(instance.useMainBeamDecal);
                instance.SwitchColumnDecal(instance.useColumnDecal);
            }
            instance.UpdateMainBeamPos(instance.initialBeamPos);
            instance.UpdateTelpherPos(instance.initialTelpherPos);
            instance.UpdateHookHeight();

            instance.UpdateRemoteHeight();
            instance.SwitchKeyboardControls(instance.keyboardControls);
        }

        void SwitchGUIColor(bool value)
        {
            if (EditorGUIUtility.isProSkin)
            {
                if (!value)
                {
                    GUI.color = iconsColorDisableDark;
                }
                else
                {
                    GUI.color = iconsColorEnableDark;
                }
            }
            else
            {
                if (!value)
                {
                    GUI.color = iconsColorDisableLight;
                }
                else
                {
                    GUI.color = iconsColorEnableLight;
                }
            }
        }

        void DrawLine()
        {
            GUI.color = new Color(0, 0, 0, 0.04f);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.EndVertical();
            GUI.color = Color.white;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUI.BeginDisabledGroup(instance.isCombined);
            GUILayout.Space(10);

            GUI.color = new Color(0, 0, 0, 0.1f);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = Color.white;

            EditorGUILayout.LabelField("SIZE SETTINGS", EditorStyles.boldLabel);

            EditorGUILayout.Slider(craneHeight, 2, 30, new GUIContent("Crane Height", "The mount height of the crane (the distance between the floor and the support beams)."));
            EditorGUILayout.Slider(craneWidth, 2, 20, new GUIContent("Crane Width", "The length of the main crane beam."));
            EditorGUILayout.Slider(craneLength, 2.5f, 30, new GUIContent("Crane Length", "The length of the support beams of the crane."));

            GUILayout.EndVertical();

            GUILayout.Space(10);

            GUI.color = new Color(0, 0, 0, 0.1f);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = Color.white;

            EditorGUILayout.LabelField("COLUMNS", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(useColumns, new GUIContent("Columns", "Enable columns which support the crane."));

            if (instance.useColumns)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("    Additional Columns" + " (" + instance.additionalColumnsCount + ")", "Add or remove extra columns."));
                if (GUILayout.Button("+"))
                {
                    instance.SetAdditionalColumns(1);

                    EditorUtility.SetDirty(instance);
                }

                EditorGUI.BeginDisabledGroup(instance.additionalColumnsCount <= 0);
                if (GUILayout.Button("-"))
                {
                    instance.SetAdditionalColumns(-1);

                    EditorUtility.SetDirty(instance);
                }
                EditorGUI.EndDisabledGroup();

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            GUILayout.Space(10);

            GUI.color = new Color(0, 0, 0, 0.1f);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = Color.white;

            EditorGUILayout.LabelField("DECALS", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(useMainBeamDecal, new GUIContent("Main Beam Decal", "Enable a main beam decal."));
            EditorGUILayout.PropertyField(useColumnDecal, new GUIContent("Column Decals", "Enable column decals."));

            GUILayout.EndVertical();

            EditorGUI.EndDisabledGroup();

            /*
            EditorGUILayout.CurveField(curve, Color.green, new Rect(), new GUIContent("Support Beam Cables Curve", "This is the support beam cables distribution curve."));
            EditorGUILayout.CurveField(cablesMainCurve, Color.green, new Rect(), new GUIContent("Main Beam Cables Curve", "This is the main beam cables distribution curve."));
            */

            GUILayout.Space(10);

            GUI.color = new Color(0, 0, 0, 0.1f);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = Color.white;

            EditorGUILayout.LabelField("CONTROLS", EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUILayout.Slider(initialBeamPos, 0, 1, new GUIContent("Beam", ""));
            EditorGUI.EndDisabledGroup();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("   Beam Speed", "The main beam speed."));
            EditorGUILayout.PropertyField(beamSpeed, GUIContent.none, GUILayout.MaxWidth(50));
            GUILayout.EndHorizontal();

            DrawLine();

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUILayout.Slider(initialTelpherPos, 0, 1, new GUIContent("Telpher", ""));
            EditorGUI.EndDisabledGroup();

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("   Telpher Speed", "The telpher speed."));
            EditorGUILayout.PropertyField(telpherSpeed, GUIContent.none, GUILayout.MaxWidth(50));
            GUILayout.EndHorizontal();

            DrawLine();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Slider(hookHeight, 0, 1, new GUIContent("Hook", "Is not allowed to edit through inspector. For debug only."));
            EditorGUI.EndDisabledGroup();

            /*
                if(GUILayout.Button(new GUIContent("Edit Hook Settings", "Click to select the hook object."), GUILayout.MaxWidth(120)))
                {
                    Selection.activeGameObject = instance.hook.wireHookMount.gameObject;
                }
                */

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("   Hook Speed", "The hook speed."));
            EditorGUILayout.PropertyField(hookSpeed, GUIContent.none, GUILayout.MaxWidth(50));
            GUILayout.EndHorizontal();

            DrawLine();

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUILayout.Slider(remoteHeight, 0, 1, new GUIContent("Remote", ""));
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(keyboardControls, new GUIContent("Keyboard Controls", "Enable keyboard controls (WASD QE)."));

            DrawLine();

            EditorGUILayout.PropertyField(useCableSwaying, new GUIContent("Cables Swaying", "Sway cables when the crane moves."));

            EditorGUI.BeginDisabledGroup(!instance.useCableSwaying);
            EditorGUILayout.Slider(cablesSwayAmplitude, 0, 1, new GUIContent("Cables Swaying Aplitude", "How much cables sway, when the crane moves."));
            EditorGUI.EndDisabledGroup();

            GUILayout.EndVertical();

            EditorGUI.BeginDisabledGroup(instance.isCombined);
            GUILayout.Space(10);

            GUI.color = new Color(0, 0, 0, 0.1f);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = Color.white;

            EditorGUILayout.LabelField("OPTIMIZATION", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("Combine All", "Combine all static meshes for the optimization purposes. This action will also generate colliders. WARNING! This action is not undoable!"), GUILayout.MaxWidth(90)))
            {
                bool combine = false;

                bool option = EditorUtility.DisplayDialog("Combine static meshes", "You are about to combine all static parts of the crane. This will improve performance. Also this action will generate colliders for the crane. This operation cannot be undone! \n Do you really want to continue?", "Yes", "No");

                switch (option)
                {
                    case true:
                        combine = true;
                        break;
                    case false:
                        combine = false;
                        break;
                }

                if (combine)
                {
                    instance.isCombined = true;
                    instance.BakeMeshes();
                    Undo.ClearAll();
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            EditorGUI.EndDisabledGroup();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                if (!instance.isCombined)
                {
                    if (instance.useColumns)
                    {
                        instance.SwitchColumns(true);
                    }
                    else
                    {
                        instance.SwitchColumns(false);
                    }

                    instance.CraneHeightChange();
                    instance.CraneWidthChange();
                    instance.CraneLengthChange();
                    instance.SwitchMainBeamDecal(instance.useMainBeamDecal);
                    instance.SwitchColumnDecal(instance.useColumnDecal);
                }
                instance.UpdateMainBeamPos(instance.initialBeamPos);
                instance.UpdateTelpherPos(instance.initialTelpherPos);
                instance.UpdateHookHeight();

                instance.UpdateRemoteHeight();
                instance.SwitchKeyboardControls(instance.keyboardControls);
            }
        }
    }
}