namespace NOT_Lonely
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
    [ExecuteInEditMode]
#endif

    public class NL_OverheadCrane : MonoBehaviour
    {
        public delegate void HookHeightChanged(float hookSpeed, float hookHeight, float craneHeight);
        public event HookHeightChanged OnHookHeightChanged;

        public delegate void HookRemoteChanged(float craneHeight, float remoteHeight);
        public event HookRemoteChanged OnRemoteHeightChanged;

        public delegate void SwitchKeyboard(bool value);
        public event SwitchKeyboard OnKeyboardControlsSwitched;

        private Transform supportBeams;
        private Transform movingParts;
        private Transform endBeams;
        private Transform endBeam_01;
        private Transform endBeam_02;
        private Transform mainBeam_end;
        private Transform telpherParent;
        private Transform telpherFan;
        private Transform supportBeam_01;
        private Transform supportBeam_01_end;
        private Transform supportBeam_02;
        private Transform supportBeam_02_end;
        private Transform columns_A;
        private Transform columns_B;
        private Transform column_01;
        private Transform column_01_end;
        private Transform column_02;
        private Transform column_02_end;
        private Transform column_03;
        private Transform column_03_end;
        private Transform column_04;
        private Transform column_04_end;
        private NL_Wheel[] beamWheels;
        private NL_Gear[] telpherGears;
        private MeshRenderer drumWires;
        private Transform telpherWireHolder;
        private Transform mainBeam_decal;

        private Transform cablesSupportBeam_parent;
        private Transform cablesSupportBeam_endPoint;
        private List<Transform> cablesSupportBeam_endPointParents = new List<Transform>();
        private List<Transform> cablesSupportBeam_endPoints = new List<Transform>();
        private List<Rigidbody> cablesSupportBeam_endPoints_rb = new List<Rigidbody>();
        private List<Rigidbody> cablesSupportBeam_startPoints_rb = new List<Rigidbody>();
        private List<Transform> cablesSupportBeam_startPoints = new List<Transform>();
        private List<SkinnedMeshRenderer> cablesSupportBeam_meshes = new List<SkinnedMeshRenderer>();

        private Transform cablesMainBeam_parent;
        private Transform cablesMainBeam_endPoint;
        private List<Transform> cablesMainBeam_endPointParents = new List<Transform>();
        private List<Transform> cablesMainBeam_endPoints = new List<Transform>();
        private List<Rigidbody> cablesMainBeam_endPoints_rb = new List<Rigidbody>();
        private List<Rigidbody> cablesMainBeam_startPoints_rb = new List<Rigidbody>();
        private List<Transform> cablesMainBeam_startPoints = new List<Transform>();
        private List<SkinnedMeshRenderer> cablesMainBeam_meshes = new List<SkinnedMeshRenderer>();

        private SkinnedMeshRenderer supportBeams_mesh;
        private SkinnedMeshRenderer endBeams_mesh;
        private SkinnedMeshRenderer mainBeam_mesh;

        public List<Transform> basicColumns = new List<Transform>();
        public List<Transform> basicColumnControls = new List<Transform>();
        public List<SkinnedMeshRenderer> basicColumnMeshes = new List<SkinnedMeshRenderer>();

        public List<Transform> extraColumns_A = new List<Transform>();
        public List<Transform> extraColumns_B = new List<Transform>();
        public List<Transform> extraColumnControls_A = new List<Transform>();
        public List<Transform> extraColumnControls_B = new List<Transform>();
        public List<SkinnedMeshRenderer> extraColumnMeshes_A = new List<SkinnedMeshRenderer>();
        public List<SkinnedMeshRenderer> extraColumnMeshes_B = new List<SkinnedMeshRenderer>();

        private List<Transform> columnDecals = new List<Transform>();

        public GameObject combinedColumns;
        public GameObject supportBeamObj;
        public GameObject mainBeamObj;

        private float fanRotationIncrement;
        private float fanTargetSpeed;
        private float fanSpeedEasing;
        private float curVel;

        private float refVel;

        private float rotation;
        private float lastPos;

        private AnimationCurve cablesSupportBeamCurve;
        private AnimationCurve cablesMainBeamCurve;
        private AnimationCurve scaleCurve;

        private AnimationCurve _curve = new AnimationCurve(new Keyframe(0.001f, 0.09f, 0.09f, 0.09f), new Keyframe(12.6f, 1, 0.99f, 1.01f));
        private float[] cablesSupportBeamRotValues;
        private float[] cablesSupportBeamScaleValues;

        private float[] cablesMainBeamRotValues;
        private float[] cablesMainBeamScaleValues;

        [SerializeField] public bool useColumns = true;
        public int additionalColumnsCount;
        [SerializeField] public float craneHeight = 2f;
        [SerializeField] public float craneWidth = 2f;
        [SerializeField] public float craneLength = 2.5f;
        [SerializeField] public float initialBeamPos = 0;
        [SerializeField] public float initialTelpherPos = 0;
        [SerializeField] public float hookHeight = 1;
        [SerializeField] public float remoteHeight = 0.5f;
        [SerializeField] public MaterialPropertyBlock mtlPropertyBlock;
        [SerializeField] public MaterialPropertyBlock wiresDrumPropBlock;
        [SerializeField] public bool useMainBeamDecal = true;
        [SerializeField] public bool useColumnDecal = true;
        [SerializeField] public bool isCombined = false;
        [SerializeField] public bool keyboardControls = true;
        [SerializeField] public float beamSpeed = 0.2f;
        [SerializeField] public float telpherSpeed = 0.2f;
        [SerializeField] public float hookSpeed = 0.2f;
        [SerializeField] public float cablesSwayAmplitude = 0.2f;
        [SerializeField] public bool useCableSwaying = true;

        public void Start()
        {
            //isCombined = false;
            Keyframe key0 = new Keyframe(0.001f, 0.09f, 0.006460873f, 0.006460873f, 0, 0.05366358f);
            Keyframe key1 = new Keyframe(12.6f, 1, 0.2544295f, 0.2544295f, 0.03715167f, 0);
            key0.weightedMode = WeightedMode.None;
            key1.weightedMode = WeightedMode.None;

            cablesSupportBeamCurve = new AnimationCurve(key0, key1);

            key0 = new Keyframe(0, 0.1523636f, 0.003327273f, 0.003327273f, 0, 0.07250284f);
            key1 = new Keyframe(8.261065f, 0.5611866f, 0.1484469f, 0.1484469f, 0.03614002f, 0);

            key0.weightedMode = WeightedMode.None;
            key1.weightedMode = WeightedMode.None;

            cablesMainBeamCurve = new AnimationCurve(key0, key1);

            key0 = new Keyframe(0, 0, 0, 0, 0, 0);
            key1 = new Keyframe(1, 1, 2, 2, 0, 0);

            key0.weightedMode = WeightedMode.None;
            key1.weightedMode = WeightedMode.None;

            scaleCurve = new AnimationCurve(key0, key1);

            supportBeams = GetChildByName(transform, "SupportBeams");
            movingParts = GetChildByName(transform, "MovingParts");
            endBeams = GetChildByName(transform, "EndBeams");
            endBeam_01 = GetChildByName(transform, "EndBeam_01");
            endBeam_02 = GetChildByName(transform, "EndBeam_02");
            mainBeam_end = GetChildByName(transform, "MainBeam_end");
            telpherParent = GetChildByName(transform, "TelpherParent");
            telpherFan = GetChildByName(telpherParent, "Telpher_fan");
            supportBeam_01= GetChildByName(transform, "SupportBeam_01");
            supportBeam_01_end = GetChildByName(transform, "SupportBeam_01_end");
            supportBeam_02 = GetChildByName(transform, "SupportBeam_02");
            supportBeam_02_end = GetChildByName(transform, "SupportBeam_02_end");
            columns_A = GetChildByName(transform, "Columns_A");
            columns_B = GetChildByName(transform, "Columns_B");
            column_01 = GetChildByName(transform, "Column_01");
            column_01_end = GetChildByName(column_01, "Column_01_end");
            column_02 = GetChildByName(transform, "Column_02");
            column_02_end = GetChildByName(column_02, "Column_02_end");
            column_03 = GetChildByName(transform, "Column_03");
            column_03_end = GetChildByName(column_03, "Column_03_end");
            column_04 = GetChildByName(transform, "Column_04");
            column_04_end = GetChildByName(column_04, "Column_04_end");
            drumWires = GetChildByName(telpherParent, "Drum_wires").GetComponent<MeshRenderer>();
            telpherWireHolder = GetChildByName(telpherParent, "Telpher_wireHolder");
            mainBeam_decal = GetChildByName(transform, "MainBeam_decal");

            cablesSupportBeam_endPoint = GetChildByName(transform, "CablesSupportBeam_endPoint");
            cablesSupportBeam_parent = GetChildByName(transform, "Cables_supportBeam");
            cablesSupportBeam_endPointParents = new List<Transform>();
            cablesSupportBeam_endPoints = new List<Transform>();
            cablesSupportBeam_startPoints = new List<Transform>();
            cablesSupportBeam_meshes = new List<SkinnedMeshRenderer>();

            cablesMainBeam_endPoint = GetChildByName(transform, "CablesMainBeam_endPoint");
            cablesMainBeam_parent = GetChildByName(transform, "Cables_mainBeam");
            cablesMainBeam_endPointParents = new List<Transform>();
            cablesMainBeam_endPoints = new List<Transform>();
            cablesMainBeam_startPoints = new List<Transform>();
            cablesMainBeam_meshes = new List<SkinnedMeshRenderer>();


            SwitchKeyboardControls(keyboardControls);

            InitCables(cablesSupportBeam_parent, cablesSupportBeam_endPointParents, cablesSupportBeam_endPoints, cablesSupportBeam_endPoints_rb, cablesSupportBeam_startPoints, cablesSupportBeam_startPoints_rb, cablesSupportBeam_meshes);
            cablesSupportBeamRotValues = GetRandomFloatValues(cablesSupportBeamRotValues, cablesSupportBeam_endPointParents, -30, 30);
            cablesSupportBeamScaleValues = GetRandomFloatValues(cablesSupportBeamScaleValues, cablesSupportBeam_endPointParents, 0.9f, 1);

            InitCables(cablesMainBeam_parent, cablesMainBeam_endPointParents, cablesMainBeam_endPoints, cablesMainBeam_endPoints_rb, cablesMainBeam_startPoints, cablesMainBeam_startPoints_rb, cablesMainBeam_meshes);
            cablesMainBeamRotValues = GetRandomFloatValues(cablesMainBeamRotValues, cablesSupportBeam_endPointParents, -30, 30);
            cablesMainBeamScaleValues = GetRandomFloatValues(cablesMainBeamScaleValues, cablesSupportBeam_endPointParents, 0.9f, 1);

            /*
            extraColumns_A = new List<Transform>();
            extraColumns_B = new List<Transform>();
            extraColumnControls_A = new List<Transform>();
            extraColumnControls_B = new List<Transform>();
            extraColumnMeshes_A = new List<SkinnedMeshRenderer>();
            extraColumnMeshes_B = new List<SkinnedMeshRenderer>();
            */

            mtlPropertyBlock = new MaterialPropertyBlock();
            wiresDrumPropBlock = new MaterialPropertyBlock();

            supportBeams_mesh = supportBeam_01.GetComponentInChildren<SkinnedMeshRenderer>();
            endBeams_mesh = endBeam_01.GetComponentInChildren<SkinnedMeshRenderer>();
            mainBeam_mesh = FindNeighbourMesh(mainBeam_end);

            beamWheels = endBeams.GetComponentsInChildren<NL_Wheel>();
            telpherGears = telpherParent.GetComponentsInChildren<NL_Gear>();

            isCombined = !mainBeam_mesh.enabled;

            InitBasicColumns();
            SwitchColumns(useColumns);

            UpdateMainBeamPos(initialBeamPos);
            UpdateTelpherPos(initialTelpherPos);
            UpdateHookHeight();
            UpdateRemoteHeight();
            if (!isCombined)
            {
                CraneHeightChange();
                CraneWidthChange();
                CraneLengthChange();
            }
            else if (combinedColumns != null && supportBeamObj != null)
            {
                SetPropBlocksForCombinedMeshes();
            }

            for (int i = 0; i < basicColumns.Count; i++)
            {
                basicColumns[i].localEulerAngles = new Vector3(0, StepAngleRandomize(), 0);
            }

#if UNITY_EDITOR
            if (PrefabUtility.GetCorrespondingObjectFromSource(this.gameObject) != null)
            {
                PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }
#endif
        }

#if UNITY_EDITOR
        public void BakeMeshes()
        {
            //PrefabUtility.UnpackPrefabInstance(this.gameObject, UnityEditor.PrefabUnpackMode.Completely, UnityEditor.InteractionMode.AutomatedAction);

            //merge all column lists
            List<SkinnedMeshRenderer> allColumns = new List<SkinnedMeshRenderer>();
            for (int i = 0; i < basicColumnMeshes.Count; i++)
            {
                allColumns.Add(basicColumnMeshes[i]);
            }
            for (int i = 0; i < extraColumnMeshes_A.Count; i++)
            {
                allColumns.Add(extraColumnMeshes_A[i]);
            }
            for (int i = 0; i < extraColumnMeshes_B.Count; i++)
            {
                allColumns.Add(extraColumnMeshes_B[i]);
            }

            //columns
            List<MeshFilter> columnMeshFilters = new List<MeshFilter>();
            for (int i = 0; i < allColumns.Count; i++)
            {
                Mesh columnBakeMesh = new Mesh();
                columnBakeMesh.name = allColumns[i].transform.parent.name + "_mesh";

                Material[] columnMaterials = allColumns[i].sharedMaterials;

                allColumns[i].BakeMesh(columnBakeMesh);
                allColumns[i].enabled = false;

                MeshFilter mFilter = allColumns[i].gameObject.AddComponent<MeshFilter>();
                mFilter.mesh = columnBakeMesh;

                MeshRenderer mRenderer = allColumns[i].gameObject.AddComponent<MeshRenderer>();
                mRenderer.sharedMaterials = columnMaterials;
                columnMeshFilters.Add(mFilter);
            }

            //combine columns into a single mesh   
            List<CombineInstance> combine = new List<CombineInstance>();

            for (int i = 0; i < columnMeshFilters.Count; i++)
            {
                for (int j = 0; j < columnMeshFilters[i].sharedMesh.subMeshCount; j++)
                {
                    CombineInstance ci = new CombineInstance();

                    ci.mesh = columnMeshFilters[i].sharedMesh;
                    ci.subMeshIndex = j;
                    ci.transform = columnMeshFilters[i].transform.localToWorldMatrix;

                    combine.Add(ci);
                }

                columnMeshFilters[i].gameObject.SetActive(false);
            }

            combinedColumns = Instantiate(allColumns[0].gameObject);
            combinedColumns.name = "Columns";
            DestroyImmediate(combinedColumns.GetComponentInChildren<SkinnedMeshRenderer>(true));
            combinedColumns.transform.SetParent(transform);
            combinedColumns.SetActive(true);

            combinedColumns.GetComponent<MeshFilter>().sharedMesh = new Mesh();
            combinedColumns.GetComponent<MeshFilter>().sharedMesh.name = "Columns_mesh";
            combinedColumns.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine.ToArray(), true, true);
            combinedColumns.AddComponent<MeshRenderer>().sharedMaterials = allColumns[0].sharedMaterials;

            Mesh combinedMesh = combinedColumns.GetComponent<MeshFilter>().sharedMesh;
            combinedMesh.subMeshCount = allColumns[0].sharedMesh.subMeshCount;

            

            int[] mainSubTri;
            int[] newSubTri;
            for (int i = 0; i < columnMeshFilters[0].sharedMesh.subMeshCount; i++)
            {
                mainSubTri = columnMeshFilters[0].sharedMesh.GetTriangles(i);

                newSubTri = new int[mainSubTri.Length * columnMeshFilters.Count];

                for (int ii = 0; ii < columnMeshFilters.Count; ii++)
                {
                    for (int iii = 0; iii < mainSubTri.Length; iii++)
                    {
                        newSubTri[(ii * mainSubTri.Length) + iii] = mainSubTri[iii] + (ii * columnMeshFilters[0].sharedMesh.vertexCount);
                    }
                }
                combinedMesh.SetTriangles(newSubTri, i);
            }
            
            UnwrapParam unwrapSettings = new UnwrapParam();
            unwrapSettings.hardAngle = 88;
            unwrapSettings.packMargin = 0.04f;
            unwrapSettings.angleError = 8;
            unwrapSettings.areaError = 15;

            Unwrapping.GenerateSecondaryUVSet(combinedMesh, unwrapSettings);
            

            SerializedObject so = new SerializedObject(combinedColumns.GetComponent<MeshRenderer>());

            SerializedProperty stitchProp = so.FindProperty("m_StitchLightmapSeams");
            stitchProp.boolValue = true;

            SerializedProperty scaleInLightmap = so.FindProperty("m_ScaleInLightmap");
            scaleInLightmap.floatValue = 6;

            so.ApplyModifiedProperties();


            combinedColumns.GetComponent<MeshFilter>().mesh = combinedMesh;

            combinedColumns.AddComponent<MeshCollider>().sharedMesh = combinedMesh;

            if (!useColumns)
            {
                combinedColumns.SetActive(false);
            }

            //support beam
            Mesh supportBeamBakeMesh = new Mesh();
            supportBeamBakeMesh.name = "SupportBeams_mesh";

            supportBeamObj = Instantiate(supportBeams_mesh.gameObject);
            supportBeamObj.name = "SupportBeams_converted";
            DestroyImmediate(supportBeamObj.GetComponent<SkinnedMeshRenderer>());

            supportBeamObj.transform.parent = supportBeam_01;
            supportBeamObj.transform.localPosition = Vector3.zero;
            supportBeamObj.transform.localEulerAngles = Vector3.zero;

            Material[] supportBeamsMaterials = supportBeams_mesh.sharedMaterials;

            supportBeams_mesh.BakeMesh(supportBeamBakeMesh);
            supportBeams_mesh.enabled = false;

            MeshFilter mFilterSupportBeams = supportBeamObj.AddComponent<MeshFilter>();
            mFilterSupportBeams.sharedMesh = supportBeamBakeMesh;

            supportBeamObj.AddComponent<MeshRenderer>();
            supportBeamObj.GetComponent<MeshRenderer>().sharedMaterials = supportBeamsMaterials;

            supportBeamObj.AddComponent<MeshCollider>().sharedMesh = supportBeamBakeMesh;

            SerializedObject supportBeamSO = new SerializedObject(supportBeamObj.GetComponent<MeshRenderer>());
            SerializedObject supportBeamObjSO = new SerializedObject(supportBeamObj);

            SerializedProperty supportBeamStaticFlags = supportBeamObjSO.FindProperty("m_StaticEditorFlags");
            supportBeamStaticFlags.intValue = 127;

            supportBeamObjSO.ApplyModifiedProperties();

            SerializedProperty supportBeamStitchProp = supportBeamSO.FindProperty("m_StitchLightmapSeams");
            supportBeamStitchProp.boolValue = true;

            SerializedProperty supportBeamScaleInLightmap = supportBeamSO.FindProperty("m_ScaleInLightmap");
            supportBeamScaleInLightmap.floatValue = 2;

            supportBeamSO.ApplyModifiedProperties();

            //main beam
            Mesh mainBeamBakeMesh = new Mesh();
            mainBeamBakeMesh.name = "MainBeam_mesh";

            mainBeamObj = Instantiate(mainBeam_mesh.gameObject);
            mainBeamObj.name = "MainBeam_converted";
            DestroyImmediate(mainBeamObj.GetComponent<SkinnedMeshRenderer>());

            mainBeamObj.transform.parent = mainBeam_mesh.transform.parent;
            mainBeamObj.transform.localPosition = Vector3.zero;
            mainBeamObj.transform.localEulerAngles = Vector3.zero;

            Material[] mainBeamsMaterials = mainBeam_mesh.sharedMaterials;

            mainBeam_mesh.BakeMesh(mainBeamBakeMesh);
            mainBeam_mesh.enabled = false;

            MeshFilter mFilterMainBeams = mainBeamObj.gameObject.AddComponent<MeshFilter>();
            mFilterMainBeams.sharedMesh = mainBeamBakeMesh;

            mainBeamObj.gameObject.AddComponent<MeshRenderer>();
            mainBeamObj.GetComponent<MeshRenderer>().sharedMaterials = mainBeamsMaterials;

            mainBeamObj.AddComponent<MeshCollider>().sharedMesh = mainBeamBakeMesh;

            //column decals
            List<MeshFilter> columnDecalFilters = new List<MeshFilter>();
            for (int i = 0; i < columnMeshFilters.Count; i++)
            {
                columnDecalFilters.Add(columnMeshFilters[i].transform.parent.GetComponentInChildren<MeshFilter>());
            }

            CombineInstance[] decalsCombine = new CombineInstance[columnDecalFilters.Count];

            GameObject combinedDecals = Instantiate(columnMeshFilters[0].gameObject);
            combinedDecals.name = "ColumnDecals";
            combinedDecals.gameObject.SetActive(true);
            DestroyImmediate(combinedDecals.GetComponent<SkinnedMeshRenderer>());

            combinedDecals.GetComponent<MeshFilter>().mesh = new Mesh();
            combinedDecals.GetComponent<MeshFilter>().sharedMesh.name = "ColumnDecals_combined";
            combinedDecals.AddComponent<MeshRenderer>().sharedMaterial = columnDecalFilters[0].GetComponent<MeshRenderer>().sharedMaterial;

            combinedDecals.transform.SetParent(columns_A);

            for (int i = 0; i < columnDecalFilters.Count; i++)
            {
                decalsCombine[i].mesh = columnDecalFilters[i].sharedMesh;
                decalsCombine[i].transform = columnDecalFilters[i].transform.localToWorldMatrix;
                columnDecalFilters[i].gameObject.SetActive(false);
            }

            combinedDecals.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(decalsCombine, true, true);

            Unwrapping.GenerateSecondaryUVSet(combinedDecals.GetComponent<MeshFilter>().sharedMesh, unwrapSettings);

            SerializedObject decalsSO = new SerializedObject(combinedDecals.GetComponent<MeshRenderer>());

            SerializedProperty decalsStitchProp = decalsSO.FindProperty("m_StitchLightmapSeams");
            decalsStitchProp.boolValue = true;

            SerializedProperty castShadows = decalsSO.FindProperty("m_CastShadows");
            castShadows.enumValueIndex = 0;

            decalsSO.ApplyModifiedProperties();

            SetPropBlocksForCombinedMeshes();
        }
#endif

        void SetPropBlocksForCombinedMeshes()
        {
            MaterialPropertyBlock columnsPropBlock = new MaterialPropertyBlock();
            columnsPropBlock.SetVector("_TilingXY", new Vector2(craneHeight * 0.333f, 1));
            combinedColumns.GetComponent<MeshRenderer>().SetPropertyBlock(columnsPropBlock);

            MaterialPropertyBlock supportBeamPropBlock = new MaterialPropertyBlock();
            supportBeamPropBlock.SetVector("_TilingXY", new Vector2(craneLength * 0.2f, 1));
            supportBeamObj.GetComponent<MeshRenderer>().SetPropertyBlock(supportBeamPropBlock);

            MaterialPropertyBlock maintBeamPropBlock = new MaterialPropertyBlock();
            maintBeamPropBlock.SetVector("_TilingXY", new Vector2(craneWidth * 0.2f, 1));
            mainBeamObj.GetComponent<MeshRenderer>().SetPropertyBlock(maintBeamPropBlock);
        }

        private float[] GetRandomFloatValues(float[] valuesArray, List<Transform> endPointParents, float min, float max)
        {
            valuesArray = new float[endPointParents.Count];
            for (int i = 0; i < valuesArray.Length; i++)
            {
                valuesArray[i] = Random.Range(min, max);
            }
            return valuesArray;
        }

        private void InitCables(Transform cablesParent, List<Transform> endPointParents, List<Transform> endPoints, List<Rigidbody> endPoints_rb, List<Transform> startPoints, List<Rigidbody> startPoints_rb, List<SkinnedMeshRenderer> meshes)
        {
            NL_CablePoint[] tempPoints = cablesParent.GetComponentsInChildren<NL_CablePoint>();
            for (int i = 0; i < tempPoints.Length; i++)
            {
                if (tempPoints[i].pointName == "End_parent")
                {
                    endPointParents.Add(tempPoints[i].transform);
                    meshes.Add(FindNeighbourMesh(tempPoints[i].transform));
                }
                if (tempPoints[i].pointName == "End")
                {
                    endPoints.Add(tempPoints[i].transform);

                    if (useCableSwaying && Application.isPlaying)
                    {
                        SetCablesForSwaying(i, endPoints_rb, tempPoints);
                    }
                }
                if (tempPoints[i].pointName == "Start")
                {
                    startPoints.Add(tempPoints[i].transform);

                    if (useCableSwaying && Application.isPlaying)
                    {
                        SetCablesForSwaying(i, startPoints_rb, tempPoints);
                    }
                }
            }        
        }

        void SetCablesForSwaying(int i, List<Rigidbody> points_rb, NL_CablePoint[] tempPoints)
        {
            Rigidbody parentRb;
            if (tempPoints[i].transform.parent.GetComponent<Rigidbody>() == null)
            {
                parentRb = tempPoints[i].transform.parent.gameObject.AddComponent<Rigidbody>();
            }
            else
            {
                parentRb = tempPoints[i].transform.parent.GetComponent<Rigidbody>();
            }

            parentRb.isKinematic = true;
            parentRb.useGravity = false;

            Rigidbody rb;
            if (tempPoints[i].GetComponent<Rigidbody>() == null)
            {
                rb = tempPoints[i].gameObject.AddComponent<Rigidbody>();
                points_rb.Add(rb);

            }
            else
            {
                rb = tempPoints[i].GetComponent<Rigidbody>();
                points_rb.Add(rb);
            }

            rb.useGravity = false;

            HingeJoint joint = tempPoints[i].gameObject.AddComponent<HingeJoint>();
            joint.axis = new Vector3(0, 0, 1);
            joint.useSpring = true;
            JointSpring spring = new JointSpring();
            spring.spring = 10;
            joint.spring = spring;
            joint.connectedBody = parentRb;
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                fanRotationIncrement = Mathf.SmoothDamp(fanRotationIncrement, fanTargetSpeed, ref curVel, fanSpeedEasing);
                telpherFan.localEulerAngles = new Vector3(0, 0, telpherFan.localEulerAngles.z + fanRotationIncrement);

                fanTargetSpeed = 0;
                fanSpeedEasing = 2;
            }
        }

        public void SwitchKeyboardControls(bool value)
        {
            OnKeyboardControlsSwitched?.Invoke(value);
        }

        public void SwitchMainBeamDecal(bool value)
        {
            mainBeam_decal.gameObject.SetActive(value);
        }

        public void SwitchColumnDecal(bool value)
        {
            for (int i = 0; i < basicColumns.Count; i++)
            {
                Transform decal = GetChildByName(basicColumns[i], "Column_decal");
                decal.gameObject.SetActive(value);
            }

            for (int i = 0; i < extraColumns_A.Count; i++)
            {
                Transform decal = GetChildByName(extraColumns_A[i], "Column_decal");
                decal.gameObject.SetActive(value);
            }

            for (int i = 0; i < extraColumns_B.Count; i++)
            {
                Transform decal = GetChildByName(extraColumns_B[i], "Column_decal");
                decal.gameObject.SetActive(value);
            }
        }

        public void UpdateHookHeight()
        { 
            float maxPos = craneHeight + 0.23f;
            float interpolatedValue = Mathf.Lerp(-maxPos, -0.5f, hookHeight);

            OnHookHeightChanged?.Invoke(hookSpeed, hookHeight, craneHeight);

            float angle = (10 / (0.138f * 2 * Mathf.PI)) * 360 * hookHeight;
            drumWires.transform.parent.localEulerAngles = new Vector3(0, 0, angle);

            wiresDrumPropBlock = new MaterialPropertyBlock();
            wiresDrumPropBlock.SetFloat("_Cutoff", Mathf.Lerp(0.99f, 0.16f, hookHeight));
            drumWires.SetPropertyBlock(wiresDrumPropBlock);

            telpherWireHolder.localPosition = new Vector3(0, 0, Mathf.Lerp(-0.31f, 0, hookHeight));

            fanTargetSpeed = 20;
            fanSpeedEasing = 0.3f;
        }

        public void MoveHookUp()
        {
            if(hookHeight < 1)
            {
                hookHeight += hookSpeed * Time.deltaTime / (craneHeight + 0.23f);
                UpdateHookHeight();
            }
            else if(hookHeight > 1)
            {
                hookHeight = 1;
            }
        }

        public void MoveHookDown()
        {
            if (hookHeight > 0)
            {
                hookHeight -= hookSpeed * Time.deltaTime / (craneHeight + 0.23f);
                UpdateHookHeight();
            }
            else if (hookHeight < 0)
            {
                hookHeight = 0;
            }
        }

        public void MoveCraneForward()
        {
            if (initialBeamPos < 1)
            {
                initialBeamPos += beamSpeed * Time.deltaTime / (craneLength - 2.316f);
                UpdateMainBeamPos(initialBeamPos);
            }
        }

        public void MoveCraneBackward()
        {
            if (initialBeamPos > 0)
            {
                initialBeamPos -= beamSpeed * Time.deltaTime / (craneLength - 2.316f);
                UpdateMainBeamPos(initialBeamPos);
            }
        }

        public void MoveCraneLeft()
        {
            if (initialTelpherPos < 1)
            {
                initialTelpherPos += telpherSpeed * Time.deltaTime / (craneWidth - 1.57f);
                UpdateTelpherPos(initialTelpherPos);
            }
        }

        public void CraneXMoveChanged()
        {
            if (!useCableSwaying)
                return;

            CablesSway(cablesMainBeam_endPointParents, cablesMainBeam_endPoints_rb, cablesMainBeam_startPoints_rb, initialTelpherPos);
        }

        public void CraneZMoveChanged()
        {
            if (!useCableSwaying)
                return;

            CablesSway(cablesMainBeam_endPointParents, cablesMainBeam_endPoints_rb, cablesMainBeam_startPoints_rb, initialBeamPos);
            CablesSway(cablesSupportBeam_endPointParents, cablesSupportBeam_endPoints_rb, cablesSupportBeam_startPoints_rb, initialBeamPos);
        }

        public void MoveCraneRight()
        {
            if (initialTelpherPos > 0)
            {
                initialTelpherPos -= telpherSpeed * Time.deltaTime / (craneWidth - 1.57f);
                UpdateTelpherPos(initialTelpherPos);
            }
        }

        public void UpdateRemoteHeight()
        {
            OnRemoteHeightChanged?.Invoke(craneHeight + 0.768f, remoteHeight);
        }

        public void UpdateMainBeamPos(float position)
        {
            //beam pos
            float maxPos = (craneLength - 2.316f);
            movingParts.localPosition = new Vector3(0, 0, maxPos * position);

            //wheels rotation
            for (int i = 0; i < beamWheels.Length; i++)
            {
                float angle = (maxPos / (beamWheels[i].radius * 2 * Mathf.PI)) * 360 * position;
                beamWheels[i].transform.localEulerAngles = new Vector3(angle, 0, 0);
            }
            CablesUpdate(cablesSupportBeam_parent, cablesSupportBeam_endPoint, cablesSupportBeam_endPointParents, cablesSupportBeam_endPoints, cablesSupportBeam_startPoints, cablesSupportBeamRotValues, cablesSupportBeamScaleValues, cablesSupportBeam_meshes, cablesSupportBeamCurve, scaleCurve);
        }

        public void UpdateTelpherPos(float position)
        {
            float maxPos = -(craneWidth - 1.57f);
            telpherParent.localPosition = new Vector3(maxPos * position, 0, 0);

            //gears rotation
            for (int i = 0; i < telpherGears.Length; i++)
            {
                float angle = (maxPos / (telpherGears[i].radius * 2 * Mathf.PI)) * 360 * position;
                telpherGears[i].transform.localEulerAngles = new Vector3(angle, 0, 0);
            }

            CablesUpdate(cablesMainBeam_parent, cablesMainBeam_endPoint, cablesMainBeam_endPointParents, cablesMainBeam_endPoints, cablesMainBeam_startPoints, cablesMainBeamRotValues, cablesMainBeamScaleValues, cablesMainBeam_meshes, cablesMainBeamCurve, scaleCurve);
        }

        private void CablesSway(List<Transform> endPointParents, List<Rigidbody> endPoints_rb, List<Rigidbody> startPoints_rb, float t)
        {
            if (t > 0 && t < 1)
            {
                for (int i = 0; i < endPointParents.Count; i++)
                {
                    float value = Mathf.Clamp(i * t * cablesSwayAmplitude, 0.05f, 0.8f);
                    endPoints_rb[i].AddRelativeTorque(0, 0, Random.Range(-value, value), ForceMode.Impulse);
                }

                for (int i = endPointParents.Count - 1; i >= 0; i--)
                {
                    float value = Mathf.Clamp((i - (startPoints_rb.Count - 1)) * -t * cablesSwayAmplitude, 0.05f, 0.8f);
                    startPoints_rb[i].AddRelativeTorque(0, 0, Random.Range(-value, value), ForceMode.Impulse);
                }
            }
        }

        private void CablesUpdate(Transform cablesParent, Transform cablesEndPoint, List<Transform> endPointParents, List<Transform> endPoints, List<Transform> startPoints, float[] rotValues, float[] scaleValues, List<SkinnedMeshRenderer> meshes, AnimationCurve distributionCurve, AnimationCurve _scaleCurve)
        {
            float totalCableLength = Vector3.Distance(cablesParent.position, cablesEndPoint.position);

            for (int i = 0; i < endPointParents.Count; i++)
            {
                //position
                endPointParents[i].localPosition = new Vector3(0, 0, Mathf.Clamp((totalCableLength * distributionCurve.Evaluate(i - 1)), 0.00001f, 9));

                //rotation
                //endPoints[i].localEulerAngles = new Vector3(0, rotValues[i], 0);

                //scale

                float fraction = Mathf.InverseLerp(0.1f, 4, endPointParents[i].localPosition.z);
                float value = _scaleCurve.Evaluate(fraction);
                float scaleValue = Mathf.Lerp(1, 0.5f, value) * scaleValues[i];

                endPoints[i].localScale = new Vector3(1, scaleValue, 1);

                //bounds update
                Vector3 boundsCenter = meshes[i].localBounds.center;
                Vector3 boundsSize = meshes[i].localBounds.size;

                meshes[i].localBounds = new Bounds(new Vector3(boundsCenter.x, boundsCenter.y, endPointParents[i].localPosition.z / 2), new Vector3(boundsSize.x, boundsSize.y, endPointParents[i].localPosition.z));
            }
            for (int i = endPointParents.Count - 1; i >= 0; i--)
            {
                //rotation
                //startPoints[i].localEulerAngles = new Vector3(0, rotValues[(i - (rotValues.Length - 1)) * -1], 0);

                //scale

                float fraction = Mathf.InverseLerp(0.1f, 4, endPointParents[i].localPosition.z);
                float value = _scaleCurve.Evaluate(fraction);
                float scaleValue = Mathf.Lerp(1, 0.5f, value) * scaleValues[i];

                startPoints[(i - (startPoints.Count - 1)) * -1].localScale = new Vector3(1, scaleValue, 1);
            }
        }

        private SkinnedMeshRenderer FindNeighbourMesh(Transform neighbour)
        {
            SkinnedMeshRenderer mesh = null;
            SkinnedMeshRenderer[] allChildMeshes = neighbour.parent.GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int i = 0; i < allChildMeshes.Length; i++)
            {
                if (allChildMeshes[i].transform.parent == neighbour.parent)
                {
                    mesh = allChildMeshes[i];
                    //Debug.Log(mesh);
                }
            }

            return mesh;
        }

        private Transform GetChildByName(Transform parent, string childName)
        {
            Transform[] allChildren = parent.GetComponentsInChildren<Transform>(true);
            Transform child = null;

            for (int i = 0; i < allChildren.Length; i++)
            {
                if (allChildren[i].name == childName)
                {
                    child = allChildren[i];
                }
            }

            if (child != null)
            {
                //Debug.Log("Object " + child.name + " found");
                return child;
            }
            else
            {
                Debug.Log("Object with name " + childName + " NOT found");
                return null;
            }
        }

        private float StepAngleRandomize()
        {
            float[] values = new float[4];
            values[0] = 0;
            values[1] = 90;
            values[2] = 180;
            values[3] = 270;

            int value = Random.Range(0, values.Length);

            return values[value];
        }

        private float CalculatePositionZ(float totalDistance, int itemsCount, int itemIndex)
        {
            if (itemIndex == 0)
            {
                itemIndex = 1;
            }

            float value = (totalDistance / (itemsCount + 1)) * itemIndex;
            return value;
        }

        private void InitBasicColumns()
        {
            basicColumns = new List<Transform>();
            basicColumnControls = new List<Transform>();
            basicColumnMeshes = new List<SkinnedMeshRenderer>();

            //add basic 4 columns into the common columns list
            basicColumns.Add(column_01);
            basicColumns.Add(column_02);
            basicColumns.Add(column_03);
            basicColumns.Add(column_04);

            //add basic column control points into the list
            basicColumnControls.Add(column_01_end);
            basicColumnControls.Add(column_02_end);
            basicColumnControls.Add(column_03_end);
            basicColumnControls.Add(column_04_end);

            //add basic column SkinnedMeshRenderers into the list
            basicColumnMeshes.Add(column_01.GetComponentInChildren<SkinnedMeshRenderer>());
            basicColumnMeshes.Add(column_02.GetComponentInChildren<SkinnedMeshRenderer>());
            basicColumnMeshes.Add(column_03.GetComponentInChildren<SkinnedMeshRenderer>());
            basicColumnMeshes.Add(column_04.GetComponentInChildren<SkinnedMeshRenderer>());
        }

        public void SwitchColumns(bool value)
        {
            columns_A.gameObject.SetActive(value);
            columns_B.gameObject.SetActive(value);
        }

        public void SetAdditionalColumns(int count)
        {
            additionalColumnsCount += count;
            if (additionalColumnsCount < 0)
            {
                additionalColumnsCount = 0;
            }

            if (additionalColumnsCount > 0 && count == 1)
            {
                AddExtraColumn(extraColumns_A, extraColumnControls_A, extraColumnMeshes_A, columns_A);
                AddExtraColumn(extraColumns_B, extraColumnControls_B, extraColumnMeshes_B, columns_B);
            }
            else
            {
                //remove last two
                RemoveExtraColumn(extraColumns_A, extraColumnControls_A, extraColumnMeshes_A);
                RemoveExtraColumn(extraColumns_B, extraColumnControls_B, extraColumnMeshes_B);
            }
        }

        private void AddExtraColumn(List<Transform> listToAdd, List<Transform> extraControls, List<SkinnedMeshRenderer> meshes, Transform parentObj)
        {

            Transform newColumn = Instantiate(column_01);

            listToAdd.Add(newColumn);
            extraControls.Add(newColumn.GetComponentInChildren<NL_ColumnControl>().transform);
            meshes.Add(newColumn.GetComponentInChildren<SkinnedMeshRenderer>());

            //Debug.Log("Item added");

            newColumn.SetParent(parentObj);

            UpdateExtraColumnsPos(listToAdd);

            newColumn.localEulerAngles = new Vector3(0, StepAngleRandomize(), 0);
            //Debug.Log(extraColumns_A.Count);
        }

        private void RemoveExtraColumn(List<Transform> listToRemoveFrom, List<Transform> listOfColumnControls, List<SkinnedMeshRenderer> meshes)
        {
            DestroyImmediate(listToRemoveFrom[listToRemoveFrom.Count - 1].gameObject);
            listToRemoveFrom.RemoveAt(listToRemoveFrom.Count - 1);

            listOfColumnControls.RemoveAt(listOfColumnControls.Count - 1);
            meshes.RemoveAt(meshes.Count - 1);

            UpdateExtraColumnsPos(listToRemoveFrom);
        }

        private void UpdateExtraColumnsPos(List<Transform> listOfColumns)
        {
            for (int i = 0; i < listOfColumns.Count; i++)
            {
                listOfColumns[i].localPosition = new Vector3(0, 0, CalculatePositionZ(column_02.localPosition.z, listOfColumns.Count, i + 1));
            }
        }

        private void UpdateExtraColumnsHeight(List<Transform> listOfColumnControls, List<SkinnedMeshRenderer> meshes)
        {
            for (int i = 0; i < listOfColumnControls.Count; i++)
            {
                listOfColumnControls[i].localPosition = new Vector3(0, craneHeight, 0);

                Vector3 boundsCenter = meshes[i].localBounds.center;
                Vector3 boundsSize = meshes[i].localBounds.size;
                meshes[i].localBounds = new Bounds(new Vector3(boundsCenter.x, listOfColumnControls[i].localPosition.y / 2, boundsCenter.z), new Vector3(boundsSize.x, listOfColumnControls[i].localPosition.y, boundsSize.z));

                meshes[i].SetPropertyBlock(SetMtlPropertyBlock(meshes[i], mtlPropertyBlock, extraColumnControls_A[i].localPosition.y * 0.333f));
            }
        }

        public void CraneHeightChange()
        {
            supportBeams.localPosition = new Vector3(0, craneHeight, 0);
            
            for (int i = 0; i < basicColumnControls.Count; i++)
            {
                basicColumnControls[i].localPosition = new Vector3(0, craneHeight, 0);

                Vector3 boundsCenter = basicColumnMeshes[i].localBounds.center;
                Vector3 boundsSize = basicColumnMeshes[i].localBounds.size;
                basicColumnMeshes[i].localBounds = new Bounds(new Vector3(boundsCenter.x, basicColumnControls[i].localPosition.y / 2, boundsCenter.z), new Vector3 (boundsSize.x, basicColumnControls[i].localPosition.y, boundsSize.z));

                basicColumnMeshes[i].SetPropertyBlock(SetMtlPropertyBlock(basicColumnMeshes[i], mtlPropertyBlock, basicColumnControls[i].localPosition.y * 0.333f));
                basicColumnMeshes[i].SetPropertyBlock(mtlPropertyBlock);
            }

            UpdateExtraColumnsHeight(extraColumnControls_A, extraColumnMeshes_A);
            UpdateExtraColumnsHeight(extraColumnControls_B, extraColumnMeshes_B);
        }

        private MaterialPropertyBlock SetMtlPropertyBlock(SkinnedMeshRenderer mesh, MaterialPropertyBlock propBlock, float xTiling)
        {
            propBlock = new MaterialPropertyBlock();
            mtlPropertyBlock = propBlock;

            mtlPropertyBlock.SetVector("_TilingXY", new Vector2(xTiling, 1));
            //mtlPropertyBlock.SetFloat("_OffsetX", Random.Range(0f, 1f));

            return mtlPropertyBlock;
        }

        public void CraneWidthChange()
        {
            Vector3 widthVector = new Vector3(craneWidth, 0, 0);

            supportBeam_02.localPosition = -widthVector;
            UpdateSupportBeamBounds();

            endBeam_02.localPosition = -widthVector;
            UpdateEndBeamBounds();
            mainBeam_end.localPosition = -widthVector;
            UpdateMainBeamBounds();
            mainBeam_mesh.SetPropertyBlock(SetMtlPropertyBlock(mainBeam_mesh, mtlPropertyBlock, craneWidth * 0.2f));

            columns_B.localPosition = widthVector;

            mainBeam_decal.localPosition = new Vector3(-craneWidth/2, 0, 0);
        }

        public void CraneLengthChange()
        {
            Vector3 lengthVector = new Vector3(0, 0, craneLength);

            supportBeam_01_end.localPosition = lengthVector;
            supportBeam_02_end.localPosition = lengthVector;

            UpdateSupportBeamBounds();
            supportBeams_mesh.SetPropertyBlock(SetMtlPropertyBlock(supportBeams_mesh, mtlPropertyBlock, craneLength * 0.2f));

            column_02.localPosition = lengthVector;
            column_04.localPosition = lengthVector;

            UpdateExtraColumnsPos(extraColumns_A);
            UpdateExtraColumnsPos(extraColumns_B);
        }

        private void UpdateSupportBeamBounds()
        {
            Vector3 boundsCenter = supportBeams_mesh.localBounds.center;
            Vector3 boundsSize = supportBeams_mesh.localBounds.size;

            supportBeams_mesh.localBounds = new Bounds(new Vector3(craneWidth/2, boundsCenter.y, craneLength/2), new Vector3(craneWidth + 0.4f, boundsSize.y, craneLength + 0.4f));
        }

        private void UpdateEndBeamBounds()
        {
            Vector3 boundsCenter = endBeams_mesh.localBounds.center;
            Vector3 boundsSize = endBeams_mesh.localBounds.size;

            endBeams_mesh.localBounds = new Bounds(new Vector3(-craneWidth / 2, boundsCenter.y, boundsCenter.z), new Vector3(craneWidth + 0.4f, boundsSize.y, boundsSize.z));
        }

        private void UpdateMainBeamBounds()
        {
            Vector3 boundsCenter = mainBeam_mesh.localBounds.center;
            Vector3 boundsSize = mainBeam_mesh.localBounds.size;

            mainBeam_mesh.localBounds = new Bounds(new Vector3(-craneWidth / 2, boundsCenter.y, boundsCenter.z), new Vector3(craneWidth + 0.4f, boundsSize.y, boundsSize.z));
        }
    }
}
