﻿using System.IO;

namespace Byt3.Callbacks
{
    public interface IOCallback
    {
        string ReadText(string path);
        string[] ReadLines(string path);
        Stream GetStream(string path);
        bool FileExists(string file);
        string[] GetFiles(string path, string searchPattern = "*.*");
    }
}