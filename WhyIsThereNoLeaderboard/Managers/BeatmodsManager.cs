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

using IPA.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using WhyIsThereNoLeaderboard.Data;
using WhyIsThereNoLeaderboard.HTTP;
using WhyIsThereNoLeaderboard.Interfaces;

namespace WhyIsThereNoLeaderboard.Managers
{
    // :(
    public class BeatmodsManager : IDisposable
    {
        private readonly IHttpService _httpService;
        public BeatmodsManager()
        {
            _httpService = new UnityWebRequestService()
            {
                BaseURL = "", // :(
                Timeout = TimeSpan.FromSeconds(30),
                UserAgent = "WhyIsThereNoLeaderboard",
            };

            ModWrapper();
        }

        public async void ModWrapper()
        {
        }
        public async Task<bool> DownloadModFromID(string modID)
        {
            // Some of these things (versions, aliases, etc) should probably be cached
            // However, this will in all likelyhood run twice at the most, so it's probably not worth the effort

            var fixedGameVersion = IPA.Utilities.UnityGame.GameVersion;
            if (fixedGameVersion.ToString().Contains('_')) fixedGameVersion = new AlmostVersion(fixedGameVersion.ToString().Split('_')[0]);

            // Versions
            var beatmodsVersionResponse = await _httpService.GetAsync(Constants.BeatModsVersions).ConfigureAwait(false);
            if (!beatmodsVersionResponse.Successful) return false;
            var beatmodsVersionData = await beatmodsVersionResponse.ReadAsStringAsync();

            List<string> versions = JsonConvert.DeserializeObject<string[]>(beatmodsVersionData).ToList();

            // Alias Versions
            var beatmodsAliasResponse = await _httpService.GetAsync(Constants.BeatModsAlias).ConfigureAwait(false);
            if (!beatmodsAliasResponse.Successful) return false;
            var beatmodsAliasData = await beatmodsAliasResponse.ReadAsStringAsync();

            Dictionary<string, string[]> aliases = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(beatmodsAliasData);

            var beatmodsVersion = CheckVersion(versions, aliases, fixedGameVersion.ToString());
            if (beatmodsVersion == "") return false;

            // Mod List
            var beatmodsModResponse = await _httpService.GetAsync(Constants.BeatModsAPIUrl + Constants.BeatModsModsOptions + "&gameVersion=" + beatmodsVersion);
            if (!beatmodsModResponse.Successful) return false;
            var beatmodsModData = await beatmodsModResponse.ReadAsStringAsync();

            var mods = JsonConvert.DeserializeObject<Mod[]>(beatmodsModData);

            foreach(var mod in mods)
            {
                RegisterDependencies(mod, mods);
            }

            Mod? matchingMod = null;
            foreach(var mod in mods)
            {
                if(mod.name == modID) matchingMod = mod;
            }
            if(matchingMod == null) return false;

            // Finally download

            var directory = Path.Combine(IPA.Utilities.UnityGame.InstallPath, "IPA", "Pending");

            return await InstallMod(matchingMod, directory, mods);
        }

        private async Task<bool> InstallMod(Mod mod, string directory, Mod[] ModList)
        {
            Mod.DownloadLink matchingModDownload = null;
            foreach (var download in mod.downloads)
            {
                if (download.type == "universal") matchingModDownload = download;
            }
            if (matchingModDownload == null) return false;

            foreach(var dependency in mod.dependencies)
            {
                if (dependency.name == "BSIPA") continue;
                foreach(var tempMod in ModList)
                {
                    if(tempMod.name == dependency.name)
                    {
                        await InstallMod(tempMod, directory, ModList);
                    }
                }
            }

            using (Stream stream = await DownloadMod(Constants.BeatModsURL + matchingModDownload.url))
            using (ZipArchive archive = new ZipArchive(stream))
            {
                foreach (ZipArchiveEntry file in archive.Entries)
                {
                    string fileDirectory = Path.GetDirectoryName(Path.Combine(directory, file.FullName));
                    if (!Directory.Exists(fileDirectory))
                    {
                        Directory.CreateDirectory(fileDirectory);
                    }

                    if (!string.IsNullOrEmpty(file.Name))
                    {
                        await ExtractFile(file, Path.Combine(directory, file.FullName), 3.0, 10);
                    }
                }

                return true;
            }

        }

        private async Task<bool> ExtractFile(ZipArchiveEntry file, string path, double seconds, int maxTries, int tryNumber = 0)
        {
            if (tryNumber < maxTries)
            {
                try
                {
                    file.ExtractToFile(path, true);

                    return true;
                }
                catch
                {
                    await Task.Delay((int)(seconds * 1000));
                    return await ExtractFile(file, path, seconds, maxTries, tryNumber + 1);
                }
            }
            else return false;
        }

        private async Task<Stream> DownloadMod(string link)
        {
            var resp = await _httpService.GetAsync(link);
            return await resp.ReadAsStreamAsync();
        }

        private void RegisterDependencies(Mod dependent, Mod[] ModsList)
        {
            if (dependent.dependencies.Length == 0)
                return;

            foreach (Mod mod in ModsList)
            {
                foreach (Mod.Dependency dep in dependent.dependencies)
                {

                    if (dep.name == mod.name)
                    {
                        dep.Mod = mod;
                        mod.Dependents.Add(dependent);

                    }
                }
            }
        }

        private string CheckVersion(List<string> versions, Dictionary<string, string[]> aliasesDict, string detectedVersion)
        {
            Dictionary<string, List<string>> aliases = aliasesDict.ToDictionary(x => x.Key, x => x.Value.ToList());
            foreach (string version in versions)
            {
                if (version.Contains(detectedVersion)) return version;
                if (aliases.TryGetValue(version, out var x))
                {
                    if (x.Contains(detectedVersion))
                    {
                        return version;
                    }
                }
            }

            return string.Empty;
        }

        public void Dispose()
        {
            if (_httpService is IDisposable disposable)
                disposable.Dispose();
        }
    }
}
