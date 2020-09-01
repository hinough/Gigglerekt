using System;
using System.Collections.Generic;
using System.Text;

namespace obs_websocket.Types
{
    public class SceneItem
    {
        public bool locked;             //True if item is locked, false if unlocked
        public bool muted;              //True if item is muted, false if unmuted
        public bool render;             //True if item is set to render, false if hidden

        public int alignment;           //Alighment of this item, 1=Left, 2=Right, 4=Top, 8=Bottom, Omit=Center to axis
        public int cx;                  //X Coordinate of this item
        public int cy;                  //Y Coordinate of this item
        public int id;                  //Scene item ID
        public int source_cx;           //Source CX
        public int source_cy;           //Source CY
        public int volume;              //Volume of this item
        public int x;                   //X Coordinate of this item
        public int y;                   //Y Coordinate of this item

        public string name;             //Name of this item 
        public string parentGroupName;  //(Optional) Name of parent item if this item is in a group
        public string type;             //Type if this item

        public List<SceneItem> groupChildren;//(Optional) List of children (If this item is a group parent)

        public SceneItem(bool locked, bool muted, bool render, int alignment, int cx, int cy, int id, int source_cx, int source_cy, int volume, int x, int y,
                         string name, string type, List<SceneItem> groupChildren = null, string parentGroupName = null)
        {
            this.locked = locked;
            this.muted = muted;
            this.render = render;

            this.alignment = alignment;
            this.cx = cx;
            this.cy = cy;
            this.id = id;
            this.source_cx = source_cx;
            this.source_cy = source_cy;
            this.volume = volume;
            this.x = x;
            this.y = y;
            this.name = name;
            this.type = type;

            this.groupChildren = groupChildren;
            this.parentGroupName = parentGroupName;
        }
    }   
}
