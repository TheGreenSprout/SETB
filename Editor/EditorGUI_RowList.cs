/*
⚠️‼️ AI ASSISTED CODE

This code was written with the assistance of AI.
*/



#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace SETB
{
    public static class EditorGUI_RowList
    {
        #region Logic
        #region XML doc
        /// <summary>
        /// Draws a header bar (title + optional right-aligned "+" button) followed by a
        /// scrollable list of rowCount fixed-height rows. drawRow is called once per row, in
        /// order, with the row's Rect already laid out at (0, index*rowHeight, innerWidth,
        /// rowHeight) in scroll-content space.
        /// </summary>
        /// <param name="area">The full area (header + scroll body) to draw into.</param>
        /// <param name="headerHeight">The header bar's height.</param>
        /// <param name="rowHeight">Each row's height.</param>
        /// <param name="rowCount">The number of rows to draw.</param>
        /// <param name="scrollPos">The scroll position; the caller owns its storage.</param>
        /// <param name="drawRow">Called once per row with its RowContext.</param>
        /// <param name="headerTitle">Optional header label.</param>
        /// <param name="onAddClicked">Optional "+" button handler shown at the header's right edge.</param>
        /// <param name="addButtonWidth">The "+" button's width.</param>
        /// <param name="headerStyle">The header label's GUIStyle.</param>
        /// <param name="headerBgColor">Optional header background color.</param>
        /// <param name="extraHeaderContent">Optional extra header drawing, invoked after the title and before the "+" button.</param>
        /// <param name="extraScrollContent">Optional extra drawing invoked after all rows, still inside BeginScrollView/EndScrollView — use this for anything that needs scroll-content-local coordinates, e.g. a drag-insertion-line indicator. Drawing such content after this method returns would be in the wrong coordinate space.</param>
        /// <returns>Returns the current mouse Y in scroll-content-local space (same space the row Rects use), for callers that need raw hit-testing beyond per-row clicks (e.g. drag math).</returns>
        #endregion
        public static float DrawScrollableRowList(
            Rect area,
            float headerHeight,
            float rowHeight,
            int rowCount,
            ref Vector2 scrollPos,
            Action<RowContext> drawRow,
            string headerTitle = null,
            Action onAddClicked = null,
            float addButtonWidth = 20f,
            GUIStyle headerStyle = null,
            Color? headerBgColor = null,
            Action extraHeaderContent = null,
            Action extraScrollContent = null)
        {
            Rect hdr = new Rect(area.x, area.y, area.width, headerHeight);
            if (headerBgColor.HasValue) EditorGUI.DrawRect(hdr, headerBgColor.Value);

            float addW = onAddClicked != null ? addButtonWidth : 0f;
            if (headerTitle != null) GUI.Label(new Rect(hdr.x + 6f, hdr.y + 3f, hdr.width - 6f - addW - 4f, headerHeight), headerTitle, headerStyle ?? EditorStyles.boldLabel);
            extraHeaderContent?.Invoke();
            if (onAddClicked != null && GUI.Button(new Rect(hdr.xMax - addW - 2f, hdr.y + 2f, addW, headerHeight - 4f), "+", EditorStyles.miniButton)) onAddClicked.Invoke();

            Rect scroll = new Rect(area.x, area.y + headerHeight, area.width, area.height - headerHeight);
            float listH = Mathf.Max(rowCount * rowHeight, scroll.height);
            float innerW = area.width - 14f;

            scrollPos = GUI.BeginScrollView(scroll, scrollPos, new Rect(0, 0, innerW, listH));

            for (int i = 0; i < rowCount; i++)
            {
                var ctx = new RowContext
                {
                    rect = new Rect(0, i * rowHeight, innerW, rowHeight),
                    index = i,
                    alternateStripe = i % 2 != 0
                };
                drawRow?.Invoke(ctx);
            }

            extraScrollContent?.Invoke();

            float scrollSpaceMouseY = Event.current.mousePosition.y;

            GUI.EndScrollView();

            return scrollSpaceMouseY;
        }

        #region XML doc
        /// <summary>
        /// Draws the standard alternating-stripe / selected-highlight row background. Call this
        /// first inside your drawRow callback, before drawing labels/buttons on top.
        /// </summary>
        /// <param name="row">The row's Rect.</param>
        /// <param name="selected">Whether this row is selected.</param>
        /// <param name="alternateStripe">Whether this row gets the odd-row stripe color when not selected.</param>
        /// <param name="selectedColor">Override for the selected background color.</param>
        /// <param name="evenColor">Override for the even-row (non-alternate) background color.</param>
        /// <param name="oddColor">Override for the odd-row (alternate) background color.</param>
        #endregion
        public static void DrawRowBackground(Rect row, bool selected, bool alternateStripe,
            Color? selectedColor = null, Color? evenColor = null, Color? oddColor = null)
        {
            Color bg = selected
                ? (selectedColor ?? new Color(0.22f, 0.48f, 0.85f, 1f))
                : (alternateStripe
                    ? (oddColor  ?? new Color(0.23f, 0.23f, 0.23f, 1f))
                    : (evenColor ?? new Color(0.20f, 0.20f, 0.20f, 1f)));
            EditorGUI.DrawRect(row, bg);
        }
        #endregion
    }




    #region Helper class
    #region XML doc
    /// <summary>
    /// Per-row layout info handed to a DrawScrollableRowList row callback. Selection is
    /// deliberately not included here — it's app-specific (e.g. a two-part group+index
    /// addressing scheme wouldn't fit a single generic index), so the caller computes it itself
    /// and passes it to DrawRowBackground if it wants the standard look.
    /// </summary>
    #endregion
    public struct RowContext
    {
        public Rect rect;
        
        public int  index;
        public bool alternateStripe;
    }
    #endregion
}
#endif
