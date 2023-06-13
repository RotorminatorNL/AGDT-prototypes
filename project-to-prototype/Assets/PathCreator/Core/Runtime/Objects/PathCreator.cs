using System.Collections.Generic;
using UnityEngine;

namespace PathCreation 
{
    public class PathCreator : MonoBehaviour 
    {
        /// This class stores data for the path editor, and provides accessors to get the current vertex and bezier path.
        /// Attach to a GameObject to create a new path editor.

        public event System.Action PathUpdated;

        [SerializeField, HideInInspector] PathCreatorData editorData;
        [SerializeField, HideInInspector] bool initialized;

        GlobalDisplaySettings globalEditorDisplaySettings;

        // Vertex path created from the current bezier path
        public VertexPath Path 
        {
            get
            {
                if (!initialized) InitializeEditorData(false);
                return editorData.GetVertexPath(transform);
            }
        }

        // The bezier path created in the editor
        public BezierPath BezierPath 
        {
            get
            {
                if (!initialized) InitializeEditorData(false);
                return editorData.BezierPath;
            }
            set 
            {
                if (!initialized) InitializeEditorData(false);
                editorData.BezierPath = value;
            }
        }

        #region Internal methods

        /// Used by the path editor to initialise some data
        public void InitializeEditorData(bool in2DMode) 
        {
            editorData ??= new PathCreatorData ();
            editorData.BezierOrVertexPathModified -= TriggerPathUpdate;
            editorData.BezierOrVertexPathModified += TriggerPathUpdate;

            editorData.Initialize (in2DMode);
            initialized = true;
        }

        public PathCreatorData EditorData { get { return editorData; } }

        public void TriggerPathUpdate() 
        {
            PathUpdated?.Invoke();
        }

#if UNITY_EDITOR

        // Draw the path when path objected is not selected (if enabled in settings)
        void OnDrawGizmos () 
        {
            // Only draw path gizmo if the path object is not selected
            // (editor script is resposible for drawing when selected)
            GameObject selectedObj = UnityEditor.Selection.activeGameObject;
            if (selectedObj != gameObject) 
            {
                if (Path != null) 
                {
                    Path.UpdateTransform(transform);

                    if (globalEditorDisplaySettings == null) globalEditorDisplaySettings = GlobalDisplaySettings.Load();

                    if (globalEditorDisplaySettings.visibleWhenNotSelected) 
                    {
                        Gizmos.color = globalEditorDisplaySettings.bezierPath;

                        for (int i = 0; i < Path.NumPoints; i++) 
                        {
                            int nextI = i + 1;
                            if (nextI >= Path.NumPoints) 
                            {
                                if (Path.isClosedLoop) nextI %= Path.NumPoints;
                                else break;
                            }
                            Gizmos.DrawLine(Path.GetPoint(i), Path.GetPoint(nextI));
                        }
                    }
                }
            }
        }
#endif

        #endregion
    }
}