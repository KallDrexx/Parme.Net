﻿#define SupportsEditMode
#define ScreenManagerHasPersistentPolygons
#define SpriteHasTolerateMissingAnimations
using Parme.Net.Frb.Example;


using System;
using System.Collections.Generic;
using System.Text;
using GlueControl.Models;
using System.Linq;

namespace GlueControl.Managers
{
    public class ObjectFinder
    {
        static ObjectFinder mSelf = new ObjectFinder();
        public static ObjectFinder Self => mSelf;
        public GlueProjectSave GlueProject { get; set; }

        #region Replace (needed only for game to replace elements from disk/glue)

        public void Replace(GlueElement element)
        {
            if (element is ScreenSave screenSave)
            {
                var existing = GlueProject.Screens.FirstOrDefault(item => item.Name == element.Name);

                if (existing != null)
                {
                    GlueProject.Screens.Remove(existing);
                }
                GlueProject.Screens.Add(screenSave);
            }
            else if (element is EntitySave entitySave)
            {
                var existing = GlueProject.Entities.FirstOrDefault(item => item.Name == element.Name);

                if (existing != null)
                {
                    GlueProject.Entities.Remove(existing);
                }
                GlueProject.Entities.Add(entitySave);
            }
        }

        #endregion


        #region Get Element

        public EntitySave GetEntitySave(string entityName)
        {
            // This method is called by NamedObjectSave whenever its
            // SourceClassType is set.  This is set when the project
            // is first deserialized, and when that happens the project
            // isn't valid yet.  Therefore, we should check to see if it
            // is valid.
            if (GlueProject != null)
            {
                if (!string.IsNullOrEmpty(entityName))
                {
                    // We don't know what project is using the Glue classes, and it may prefer
                    // forward slashes or back slashes.  Therefore we should tolerate either when
                    // making comparisons
                    entityName = entityName.Replace('/', '\\');

                    for (int i = 0; i < GlueProject.Entities.Count; i++)
                    {
                        EntitySave entitySave = GlueProject.Entities[i];

                        if (entitySave.Name.Replace('/', '\\') == entityName)
                        {
                            return entitySave;
                        }
                    }
                }
            }

            return null;
        }


        public ScreenSave GetScreenSave(string screenName)
        {
            // try to get qualified first...
            if (screenName != null)
            {
                screenName = screenName.Replace('/', '\\');

                if (GlueProject != null)
                {
                    for (int i = 0; i < GlueProject.Screens.Count; i++)
                    {
                        ScreenSave screenSave = GlueProject.Screens[i];

                        if (screenSave.Name == screenName)
                        {
                            return screenSave;
                        }
                    }

                    for (int i = 0; i < GlueProject.Screens.Count; i++)
                    {
                        ScreenSave screenSave = GlueProject.Screens[i];

                        if (screenSave.ClassName == screenName)
                        {
                            return screenSave;
                        }
                    }


                }
            }


            return null;
        }



        public GlueElement GetElement(string elementName)
        {
            GlueElement retval;

            retval = GetScreenSave(elementName);

            if (retval == null)
            {
                retval = GetEntitySave(elementName);
            }

            return retval;
        }


        public GlueElement GetElementContaining(NamedObjectSave namedObjectSave)
        {
            for (int i = 0; i < ObjectFinder.Self.GlueProject.Screens.Count; i++)
            {
                var screen = ObjectFinder.Self.GlueProject.Screens[i];
                if (IsContainedInListOrAsChild(screen.NamedObjects, namedObjectSave))
                {
                    return screen;
                }
            }
            for (int i = 0; i < ObjectFinder.Self.GlueProject.Entities.Count; i++)
            {
                if (IsContainedInListOrAsChild(ObjectFinder.Self.GlueProject.Entities[i].NamedObjects, namedObjectSave))
                {
                    return ObjectFinder.Self.GlueProject.Entities[i];
                }
            }

            return null;

        }

        bool IsContainedInListOrAsChild(List<NamedObjectSave> namedObjects, NamedObjectSave objectToFind)
        {
            foreach (NamedObjectSave nos in namedObjects.ToArray())
            {
                if (nos == objectToFind || IsContainedInListOrAsChild(nos.ContainedObjects, objectToFind))
                {
                    return true;
                }
            }
            return false;
        }



        #endregion

        #region NamedObjectSave properties

        public T GetPropertyValueRecursively<T>(NamedObjectSave nos, string propertyName)
        {
            var propertyOnNos = nos.Properties.FirstOrDefault(item => item.Name == propertyName);

            if (propertyOnNos != null)
            {
                if (typeof(T).IsEnum && propertyOnNos.Value is long asLong)
                {
                    return (T)((object)(int)asLong);
                }
                // There seem to be some type leaks...Maybe from old projects.
                else if (typeof(T) == typeof(int) && propertyOnNos.Value is long asLong2)
                {
                    return (T)((object)(int)asLong2);
                }
                else
                {
                    return (T)((object)propertyOnNos.Value);
                }
            }
            else if (nos.DefinedByBase)
            {

                // this NOS doesn't have it, but maybe it's defined in a base:
                var elementContaining = GetElementContaining(nos);

                if (!string.IsNullOrEmpty(elementContaining?.BaseElement))
                {
                    var baseElement = GetBaseElement(elementContaining);
                    var nosInBase = baseElement.GetNamedObject(nos.InstanceName);

                    return GetPropertyValueRecursively<T>(nosInBase, propertyName);
                }
            }
            return default(T);
        }

        #endregion

        #region Inheritance

        public GlueElement GetBaseElement(GlueElement derivedElement)
        {
            if (!string.IsNullOrEmpty(derivedElement?.BaseElement))
            {
                return GetElement(derivedElement.BaseElement);
            }
            else
            {
                return null;
            }
        }


        #endregion
    }
}
