using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Kandu
{
    public struct structScaffold
    {
        public Dictionary<string, string> Data;
        public Dictionary<string, string> arguments;
        public List<structScaffoldElement> elements;
    }

    public struct structScaffoldElement
    {
        public string name;
        public string htm;
    }

    public class Scaffold
    {
        [JsonIgnore]
        private Core S;
        
        public Dictionary<string, string> Data;
        public List<structScaffoldElement> elements;
        public string serializedElements;
        
        public Scaffold(Core KanduCore, string file = "", string html = "", string section = "")
        {
            S = KanduCore;
            Data = new Dictionary<string, string>();

            if (S.Server.Scaffold.ContainsKey(file + '/' + section) == false)
            {
                elements = new List<structScaffoldElement>();

                //first, check if html is already provided
                var htm = html;
                if(htm == "")
                {
                    //try loading file from disk or cache next
                    if (S.Server.Cache.ContainsKey(file) == false)
                    {
                        htm = File.ReadAllText(S.Server.MapPath(file));
                    }
                    else
                    {
                        htm = (string)S.Server.Cache[file];
                    }
                }

                //next, find the group of code matching the scaffold name
                int[] e = new int[3];
                if (section != "")
                {
                    string s = "";
                    e[0] = -1;
                    while(e[0] < 0) { 
                        //find starting tag (optionally with arguments)
                        //for example: {{button (name:submit, style:outline)}}
                        e[0] = htm.IndexOf("{{" + section);
                        if(e[0] >= 0)
                        {
                            e[1] = e[0] + 2 + section.Length;
                            s = htm.Substring(e[1], 1);
                            switch (s)
                            {
                                case "}":
                                    //found end of tag
                                    break;
                                    
                                default:
                                    e[0] = -1;
                                    break;
                            }
                        } else { break; }
                    }
                    e[1] = htm.IndexOf("{{/" + section + "}}");
                    if (e[0] >= 0 & e[1] > e[0])
                    {
                        e[2] = e[0] + 4 + section.Length;
                        htm = htm.Substring(e[2], e[1] - e[2]);
                    }
                }

                //get scaffold from html code
                var arr = htm.Split(new string[] { "{{" }, StringSplitOptions.RemoveEmptyEntries);
                var i = 0;
                structScaffoldElement scaff;

                for (var x = 0; x < arr.Length; x++)
                {
                    i = arr[x].IndexOf("}}");
                    scaff = new structScaffoldElement();
                    if (i > 0)
                    {
                        scaff.name = arr[x].Substring(0, i);
                        scaff.htm = arr[x].Substring(i + 2);
                    }
                    else
                    {
                        scaff.name = "";
                        scaff.htm = arr[x];
                    }
                    elements.Add(scaff);
                }
                if(S.Server.environment != Server.enumEnvironment.development){
                    //cache the scaffold file
                    var scaffold = new structScaffold();
                    scaffold.Data = Data;
                    scaffold.elements = elements;
                    S.Server.Scaffold.Add(file + '/' + section, scaffold);
                }
            }
            else
            {
                //get scaffold object from memory
                var scaffold = S.Server.Scaffold[file + '/' + section];
                Data = scaffold.Data;
                elements = scaffold.elements;
            }
            serializedElements = S.Util.Serializer.WriteObjectToString(elements);
        }

        public string Render()
        {
            return Render(Data);
        }

        public string Render(Dictionary<string, string> nData)
        {
            //deserialize list of elements since we will be manipulating the list,
            //so we don't want to permanently mutate the public elements array
            var elems = (List<structScaffoldElement>)S.Util.Serializer.ReadObject(serializedElements, typeof(List<structScaffoldElement>));
            if (elems.Count > 0)
            {
                //render scaffold with paired nData data
                var scaff = new StringBuilder();
                var s = "";
                var useScaffold = false;
                var closing = new List<List<string>>();

                //remove any unwanted blocks of HTML from scaffold
                for (var x = 0; x < elems.Count; x++)
                {
                    if (x < elems.Count - 1)
                    {
                        for (var y = x + 1; y < elems.Count; y++)
                        {
                            //check for closing tag
                            if (elems[y].name == "/" + elems[x].name)
                            {
                                //add enclosed group of HTML to list for removing
                                List<string> closed = new List<string>();
                                closed.Add(elems[x].name);
                                closed.Add(x.ToString());
                                closed.Add(y.ToString());

                                if (nData.ContainsKey(elems[x].name) == true)
                                {
                                    //check if user wants to include HTML 
                                    //that is between start & closing tag   
                                    s = nData[elems[x].name];
                                    if (string.IsNullOrEmpty(s) == true) { s = ""; }
                                    if (s == "true" | s == "1")
                                    {
                                        closed.Add("true");
                                    }
                                    else { closed.Add(""); }
                                }
                                else { closed.Add(""); }

                                closing.Add(closed);
                            }
                        }

                    }
                }

                //remove all groups of HTML in list that should not be displayed
                List<int> removeIndexes = new List<int>();
                bool isInList = false;
                for (int x = 0; x < closing.Count; x++)
                {
                    if (closing[x][3] != "true")
                    {
                        //add range of indexes from closing to the removeIndexes list
                        for (int y = int.Parse(closing[x][1]); y < int.Parse(closing[x][2]); y++)
                        {
                            isInList = false;
                            for (int z = 0; z < removeIndexes.Count; z++)
                            {
                                if (removeIndexes[z] == y) { isInList = true; break; }
                            }
                            if (isInList == false) { removeIndexes.Add(y); }
                        }
                    }
                }

                //physically remove HTML list items from scaffold
                int offset = 0;
                for (int z = 0; z < removeIndexes.Count; z++)
                {
                    elems.RemoveAt(removeIndexes[z] - offset);
                    offset += 1;
                }

                //finally, replace scaffold variables with custom data
                for (var x = 0; x < elems.Count; x++)
                {
                    //check if scaffold item is an enclosing tag or just a variable
                    useScaffold = true;
                    if (elems[x].name.IndexOf('/') < 0)
                    {
                        for (int y = 0; y < closing.Count; y++)
                        {
                            if (elems[x].name == closing[y][0]) { useScaffold = false; break; }
                        }
                    }
                    else { useScaffold = false; }

                    if ((nData.ContainsKey(elems[x].name) == true
                    || elems[x].name.IndexOf('/') == 0) & useScaffold == true)
                    {
                        //inject string into scaffold variable
                        s = nData[elems[x].name.Replace("/", "")];
                        if (string.IsNullOrEmpty(s) == true) { s = ""; }
                        scaff.Append(s + elems[x].htm);
                    }
                    else
                    {
                        //passively add htm, ignoring scaffold variable
                        scaff.Append(elems[x].htm);
                    }
                }

                //render scaffolding as HTML string
                return scaff.ToString();
            }
            return "";
        }
    }
}
