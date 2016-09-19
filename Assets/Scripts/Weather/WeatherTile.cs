using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

// Original Code by:
// Sean Barton
// http://www.seanba.com/tiled2unity

namespace Tiled2Unity
{
	public class WeatherTile : MonoBehaviour
	{
		[System.Serializable]
        public class Frame
        {
            public float Vertex_z = 0;
        }
	
		private int x, width;
		
		private WeatherManager.WeatherType type;
		private WeatherManager.WeatherType currentType;
		
		public List<Frame> frames = new List<Frame>();
		
		private Frame prevFrame;
        private Frame instFrame;

		private void Awake()
		{
            WeatherManager.Instance.weatherTileList.Add(this);
		
			type = WeatherManager.Instance.type;
            WeatherChange(type);

            if(type == WeatherManager.WeatherType.rain)
            {
                prevFrame = frames[0];
            }
            else if (type == WeatherManager.WeatherType.snow)
            {
                prevFrame = frames[1];
            }
            else
            {
                prevFrame = frames[(int)type];
            }
			
			if (frames.Count == 0)
            {
                Debug.LogError(String.Format("TileAnimation on '{0}' has no frames.", this.name));
            }
            else if(frames.Count < 2)
            {
                Debug.LogError(String.Format("TileAnimation on '{0}' has insufficient frames.", this.name));
            }
		}
		
		void LateUpdate()
		{
			currentType = WeatherManager.Instance.type;

            if(type != currentType)
            {
                WeatherChange(currentType);

                type = currentType;
            }
		}
		
		// Russell Brabers - 05/28
        // Modified from coroutine to method for global weather
        public void WeatherChange(WeatherManager.WeatherType _type)
        {
            Frame frame;

            if (_type == WeatherManager.WeatherType.rain && prevFrame == this.frames[0])
            {
                return;
            }
            else if(_type == WeatherManager.WeatherType.rain && prevFrame == this.frames[1])
            {
                frame = this.frames[0];
            }
            else if(_type == WeatherManager.WeatherType.snow)
            {
                frame = this.frames[1];
            }
            else
            {
                frame = this.frames[0];
            }

            if(prevFrame == null)
            {
                prevFrame = frame;
            }
            
            // Make the frame 'visible' by making negative 'z' vertex positions positive
            ModifyVertices(-frame.Vertex_z);

            // Check if the current frame is different from the previous frame and
            // make the previous frame 'invisible' again. Make matching positive 'z' values negative
            if (frame != prevFrame)
            {
                ModifyVertices(prevFrame.Vertex_z);
            }

            prevFrame = frame;
        }

        // Called to change the tile based on weather type given
        public void castInstance(Rect r, int typeNum, bool instance)
        {
            /*
             * typeNum:
             * 0 = sun
             * 1 = rain
             * 2 = snow
             */

            if ((WeatherManager.Instance.type == WeatherManager.WeatherType.sun || WeatherManager.Instance.type == WeatherManager.WeatherType.rain) && (typeNum == 0 || typeNum == 1))
            {
                return;
            }
            else if(WeatherManager.Instance.type == WeatherManager.WeatherType.snow && typeNum == 1)
            {
                instFrame = this.frames[0];
            }
            else if(typeNum == 2)
            {
                instFrame = this.frames[1];
            }
            else
            {
                instFrame = this.frames[typeNum];
            }
            if (instance)
            {
                // Make the frame 'visible' by making negative 'z' vertex positions positive
                ModifyVertices(-instFrame.Vertex_z, r);
                ModifyVertices(prevFrame.Vertex_z, r);
            }
            else
            {
                ModifyVertices(-prevFrame.Vertex_z, r);
                ModifyVertices(instFrame.Vertex_z, r);
            }
        }
		
        // Method for global weather
		// Find 'z' values on vertices that match and negate them
        private void ModifyVertices(float z)
        {
            float negated = -z;

            // Because meshes may be split we have to go over all them in our tree
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            foreach (var mf in meshFilters)
            {
                Vector3[] vertices = mf.mesh.vertices;

                // Kurtis Thiessen - 05/28
                // Only changes tiles within a horizontal region
                for (int i = 0; i < vertices.Length; i ++)
                {
                    if (vertices[i].z == z)
                    {
                        vertices[i].z = negated;
                    }
                }

                // Save the vertices back
                mf.mesh.vertices = vertices;
            }
        }
		
		// Overloaded method for instance weather
		// Find 'z' values on vertices that match and negate them
        private void ModifyVertices(float z, Rect r)
        {
            float negated = -z;

            // Because meshes may be split we have to go over all them in our tree
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            foreach (var mf in meshFilters)
            {
                Vector3[] vertices = mf.mesh.vertices;

                // Kurtis Thiessen - 05/28
                // Only changes tiles within a horizontal region
                for (int i = 0; i < vertices.Length; i += 4)
                {
                    bool allPassed = true;
                    for (int j = 0; j < 4; j++)
                    {
                        if (vertices[i + j].x < r.x || vertices[i + j].x > r.x + r.width || vertices[i + j].z != z)
                        {
                            allPassed = false;
                            break;
                        }
                    }

                    if (allPassed)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            vertices[i + j].z = negated;
                        }
                    }
                }

                // Save the vertices back
                mf.mesh.vertices = vertices;
            }
        }

        // Remove this weathertile OnDestroy when new level is loaded
        void OnDestroy()
        {
            WeatherManager.Instance.weatherTileList.Remove(this);
        }

        // Kurtis Thiessen - 05/28
        // Display region
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Vector3 pos = transform.position;
            
            Gizmos.DrawRay(new Vector3(x , 100, -5) + pos, Vector3.down * 500);
            Gizmos.DrawRay(new Vector3(x + width, 100, -5) + pos, Vector3.down * 500);
        }
	}
}