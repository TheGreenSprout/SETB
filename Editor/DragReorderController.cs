/*
⚠️‼️ AI ASSISTED CODE

This code was written with the assistance of AI.
*/



#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SETB
{
    #region XML doc
    /// <summary>
    /// Tracks a single-list drag-reorder gesture purely in terms of row indices. Holds no
    /// reference to what is being dragged — only "dragging started at row N" — so callers with
    /// extra addressing on top (e.g. which sub-container a row belongs to) layer that themselves
    /// alongside an instance of this, the same way they'd track any other per-drag context.
    /// Instantiable (not static) since a single window can need more than one independent drag
    /// gesture at once (e.g. two separate list columns).
    /// </summary>
    #endregion
    [System.Serializable]
    public class DragReorderController
    {
        #region Variables
        [SerializeField] private bool active;
        [SerializeField] private int fromIndex;

        public bool Active => active;
        public int FromIndex => fromIndex;
        #endregion




        #region Main
        #region XML doc
        /// <summary>
        /// Call on MouseDown for the row the user grabbed, after your own row-content hit-test
        /// (e.g. excluding a delete button's sub-rect) has already decided this is a drag-start.
        /// </summary>
        /// <param name="index">The row index the drag started from.</param>
        #endregion
        public void Begin(int index)
        {
            active = true;
            fromIndex = index;
        }
        #endregion



        #region Logic
        #region XML doc
        /// <summary>
        /// Call once per OnGUI while Active is true, after drawing your rows, to render the blue
        /// insertion-line indicator at the row boundary nearest the mouse. Skips drawing when the
        /// resolved target row equals the source (no-op drop).
        /// </summary>
        /// <param name="mouseY">The mouse Y, in the same coordinate space as your row Rects.</param>
        /// <param name="rowHeight">Each row's height.</param>
        /// <param name="rowCount">The total number of rows.</param>
        /// <param name="width">How wide to draw the insertion line.</param>
        /// <param name="color">Optional override for the line's color.</param>
        #endregion
        public void DrawInsertionLine(float mouseY, int rowHeight, int rowCount, float width, Color? color = null)
        {
            if (!active) return;
            if (Event.current.type != EventType.Repaint && Event.current.type != EventType.Layout && Event.current.type != EventType.MouseDrag) return;

            int target = Mathf.Clamp((int)(mouseY / rowHeight), 0, rowCount - 1);
            if (target == fromIndex) return;

            float lineY = target * rowHeight + (target > fromIndex ? rowHeight : 0f);
            EditorGUI.DrawRect(new Rect(0, lineY - 1f, width, 2f), color ?? new Color(0.25f, 0.65f, 1f, 1f));
        }


        #region XML doc
        /// <summary>
        /// Call on MouseUp while Active is true. Resolves the drop row from mouseY (clamped to
        /// [0, rowCount-1]) and clears the gesture.
        /// </summary>
        /// <param name="mouseY">The mouse Y, in the same coordinate space as your row Rects.</param>
        /// <param name="rowHeight">Each row's height.</param>
        /// <param name="rowCount">The total number of rows.</param>
        /// <param name="toIndex">The resolved drop index.</param>
        /// <returns>Returns false if rowCount is 0 or the resolved index equals FromIndex (no-op).</returns>
        #endregion
        public bool TryEnd(float mouseY, int rowHeight, int rowCount, out int toIndex)
        {
            active = false;

            if (rowCount <= 0) { toIndex = 0; return false; }

            toIndex = Mathf.Clamp((int)(mouseY / rowHeight), 0, rowCount - 1);
            return toIndex != fromIndex;
        }

        #region XML doc
        /// <summary>
        /// Force-aborts the gesture without resolving it (e.g. from a global stray-mouse-up
        /// safety net, or on losing focus mid-drag).
        /// </summary>
        #endregion
        public void Cancel() => active = false;
        #endregion
    }
}
#endif
