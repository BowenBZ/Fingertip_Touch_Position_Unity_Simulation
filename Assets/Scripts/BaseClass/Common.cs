﻿using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;

/// <summary>
/// Includes some common functions used in this projection
/// 1. Enum DOF, Finger
/// 2. Customized script finding class
/// 3. Open file / folder dialog in windows
/// </summary>
namespace Common
{
    #region EnumElements
    public enum DOF
    {
        gamma1,
        gamma2,
        gamma3,
        alpha1,
        alpha2,
        beta,
    }

    public enum Finger
    {
        thumb,
        index,
    }

    public enum DataRange
    {
        min,
        max,
        step,
    }
    #endregion

    #region FindScripts
    public class ScriptFind
    {
        public static TouchDetection FindTouchDetection(Finger fingertype)
        {
            TouchDetection[] fingeres = GameObject.FindObjectsOfType<TouchDetection>();
            foreach (var finger in fingeres)
            {
                if (finger.FingerType == fingertype)
                {
                    return finger;
                }
            }
            return null;
        }
    }
    #endregion

    #region CommonDelegate
    public delegate void VoidEventHandler();
    public delegate void BooleanEventHandler(bool flag);
    public delegate void StringEventHandler(string str);
    public delegate void Vector2EventHandler(Vector2 pos);
    #endregion

    #region CommonInterface
    public interface IStreamGeneratorAction
    {
        string GenerateStreamFileHeader();
        bool GenerateStreamFileData(out string data, out string imgName);
    }

    public interface IJointMangerAction
    {
        float GetJointValue(DOF joint);
        void SetJointValue(DOF joint, float value);
        event VoidEventHandler OnJointUpdate;
    }

    public interface IFingerAction
    {
        bool IsTouching { get; }
        bool IsOverlapped { get; }
        Vector2 TouchPosition { get; }
        event BooleanEventHandler OnTouchStatusChange;
        event BooleanEventHandler OnOverlapStatusChange;
        event Vector2EventHandler OnTouchPositionChange;
    }

    public interface IDatasetGeneratorAction
    {
        string FolderName { get; set; }
        string CSVFileName { get; set; }
        void Initialize(Camera cameraToTakeShot,
                        bool sameSizeWithWindow,
                        IStreamGeneratorAction streamDataGenerator,
                        IJointMangerAction jointManager,
                        IPanelAction datasetPanel,
                        string folderName,
                        string csvFileName);
        void StartGeneratingDataset();
        void StopCancelGenerating();
        void SaveSingleImage();
        void SearchGeneratingDataset();
    }

    public interface IGameAction
    {
        event BooleanEventHandler OnDatasetPanelChange;
        event StringEventHandler OnFolderNameChange;
    }

    public interface IPanelAction
    {
        bool PackData(
            out Dictionary<DOF, Dictionary<DataRange, float>> datasetPara);
        void UpdateTotalSampleCnt(long value);
        void UpdateCurrentSampleCnt(long value);
    }
    #endregion

    #region FileFolderDialog
    /// <summary>
    /// File Dialog
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class FileDlg
    {
        public int structSize = 0;
        public IntPtr dlgOwner = IntPtr.Zero;
        public IntPtr instance = IntPtr.Zero;
        public String filter = null;
        public String customFilter = null;
        public int maxCustFilter = 0;
        public int filterIndex = 0;
        public String file = null;
        public int maxFile = 0;
        public String fileTitle = null;
        public int maxFileTitle = 0;
        public String initialDir = null;
        public String title = null;
        public int flags = 0;
        public short fileOffset = 0;
        public short fileExtension = 0;
        public String defExt = null;
        public IntPtr custData = IntPtr.Zero;
        public IntPtr hook = IntPtr.Zero;
        public String templateName = null;
        public IntPtr reservedPtr = IntPtr.Zero;
        public int reservedInt = 0;
        public int flagsEx = 0;
    }

    /// <summary>
    /// Open File Dialog
    /// </summary>
    public class OpenFileDialog
    {
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetOpenFileName([In, Out] FileDlg ofd);
    }

    /// <summary>
    /// Save File Dialog
    /// </summary>
    public class SaveFileDialog
    {
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetSaveFileName([In, Out] FileDlg ofd);
    }

    /// <summary>
    /// Folder Dialog
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class DirDialog
    {
        public IntPtr hwndOwner = IntPtr.Zero;
        public IntPtr pidlRoot = IntPtr.Zero;
        public String pszDisplayName = null;
        public String lpszTitle = null;
        public UInt32 ulFlags = 0;
        public IntPtr lpfn = IntPtr.Zero;
        public IntPtr lParam = IntPtr.Zero;
        public int iImage = 0;
    }

    /// <summary>
    /// Open folder dialog
    /// </summary>
    public class OpenBroswerDialog
    {
        [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SHBrowseForFolder([In, Out] DirDialog ofn);

        [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool SHGetPathFromIDList([In] IntPtr pidl, [In, Out] char[] fileName);

    }

    public class WinFormTools
    {
        [DllImport("User32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern int MessageBox(IntPtr handle, String message, String title, int type);
    }
    #endregion
}