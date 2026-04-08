using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

using static SETB.HandyEditorFunctions;

namespace SETB
{
    public abstract class PropertyDrawer_Base<T> : PropertyDrawer where T : PropertyDrawer_Base<T>
    {
        #region Variables
        protected struct LayoutContext
        {
            public Rect position;
            public float y;

            public LayoutContext(Rect position)
            {
                this.position = position;
                this.y = position.y;
            }

            public Rect GetRect(float height)
            {
                Rect r = new Rect(position.x, y, position.width, height);
                y += height + EditorGUIUtility.standardVerticalSpacing;
                return r;
            }

            public void Space(float height)
            {
                y += height + EditorGUIUtility.standardVerticalSpacing;
            }

            public float HeightUsed => y - position.y;
        }



        protected LayoutContext ctx;
        protected bool isDrawing;


        protected SerializedProperty targetProperty;
        protected UnityEngine.Object target => targetProperty?.serializedObject.targetObject;



        private static Dictionary<string, Dictionary<string, object>> stateCache = new();

        protected E GetState<E>(SerializedProperty property, string key, E defaultValue = default)
        {
            string path = property.propertyPath;

            if (!stateCache.TryGetValue(path, out var dict))
            {
                dict = new Dictionary<string, object>();
                stateCache[path] = dict;
            }

            if (!dict.TryGetValue(key, out var value))
            {
                dict[key] = defaultValue;
                return defaultValue;
            }

            return (E)value;
        }

        protected void SetState<E>(SerializedProperty property, string key, E value)
        {
            string path = property.propertyPath;

            if (!stateCache.TryGetValue(path, out var dict))
            {
                dict = new Dictionary<string, object>();
                stateCache[path] = dict;
            }

            dict[key] = value;
        }
        #endregion




        #region Unity Methods
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);


            if (targetProperty == null) targetProperty = property;


            position = EditorGUI.PrefixLabel(position, label);


            Indent(() =>
            {
                ctx = new LayoutContext(position);
                isDrawing = true;

                Draw(property);

                isDrawing = false;
            });


            EditorGUI.EndProperty();
        }

        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            LayoutContext context = new LayoutContext(new Rect(0, 0, EditorGUIUtility.currentViewWidth, 0));
            
            Build(property);


            return context.HeightUsed;
        }
        #endregion



        #region Override Points
        protected virtual void Draw(SerializedProperty property) => DrawProperty(property, true);

        protected virtual void Build(SerializedProperty property) => ctx.Space(EditorGUI.GetPropertyHeight(property, true));
        #endregion



        #region GUI Helpers
            #region Layout Helpers
            protected Rect GetLastRect(float height) => new Rect(ctx.position.x, ctx.y, ctx.position.width, height);


            protected void DrawProperty(SerializedProperty prop, bool includeChildren = false, string label = null)
            {
                float h = EditorGUI.GetPropertyHeight(prop, includeChildren);
                Rect r = ctx.GetRect(h);

                if (label == null) EditorGUI.PropertyField(r, prop, includeChildren);
                else EditorGUI.PropertyField(r, prop, new GUIContent(label), includeChildren);
            }


            protected void Space(float height = 6f) => ctx.Space(height);

            public void SetIndent(int indent) => EditorGUI_Base.SetIndent(indent);
            public void IterateIndent(int iteration) => EditorGUI_Base.IterateIndent(iteration);
            public int GetIndent() => EditorGUI_Base.GetIndent();
            protected void Indent(Action action) => EditorGUI_Base.Indent(action);
            #endregion



            #region Text Display
            protected void DrawLabel(string text)
            {
                Rect r = ctx.GetRect(EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(r, text);
            }


            protected void DrawBox(string text)
            {
                float h = EditorGUIUtility.singleLineHeight * 1.5f;
                Rect r = ctx.GetRect(h);
                EditorGUI.HelpBox(r, text, MessageType.None);
            }

            protected void DrawHelpBox(string text, MessageType type)
            {
                float h = EditorGUIUtility.singleLineHeight * 2f;
                Rect r = ctx.GetRect(h);
                EditorGUI.HelpBox(r, text, type);
            }
            #endregion



            #region With Logic
            protected void DrawButton(string label, Action logic)
            {
                Rect r = ctx.GetRect(EditorGUIUtility.singleLineHeight);

                if (GUI.Button(r, label)) logic?.Invoke();
            }


            protected void DrawFoldout(SerializedProperty property, Action logic, string label = null, GUIStyle style = null)
            {
                Rect r = ctx.GetRect(EditorGUIUtility.singleLineHeight);

                property.isExpanded = EditorGUI.Foldout(
                    r,
                    property.isExpanded,
                    label ?? property.displayName,
                    true,
                    style ?? EditorStyles.foldout
                );

                if (property.isExpanded) logic?.Invoke();
            }
            #endregion
        #endregion



        #region Misc
        protected SerializedProperty Find(string relativePath) => targetProperty.FindPropertyRelative(relativePath);


        protected E GetValue<E>(string relativePath)
        {
            var prop = Find(relativePath);

            return prop switch
            {
                { propertyType: SerializedPropertyType.Integer } => (E)(object)prop.intValue,
                { propertyType: SerializedPropertyType.Float } => (E)(object)prop.floatValue,
                { propertyType: SerializedPropertyType.Boolean } => (E)(object)prop.boolValue,
                { propertyType: SerializedPropertyType.String } => (E)(object)prop.stringValue,
                _ => default
            };
        }

        protected void SetValue<E>(string relativePath, T value)
        {
            var prop = Find(relativePath);

            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    prop.intValue = Convert.ToInt32(value);
                    break;
                    
                case SerializedPropertyType.Float:
                    prop.floatValue = Convert.ToSingle(value);
                    break;

                case SerializedPropertyType.Boolean:
                    prop.boolValue = Convert.ToBoolean(value);
                    break;

                case SerializedPropertyType.String:
                    prop.stringValue = value?.ToString();
                    break;
            }
        }


        protected bool BeginChangeCheck()
        {
            EditorGUI.BeginChangeCheck();
            return true;
        }

        protected bool EndChangeCheck(SerializedProperty property)
        {
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
                return true;
            }
            return false;
        }
        #endregion
    }
}
