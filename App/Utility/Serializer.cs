using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProtoBuf;

namespace Kandu.Utility
{
    public class Serializer
    {
        private Util Util;

        public Serializer(Util util)
        {
            Util = util;
        }

        #region "Write"
        public string WriteObjectToString(object obj, Formatting formatting = Formatting.None, TypeNameHandling nameHandling = TypeNameHandling.Auto, Newtonsoft.Json.Serialization.IContractResolver contractResolver = null)
        {
            var resolver = contractResolver;
            if(resolver == null)
            {
                resolver = new IgnorableContractResolver();
            }
            return JsonConvert.SerializeObject(obj, formatting,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        TypeNameHandling = nameHandling,
                        ContractResolver = resolver
                    });
        }

        public byte[] WriteObject(object obj, Formatting formatting = Formatting.None, TypeNameHandling nameHandling = TypeNameHandling.Auto)
        {
            return Util.Str.GetBytes(WriteObjectToString(obj, formatting, nameHandling));
        }

        public void WriteObjectToFile(object obj, string file, Formatting formatting = Formatting.Indented, TypeNameHandling nameHandling = TypeNameHandling.Auto)
        {
            var path = Util.Str.getFolder(file);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllText(file, WriteObjectToString(obj, formatting, nameHandling));
        }
        #endregion

        #region "Read"
        public object ReadObject(string str, Type objType, TypeNameHandling nameHandling = TypeNameHandling.Auto)
        {
            return JsonConvert.DeserializeObject(str, objType, new JsonSerializerSettings() { TypeNameHandling = nameHandling });
        }

        public object OpenFromFile(Type objType, string file, TypeNameHandling nameHandling = TypeNameHandling.Auto)
        {
            return ReadObject(File.ReadAllText(file), objType);
        }

        #endregion

        #region "Write Compressed"
        public string CompressObjectToString(object obj)
        {
            using (var ms = new MemoryStream())
            {
                var sr = new StreamReader(ms);
                ProtoBuf.Serializer.SerializeWithLengthPrefix(ms, obj, PrefixStyle.Base128);
                ms.Position = 0;
                return sr.ReadToEnd();
            }
        }

        public byte[] CompressObject(object obj)
        {
            return Util.Str.GetBytes(CompressObjectToString(obj));
        }

        public void CompressObjectToFile(object obj, string file)
        {
            var path = Util.Str.getFolder(file);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (var fs = new FileStream(file, FileMode.OpenOrCreate))
            {
                ProtoBuf.Serializer.SerializeWithLengthPrefix(fs, obj, PrefixStyle.Base128);
            }
        }
        #endregion

        #region "Read Compressed"
        public object DecompressObject<T>(string str)
        {
            using (var ms = new MemoryStream())
            {
                var sw = new StreamWriter(ms);
                sw.Write(str);
                ms.Position = 0;
                return ProtoBuf.Serializer.DeserializeWithLengthPrefix<T>(ms, PrefixStyle.Base128);
            }
            
            //return ProtoBuf.Serializer.Deserialize(objType, new MemoryStream(b));
        }

        public object DecompressFromFile<T>(string file)
        {
            return DecompressObject<T>(File.ReadAllText(file));
        }
        #endregion
    }

    public class IgnorableContractResolver : DefaultContractResolver
    {
        protected readonly Dictionary<Type, HashSet<string>> Ignores;

        public IgnorableContractResolver()
        {
            Ignores = new Dictionary<Type, HashSet<string>>();
        }

        /// <summary>
        /// Explicitly ignore the given property(s) for the given type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName">one or more properties to ignore.  Leave empty to ignore the type entirely.</param>
        public void Ignore(Type type, params string[] propertyName)
        {
            // start bucket if DNE
            if (!Ignores.ContainsKey(type)) this.Ignores[type] = new HashSet<string>();

            foreach (var prop in propertyName)
            {
                Ignores[type].Add(prop);
            }
        }

        /// <summary>
        /// Is the given property for the given type ignored?
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool IsIgnored(Type type, string propertyName)
        {
            if (!Ignores.ContainsKey(type)) return false;

            // if no properties provided, ignore the type entirely
            if (Ignores[type].Count == 0) return true;

            return Ignores[type].Contains(propertyName);
        }

        /// <summary>
        /// The decision logic goes here
        /// </summary>
        /// <param name="member"></param>
        /// <param name="memberSerialization"></param>
        /// <returns></returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (IsIgnored(property.DeclaringType, property.PropertyName))
            {
                property.ShouldSerialize = instance => { return false; };
            }

            return property;
        }
    }
}
