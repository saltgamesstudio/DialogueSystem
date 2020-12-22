using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using Salt.DialogueSystem.Editor;

namespace Salt.DialogueSystem.Editor
{
    public class StartNode : CustomNode
    {
        public static StartNode Create()
        {

            var node = new StartNode()
            {
                title = "Entry Point",
                Guid = System.Guid.NewGuid().ToString(),
                isEntryPoint = true,
                name = "CustomNode"
            };
            var port = DialoguePort.Create(Direction.Output, Port.Capacity.Single);
            port.portName = "Next";
            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;

            node.outputContainer.Add(port);
            node.SetPosition(new Rect(100, 100, 0, 0));
            node.RefreshPorts();
            node.RefreshExpandedState();
            return node;
        }
    }
}
