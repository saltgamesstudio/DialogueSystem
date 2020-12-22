using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Salt.DialogueSystem.Data;
using UnityEditor.UIElements;

namespace Salt.DialogueSystem.Editor
{
    public class DialogueGraph : GraphView
    {
        public readonly Vector2 nodeSize = new Vector2(150,200);
        private NodeSearchWindow searchWindow;
        public DialogueGraph()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("EditorStyle"));

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new RectangleSelector());
            var grid = new GridBackground();
            Insert(0, grid);
            AddElement(StartNode.Create());
            AddSearchWindow();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (evt.target is GraphView)
            {
                evt.menu.AppendAction("Create Dialogue Node", delegate (DropdownMenuAction a)
                {
                    var node = DialogueNode.Create("Dialogue Node", new DialogueProperties());
                    var nodePos = this.contentViewContainer.WorldToLocal(a.eventInfo.mousePosition);
                    node.SetPosition(new Rect(nodePos, nodeSize));
                    AddElement(node);
                });
            }
            if (evt.target is GraphView)
            {
                evt.menu.AppendAction("Create Choice Node", delegate (DropdownMenuAction a)
                {
                    var node = ChoiceNode.Create("Choice Node");
                    var nodePos = this.contentViewContainer.WorldToLocal(a.eventInfo.mousePosition);
                    node.SetPosition(new Rect(nodePos, nodeSize));
                    AddElement(node);
                });
            }
            if (evt.target is BlackboardSection)
            {
                
                evt.menu.AppendAction("Create", null);
            }
            base.BuildContextualMenu(evt);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach((port) => {
                if (startPort != port && startPort.node != port.node && startPort.direction != port.direction) compatiblePorts.Add(port);
            });
            return compatiblePorts;
        }

        private void AddSearchWindow()
        {
            searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            searchWindow.CreateSearchWindow(this);
            nodeCreationRequest = ctx =>
            {
                SearchWindow.Open(new SearchWindowContext(ctx.screenMousePosition), searchWindow);
            };

        }
      
    }

}
