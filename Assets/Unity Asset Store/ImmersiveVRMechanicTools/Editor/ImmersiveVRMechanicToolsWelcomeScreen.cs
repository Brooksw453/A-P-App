using System;
using System.Collections.Generic;
using System.Linq;
using ImmersiveVRTools.PublisherTools.WelcomeScreen;
using ImmersiveVRTools.PublisherTools.WelcomeScreen.GuiElements;
using ImmersiveVRTools.PublisherTools.WelcomeScreen.PreferenceDefinition;
using ImmersiveVRTools.PublisherTools.WelcomeScreen.Utilities;
using ReliableSolutions.Unity.Common.Editor.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class ImmersiveVRMechanicToolsWelcomeScreen : ProductWelcomeScreenBase
{
    public static string BaseUrl = "https://immersivevrtools.com";
    public static string GenerateGetUpdatesUrl(string userId, string versionId)
    {
        return $"{BaseUrl}/updates/immersive-vr-mechanic-tools/{userId}?CurrentVersion={versionId}";
    }
    public static string VersionId = "1.2";
    private static readonly string ProjectIconName = "ProductIcon64";
    public static readonly string ProjectName = "immersive-vr-mechanic-tools";

    private static Vector2 _WindowSizePx = new Vector2(650, 500);
    private static string _WindowTitle = "Immersive VR Mechanic Tools";


    private static readonly List<GuiSection> LeftSections = new List<GuiSection>() {
        new GuiSection("", new List<ClickableElement>
        {
            new LastUpdateButton("New Update!", (screen) => LastUpdateUpdateScrollViewSection.RenderMainScrollViewSection(screen)),
            new ChangeMainViewButton("Welcome", (screen) => MainScrollViewSection.RenderMainScrollViewSection(screen)),
        }),
        new GuiSection("Options", new List<ClickableElement>
        {
            new ChangeMainViewButton("VR Integrations", (screen) =>
            {
                GUILayout.Label(
                    @"XR Toolkit will require some dependencies to run, please have a look in documentation, it should only take a few moments to set up.",
                    screen.TextStyle
                );

                using (LayoutHelper.LabelWidth(200))
                {
                    ProductPreferenceBase.RenderGuiAndPersistInput(ImmersiveVRMechanicToolsPreference.EnableXrToolkitIntegrationPreferenceDefinition);
                }

                const int sectionBreakHeight = 15;
                GUILayout.Space(sectionBreakHeight);

                GUILayout.Label(
                    @"VRTK require some dependencies to run, please have a look in documentation, it should only take a few moments to set up.",
                    screen.TextStyle
                );

                using (LayoutHelper.LabelWidth(200))
                {
                    ProductPreferenceBase.RenderGuiAndPersistInput(ImmersiveVRMechanicToolsPreference.EnableVrtkIntegrationPreferenceDefinition);
                }
                GUILayout.Space(sectionBreakHeight);

                GUILayout.Label(
                    @"VRIF is another paid Unity asset, you need to first import their framework.",
                    screen.TextStyle
                );

                using (LayoutHelper.LabelWidth(200))
                {
                    ProductPreferenceBase.RenderGuiAndPersistInput(ImmersiveVRMechanicToolsPreference.EnableVrifIntegrationPreferenceDefinition);
                }
                GUILayout.Space(sectionBreakHeight);

            }),
            new ChangeMainViewButton("Shaders", (screen) =>
            {
                GUILayout.Label(
                    @"By default package uses HDRP shaders, you can change those to standard surface shaders from dropdown below",
                    screen.TextStyle
                );

                using (LayoutHelper.LabelWidth(200))
                {
                    ProductPreferenceBase.RenderGuiAndPersistInput(ImmersiveVRMechanicToolsPreference.ShaderModePreferenceDefinition);
                }
            })
        }),
        new GuiSection("Launch Demo", new List<ClickableElement>
        {
            new LaunchSceneButton("XR Toolkit", (s) => GetScenePath("XRToolkitDemoScene")),
            new ChangeMainViewButton("VRTK Toolkit", (screen) =>
            {
                GUILayout.Label(
                    @"Using VRTK is slightly more difficult than other frameworks, best to see up to date documentation online.",
                    screen.TextStyle
                );

                if (GUILayout.Button("Open VRTK integration documentation", GUILayout.ExpandWidth(false)))
                {
                    Application.OpenURL($"{RedirectBaseUrl}/vrtk-integration");
                }
            }),
            new LaunchSceneButton("VRIF Toolkit", (s) => GetScenePath("VRIFDemoScene")),
        })
    };

    private static readonly string RedirectBaseUrl = "https://immersivevrtools.com/redirect/immersive-vr-mechanic-tools"; 
    private static readonly GuiSection TopSection = new GuiSection("Support", new List<ClickableElement>
        {
            new OpenUrlButton("Documentation", $"{RedirectBaseUrl}/documentation"),
            new OpenUrlButton("Unity Forum", $"{RedirectBaseUrl}/unity-forum"),
            new OpenUrlButton("Contact", $"{RedirectBaseUrl}/contact")
        }
    );

    private static readonly GuiSection BottomSection = new GuiSection(
        "Can I ask for a quick favour?",
        $"I'd be great help if you could spend few minutes to leave a review on:",
        new List<ClickableElement>
        {
            new OpenUrlButton("  Unity Asset Store", $"{RedirectBaseUrl}/unity-asset-store"),
        }
    );

    private static readonly ScrollViewGuiSection MainScrollViewSection = new ScrollViewGuiSection(
        "", (screen) =>
        {
            GenerateCommonWelcomeText(ImmersiveVRMechanicToolsPreference.ProductName, screen);

            GUILayout.Label("Quick adjustments:", screen.LabelStyle);
            using (LayoutHelper.LabelWidth(220))
            {
                ProductPreferenceBase.RenderGuiAndPersistInput(ImmersiveVRMechanicToolsPreference.EnableXrToolkitIntegrationPreferenceDefinition);
                ProductPreferenceBase.RenderGuiAndPersistInput(ImmersiveVRMechanicToolsPreference.EnableVrtkIntegrationPreferenceDefinition);
                ProductPreferenceBase.RenderGuiAndPersistInput(ImmersiveVRMechanicToolsPreference.EnableVrifIntegrationPreferenceDefinition);
                ProductPreferenceBase.RenderGuiAndPersistInput(ImmersiveVRMechanicToolsPreference.ShaderModePreferenceDefinition);
            }
        }
    );

    private static readonly ScrollViewGuiSection LastUpdateUpdateScrollViewSection = new ScrollViewGuiSection(
        "New Update", (screen) =>
        {
            GUILayout.Label(screen.LastUpdateText, screen.BoldTextStyle, GUILayout.ExpandHeight(true));
        }
    );

    public override string WindowTitle { get; } = _WindowTitle;
    public override Vector2 WindowSizePx { get; } = _WindowSizePx;


    [MenuItem("Window/Immersive VR Mechanic Tools/Start Screen", false, 1999)]
    public static void Init()
    {
        OpenWindow<ImmersiveVRMechanicToolsWelcomeScreen>(_WindowTitle, _WindowSizePx);
    }

    public void OnEnable()
    {
        OnEnableCommon(ProjectIconName);
    }

    public void OnGUI()
    {
        RenderGUI(LeftSections, TopSection, BottomSection, MainScrollViewSection);
    }
}

public class ImmersiveVRMechanicToolsPreference : ProductPreferenceBase
{
    public static string BuildSymbol_EnableXrToolkit = "INTEGRATIONS_XRTOOLKIT";
    public static string BuildSymbol_EnableVRTK = "INTEGRATIONS_VRTK";
    public static string BuildSymbol_EnableVRIF = "INTEGRATIONS_VRIF";

    public const string ProductName = "Immersive VR Mechanic Tools";
    private static string[] ProductKeywords = new[] { "start", "vr", "tools" };

    public static readonly ToggleProjectEditorPreferenceDefinition EnableXrToolkitIntegrationPreferenceDefinition = new ToggleProjectEditorPreferenceDefinition(
        "Enable Unity XR Toolkit integration", "XRToolkitIntegrationEnabled", true,
        (newValue, oldValue) =>
        {
            BuildDefineSymbolManager.SetBuildDefineSymbolState(BuildSymbol_EnableXrToolkit, (bool)newValue);
        });

    public static readonly ToggleProjectEditorPreferenceDefinition EnableVrtkIntegrationPreferenceDefinition = new ToggleProjectEditorPreferenceDefinition(
        "Enable VRTK integration", "VRTKIntegrationEnabled", false,
        (newValue, oldValue) =>
        {
            BuildDefineSymbolManager.SetBuildDefineSymbolState(BuildSymbol_EnableVRTK, (bool)newValue);
        });

    public static readonly ToggleProjectEditorPreferenceDefinition EnableVrifIntegrationPreferenceDefinition = new ToggleProjectEditorPreferenceDefinition(
        "Enable VRIF integration", "VRIFIntegrationEnabled", false,
        (newValue, oldValue) =>
        {
            BuildDefineSymbolManager.SetBuildDefineSymbolState(BuildSymbol_EnableVRIF, (bool)newValue);
        });

    public static readonly EnumProjectEditorPreferenceDefinition ShaderModePreferenceDefinition = new EnumProjectEditorPreferenceDefinition("Shaders",
        "ShadersMode",
        ShadersMode.HDRP,
        typeof(ShadersMode),
        (newValue, oldValue) =>
        {
            if (oldValue == null) oldValue = default(ShadersMode);

            var newShaderModeValue = (ShadersMode)newValue;
            var oldShaderModeValue = (ShadersMode)oldValue;

            if (newShaderModeValue != oldShaderModeValue) 
            {
                SetCommonMaterialsShader(newShaderModeValue);
            }
        }
    );

    public static void SetCommonMaterialsShader(ShadersMode newShaderModeValue)
    {
        var rootToolFolder = AssetPathResolver.GetAssetFolderPathRelativeToScript(ScriptableObject.CreateInstance(typeof(ImmersiveVRMechanicToolsWelcomeScreen)), 1);
        var assets = AssetDatabase.FindAssets("t:Material", new[] { rootToolFolder });

        try
        {
            Shader shaderToUse = null;
            switch (newShaderModeValue)
            {
                case ShadersMode.HDRP: shaderToUse = Shader.Find("HDRP/Lit"); break;
                case ShadersMode.URP: shaderToUse = Shader.Find("Universal Render Pipeline/Lit"); break;
                case ShadersMode.Surface: shaderToUse = Shader.Find("Standard"); break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (var guid in assets)
            {
                var material = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
                if (material.shader.name != shaderToUse.name)
                {
                    material.shader = shaderToUse;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Shader does not exist: {ex.Message}");
        }
    }

    public static List<ProjectEditorPreferenceDefinitionBase> PreferenceDefinitions = new List<ProjectEditorPreferenceDefinitionBase>()
    {
        CreateDefaultShowOptionPreferenceDefinition(),
        EnableXrToolkitIntegrationPreferenceDefinition,
        EnableVrtkIntegrationPreferenceDefinition,
        EnableVrifIntegrationPreferenceDefinition,
        ShaderModePreferenceDefinition
    };

    private static bool PrefsLoaded = false;



#if UNITY_2019_1_OR_NEWER
    [SettingsProvider]
    public static SettingsProvider ImpostorsSettings()
    {
        return GenerateProvider(ProductName, ProductKeywords, PreferencesGUI);
    }

#else
	[PreferenceItem(ProductName)]
#endif
    public static void PreferencesGUI()
    {
        if (!PrefsLoaded)
        {
            LoadDefaults(PreferenceDefinitions);
            PrefsLoaded = true;
        }

        RenderGuiCommon(PreferenceDefinitions);
    }

    public enum ShadersMode
    {
        HDRP,
        URP,
        Surface
    }
}

[InitializeOnLoad]
public class ImmersiveVRMechanicToolsWelcomeScreenInitializer : WelcomeScreenInitializerBase
{
    static ImmersiveVRMechanicToolsWelcomeScreenInitializer()
    {
        var userId = ProductPreferenceBase.CreateDefaultUserIdDefinition(ImmersiveVRMechanicToolsWelcomeScreen.ProjectName).GetEditorPersistedValueOrDefault().ToString();

        HandleUnityStartup(
            ImmersiveVRMechanicToolsWelcomeScreen.Init,
            ImmersiveVRMechanicToolsWelcomeScreen.GenerateGetUpdatesUrl(userId, ImmersiveVRMechanicToolsWelcomeScreen.VersionId), 
            (isFirstRun) =>
        {
            AutoDetectAndSetShaderMode();
        });
    }

    private static void AutoDetectAndSetShaderMode()
    {
        var usedShaderMode = ImmersiveVRMechanicToolsPreference.ShadersMode.Surface;
        var renderPipelineAsset = GraphicsSettings.renderPipelineAsset;
        if (renderPipelineAsset == null)
        {
            usedShaderMode = ImmersiveVRMechanicToolsPreference.ShadersMode.Surface;
        }
        else if (renderPipelineAsset.GetType().Name.Contains("HDRenderPipelineAsset"))
        {
            usedShaderMode = ImmersiveVRMechanicToolsPreference.ShadersMode.HDRP;
        }
        else if (renderPipelineAsset.GetType().Name.Contains("UniversalRenderPipelineAsset"))
        {
            usedShaderMode = ImmersiveVRMechanicToolsPreference.ShadersMode.URP;
        }

        ImmersiveVRMechanicToolsPreference.ShaderModePreferenceDefinition.SetEditorPersistedValue(usedShaderMode);
        ImmersiveVRMechanicToolsPreference.SetCommonMaterialsShader(usedShaderMode);
    }
}