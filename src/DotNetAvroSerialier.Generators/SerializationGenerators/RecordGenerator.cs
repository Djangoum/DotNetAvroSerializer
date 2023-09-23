﻿using Avro;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.Models;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetAvroSerializer.Generators.SerializationGenerators
{
    internal static class RecordGenerator
    {
        internal static void GenerateSerializationSourceForRecordField(Field field, StringBuilder serializationCode, PrivateFieldsCode privateFieldsCode, RecordSerializableTypeMetadata recordTypeMetadata, IEnumerable<CustomLogicalTypeMetadata> customLogicalTypes, string sourceAccesor)
        {
            var property = recordTypeMetadata.Fields.FirstOrDefault(f => 
                f.Name.Equals(field.Name, StringComparison.InvariantCultureIgnoreCase)
                || f.Names.Contains(field.Name)
            );

            if (property is null)
                throw new AvroGeneratorException($"Property {field.Name} not found in {recordTypeMetadata}");

            SerializationGenerator.GenerateSerializatonSourceForSchema(field.Schema, serializationCode, privateFieldsCode, property.InnerSerializableType, customLogicalTypes, $"{sourceAccesor}.{property.Name}");
        }
    }
}