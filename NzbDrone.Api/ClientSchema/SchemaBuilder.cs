﻿using System.Collections.Generic;
using NzbDrone.Common.Reflection;
using NzbDrone.Core.Annotations;

namespace NzbDrone.Api.ClientSchema
{
    public static class SchemaBuilder
    {
        public static List<Field> GenerateSchema(object model)
        {
            var properties = model.GetType().GetSimpleProperties();

            var result = new List<Field>(properties.Count);

            foreach (var propertyInfo in properties)
            {
                var fieldAttribute = propertyInfo.GetAttribute<FieldDefinitionAttribute>(false);

                if (fieldAttribute != null)
                {

                    var field = new Field()
                        {
                            Name = propertyInfo.Name,
                            Label = fieldAttribute.Label,
                            HelpText = fieldAttribute.HelpText,
                            Order = fieldAttribute.Order,
                            Type = fieldAttribute.Type.ToString().ToLowerInvariant()
                        };

                    var value = propertyInfo.GetValue(model, null);
                    if (value != null)
                    {
                        field.Value = value;
                    }

                    result.Add(field);
                }
            }

            return result;

        }
    }
}