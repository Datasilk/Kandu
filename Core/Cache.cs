using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;

namespace Kandu
{
    public static class Cache
    {
        //Dictionary used for caching non-serialized objects, files from disk, or raw text
        public static Dictionary<string, object> Store { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Loads a file from cache. If the file hasn't been cached yet, then load file from a drive.
        /// </summary>
        /// <param name="filename">The relevant path to the file</param>
        /// <param name="noDevEnvCache">If true, it will not load a file from cache if the app is running in a development environment. Instead, it will always load the file from a drive.</param>
        /// <param name="noCache">If true, will not save to cache, but will instead load file from disk every time</param>
        /// <returns></returns>
        public static string LoadFile(string filename)
        {
            if (App.Environment != Environment.development)
            {
                //next, check cache
                if (Store.ContainsKey(filename))
                {
                    return (string)Store[filename];
                }
            }
            if (File.Exists(App.MapPath(filename)))
            {
                //finally, check file system
                var file = File.ReadAllText(App.MapPath(filename));
                if (App.Environment != Environment.development)
                {
                    Store.Add(filename, file);
                }
                return file;
            }
            return "";
        }

        public static void SaveFile(string filename, string value)
        {
            File.WriteAllText(App.MapPath(filename), value);
            if (Store.ContainsKey(filename))
            {
                Store[filename] = value;
            }
            else
            {
                Store.Add(filename, value);
            }
        }

        public static void Save(string key, object value)
        {
            if (Store.ContainsKey(key))
            {
                Store[key] = value;
            }
            else
            {
                Store.Add(key, value);
            }
        }

        public static T Load<T>(string key, Func<T> value, bool serialize = true)
        {
            if (!Store.ContainsKey(key) || Store[key] == null)
            {
                var obj = value();
                Save(key, serialize ? (object)JsonSerializer.Serialize(obj) : obj);
                return obj;
            }
            else
            {
                return serialize ? JsonSerializer.Deserialize<T>((string)Store[key]) : (T)Store[key];
            }
        }

        public static void Remove(string key)
        {
            if (Store.ContainsKey(key))
            {
                Store.Remove(key);
            }
        }

        public static void Add(string key, object value)
        {
            Store.Add(key, value);
        }
    }
}
