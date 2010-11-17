﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace GravityShift.Import_Code
{
    /// <summary>
    /// This class represents the inbetween stage of XML and translated GameObject
    /// </summary>
    class EntityInfo
    {
        //Opted not to do Properties to interface with the variables for ease of writing. I figure that its not too big of a deal in this case
        public string mName;
        public string mType;
        public string mTextureFile;

        public bool mVisible;

        public Vector2 mLocation = new Vector2();

        public Dictionary<string, string> mProperties;

        /// <summary>
        /// Creates an entity out of an XElement that defiens an entity
        /// </summary>
        /// <param name="entity">The XML chunck that defines this entity</param>
        public EntityInfo(XElement entity)
        {
            mProperties = new Dictionary<string,string>();
            foreach(XElement item in entity.Elements())
            {
                if (item.Name == XmlKeys.NAME)
                    mName = item.Value;
                if (item.Name == XmlKeys.TYPE)
                    mType = item.Value;
                if (item.Name == XmlKeys.TEXTURE)
                    mTextureFile = item.Value;
                if (item.Name == XmlKeys.VISIBLE)
                    mVisible = "True".Equals(item.Value);
                if (item.Name == XmlKeys.LOCATION)
                    mLocation = new Vector2(int.Parse(item.Attribute(XName.Get("X", "")).Value),
                        int.Parse(item.Attribute(XName.Get("Y", "")).Value));
                if (item.Name == XmlKeys.PROPERTIES)
                    foreach (XElement property in item.Elements())
                        mProperties.Add(property.Name.ToString(), property.Value);

            }
        }
    }
}