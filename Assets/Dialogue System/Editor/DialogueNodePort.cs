using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;

namespace Salt.DialogueSystem.Editor
{
    public class DialogueNodePort : Port
    {
        public string data;
        
       
        protected DialogueNodePort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
        {
            
        }

       
    }
}
