using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32.SafeHandles;
using getReal3D;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("getReal3D/Updater/Camera Updater")]

public class getRealCameraUpdater
    : MonoBehaviour
{
    private Transform m_transform;
    private Camera m_camera;

    public int cameraIndex = 0;
    public bool applyHeadPosition = true;
    public bool applyHeadRotation = true;
    public bool applyCameraProjection = true;

    private float m_mainNearClip;
    private float m_mainFarClip;

    private bool m_pluginInitialized = false;

    public GameObject CameraPrefab = null;

    public enum ViewportType { Automatic, UserModulated, UserOverride };
    public ViewportType viewportType;
    public Rect userViewport = new Rect(0f, 0f, 1f, 1f);

    public bool updateOnPreCull = true;

    void Awake()
    {
        m_transform = transform;
        m_camera = GetComponent<Camera>();
        m_pluginInitialized = getReal3D.Input.Init();
        Debug.Log("getReal3D.Input.cameras.Count = " + getReal3D.Input.cameras.Count.ToString());
    }

    void Start()
    {
        m_mainNearClip = m_camera.nearClipPlane;
        m_mainFarClip = m_camera.farClipPlane;

        if(!m_pluginInitialized) {
            Debug.LogError("Failed to initialize GetReal3DPlugin");
            return;
        }
        else if(cameraIndex >= getReal3D.Input.cameras.Count) {
            cameraIndex = getReal3D.Input.cameras.Count - 1;
            Debug.LogWarning("Invalid camera index. Setting to " + cameraIndex);
        }
        else if(cameraIndex == 0 && getReal3D.Input.cameras.Count > 1) {
            CreateCameras();
        }

        Rect viewport = new Rect(0f, 0f, 1f, 1f);

        switch(viewportType) {
        case ViewportType.Automatic:
            getReal3D.Plugin.getCameraViewport((uint) cameraIndex, ref viewport);
            break;
        case ViewportType.UserModulated:
            getReal3D.Plugin.getCameraViewport((uint) cameraIndex, ref viewport);
            viewport.Set(userViewport.x * viewport.width + viewport.x, userViewport.y * viewport.height + viewport.y,
                          userViewport.width * viewport.width, userViewport.height * viewport.height);
            break;
        case ViewportType.UserOverride:
            viewport = userViewport;
            break;
        }

        GetComponent<Camera>().rect = viewport;

        GetComponent<Camera>().renderingPath = getReal3D.Config.renderingPath;

        if(getReal3D.Plugin.getCameraUseRTT((uint) cameraIndex)) {
            ensureRenderToTexture();
        }
    }

    void CreateCameras()
    {
        List<int> needCameras = new List<int>();
        for(int i = 1; i < getReal3D.Input.cameras.Count; ++i) needCameras.Add(i);

        // find cameras, see which we can remove from needCameras
        foreach(Camera cam in Camera.allCameras) {
            if(cam.GetComponent<getRealCameraUpdater>() != null && (cam.name == name || cam.name == name + "(Clone)")) {
                int idx = cam.GetComponent<getRealCameraUpdater>().cameraIndex;
                if(idx > 0) needCameras.Remove(idx);
            }
        }

        // make missing cameras
        foreach(int idx in needCameras) {
            GameObject newCamObject = null;
            if(CameraPrefab == null) {
                newCamObject = Instantiate(gameObject) as GameObject;
            }
            else {
                newCamObject = Instantiate(CameraPrefab) as GameObject;
            }

            foreach(AudioListener listener in newCamObject.GetComponents<AudioListener>()) {
                Destroy(listener);
            }

            newCamObject.transform.parent = transform.parent;
            newCamObject.tag = gameObject.tag;
            newCamObject.layer = gameObject.layer;

            getRealCameraUpdater camUpdater = newCamObject.GetComponent<getRealCameraUpdater>();
            if(camUpdater == null) camUpdater = newCamObject.AddComponent<getRealCameraUpdater>();
            camUpdater.cameraIndex = idx;

            newCamObject.GetComponent<Camera>().CopyFrom(GetComponent<Camera>());
        }
    }

    void Update()
    {
        if(!updateOnPreCull)
            UpdateCamera();
    }

    void OnPreCull()
    {
        if(updateOnPreCull)
            UpdateCamera();
    }

    void UpdateCamera()
    {
        if(m_pluginInitialized) {
            if(applyHeadPosition) {
                m_transform.localPosition = getReal3D.Input.GetCameraSensor((uint) cameraIndex).position;
            }
            if(applyHeadRotation) {
                m_transform.localRotation = getReal3D.Input.GetCameraSensor((uint) cameraIndex).rotation;
            }
            if(applyCameraProjection) {
                m_camera.projectionMatrix = getReal3D.Input.GetCameraProjection((uint) cameraIndex, m_mainFarClip, m_mainNearClip);
            }
        }
    }



    void OnPostRender()
    {
        if(m_camera.targetTexture) {
            Plugin.setTextureFromUnity(cameraIndex, m_camera.targetTexture.GetNativeTexturePtr());
            GL.IssuePluginEvent(cameraIndex);
        }
        else {
            StartCoroutine(WaitAndIssuePluginEvent());
        }
    }

    private IEnumerator WaitAndIssuePluginEvent()
    {
        yield return new WaitForEndOfFrame();
        GL.IssuePluginEvent(cameraIndex);
    }

    private void ensureRenderToTexture()
    {
        int w = getReal3D.Plugin.getCameraWidth((uint) cameraIndex);
        int h = getReal3D.Plugin.getCameraHeight((uint) cameraIndex);
        if(!m_camera.targetTexture ||
            m_camera.targetTexture.width != w ||
            m_camera.targetTexture.height != h) {
            getReal3D.Plugin.debug("Creating new RenderTexture of size " + w.ToString() + "x" + h.ToString() + ".");
            GetComponent<Camera>().targetTexture = new RenderTexture(w, h, 24);
        }
    }
}
