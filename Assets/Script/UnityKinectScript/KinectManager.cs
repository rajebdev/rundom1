using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;


public class KinectManager : MonoBehaviour
{
	public enum Smoothing : int { None, Default, Medium, Aggressive }
	
	// Public Bool to determine whether to receive and compute the user map
	public bool ComputeUserMap = true;
	
	// Public Bool to determine whether to receive and compute the color map
	public bool ComputeColorMap = false;

		// if percent is zero, it is calculated internally to match the selected width and height of the depth image
	public float DisplayMapsWidthPercent = 20f;
	
	// How high off the ground is the sensor (in meters).
	public float SensorHeight = 1.0f;
	
	// Kinect elevation angle (in degrees)
	public int SensorAngle = 0;
	
	// Minimum user distance in order to process skeleton data
	public float MinUserDistance = 1.0f;
	
	// Maximum user distance, if any. 0 means no max-distance limitation
	public float MaxUserDistance = 0f;
	
	// Public Bool to determine whether to detect only the closest user or not
	public bool DetectClosestUser = true;
	
	// Public Bool to determine whether to use only the tracked joints (and 	ignore the inferred ones)
	public bool IgnoreInferredJoints = true;
	
	// Selection of smoothing parameters
	public Smoothing smoothing = Smoothing.Default;
	
	// Bool to keep track of whether Kinect has been initialized
	private bool KinectInitialized = false; 
	
	// User Map vars.
	private Texture2D usersLblTex;
	private Color32[] usersMapColors;
	private ushort[] usersPrevState;
	private Rect usersMapRect;
	private int usersMapSize;
	
	private Texture2D usersClrTex;
	//Color[] usersClrColors;
	private Rect usersClrRect;
	
	//short[] usersLabelMap;
	private ushort[] usersDepthMap;
	private float[] usersHistogramMap;
	
	// List of all users
	private List<uint> allUsers;
	
	// Image stream handles for the kinect
	private IntPtr colorStreamHandle;
	private IntPtr depthStreamHandle;
	
	// Color image data, if used 
	private Color32[] colorImage;
	private byte[] usersColorMap;
	
	// Skeleton related structures
	private KinectWrapper.NuiSkeletonFrame skeletonFrame;
	private KinectWrapper.NuiTransformSmoothParameters smoothParameters;
	
	// Skeleton tracking states, positions and joints' orientations
	//private Vector3 playerPos;
	//private Matrix4x4 playerOri;
	private bool[] playerJointsTracked;
	private bool[] playerPrevTracked;
	private Vector3[] playerJointsPos;
	private KinectWrapper.NuiSkeletonBoneOrientation[] jointOrientations;
	
	
	private Matrix4x4 kinectToWorld, flipMatrix;
	private static KinectManager instance;
	
	// returns the single KinectManager instance
	public static KinectManager Instance
	{
		get
		{
			return instance;
		}
	}
	
	// checks if Kinect is initialized and ready to use. If not, there was an error during Kinect-sensor initialization
		public static bool IsKinectInitialized()
	{
		return instance != null ? instance.KinectInitialized : false;
	}
	
	// checks if Kinect is initialized and ready to use. If not, there was an error during Kinect-sensor initialization
		public bool IsInitialized()
	{
		return KinectInitialized;
	}
	
	// returns the depth image/users histogram texture,if ComputeUserMap is true
	public Texture2D GetUsersLblTex()
	{ 
		return usersLblTex;
	}
	
	// returns the color image texture,if ComputeColorMap is true
	public Texture2D GetUsersClrTex()
	{ 
		return usersClrTex;
	}
	
	// returns true if at least one user is currently detected by the sensor
	public bool IsUserDetected()
	{
		return KinectInitialized && (allUsers.Count > 0);
	}
	
	// returns true if the given joint of the specified user is being tracked
	public bool IsJointTracked(int joint)
	{
		return joint >= 0 && joint < playerJointsTracked.Length ? playerJointsTracked[joint] : false;
	}
	
	// returns the joint position of the specified user, relative to the  Kinect-sensor, in meters
	public Vector3 GetJointPosition(int joint)
	{
		return joint >= 0 && joint < playerJointsPos.Length ? playerJointsPos[joint] : Vector3.zero;
	}
	
	
	// removes the currently detected kinect users, allowing a new detection/calibration process to start
	public void ClearKinectUsers()
	{
		if(!KinectInitialized)
			return;
		
		// remove current users
		for(int i = allUsers.Count - 1; i >= 0; i--)
		{
			uint userId = allUsers[i];
			RemoveUser(userId);
		}
	}
	
	
	
	void Awake()
	{
		int hr = 0;
		
		try
		{
			hr = KinectWrapper.NuiInitialize(KinectWrapper.NuiInitializeFlags.UsesSkeleton |
				 KinectWrapper.NuiInitializeFlags.UsesDepthAndPlayerIndex |
				 (ComputeColorMap ? KinectWrapper.NuiInitializeFlags.UsesColor : 0));
			if (hr != 0)
			{
				throw new Exception("NuiInitialize Failed");
			}
			
			hr = KinectWrapper.NuiSkeletonTrackingEnable(IntPtr.Zero, 
			                                             8);  // 0, 12,8
			if (hr != 0)
			{
				throw new Exception("Cannot initialize Skeleton Data");
			}
			
			depthStreamHandle = IntPtr.Zero;
			if(ComputeUserMap)
			{
				hr = KinectWrapper.NuiImageStreamOpen
					(KinectWrapper.NuiImageType.DepthAndPlayerIndex, 
					 
					 KinectWrapper.Constants.DepthImageResolution, 0, 2, IntPtr.Zero, ref 
					 depthStreamHandle);
				if (hr != 0)
				{
					throw new Exception("Cannot open depth stream");
				}
			}
			
			colorStreamHandle = IntPtr.Zero;
			if(ComputeColorMap)
			{
				hr = KinectWrapper.NuiImageStreamOpen
					(KinectWrapper.NuiImageType.Color, 
					 
					 KinectWrapper.Constants.ColorImageResolution, 0, 2, IntPtr.Zero, ref 
					 colorStreamHandle);
				if (hr != 0)
				{
					throw new Exception("Cannot open color stream");
				}
			}
			
			// set kinect elevation angle
			KinectWrapper.NuiCameraElevationSetAngle(SensorAngle);
			
			// init skeleton structures
			skeletonFrame = new KinectWrapper.NuiSkeletonFrame() 
			{ 
				SkeletonData = new KinectWrapper.NuiSkeletonData
				[KinectWrapper.Constants.NuiSkeletonCount] 
			};
			
			// values used to pass to smoothing function
			smoothParameters = new 
				KinectWrapper.NuiTransformSmoothParameters();
			
			switch(smoothing)
			{
			case Smoothing.Default:
				smoothParameters.fSmoothing = 0.5f;
				smoothParameters.fCorrection = 0.5f;
				smoothParameters.fPrediction = 0.5f;
				smoothParameters.fJitterRadius = 0.05f;
				smoothParameters.fMaxDeviationRadius = 0.04f;
				break;
			case Smoothing.Medium:
				smoothParameters.fSmoothing = 0.5f;
				smoothParameters.fCorrection = 0.1f;
				smoothParameters.fPrediction = 0.5f;
				smoothParameters.fJitterRadius = 0.1f;
				smoothParameters.fMaxDeviationRadius = 0.1f;
				break;
			case Smoothing.Aggressive:
				smoothParameters.fSmoothing = 0.7f;
				smoothParameters.fCorrection = 0.3f;
				smoothParameters.fPrediction = 1.0f;
				smoothParameters.fJitterRadius = 1.0f;
				smoothParameters.fMaxDeviationRadius = 1.0f;
				break;
			}
			
			// create arrays for joint positions and joint orientations
			int skeletonJointsCount = (int)
				KinectWrapper.NuiSkeletonPositionIndex.Count;
			
			playerJointsTracked = new bool[skeletonJointsCount];
			playerPrevTracked = new bool[skeletonJointsCount];
			
			playerJointsPos = new Vector3[skeletonJointsCount];
			
			
			//create the transform matrix that converts from kinect-space to world-space
			Quaternion quatTiltAngle = new Quaternion();
			quatTiltAngle.eulerAngles = new Vector3(-SensorAngle, 0.0f, 0.0f);
			
			//float heightAboveHips = SensorHeight - 1.0f;
			
			// transform matrix - kinect to world
			//kinectToWorld.SetTRS(new Vector3(0.0f, heightAboveHips, 0.0f), quatTiltAngle, Vector3.one);
			kinectToWorld.SetTRS(new Vector3(0.0f, SensorHeight, 0.0f), quatTiltAngle, Vector3.one);
			flipMatrix = Matrix4x4.identity;
			flipMatrix[2, 2] = -1;
			
			instance = this;
		}
		catch(DllNotFoundException e)
		{
			string message = "Please check the Kinect SDK installation.";
			Debug.LogError(message);
			Debug.LogError(e.ToString());
			return;
		}
		catch (Exception e)
		{
			string message = e.Message + " - " + 
				KinectWrapper.GetNuiErrorString(hr);
			Debug.LogError(message);
			Debug.LogError(e.ToString());
			return;
		}
		
		if(ComputeUserMap)
		{
			// Initialize depth & label map related stuff
			usersMapSize = KinectWrapper.GetDepthWidth() * 
				KinectWrapper.GetDepthHeight();
			usersLblTex = new Texture2D(KinectWrapper.GetDepthWidth(), 
			                            KinectWrapper.GetDepthHeight());
			usersMapColors = new Color32[usersMapSize];
			usersPrevState = new ushort[usersMapSize];
			
			usersDepthMap = new ushort[usersMapSize];
			usersHistogramMap = new float[8192];
		}
		
		if(ComputeColorMap)
		{
			// Initialize color map related stuff
			usersClrTex = new Texture2D(KinectWrapper.GetColorWidth(), 
			                            KinectWrapper.GetColorHeight());
			
			colorImage = new Color32[KinectWrapper.GetColorWidth() * 
			                         KinectWrapper.GetColorHeight()];
			usersColorMap = new byte[colorImage.Length << 2];
		}
		
		
		// Initialize user list to contain ALL users.
		allUsers = new List<uint>();
		
		KinectInitialized = true;
	}
	
	void Update()
	{
		if(KinectInitialized)
		{
			if(ComputeUserMap)
			{
				if(depthStreamHandle != IntPtr.Zero &&
				   KinectWrapper.PollDepth(depthStreamHandle, 
				                        KinectWrapper.Constants.IsNearMode, ref usersDepthMap))
				{
					UpdateUserMap();
				}
			}
			
			if(ComputeColorMap)
			{
				if(colorStreamHandle != IntPtr.Zero && KinectWrapper.PollColor(colorStreamHandle, ref usersColorMap, ref colorImage))
				{
					UpdateColorMap();
				}
			}
			
			if(KinectWrapper.PollSkeleton(ref smoothParameters, ref skeletonFrame))
			{
				ProcessSkeleton();
				
			}
		}
	}
	
	public void OnApplicationQuit()
	{
		if(KinectInitialized)
		{
			KinectWrapper.NuiShutdown();
			instance = null;
		}
	}
	
	void UpdateUserMap()
	{
		int numOfPoints = 0;
		Array.Clear(usersHistogramMap, 0, usersHistogramMap.Length);
		
		// Calculate cumulative histogram for depth
		for (int i = 0; i < usersMapSize; i++)
		{
			// Only calculate for depth that contains users
			if ((usersDepthMap[i] & 7) != 0)
			{
				ushort userDepth = (ushort)(usersDepthMap[i] >> 3);
				usersHistogramMap[userDepth]++;
				numOfPoints++;
			}
		}
		
		if (numOfPoints > 0)
		{
			for (int i = 1; i < usersHistogramMap.Length; i++)
			{   
				usersHistogramMap[i] += usersHistogramMap[i - 1];
			}
			
			for (int i = 0; i < usersHistogramMap.Length; i++)
			{
				usersHistogramMap[i] = 1.0f - (usersHistogramMap[i] 
				                               / numOfPoints);
			}
		}
		
		// dummy structure needed by the coordinate mapper
		KinectWrapper.NuiImageViewArea pcViewArea = new 
			KinectWrapper.NuiImageViewArea 
		{
			eDigitalZoom = 0,
			lCenterX = 0,
			lCenterY = 0
		};
		
		// Create the actual users texture based on label map and depth histogram
		Color32 clrClear = Color.clear;
		for (int i = 0; i < usersMapSize; i++)
		{
			// Flip the texture as we convert label map to color array
			int flipIndex = i; // usersMapSize - i - 1;
			
			ushort userMap = (ushort)(usersDepthMap[i] & 7);
			ushort userDepth = (ushort)(usersDepthMap[i] >> 3);
			
			ushort nowUserPixel = userMap != 0 ? (ushort)((userMap << 13) | userDepth) : userDepth;
			ushort wasUserPixel = usersPrevState[flipIndex];
			
			// draw only the changed pixels
			if(nowUserPixel != wasUserPixel)
			{
				usersPrevState[flipIndex] = nowUserPixel;
				
				if (userMap == 0)
				{
					usersMapColors[flipIndex] = clrClear;
				}
				else
				{
					if(colorImage != null)
					{
						int x = i % 
							KinectWrapper.Constants.DepthImageWidth;
						int y = i / 
							KinectWrapper.Constants.DepthImageWidth;
						
						int cx, cy;
						int hr = 
							KinectWrapper.NuiImageGetColorPixelCoordinatesFromDepthPixelAtResolution(
								
								KinectWrapper.Constants.ColorImageResolution,
								
								KinectWrapper.Constants.DepthImageResolution,
								ref pcViewArea,
								x, y, usersDepthMap[i],
								out cx, out cy);
						
						if(hr == 0)
						{
							int colorIndex = cx + cy * 
								KinectWrapper.Constants.ColorImageWidth;
							//colorIndex = usersMapSize - colorIndex - 1;
							if(colorIndex >= 0 && 
							   colorIndex < usersMapSize)
							{
								Color32 colorPixel = 
									colorImage[colorIndex];
								usersMapColors[flipIndex] = colorPixel;  // new Color(colorPixel.r / 256f, colorPixel.g / 256f, colorPixel.b / 256f, 0.9f);
								usersMapColors[flipIndex].a = 230; // 0.9f
							}
						}
					}
					else
					{
						// Create a blending color based on the depth histogram
						float histDepth = usersHistogramMap[userDepth];
						Color c = new Color(histDepth, histDepth, histDepth, 0.9f);
						
						switch(userMap % 4)
						{
						case 0:
							usersMapColors[flipIndex] = Color.red * c;
							break;
						case 1:
							usersMapColors[flipIndex] = Color.green * c;
							break;
						case 2:
							usersMapColors[flipIndex] = Color.blue * c;
							break;
						case 3:
							usersMapColors[flipIndex] = Color.magenta * c;
							break;
						}
					}
				}
				
			}
		}
		
		// Draw it!
		usersLblTex.SetPixels32(usersMapColors);
		usersLblTex.Apply();
	}
	
	// Update the Color Map
	void UpdateColorMap()
	{
		usersClrTex.SetPixels32(colorImage);
		usersClrTex.Apply();
	}
	
	
	void RemoveUser(uint UserId)
	{
		allUsers.Remove(UserId);
	}
	
	// Some internal constants
	private const int stateTracked = (int)KinectWrapper.NuiSkeletonPositionTrackingState.Tracked;
	private const int stateNotTracked = (int)KinectWrapper.NuiSkeletonPositionTrackingState.NotTracked;
	
	private int [] mustBeTrackedJoints = { 
		(int)KinectWrapper.NuiSkeletonPositionIndex.AnkleLeft,
		(int)KinectWrapper.NuiSkeletonPositionIndex.FootLeft,
		(int)KinectWrapper.NuiSkeletonPositionIndex.AnkleRight,
		(int)KinectWrapper.NuiSkeletonPositionIndex.FootRight,
	};
	
	// Process the skeleton data
	void ProcessSkeleton()
	{
		List<uint> lostUsers = new List<uint>();
		lostUsers.AddRange(allUsers);
		
		for(int i = 0; i < KinectWrapper.Constants.NuiSkeletonCount; i++){
			KinectWrapper.NuiSkeletonData skeletonData = skeletonFrame.SkeletonData[i];
			uint userId = skeletonData.dwTrackingID;
			
			if(skeletonData.eTrackingState == KinectWrapper.NuiSkeletonTrackingState.SkeletonTracked){

				if (!allUsers.Contains(userId)){
					allUsers.Add(userId);
				}
				uint[] user = allUsers.ToArray();
				if(userId == user[0]){
					for (int j = 0; j < (int)KinectWrapper.NuiSkeletonPositionIndex.Count; j++)
					{
						bool playerTracked = IgnoreInferredJoints ? 
							(int)skeletonData.eSkeletonPositionTrackingState[j] == stateTracked :
							(Array.BinarySearch(mustBeTrackedJoints, j) >= 0 ? (int)skeletonData.eSkeletonPositionTrackingState[j] == stateTracked :
							(int)skeletonData.eSkeletonPositionTrackingState[j] != stateNotTracked);

						playerJointsTracked[j] = playerPrevTracked[j] && playerTracked;
						playerPrevTracked[j] = playerTracked;

						if(playerJointsTracked[j])
						{
							playerJointsPos[j] = kinectToWorld.MultiplyPoint3x4(skeletonData.SkeletonPositions[j]);
						}
					}
					
					// draw the skeleton on top of texture
					if(ComputeUserMap)
					{
						usersLblTex.Apply();
					}
				}
				lostUsers.Remove(userId);
			}
		}
		
		// remove the lost users if any
		if(lostUsers.Count > 0)
		{
			foreach(uint userId in lostUsers)
			{
				RemoveUser(userId);
			}
			
			lostUsers.Clear();
		}
	}
	
}


