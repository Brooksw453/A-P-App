Hi!

Thank you for purchasing this package!


INSTALLATION:

>>>>>>Graphics<<<<<<<

To achieve the same graphics as in the video demo you have to be sure that you:

1. Set your project to Deferred Rendering Path;

2. Set your project Color Space to Linear;

3. Switch your camera to HDR mode (Checkbox in the camera component);

4. Use Post Processing Stack, which is available through the Package Manager, and use the PostProcessing Profile included (Placed in the \Assets\NOT_Lonely\OverheadCrane\DemoScene folder)


>>>>>>ObiRope integration<<<<<<<

If you use ObiRope plugin and want to use this crane with it, you can simply import the ObiRope_integration.unitypackage file which is placed in the 
\Assets\NOT_Lonely\OverheadCrane folder and use the OverheadCrane_ObiRope prefab.
To avoid errors you have to import the ObiRope plugin into your project first and only after that you can import the ObiRope_integration.unitypackage.


>>>>>>Scriptable Render Pipelines<<<<<<<

Please look at the "Upgrade to HDRP or URP" text file placed in the \Assets\NOT_Lonely\OverheadCrane folder.

--------------------------------------------------------------------------------------------------------------------------------------------------


USAGE:

First of all it's recommended to open a demo scene placed in the \Assets\NOT_Lonely\OverheadCrane folder.
In this scene you will see the crane which settings could be adjusted by selecting the root OverheadCrane object in the scene.
Once you selected this object you can tweak all the sliders and values in its interface.

If you press Play in the Unity Editor you will be able to operate the crane in two ways:

1. When the free camera is active you can use WASDQE keys on your keyboard to move the crane parts. You can move the crane object itself along the scene
by holding the arrow keys. Also you can attach a cargo (a concrete block in the scene) by pressing the Space key.

2. When the First Person camera is active, you can operate the crane like in a first person shooter or similar game - by looking on the crane remote buttons an holding the E key on the keyboard.


This First Person camera example consists of two parts:

1. The FPInteractionExample component which is placed on the Remote game object (you can find it in the OverheadCrane object hierarchy under: 
OverheadCrane > SupportBeams > SupportBeam_01 > Cable_topMount > Cable_remoteMount).

2. SimpleFPController - very basic first person controller which allows to walk through the scene.

FPInteractionExample script use a raycast to check if the player look at the crane remote buttons and call the OnPressed, OnHold and OnRelease public method from the button script that it hits if the E key is pressed.

If you want to use this crane in a VR project, then you can use your controller colliders (for instance, fingertips) to press the remote buttons just like in a real life, because the remove buttons already setup for that.

For the more information about the crane settings you can move your mouse cursor over any setting name and you will see a tooltip about it.

ATTENTION:
Once you dropped the crane prefab into the scene, this prefab instance will be unpacked automatically. This is done to prevent conflicts and issues
between the root prefab and its instances in the scene, because the NL_OverheadCrane script instantiates new objects in the scene which can not be added into the prefab. Also you should not make prefabs of this object in your scene on your own to prevent any conflicts and errors.

If you don't have Amplify Shader Editor installed in your project you will see the warning: "Could not create a custom UI for the shader..."
This happens because the shader code have a line about the custom UI which is needed for the Amplify Shader Editor to work,
so you can simply ignore this mesage.


If you have any other questions, please write an email: support@not-lonely.com

Don't forget to rate this pack at the Asset Store and write a review. It's very important for me!

Thank you!
