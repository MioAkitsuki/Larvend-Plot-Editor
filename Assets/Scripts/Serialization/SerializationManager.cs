using System.Collections;
using System.Collections.Generic;
using Schwarzer.Windows;
using UnityEngine;
using UnityEditor;
using System.IO;
using QFramework;
using ICSharpCode.SharpZipLib.Zip;
using Larvend.PlotEditor.DataSystem;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System;
using YamlDotNet.Core;

namespace Larvend.PlotEditor.Serialization
{
    public class ProjectHelper
    {
        /// <summary>
        /// When you are trying to create a project:
        /// Create a guid -> Create temporary folder
        /// </summary>
        public static bool NewProject(out string _guid)
        {
            _guid = System.Guid.NewGuid().ToString("D");
            Directory.CreateDirectory(Path.Combine(Application.temporaryCachePath, _guid));

            return true;
        }

        /// <summary>
        /// When you are trying to open a project:
        /// Select a path -> Compress the project file into temp folder -> Deserialize the project
        /// </summary>
        public static bool OpenProject(string _path, out string _guid)
        {
            _guid = null;
            try
            {
                _guid = ZipHelper.UnCompress(_path);
                if (string.IsNullOrEmpty(_guid)) return false;

                SerializationHelper.DeSerializeProject(_guid);

                return true;
            }
            catch (System.Exception _e)
            {
                Debug.LogError("[ProjectHelper.OpenProject]: " + _e.ToString());
                return false;
            }
        }

        /// <summary>
        /// When you are trying to save a project:
        /// Select a path -> Serialize the project -> Compress the serialized project folder -> Save to target path
        /// </summary>
        public static bool SaveProject(string _targetPath)
        {
            if (File.Exists(_targetPath)) File.Delete(_targetPath);

            try
            {
                SerializationHelper.SerializeProject();

                List<string> _paths = new List<string>();
                _paths.AddRange(Directory.GetFiles(ProjectManager.ProjectFolderPath));
                _paths.AddRange(Directory.GetDirectories(ProjectManager.ProjectFolderPath));

                ZipHelper.CompressFilesToZip(_paths.ToArray(), _targetPath);

                return true;
            }
            catch (System.Exception _e)
            {
                Debug.LogError("[ProjectHelper.SaveProject]: " + _e.ToString());
                return false;
            }
        }
    }

    public class ResourceHelper
    {
        public static bool OpenImageResource(string _path, out ImageResource _resource)
        {
            _resource = null;

            if (string.IsNullOrEmpty(ProjectManager.GUID)) return false;
            if (_path == null || !File.Exists(_path)) return false;

            try
            {
                var _texture = ImageHelper.LoadImage(_path);
                var _guid = System.Guid.NewGuid().ToString("D");

                var newImageResource = new ImageResource(_guid, _path.GetFileNameWithoutExtend(), _texture);

                _resource = newImageResource;
                return true;
            }
            catch (System.Exception _e)
            {
                Debug.LogError("[ResourceHelper.OpenImageResource]: " + _e.ToString());
                return false;
            }
        }

        public static bool SaveImageResource(ImageResource _resource, string _directoryPath = null)
        {
            if (_directoryPath == null) _directoryPath = Path.Combine(ProjectManager.ProjectFolderPath, $"resources/image");
            if (!Directory.Exists(_directoryPath)) Directory.CreateDirectory(_directoryPath);

            try
            {
                var _imagePath = Path.Combine(_directoryPath, $"{_resource.Guid}.png");
                ImageHelper.SaveImage(_resource.texture, _imagePath);

                return true;
            }
            catch (System.Exception _e)
            {
                Debug.LogError("[ResourceHelper.SaveImageResource]: " + _e.ToString());
                return false;
            }
        }

        public static bool OpenAudioResource(string _path, out AudioResource _resource)
        {
            _resource = null;

            if (string.IsNullOrEmpty(ProjectManager.GUID)) return false;
            if (_path == null || !File.Exists(_path)) return false;

            try
            {
                var _clip = OggVorbis.VorbisPlugin.Load(_path);
                var _guid = System.Guid.NewGuid().ToString("D");

                var newAudioResource = new AudioResource(_guid, _path.GetFileNameWithoutExtend(), _clip);

                _resource = newAudioResource;
                return true;
            }
            catch (System.Exception _e)
            {
                Debug.LogError("[ResourceHelper.OpenAudioResource]: " + _e.ToString());
                return false;
            }
        }

        public static bool SaveAudioResource(AudioResource _resource, string _directoryPath = null)
        {
            if (_directoryPath == null) _directoryPath = Path.Combine(ProjectManager.ProjectFolderPath, $"resources/audio");
            if (!Directory.Exists(_directoryPath)) Directory.CreateDirectory(_directoryPath);

            try
            {
                var _audioPath = Path.Combine(_directoryPath, $"{_resource.Guid}.ogg");

                if (File.Exists(_audioPath)) File.Delete(_audioPath);
                OggVorbis.VorbisPlugin.Save(_audioPath, _resource.audioClip, 1);

                return true;
            }
            catch (System.Exception _e)
            {
                Debug.LogError("[ResourceHelper.SaveAudioResource]: " + _e.ToString());
                return false;
            }
        }
    }

    public class SerializationHelper
    {
        public static void DeSerializeProject(string _guid)
        {
            var path = Path.Combine(Application.temporaryCachePath, _guid);

            if (!Directory.Exists(path)) return;

            try
            {
                DeSerializeProjectData(Path.Combine(path, "base.yaml"));
                DeSerializeMeta(Path.Combine(path, "meta.yaml"));
                DeSerializeResources(Path.Combine(path, "resources"));

                ProjectManager.GUID = _guid;
            }
            catch (System.Exception _e)
            {
                Debug.LogError("[SerializationHelper.DeSerializeProject]: " + _e.ToString());
                return;
            }
        }

        public static void SerializeProject()
        {
            if (!ProjectManager.IsProjectExist()) return;

            try
            {
                ResourceManager.SaveAllResources();

                SerializeMeta();
                SerializeProjectData();
            }
            catch (System.Exception _e)
            {
                Debug.LogError("[SerializationHelper.SerializeProject]: " + _e.ToString());
                return;
            }
        }

        public static void SerializeProjectData(string _path = null)
        {
            _path ??= Path.Combine(ProjectManager.ProjectFolderPath, "base.yaml");

            if (File.Exists(_path)) File.Delete(_path);
            File.Create(_path).Dispose();

            try
            {
                var serializer = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .WithTagMapping("!text", typeof(TextData))
                    .WithIndentedSequences()
                    .Build();
                
                var yaml = serializer.Serialize(ProjectManager.Project);

                File.WriteAllText(_path, yaml);
            }
            catch (System.Exception _e)
            {
                Debug.LogError("[SerializationHelper.SerializeProjectData]: " + _e.ToString());
                return;
            }
        }

        public static void DeSerializeProjectData(string _path = null)
        {
            _path ??= Path.Combine(ProjectManager.ProjectFolderPath, "base.yaml");

            if (!File.Exists(_path)) return;

            try
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .WithTagMapping("!text", typeof(TextData))
                    .IgnoreUnmatchedProperties()
                    .Build();
                
                var yaml = File.ReadAllText(_path);

                ProjectManager.InitializeProject(deserializer.Deserialize<ProjectData>(yaml));
            }
            catch (YamlException _e)
            {
                Debug.LogError("[SerializationHelper.DeSerializeProjectData]: " + $" {_e.Message}\n{_e.InnerException.Message}");
                return;
            }
        }

        public static void SerializeMeta(string _path = null)
        {
            _path ??= Path.Combine(ProjectManager.ProjectFolderPath, "meta.yaml");

            if (File.Exists(_path)) File.Delete(_path);
            File.Create(_path).Dispose();

            try
            {
                var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).WithIndentedSequences().Build();
                var yaml = serializer.Serialize(ResourceManager.Instance);

                File.WriteAllText(_path, yaml);
            }
            catch (System.Exception _e)
            {
                Debug.LogError("[SerializationHelper.SerializeMeta]: " + _e.ToString());
                return;
            }
        }

        public static void DeSerializeMeta(string _path = null)
        {
            _path ??= Path.Combine(ProjectManager.ProjectFolderPath, "meta.yaml");

            if (!File.Exists(_path)) return;

            try
            {
                var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).IgnoreUnmatchedProperties().Build();
                var yaml = File.ReadAllText(_path);

                ResourceManager.Instance = deserializer.Deserialize<ResourceManager>(yaml);
            }
            catch (YamlException _e)
            {
                Debug.LogError("[SerializationHelper.DeSerializeMeta]: " + $" {_e.Message}\n{_e.InnerException.Message}");
                return;
            }
        }

        public static void DeSerializeImageResources(string _path)
        {
            if (!Directory.Exists(_path) || Directory.GetFiles(_path).Length == 0) return;
            var _files = Directory.GetFiles(_path);

            foreach (var _file in _files)
            {
                var _guid = _file.GetFileNameWithoutExtend();
                var _texture = ImageHelper.LoadImage(_file);

                var _imageResource = new ImageResource(_guid, _guid, _texture);

                ResourceManager.AddResource(_imageResource);
            }
        }

        public static void DeSerializeAudioResources(string _path)
        {
            if (!Directory.Exists(_path) || Directory.GetFiles(_path).Length == 0) return;
            var _files = Directory.GetFiles(_path);

            foreach (var _file in _files)
            {
                var _guid = _file.GetFileNameWithoutExtend();
                var _clip = OggVorbis.VorbisPlugin.Load(_file);

                var _audioResource = new AudioResource(_guid, _guid, _clip);

                ResourceManager.AddResource(_audioResource);
            }
        }

        public static void DeSerializeResources(string _path)
        {
            if (!Directory.Exists(_path)) return;

            var _directories = Directory.GetDirectories(_path);

            foreach (var _directory in _directories)
            {
                switch (_directory.GetFileNameWithoutExtend())
                {
                    case "image":
                        DeSerializeImageResources(_directory);
                        break;
                    case "audio":
                        DeSerializeAudioResources(_directory);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public class ZipHelper
    {
        public static string UnCompress(string _filePathName, string _password = null)
        {
            var _guid = System.Guid.NewGuid().ToString("D");
            var _outputPath = Path.Combine(Application.temporaryCachePath, _guid);

            if (string.IsNullOrEmpty(_filePathName) || string.IsNullOrEmpty(_outputPath)) return null;
    
            try
            {
                return UnCompressFromZip(_filePathName, _outputPath, _password) ? _guid : null;
            }
            catch (System.Exception _e)
            {
                Debug.LogError("[ZipHelper.UnCompress]: " + _e.ToString());
    
                return null;
            }
        }

        public static bool CompressDirectoryToZip(string _srcPath, string _targetPath, int _level = 5, string _password = null)
        {
            if (!File.Exists(_srcPath) && !Directory.Exists(_srcPath)) return false;

            try
            {
                ZipOutputStream zipOutputStream = new ZipOutputStream(File.Create(_targetPath));
                if (!string.IsNullOrEmpty(_password))
                {
                    zipOutputStream.Password = _password;
                }
                zipOutputStream.SetLevel(_level);

                string _baseDirName = "";
                _srcPath = FormatPath(_srcPath);
                if (IsRoot(_srcPath))
                {
                    _baseDirName = _srcPath;
                }
                else
                {
                    _baseDirName = FormatPath(Path.GetDirectoryName(_srcPath)) + "/";
                }
                AddZipEntry(_srcPath, zipOutputStream, _baseDirName);
                
                zipOutputStream.Finish();
                zipOutputStream.Close();

                return true;
            }
            catch (System.Exception _e)
            {
                Debug.LogError("[ZipHelper.CompressToZip]: " + _e.ToString());
    
                return false;
            }
        }

        public static void CompressFilesToZip(string[] _srcPaths, string _targetPath, int _level = 5, string _password = null)
        {
            if (_srcPaths == null) return;

            foreach (var _srcPath in _srcPaths)
            {
                if (!File.Exists(_srcPath) && !Directory.Exists(_srcPath)) return;
            }

            ZipOutputStream zipOutputStream = new ZipOutputStream(File.Create(_targetPath));
            if (!string.IsNullOrEmpty(_password))
            {
                zipOutputStream.Password = _password;
            }
            zipOutputStream.SetLevel(_level);

            for(int i = 0;i < _srcPaths.Length;++i)
            {
                string srcPath = _srcPaths[i];
                string _baseDirName = "";
                
                srcPath = FormatPath(srcPath);
                if (IsRoot(srcPath))
                {
                    _baseDirName = srcPath;
                }
                else
                {
                    _baseDirName = FormatPath(Path.GetDirectoryName(srcPath)) + "/";
                }
                AddZipEntry(srcPath, zipOutputStream, _baseDirName);
            }

            zipOutputStream.Finish();
            zipOutputStream.Close();
        }

        private static void AddZipEntry(string _srcPath, ZipOutputStream _zipOutputStream, string _baseDirName)
        {
            if (Directory.Exists(_srcPath))
            {
                string[] _paths = Directory.GetFileSystemEntries(_srcPath);
                if (_paths != null)
                {
                    foreach (var _e in _paths)
                    {
                        AddZipEntry(FormatPath(_e), _zipOutputStream, _baseDirName);
                    }
                }
            }
            else if (File.Exists(_srcPath))
            {
                ZipEntry zipEntry = new ZipEntry(_srcPath.Replace(_baseDirName, ""));
                zipEntry.IsUnicodeText = true;
                _zipOutputStream.PutNextEntry(zipEntry);
                FileInfo fileInfo = new FileInfo(_srcPath);
                using (FileStream fileStream = fileInfo.OpenRead())
                {
                    byte[] buffer = new byte[1024 * 1024];
                    int len = 0;
                    while ((len = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        _zipOutputStream.Write(buffer, 0, len);
                    }
                    fileStream.Dispose();
                }
            }
        }

        private static bool IsRoot(string _path)
        {
            if (string.IsNullOrEmpty(_path)) return false;

            _path = FormatPath(_path);
            if (FormatPath(Path.GetPathRoot(_path)).Equals(_path)) return true;
            
            return false;
        }

        private static string FormatPath(string _srcPath)
        {
            _srcPath = Path.GetFullPath(_srcPath);
            _srcPath = _srcPath.Replace(@"\", "/");

            while (_srcPath.Length > 1)
            {
                if ('/'.Equals(_srcPath[0]) && '/'.Equals(_srcPath[1]))
                {
                    _srcPath = _srcPath.Remove(0, 1);
                }
                else
                {
                    break;
                }
            }

            if (!Path.GetPathRoot(_srcPath).Replace(@"\", "/").Equals(_srcPath))
            {
                if (_srcPath.LastIndexOf("/") == _srcPath.Length - 1)
                {
                    _srcPath = _srcPath.Remove(_srcPath.Length - 1);
                }
            }

            return _srcPath;
        }

        public static bool UnCompressFromZip(string _srcZipFilePath, string _targetDirPath, string _password = "")
        {
            if (!Directory.Exists(_targetDirPath)) Directory.CreateDirectory(_targetDirPath);

            try
            {
                using (ZipInputStream zipInputStream = new ZipInputStream(File.OpenRead(_srcZipFilePath)))
                {
                    if (!string.IsNullOrEmpty(_password))
                    {
                        zipInputStream.Password = _password;
                    }

                    ZipEntry entry;
                    while ((entry = zipInputStream.GetNextEntry()) != null)
                    {
                        string filePath = Path.Combine(_targetDirPath, entry.Name);
                        string fileParentPath = Path.GetDirectoryName(filePath);
                        if (!Directory.Exists(fileParentPath))
                        {
                            Directory.CreateDirectory(fileParentPath);
                        }
                        using (FileStream fileStream = File.Create(filePath))
                        {
                            byte[] buffer = new byte[1024*1024];
                            int len = 0;
                            while ((len = zipInputStream.Read(buffer, 0, buffer.Length))>0)
                            {
                                fileStream.Write(buffer, 0, len);
                            }
                        }
                    }
                }

                return true;
            }
            catch (System.Exception _e)
            {
                Debug.LogError("[ZipHelper.UnCompressFromZip]: " + _e.ToString());
    
                return false;
            }
        }

    }

    public class XmlHelper
    {

    }

    public class ImageHelper
    {
        public static Texture2D LoadImage(string _path)
        {
            if (string.IsNullOrEmpty(_path) || !File.Exists(_path)) return null;

            try
            {
                byte[] _bytes = File.ReadAllBytes(_path);
                Texture2D _texture = new Texture2D(1, 1);
                _texture.LoadImage(_bytes);

                return _texture;
            }
            catch (System.Exception _e)
            {
                Debug.LogError("[ImageHelper.LoadImage]: " + _e.ToString());
                return null;
            }
        }

        public static void SaveImage(Texture2D _texture, string _path)
        {
            if (_texture == null || string.IsNullOrEmpty(_path)) return;
            if (File.Exists(_path)) File.Delete(_path);

            try
            {
                byte[] _bytes = _texture.EncodeToPNG();
                File.WriteAllBytes(_path, _bytes);
            }
            catch (System.Exception _e)
            {
                Debug.LogError("[ImageHelper.SaveImage]: " + _e.ToString());
            }
        }
    }
}