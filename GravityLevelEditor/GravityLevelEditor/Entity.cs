﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;
using System.Xml.Linq;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace GravityLevelEditor
{
    class Entity
    {
        static int ObjectID = 0;
        private int mID = ObjectID++;
        public int ID { get { return mID; } }

        private string mName;
        public string Name { get { return mName; } set { mName = value; } }

        private string mType;
        public string Type { get { return mType; } set { mType = value; } }

        private Point mLocation;
        public Point Location { get { return mLocation; } set { mLocation = value; } }
        
        private bool mVisible;
        public bool Visible { get { return mVisible; } set { mVisible = value; } }

        private bool mPaintable;
        public bool Paintable { get { return mPaintable; } set { mPaintable = value; } }

        //private bool mSelected = false;
        //public bool Selected { get { return mSelected; } set { mSelected = true; } }

        private Image mTexture;
        public Image Texture { get { return mTexture; } set { mTexture = value; } }

        private Dictionary<string, string> mProperties;
        public Dictionary<string, string> Properties { get { return mProperties; } set { mProperties = value; } } 

        /*
         * Entity
         * 
         * Constructor to create an exact copy of the given entity.
         * 
         * Entity original: Initial entity to be copied.
         */
        public Entity(Entity original)
        {
            mType = original.mType;
            mName = original.mName;
            mVisible = original.mVisible;
            mPaintable = original.mPaintable;
            mProperties = original.mProperties;
            mLocation = original.mLocation;
            mTexture = original.mTexture;
        }

        /*
         * Entity
         * 
         * Constructor that creates an entity from with the given properties
         * 
         * string type: type of entity (i.e. Wall, PlayerStart, etc).
         * bool visibility: whether or not the entity will be visible in-game.
         * bool paintable: whether or not the given entity is paintable.
         * Dictionary<string, string> properties: additional properties for this entity.
         * Image texture: image used to represent this entity in the level editor.
         */
        public Entity(string type, string name, bool visibility,
            bool paintable, Dictionary<string, string> properties, Image texture)
        {
            mType = type;
            mName = name;
            mLocation = new Point(-100, -100);
            mVisible = visibility;
            mPaintable = paintable;
            mProperties = properties;
            mTexture = texture;
        }

        /*
         * Entity
         *
         * Constructor that creates an entity from a given Xelement.
         * 
         * XElement ent : the XML XElement that contains information for this entity.
         */
        public Entity(XElement ent)
        {
            string currentDirectory = "..\\..\\..\\..\\WindowsGame1\\Content\\Images";

            DirectoryInfo d = new DirectoryInfo(currentDirectory);

            mProperties = new Dictionary<string, string>();

            foreach (XElement el in ent.Elements())
            {
                if (el.Name == "ID")
                {
                    mID = Convert.ToInt32(el.Value.ToString());
                }
                if (el.Name == "Name")
                {
                    mName = el.Value.ToString();
                }
                if (el.Name == "Type")
                {
                    mType = el.Value.ToString();
                }
                if (el.Name == "Location")
                {
                    Point xLoc = new Point(Convert.ToInt32(el.Attribute("X").Value.ToString()), Convert.ToInt32(el.Attribute("Y").Value.ToString()));
                    mLocation = xLoc;
                }
                if (el.Name == "Visible")
                {
                    if (el.Value == "True")
                    {
                        mVisible = true;
                    }
                    else mVisible = false;
                }
                if (el.Name == "Paintable")
                {
                    if (el.Value == "True")
                    {
                        mPaintable = true;
                    }
                    else mPaintable = false;
                }
                if (el.Name == "Texture")
                {
                    currentDirectory = d.FullName + "\\" + el.Value + ".png";
                    mTexture = Image.FromFile(currentDirectory);
                    mTexture.Tag = el.Value;
                }
                if (el.Name == "Properties")
                {
                    foreach (XElement property in el.Elements())
                    {
                        mProperties.Add(property.Name.ToString(), property.Value.ToString());
                    }
                }
            }
        }

        /*
         * MoveEntity
         * 
         * Moves the entity to the given location.
         * 
         * Point where: location to move the entity to.
         */
        public void MoveEntity(Point where)
        {
            mLocation = where;
        }

        /*
         * ToggleVisibility
         * 
         * Marks this entity as visible or invisible for in-game play.
         */
        public void ToggleVisibility()
        {
            mVisible = !mVisible;
        }

        /*
         * Draw
         * 
         * Draws this entity in the editor.
         * 
         * Graphics g: the Graphics Device to draw to.
         * 
         * Point offset: the offset that the level editor is at.
         */
        public void Draw(Graphics g, Point offset)
        {
            Rectangle drawLocation = GridSpace.GetDrawingRegion(mLocation, offset);
            
            g.DrawImage(mTexture, drawLocation);
        }

        /*
         * Copy
         * 
         * Copies this entity, creating a clone with the same properties.
         * 
         * Return Value: copied entity.
         */
        public Entity Copy()
        {
            return new Entity(this);
        }

        /*
         * Equals
         * 
         * Checks to see if the given object is equal to this entity.
         * 
         * object obj: object we are comparing against.
         * 
         * Return Value: true if equal, false otherwise.
         */
        public override bool Equals(object obj)
        {
            return obj is Entity && ((Entity)obj).mID == mID;
        }

        /*
         * GetHashCode
         * 
         * Gets a hash representation of this object.
         * 
         * Return Value: An integer representing this objects hash code. 
         */
        public override int GetHashCode()
        {
            int hash = 0;
            foreach (char c in (mType + "/" + mName))
                hash += c * 71;

            return base.GetHashCode() + hash;
        }

        /*
         * ToString
         * 
         * Gets the string representation of this Entity.
         * 
         * Return Value: The string representation fo this entity.
         */
        public override string ToString()
        {
            return mType + "/" + mName;
        }

        //TODO - Import/Export methods

        /*
         * Export
         * 
         * Creates an XML XElement representation of this entity.
         *
         */
        public XElement Export()
        {

            if (mTexture.Tag == null)
            { MessageBox.Show("Failed to save " + ToString() + ID + ". Invalid image."); return null; }

            XElement propertiesTree = new XElement("Properties");

            foreach (string key in mProperties.Keys)
                propertiesTree.Add(new XElement(key, mProperties[key]));

            XElement entityTree = new XElement("Entity",
                new XElement("ID", this.ID),
                new XElement("Name", this.Name),
                new XElement("Type", this.Type),
                new XElement("Location",
                    new XAttribute("X", this.Location.X),
                    new XAttribute("Y", this.Location.Y)),
                new XElement("Visible", this.Visible.ToString()),
                new XElement("Paintable", this.Paintable.ToString()),
                new XElement("Texture", this.mTexture.Tag.ToString()),
                propertiesTree);

            return entityTree;
        }
    }
}
