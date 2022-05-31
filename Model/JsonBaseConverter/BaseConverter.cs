﻿using Model.SaveableData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.JsonBaseConverter
{
    public class BaseConverter : JsonConverter
    {
        static JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new ParameterConverter() };

        public override bool CanConvert(System.Type objectType)
        {
            return (objectType == typeof(Parameter));
        }

        public object Create(Type objectType, JObject jObject)
        {
            if (jObject.ContainsKey("ParamType"))
            {
                var type = (int)jObject.Property("ParamType");
                switch (type)
                {
                    case 1:
                        return new Function();
                    case 2:
                        return new Constant();
                    case 3:
                        return new VariableRef();
                    case 4:
                        return new Value();
                    case 5:
                        return new IfThenElse();
                    case 6:
                        return new AndMultiple();
                    case 7:
                        return new OrMultiple();
                    case 8:
                        return new ForGroupMultiple();
                    case 9:
                        return new ForForceMultiple();
                    case 10:
                        return new ForLoopAMultiple();
                    case 11:
                        return new ForLoopBMultiple();
                    case 12:
                        return new ForLoopVarMultiple();
                    case 13:
                        return new SetVariable();
                    case 14:
                        return new EnumDestructablesInRectAllMultiple();
                    case 15:
                        return new EnumDestructiblesInCircleBJMultiple();
                    case 16:
                        return new EnumItemsInRectBJ();
                    default:
                        return new Parameter();
                }

                throw new ApplicationException(String.Format("The given parameter type {0} is not supported!", type));
            }
            else
                return new Parameter();

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Load JObject from stream
            JObject jObject = JObject.Load(reader);

            // Create target object based on JObject
            var target = Create(objectType, jObject);

            // Populate the object properties
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }


        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException(); // won't be called because CanWrite returns false
        }
    }
}
