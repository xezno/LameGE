using Engine.ECS.Entities;
using Engine.Common.DebugUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Quincy
{
    struct SceneEntity
    {
        public string TypeName { get; set; }
        public Dictionary<string, string> Properties { get; set; }

        public IEntity ToEntity()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes().Where(t => t?.BaseType?.GetInterfaces()?.Contains(typeof(IEntity)) ?? false))
                {
                    Logging.Log($"Found entity type {type.Name}");
                    if (type.Name.Equals(TypeName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        var newEntity = Activator.CreateInstance(type);
                        foreach (var propertyKvp in Properties)
                        {
                            var field = newEntity.GetType().GetField(propertyKvp.Key);
                            var prop = newEntity.GetType().GetProperty(propertyKvp.Key);
                            var member = (MemberInfo)field ?? prop;
                            var propType = field?.FieldType ?? prop?.PropertyType;

                            var value = Convert.ChangeType(propertyKvp.Value, propType);

                            // Assign to field or property
                            if (field != null)
                            {
                                field.SetValue(newEntity, value);
                            }
                            else if (prop != null)
                            {
                                prop.SetValue(newEntity, value);
                            }
                            else
                            {
                                Logging.Log($"No such property or field {propertyKvp.Key} on type {newEntity.GetType()}");
                            }
                        }
                        return (IEntity)newEntity;
                    }
                }
            }

            Logging.Log($"Entity type {TypeName} does not exist", Logging.Severity.Fatal);
            return null;
        }
    }

    struct Scene
    {
        public string Name { get; set; }
        public List<SceneEntity> Entities { get; set; }
    }
}
