/*
MIT License 

Copyright (c) 2021 legoandmars
Copyright (c) 2021 Auros Nexus

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is furnished
to do so, subject to the following conditions:

The above copyright notice and this permission notice (including the next
paragraph) shall be included in all copies or substantial portions of the
Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS
OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF
OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using WhyIsThereNoLeaderboard.Interfaces;

namespace WhyIsThereNoLeaderboard.HTTP
{
    internal class UnityWebRequestHttpResponse : IHttpResponse
    {
        public int Code { get; }
        public byte[] Bytes { get; }
        public bool Successful { get; }

        public UnityWebRequestHttpResponse(UnityWebRequest unityWebRequest, bool successful)
        {
            Successful = successful;
            Code = (int)unityWebRequest.responseCode;
            Bytes = unityWebRequest.downloadHandler.data;
        }

        public Task<byte[]> ReadAsByteArrayAsync()
        {
            return Task.FromResult(Bytes);
        }

        public Task<string> ReadAsStringAsync()
        {
            return Task.FromResult(Encoding.UTF8.GetString(Bytes));
        }

        public Task<Stream> ReadAsStreamAsync()
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(Bytes, 0, Bytes.Length);

            return Task.FromResult(stream as Stream);
        }

        public async Task<T> ReadAsObjectAsync<T>() where T : class
        {
            var str = await ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(str)!;
        }
    }
}