#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XBehaviour.Runtime
{
    public class DebuggerWindows
    {
        public class DebuggerWindow : EditorWindow
        {
            private const int nestedPadding = 10;

            public static Transform selectedObject;

            public static Debugger selectedDebugger;


            private Vector2 scrollPosition = Vector2.zero;

            private GUIStyle smallTextStyle, nodeCapsuleGray, nodeCapsuleFailed, nodeCapsuleStopRequested;
            private GUIStyle nestedBoxStyle;

            private Color defaultColor;

            Dictionary<string, string> nameToTagString;

            [MenuItem("Window/XBehavior Debugger")]
            public static void ShowWindow()
            {
                GameObject go = Selection.activeGameObject;
                Debugger debugger = Debugger.Get(go);
                if (debugger == null)
                {
                    Debug.LogError("请选择行为树gameobject对象");
                    return;
                }

                if (debugger == selectedDebugger)
                {
                    Debug.LogError("请勿重复选择");
                    return;
                }

                selectedObject = go.transform;
                selectedDebugger = debugger;
                DebuggerWindow window = (DebuggerWindow)EditorWindow.GetWindow(typeof(DebuggerWindow), false,
                    selectedDebugger.BehaviorTree.Name);
                window.Show();
            }

            public void Init()
            {
                nameToTagString = new Dictionary<string, string>();
                nameToTagString["Selector"] = "选择节点";
                nameToTagString["Sequence"] = "顺序节点";
                nameToTagString["Parallel"] = "并行节点";
                // To do add more

                nestedBoxStyle = new GUIStyle();
                nestedBoxStyle.margin = new RectOffset(nestedPadding, 0, 0, 0);

                smallTextStyle = new GUIStyle();
                smallTextStyle.font = EditorStyles.miniFont;

//            nodeTextStyle = new GUIStyle(EditorStyles.label);

                nodeCapsuleGray = (GUIStyle)"helpbox";
                nodeCapsuleGray.normal.textColor = Color.black;

                nodeCapsuleFailed = new GUIStyle(nodeCapsuleGray);
                nodeCapsuleFailed.normal.textColor = Color.red;
                nodeCapsuleStopRequested = new GUIStyle(nodeCapsuleGray);
                nodeCapsuleStopRequested.normal.textColor = new Color(0.7f, 0.7f, 0.0f);

                defaultColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
            }

            public void OnSelectionChange()
            {
                if (selectedObject != null)
                {
                    Debugger tempSelectedDebugger = selectedObject.GetComponent<Debugger>();
                    if (tempSelectedDebugger == null)
                    {
                        return;
                    }

                    selectedObject = Selection.activeTransform;
                    selectedDebugger = tempSelectedDebugger;
                }

                Repaint();
            }

            public void OnGUI()
            {
                if (nameToTagString == null) Init();
                GUI.color = defaultColor;
                GUILayout.Toggle(false, "BehaviorTree Debugger", GUI.skin.FindStyle("LODLevelNotifyText"));
                GUI.color = Color.white;

                if (!Application.isPlaying)
                {
                    EditorGUILayout.HelpBox("Cannot use this utility in Editor Mode", MessageType.Info);
                    return;
                }

                if (selectedObject == null)
                {
                    EditorGUILayout.HelpBox("Please select an object", MessageType.Info);
                    return;
                }

                if (selectedDebugger == null)
                {
                    EditorGUILayout.HelpBox("This object does not contain a debugger component", MessageType.Info);
                    return;
                }
                else if (selectedDebugger.BehaviorTree == null)
                {
                    EditorGUILayout.HelpBox("BehavorTree is null", MessageType.Info);
                    return;
                }

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                GUILayout.BeginHorizontal();
                DrawBlackboardKeyAndValues("Blackboard:", selectedDebugger.Blackboard);
                if (selectedDebugger.Blackboard.Keys.Count > 0)
                {
                    DrawBlackboardKeyAndValues("Custom Stats:", selectedDebugger.Blackboard);
                }

                //DrawStats(selectedDebugger);
                GUILayout.EndHorizontal();
                GUILayout.Space(10);

                if (Time.timeScale <= 2.0f)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("TimeScale: ");
                    Time.timeScale = EditorGUILayout.Slider(Time.timeScale, 0.0f, 2.0f);
                    GUILayout.EndHorizontal();
                }

                DrawBehaviourTree(selectedDebugger);
                GUILayout.Space(10);

                EditorGUILayout.EndScrollView();

                Repaint();

            }

            private void DrawBehaviourTree(Debugger debugger)
            {
                EditorGUILayout.BeginVertical();
                {
                    GUILayout.Label("Behaviour Tree:", EditorStyles.boldLabel);

                    EditorGUILayout.BeginVertical(nestedBoxStyle);
                    DrawNodeTree(debugger.BehaviorTree, 0);
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }

            private void DrawNodeTree(INode node, int depth = 0, bool firstNode = true, float lastYPos = 0f)
            {
                bool decorator = node is Decorator && !(node is Root);
                bool parentIsDecorator = (node.Parent is Decorator);
                GUI.color = (node.IsActive) ? Color.green : new Color(1f, 1f, 1f, 0.3f);

                if (!parentIsDecorator)
                {
                    DrawSpacing();
                }

                bool drawConnected = node.Collapse;
                DrawNode(node, depth, drawConnected);

                Rect rect = GUILayoutUtility.GetLastRect();

                // Set intial line position
                if (firstNode)
                {
                    lastYPos = rect.yMin;
                }

                // Draw the lines
                Handles.BeginGUI();

                // Container collapsing
                Rect interactionRect = new Rect(rect);
                interactionRect.width = 100;
                interactionRect.y += 8;
                if (Event.current.type == EventType.MouseUp && Event.current.button == 0 &&
                    interactionRect.Contains(Event.current.mousePosition))
                {
                    node.Collapse = !node.Collapse;
                    Event.current.Use();
                }

                Handles.color = new Color(0f, 0f, 0f, 1f);
                if (!decorator)
                {
                    Handles.DrawLine(new Vector2(rect.xMin - 5, lastYPos + 4),
                        new Vector2(rect.xMin - 5, rect.yMax - 4));
                }
                else
                {
                    Handles.DrawLine(new Vector2(rect.xMin - 5, lastYPos + 4),
                        new Vector2(rect.xMin - 5, rect.yMax + 6));
                }

                Handles.EndGUI();

                if (decorator) depth++;

                if (!node.Collapse && node.DebugChildren.Count > 0)
                {
                    if (!decorator) EditorGUILayout.BeginVertical(nestedBoxStyle);

                    List<INode> children = node.DebugChildren;
                    if (children == null)
                    {
                        GUILayout.Label("CHILDREN ARE NULL");
                    }
                    else
                    {
                        lastYPos = rect.yMin + 16; // Set new Line position

                        for (int i = 0; i < children.Count; i++)
                        {
                            DrawNodeTree(children[i], depth, i == 0, lastYPos);
                        }
                    }

                    if (!decorator) EditorGUILayout.EndVertical();
                }

            }

            private void DrawNode(INode node, int depth, bool connected)
            {
                float tStopRequested = Mathf.Lerp(0.85f, 0.25f, 2.0f * (Time.time - node.DebugLastStopRequestAt));
                //float tStopped = Mathf.Lerp(0.85f, 0.25f, 2.0f * (Time.time - node.DebugLastStoppedAt));
                bool inactive = !node.IsActive;
                float alpha = inactive ? Mathf.Max(0.35f, Mathf.Pow(tStopRequested, 1.5f)) : 1.0f;
                bool failed = (tStopRequested > 0.25f && tStopRequested < 1.0f && !node.DebugLastResult && inactive);
                bool stopRequested = (tStopRequested > 0.25f && tStopRequested < 1.0f && inactive);

                EditorGUILayout.BeginHorizontal();
                {
                    GUI.color = new Color(1f, 1f, 1f, alpha);

                    string tagName;
                    GUIStyle tagStyle = stopRequested
                        ? nodeCapsuleStopRequested
                        : (failed ? nodeCapsuleFailed : nodeCapsuleGray);

                    bool drawLabel = !string.IsNullOrEmpty(node.Label);
                    string label = node.Label;

                    if (node is Composite) GUI.backgroundColor = Color.red;
                    if (node is Decorator) GUI.backgroundColor = Color.yellow;
                    if (node is Task) GUI.backgroundColor = Color.cyan;


                    nameToTagString.TryGetValue(node.Name, out tagName);

                    if (node.Collapse && node.DebugChildren.Count > 0)
                    {
                        if (!drawLabel)
                        {
                            drawLabel = true;
                            label = tagName;
                        }

                        tagName = "...";
                        GUI.backgroundColor = new Color(0.4f, 0.4f, 0.4f);
                    }

                    if (string.IsNullOrEmpty(tagName)) tagName = node.Name;

                    if (!drawLabel)
                    {
                        GUILayout.Label(tagName, tagStyle);
                    }
                    else
                    {
                        GUILayout.Label("(" + tagName + ") " + label, tagStyle);
                        // Reset background color
                        GUI.backgroundColor = Color.white; //
                    }

                    GUILayout.FlexibleSpace();


                    // Draw Stats
                    GUILayout.Label(
                        (node.DebugNumStopCalls > 0 ? node.DebugLastResult.ToString() : "") + " | " +
                        node.DebugNumStartCalls + " , " + node.DebugNumStopCalls + " , ", smallTextStyle);
                }

                EditorGUILayout.EndHorizontal();

                // Draw the lines
                if (connected)
                {
                    Rect rect = GUILayoutUtility.GetLastRect();

                    Handles.color = new Color(0f, 0f, 0f, 1f);
                    Handles.BeginGUI();
                    float midY = 4 + (rect.yMin + rect.yMax) / 2f;
                    Handles.DrawLine(new Vector2(rect.xMin - 5, midY), new Vector2(rect.xMin, midY));
                    Handles.EndGUI();
                }
            }

            private void DrawSpacing()
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }

            private void DrawStats(Debugger debugger)
            {
                EditorGUILayout.BeginVertical();
                {
                    GUILayout.Label("Stats:", EditorStyles.boldLabel);

                    Root behaviorTree = debugger.BehaviorTree;

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    {

                        DrawKeyValue("Total Starts:", behaviorTree.TotalNumStartCalls.ToString());
                        DrawKeyValue("Total Stops:", behaviorTree.TotalNumStopCalls.ToString());
                        DrawKeyValue("Total Stopped:", behaviorTree.TotalNumStoppedCalls.ToString());
                        //DrawKeyValue("Active Timers:  ", behaviorTree.Clock.NumTimers.ToString());
                        //DrawKeyValue("Timer Pool Size:  ", behaviorTree.Clock.DebugPoolSize.ToString());
                        //DrawKeyValue("Active Update Observers:  ", behaviorTree.Clock.NumUpdateObservers.ToString());
                        //DrawKeyValue("Active Blackboard Observers:  ", behaviorTree.Blackboard.NumObservers.ToString());
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }

            private void DrawBlackboardKeyAndValues(string label, Blackboard blackboard)
            {
                EditorGUILayout.BeginVertical();
                {
                    GUILayout.Label(label, EditorStyles.boldLabel);

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    {
                        List<string> keys = blackboard.Keys;
                        foreach (string key in keys)
                        {
                            DrawKeyValue(key, blackboard.Get(key).ToString());
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }

            private void DrawKeyValue(string key, string value)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(key, smallTextStyle);
                GUILayout.FlexibleSpace();
                GUILayout.Label(value, smallTextStyle);
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
#endif