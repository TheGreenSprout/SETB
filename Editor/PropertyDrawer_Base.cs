using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using static SETB.HandyEditorFunctions;

namespace SETB
{
    public abstract class PropertyDrawer_Base<T> : PropertyDrawer where T : PropertyDrawer_Base<T>
    {
        #region Variables
        protected class LayoutContext
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
        protected bool isLayout => !isDrawing;


        protected SerializedProperty targetProperty;
        protected SerializedObject targetObject => targetProperty?.serializedObject;
        protected UnityEngine.Object target => targetObject?.targetObject;



        protected static Dictionary<string, Dictionary<string, object>> stateCache = new();

        protected E GetState<E>(SerializedProperty property, string key, E defaultValue = default)
        {
            if (stateCache.Count > 1000) stateCache.Clear();


            string fullKey = property.serializedObject.targetObject.GetInstanceID() + "_" + property.propertyPath;

            if (!stateCache.TryGetValue(fullKey, out var dict))
            {
                dict = new Dictionary<string, object>();

                stateCache[fullKey] = dict;
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
            if (stateCache.Count > 1000) stateCache.Clear();


            string fullKey = property.serializedObject.targetObject.GetInstanceID() + "_" + property.propertyPath;

            if (!stateCache.TryGetValue(fullKey, out var dict))
            {
                dict = new Dictionary<string, object>();

                stateCache[fullKey] = dict;
            }

            dict[key] = value;
        }


        private Dictionary<string, SerializedProperty> propCache = new();



        protected float singleLineHeight => EditorGUIUtility.singleLineHeight;

        protected float standardVerticalSpacing => EditorGUIUtility.standardVerticalSpacing;
        #endregion




        #region Unity Methods
        protected virtual void OnEnable() => propCache.Clear();


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);


            targetProperty = property;

            ctx = new LayoutContext(position);
            isDrawing = true;

            Build(property);


            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            targetProperty = property;


            ctx = new LayoutContext(new Rect(0, 0, EditorGUIUtility.currentViewWidth, 0));
            
            isDrawing = false;
            Build(property);


            return ctx.HeightUsed;
        }
        protected float GetPropertyHeight(SerializedProperty prop, bool includeChildren = true)
            => EditorGUI.GetPropertyHeight(prop, includeChildren);
        #endregion



        #region Override Points
        protected abstract void Build(SerializedProperty property);
        #endregion



        #region GUI Helpers
            #region Layout Helpers
            protected Rect ReserveSpace(float height, LayoutContext context = null) => (context ?? ctx).GetRect(height);

            protected Rect PeekRect(float height, LayoutContext context = null)
            {
                var c = context ?? ctx;

                return new Rect(c.position.x, c.y, c.position.width, height);
            }


            protected void Space(float height = 6f, LayoutContext context = null) => (context ?? ctx).Space(height);

            public void SetIndent(int indent) => EditorGUI_Base.SetIndent(indent);
            public void IterateIndent(int iteration) => EditorGUI_Base.IterateIndent(iteration);
            public int GetIndent() => EditorGUI_Base.GetIndent();
            protected void Indent(Action action) => EditorGUI_Base.Indent(action);
            #endregion



            #region Text Display
            protected void DrawLabel(string text, LayoutContext context = null)
            {
                Rect r = ReserveSpace(singleLineHeight, context);

                if (isDrawing) EditorGUI.LabelField(r, text);
            }


            protected void DrawBox(string text, LayoutContext context = null)
            {
                float h = singleLineHeight * 1.5f;
                Rect r = ReserveSpace(h, context);

                if (isDrawing) EditorGUI.HelpBox(r, text, MessageType.None);
            }

            protected void DrawHelpBox(string text, MessageType type, LayoutContext context = null)
            {
                float h = singleLineHeight * 2f;
                Rect r = ReserveSpace(h, context);

                if (isDrawing) EditorGUI.HelpBox(r, text, type);
            }
            #endregion



            #region With Logic
            protected void DrawButton(string label, Action logic, LayoutContext context = null)
            {
                Rect r = ReserveSpace(singleLineHeight, context);

                if (isDrawing && GUI.Button(r, label)) logic?.Invoke();
            }


            protected void DrawFoldout(SerializedProperty property, Action logic, string label = null, GUIStyle style = null, LayoutContext context = null)
            {
                Rect r = ReserveSpace(singleLineHeight, context);


                if (isLayout) return;

                property.isExpanded = EditorGUI.Foldout(
                    r,
                    property.isExpanded,
                    label ?? property.displayName,
                    true,
                    style ?? EditorStyles.foldout
                );

                if (property.isExpanded) logic?.Invoke();
            }


            protected int DrawPopup(string label, int selectedIndex, string[] options, LayoutContext context = null)
            {
                Rect r = ReserveSpace(singleLineHeight, context);


                if (isLayout) return selectedIndex;

                int newIndex = EditorGUI.Popup(
                    r,
                    label,
                    selectedIndex,
                    options
                );

                return newIndex;
            }
            protected int DrawPopup(string label, int selectedIndex, string[] options, out bool changed, LayoutContext context = null)
            {
                Rect r = ReserveSpace(singleLineHeight, context);


                if (isLayout)
                {
                    changed = false;
                    return selectedIndex;
                }

                int newIndex = selectedIndex;

                changed = ChangeCheck(() =>
                {
                    newIndex = EditorGUI.Popup(
                        r,
                        label,
                        selectedIndex,
                        options
                    );
                });

                return newIndex;
            }

            protected void DrawManagedReferenceDropdown(SerializedProperty property, string label, Type[] types, string[] names, bool handleDraw = true, bool indent = true)
            {
                int currentIndex = 0;

                if (property.managedReferenceValue != null)
                {
                    var currentType = property.managedReferenceValue.GetType();

                    currentIndex = Array.FindIndex(types, t => t == currentType);

                    if (currentIndex < 0) currentIndex = 0;
                }


                int selectedIndex = DrawPopup(label, currentIndex, names, out bool changed);


                RecordAndApply(property, () =>
                {
                    if (changed) property.managedReferenceValue =
                                types[selectedIndex] == null
                                ? null
                                : Activator.CreateInstance(types[selectedIndex]);
                },
                "Change Managed Reference Dropdown");


                if (!handleDraw) return;

                Space();
                if (property.managedReferenceValue != null)
                {
                    if (indent) Indent(() => AllChildren(property));
                    else AllChildren(property);
                }
            }
            #endregion



            #region Field Helpers
            protected int DrawInt(string label, int value, out bool changed, LayoutContext context = null)
            {
                Rect r = ReserveSpace(singleLineHeight, context);


                if (isLayout)
                {
                    changed = false;
                    return value;
                }

                int newValue = value;
                changed = ChangeCheck(() =>
                {
                    newValue = EditorGUI.IntField(
                        r,
                        label,
                        value
                    );
                });

                return newValue;
            }

            protected float DrawFloat(string label, float value, out bool changed, LayoutContext context = null)
            {
                Rect r = ReserveSpace(singleLineHeight, context);


                if (isLayout)
                {
                    changed = false;
                    return value;
                }

                float newValue = value;
                changed = ChangeCheck(() =>
                {
                    newValue = EditorGUI.FloatField(
                        r,
                        label,
                        value
                    );
                });

                return newValue;
            }


            protected E DrawEnum<E>(string label, E value, out bool changed, LayoutContext context = null) where E : Enum
            {
                Rect r = ReserveSpace(singleLineHeight, context);


                if (isLayout)
                {
                    changed = false;
                    return value;
                }

                E newValue = value;
                changed = ChangeCheck(() =>
                {
                    newValue = (E)EditorGUI.EnumPopup(
                        r,
                        label,
                        value
                    );
                });

                return newValue;
            }

            protected E DrawObject<E>(string label, E value, out bool changed, bool allowSceneObjects = true, LayoutContext context = null) where E : UnityEngine.Object
            {
                Rect r = ReserveSpace(singleLineHeight, context);


                if (isLayout)
                {
                    changed = false;
                    return value;
                }

                E newValue = value;
                changed = ChangeCheck(() =>
                {
                    newValue = (E)EditorGUI.ObjectField(
                        r,
                        label,
                        value,
                        typeof(E),
                        allowSceneObjects
                    );
                });

                return newValue;
            }


            protected void DrawProperty(SerializedProperty prop, bool includeChildren = false, string label = null, LayoutContext context = null)
            {
                Rect r = ReserveSpace(GetPropertyHeight(prop, includeChildren), context);


                if (isLayout) return;

                EditorGUI.PropertyField(
                    r,
                    prop,
                    label == null ? GUIContent.none : new GUIContent(label),
                    includeChildren
                );
            }
            #endregion
        #endregion



        #region Children Helpers
        protected bool HasChildren(SerializedProperty property = null)
        {
            var iterator = (property ?? targetProperty).Copy();
            var end = iterator.GetEndProperty();

            return iterator.NextVisible(true) && !SerializedProperty.EqualContents(iterator, end);
        }


        protected void DrawAllChildren(SerializedProperty property = null) => AllChildren(property);
        protected void AllChildren(SerializedProperty property = null, Action<SerializedProperty> logic = null)
        {
            var iterator = property?.Copy() ?? targetProperty.Copy();
            var end = iterator.GetEndProperty();

            if (iterator.NextVisible(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(iterator, end)) break;

                    if (logic == null) DrawProperty(iterator, true, iterator.displayName);
                    else logic?.Invoke(iterator);
                }
                while (iterator.NextVisible(false));
            }
        }


        protected float GetAllChildrenHeight(SerializedProperty property = null)
        {
            float total = 0f;

            var iterator = property?.Copy() ?? targetProperty.Copy();
            var end = iterator.GetEndProperty();

            if (iterator.NextVisible(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(iterator, end)) break;

                    total += GetPropertyHeight(iterator, true) + standardVerticalSpacing;
                }
                while (iterator.NextVisible(false));
            }

            return total;
        }
        #endregion


        
        #region Proxy
        protected SerializedProperty PropRelative(string relativePath) => FindPropertyRelative(targetProperty, relativePath, propCache);
        protected SerializedProperty PropRelative(SerializedProperty prop, string relativePath)
            => prop == null ? PropRelative(relativePath) : FindPropertyRelative(prop, relativePath, propCache);

        protected SerializedProperty Prop(string name) => FindProperty(targetObject, name, propCache);
        protected SerializedProperty Prop(SerializedObject obj, string name) => obj == null ? Prop(name) : FindProperty(obj, name, propCache);


        protected void RecordTarget(string name = "Inspector Change") => Record(target, name);
        #endregion



        #region Misc
        protected E GetValue<E>(string relativePath, SerializedProperty property = null)
        {
            var prop = PropRelative(property, relativePath);

            return prop switch
            {
                { propertyType: SerializedPropertyType.Integer } => (E)(object)prop.intValue,
                { propertyType: SerializedPropertyType.Float } => (E)(object)prop.floatValue,
                { propertyType: SerializedPropertyType.Boolean } => (E)(object)prop.boolValue,
                { propertyType: SerializedPropertyType.String } => (E)(object)prop.stringValue,
                _ => default
            };
        }

        protected void SetValue<E>(string relativePath, E value, SerializedProperty property = null)
        {
            var prop = PropRelative(property, relativePath);

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


        protected bool ChangeCheck(Action logic, SerializedProperty property = null, bool applyModifiedProperties = true)
        {
            if (isLayout)
            {
                logic?.Invoke();
                
                return false;
            }


            BeginChangeCheck();

            logic?.Invoke();

            return EndChangeCheck(property, applyModifiedProperties);
        }

        protected void BeginChangeCheck()
        {
            if (isDrawing) EditorGUI.BeginChangeCheck();
        }
        protected bool EndChangeCheck(SerializedProperty property = null, bool applyModifiedProperties = true)
        {
            if (isLayout) return false;


            if (EditorGUI.EndChangeCheck())
            {
                if (applyModifiedProperties) (property ?? targetProperty).serializedObject.ApplyModifiedProperties();

                return true;
            }
            return false;
        }


        protected void ApplyModifiedProperties(SerializedProperty property = null) => (property ?? targetProperty).serializedObject.ApplyModifiedProperties();

        protected void RecordAndApply(SerializedProperty property, Action logic, string undoMessage = "Change Property")
        {
            if (isLayout)
            {
                logic?.Invoke();
                
                return;
            }


            Record(property, undoMessage);

            logic?.Invoke();

            ApplyModifiedProperties(property);
        }
        protected void RecordAndApplyTarget(string name = "Inspector Change") => RecordAndApply(targetProperty, null, name);
        #endregion
    }
}
