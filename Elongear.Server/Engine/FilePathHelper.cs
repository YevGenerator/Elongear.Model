using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elongear.Server.Engine;

public class FilePathHelper
{
    public string RootPath { get; set; } = @"C:\Users\yesman\Desktop\ToRemove\Server";
    public string FileDirectoryName => "Podcasts";
    public string FileDirectoryPath => Path.Combine(RootPath, FileDirectoryName);
    public string GetPodcastDirectoryPath(string podcasetId) => Path.Combine(FileDirectoryPath, podcasetId);
    public void CreatePodcastDirectory(string podcastId)
    {
        var dirPath = GetPodcastDirectoryPath(podcastId);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
    }
    public static string ImageExtensionToMimeType(string extension) => "image/" + extension;

    private string GetFilePath(string podcastId, string partOption)
    {
        var directoryPath = GetPodcastDirectoryPath(podcastId);
        if(!Directory.Exists(directoryPath))
        {
            return "";
        }
        var files = Directory.GetFiles(directoryPath, "*" + partOption+"*");
        if (files.Length == 0) return "";
        return files[0];
    }
    public string GetExistingImagePath(string podcastId) => GetFilePath(podcastId, "_im");

    private string GetNewPath(string podcastId, string ending) =>
        Path.Combine(GetPodcastDirectoryPath(podcastId), podcastId + ending);

    public string GetNewPodcastPath(string podcastId, string mimeType) =>
        GetNewPath(podcastId, "_pod" + GetExtensionByAudioMime(mimeType));

    public string GetNewImagePath(string podcastId, string mimeType) =>
        GetNewPath(podcastId, "_im" + GetExtensionByImageMime(mimeType));

    public string GetExisitingPodcastPath(string podcastId) => GetFilePath(podcastId, "_pod");
    public static string GetExtension(string filePath) => Path.GetExtension(filePath).Remove(0, 1);
    public static string GetExtensionByImageMime(string mimetype) => "." + mimetype.Replace("image/", "");
    public static string GetExtensionByAudioMime(string mimetype)
    {
        mimetype = mimetype.Replace("audio/", "");
        return mimetype switch
        {
            "x-m4a" => ".m4a",
            "mpeg" => ".mp3",
            "vnd.wav" => ".wav",
            "ogg" => ".ogg",
            _ => ".mp3"
        };
    }
    public static string GetAudioMimeFromExtension(string extension) =>
        "audio/" + extension switch
        {
            "mp3" => "mpeg",
            "wav" => "wnv.wav",
            "ogg" => "ogg",
            "m4a" => "x-m4a",
            _ => "mpeg"
        };
}
