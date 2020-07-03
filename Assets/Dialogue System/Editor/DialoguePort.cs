using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEngine.UIElements;

namespace Salt.DialogueSystem.Editor
{
    public class DialoguePort : Port
    {
        public string Next;
        public string Question;
        protected DialoguePort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
        {
        }
        public static Port Create(Direction direction, Capacity capacity)
        {
            var connector = new EdgeConnectorListener();
            var port = new DialoguePort(Orientation.Horizontal, direction, capacity, null)
            {
                m_EdgeConnector = new EdgeConnector<Edge>(connector)
            };
            port.AddManipulator(port.m_EdgeConnector);
            return port;
        }
        public override void Disconnect(Edge edge)
        {
            (edge.output as DialoguePort).Next = string.Empty;
            base.Disconnect(edge);
        }

    }
}
