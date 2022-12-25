/*
MIT License

Copyright (c) 2019-2020 Assistant

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#nullable disable

using System.Collections.Generic;

namespace WhyIsThereNoLeaderboard.Data
{
    public class Mod
    {
        public string name;
        public string version;
        public string gameVersion;
        public string _id;
        public string status;
        public string authorId;
        public string uploadedDate;
        public string updatedDate;
        public Author author;
        public string description;
        public string link;
        public string category;
        public DownloadLink[] downloads;
        public bool required;
        public Dependency[] dependencies;
        public List<Mod> Dependents = new List<Mod>();

        public class Author
        {
            public string _id;
            public string username;
            public string lastLogin;
        }

        public class DownloadLink
        {
            public string type;
            public string url;
            public FileHashes[] hashMd5;
        }

        public class FileHashes
        {
            public string hash;
            public string file;
        }

        public class Dependency
        {
            public string name;
            public string _id;
            public Mod Mod;
        }
    }
}
