﻿


/*
OculusStereoGui code by OculusVR
Rearranged by Daniel "Dakor" Korgel

Renders Gui Elements in Stereoscopic 3D if placed below the marked line.
If you want other Elements than the StereoBox, write a new class which extends the OVRGui Class
and place your own stereoscopic elements there

*/

using UnityEngine;
using System.Collections;
public class HUDScript: MonoBehaviour {
   
   private Level1_Global globalObj;
   private OVRGUI        GuiHelper        = new OVRGUI();
   private GameObject      GUIRenderObject  = null;
   private RenderTexture   GUIRenderTexture = null;
   
   // Handle to OVRCameraController
   private OVRCameraController CameraController = null;
   public Font    FontReplace         = null;
   public string healthString ; 
   
      // Awake
   void Awake()
   {
      CameraController = GetComponentInChildren(typeof(OVRCameraController)) as OVRCameraController;
      if(CameraController==null){
         Debug.LogWarning("Single Method Failed!");
      // Find camera controller
      OVRCameraController[] CameraControllers;
      CameraControllers = gameObject.GetComponentsInChildren<OVRCameraController>();
      
      
      if(CameraControllers.Length == 0)
         Debug.LogWarning("OVRMainMenu: No OVRCameraController attached.");
      else if (CameraControllers.Length > 1)
         Debug.LogWarning("OVRMainMenu: More then 1 OVRCameraController attached.");
      else
         CameraController = CameraControllers[0];
      }
   }
   
   // Use this for initialization
   void Start () {
		GameObject gl = GameObject.Find("Global");
		globalObj = gl.GetComponent<Level1_Global>();
		healthString = (globalObj.currentHealth) + "/" +(globalObj.maxHealth);
	
            // Ensure that camera controller variables have been properly
      // initialized before we start reading them
      if(CameraController != null)
      {
         CameraController.InitCameraControllerVariables();
         GuiHelper.SetCameraController(ref CameraController);
      }
      
      // Set the GUI target 
      GUIRenderObject = GameObject.Instantiate(Resources.Load("OVRGUIObjectMain")) as GameObject;
      
      if(GUIRenderObject != null)
      {
         if(GUIRenderTexture == null)
         {
            int w = Screen.width;
            int h = Screen.height;

            if(CameraController.PortraitMode == true)
            {
               int t = h;
               h = w;
               w = t;
            }
            
            // We don't need a depth buffer on this texture
            GUIRenderTexture = new RenderTexture(w, h, 0);   
            GuiHelper.SetPixelResolution(w, h);
            GuiHelper.SetDisplayResolution(OVRDevice.HResolution, OVRDevice.VResolution);
         }
      }
      
      // Attach GUI texture to GUI object and GUI object to Camera
      if(GUIRenderTexture != null && GUIRenderObject != null)
      {
         GUIRenderObject.renderer.material.mainTexture = GUIRenderTexture;
         
         if(CameraController != null)
         {
            // Grab transform of GUI object
            Transform t = GUIRenderObject.transform;
            // Attach the GUI object to the camera
            CameraController.AttachGameObjectToCamera(ref GUIRenderObject);
            // Reset the transform values (we will be maintaining state of the GUI object
            // in local state)
            OVRUtils.SetLocalTransform(ref GUIRenderObject, ref t);
            // Deactivate object until we have completed the fade-in
            // Also, we may want to deactive the render object if there is nothing being rendered
            // into the UI
            // we will move the position of everything over to the left, so get
            // IPD / 2 and position camera towards negative X
            Vector3 lp = GUIRenderObject.transform.localPosition;
            float ipd = 0.0f;
            CameraController.GetIPD(ref ipd);
            lp.x -= ipd * 0.5f;
            GUIRenderObject.transform.localPosition = lp;
            
            GUIRenderObject.SetActive(false);
         }
      }
   }
   
   // Update is called once per frame
   void Update () {
		//Debug.Log(globalObj.currentHealth);
		healthString = (globalObj.currentHealth) + "/" +(globalObj.maxHealth);
   }
   
   
   void OnGUI () {
            // Important to keep from skipping render events
      if (Event.current.type != EventType.Repaint)
         return;

      
      // We can turn on the render object so we can render the on-screen menu
      if(GUIRenderObject != null)
      {
         GUIRenderObject.SetActive(true);
      /*   if (ScenesVisible || ShowVRVars || Crosshair.IsCrosshairVisible() || 
            RiftPresentTimeout > 0.0f || DeviceDetectionTimeout > 0.0f ||
            ((MagCal.Disabled () == false) && (MagCal.Ready () == false))
            )
            GUIRenderObject.SetActive(true);
         else
            GUIRenderObject.SetActive(false);*/
      }
            //***
      // Set the GUI matrix to deal with portrait mode
      Vector3 scale = Vector3.one;
      if(CameraController.PortraitMode == true)
      {
         float h = OVRDevice.HResolution;
         float v = OVRDevice.VResolution;
         scale.x = v / h;                // calculate hor scale
         scale.y = h / v;                // calculate vert scale
      }
   Matrix4x4 svMat = GUI.matrix; // save current matrix
       // substitute matrix - only scale is altered from standard
       GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);
      
      // Cache current active render texture
      RenderTexture previousActive = RenderTexture.active;
      
      // if set, we will render to this texture
      if(GUIRenderTexture != null)
      {
         RenderTexture.active = GUIRenderTexture;
         GL.Clear (false, true, new Color (0.0f, 0.0f, 0.0f, 0.0f));
      }
      
      // Update OVRGUI functions (will be deprecated eventually when 2D renderingc
      // is removed from GUI)
      GuiHelper.SetFontReplace(FontReplace);
      
      
      if(CameraController!=null){
         /* ***************************************************  */
         /* ******* PLACE YOUR GUI CODE BELOW ******* */
                  string test= healthString;//(globalObj.currentHealth) + "//" +(globalObj.maxHealth);
                  GuiHelper.StereoBox(450, 150, 100, 25, ref test, Color.red);
				  GuiHelper.StereoBox(450, 180, 100, 25, ref test, Color.green);
		
         /* ******************************************************* */
         }

         // Restore active render texture
      RenderTexture.active = previousActive;
      
      // ***
      // Restore previous GUI matrix
      GUI.matrix = svMat;

         
      }
      
   }
     
   

