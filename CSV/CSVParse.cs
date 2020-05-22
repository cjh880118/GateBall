using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using JHchoi.Models;

public class CSVParse
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static List<Dictionary<string, object>> CSVParser(string file)
    {
        List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

        using (FileStream fs = new FileStream(file, FileMode.Open))
        {
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8, false))
            {
                string strLineValue = null;
                //string[] keys = null;
                string[] values = null;

                List<string> stringValue = new List<string>();

                while ((strLineValue = sr.ReadLine()) != null)
                    stringValue.Add(strLineValue);

                Dictionary<string, object> dicValue = new Dictionary<string, object>();

                List<string> header = new List<string>();

                for (int index = 0; index < stringValue.Count; index++)
                {
                    values = stringValue[index].Split(',');
                    for (int nIndex = 0; nIndex < values.Length; nIndex++)
                    {
                        if (index == 0)
                            header.Add(values[nIndex]);
                        else
                            dicValue[header[nIndex]] = values[nIndex];
                    }
                    if (index != 0)
                    {
                        list.Add(new Dictionary<string, object>(dicValue));
                    }
                }
                return list;
            }
        }
    }
}



 //// Must not be empty.
 //                   if (string.IsNullOrEmpty(strLineValue)) return list;

 //                   if (strLineValue.Substring(0, 1).Equals("#"))
 //                   {
 //                       keys = strLineValue.Split(',');

 //                       keys[0] = keys[0].Replace("#", "");


 //                       // Output
 //                       for (int nIndex = 0; nIndex<keys.Length; nIndex++)
 //                       {
 //                           Debug.Log("Key : " + keys[nIndex]);
 //                           if (nIndex != keys.Length - 1)
 //                               Debug.Log(", ");
 //                       }

 //                       //Console.WriteLine();

 //                       continue;
 //                   }
