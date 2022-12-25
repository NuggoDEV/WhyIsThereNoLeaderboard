/*
MIT License 

Copyright (c) 2021 legoandmars
Copyright(c) 2021 Auros Nexus

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

using System.IO;
using System.Threading.Tasks;

namespace WhyIsThereNoLeaderboard.Interfaces
{
    public interface IHttpResponse
    {
        /// <summary>
        /// The HTTP status code of the response.
        /// </summary>
        int Code { get; }

        /// <summary>
        /// Whether or not the request was successful or not.
        /// </summary>
        bool Successful { get; }

        /// <summary>
        /// Read the body as a string.
        /// </summary>
        /// <returns>The body represented as a string.</returns>
        Task<string> ReadAsStringAsync();

        /// <summary>
        /// Read the body as a byte array.
        /// </summary>
        /// <returns>The body represented as an array of bytes.</returns>
        /// 
        Task<byte[]> ReadAsByteArrayAsync();
        /// <summary>
        /// Read the body as a byte array.
        /// </summary>
        /// <returns>The body represented as a stream.</returns>
        Task<Stream> ReadAsStreamAsync();

        /// <summary>
        /// Reads the body and deserializes it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> ReadAsObjectAsync<T>() where T : class;
    }
}