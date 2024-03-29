using System.Collections;
using System.Collections.Generic;
using Schwarzer.Windows;
using UnityEngine;
using UnityEditor;
using System.IO;
using QFramework;
using ICSharpCode.SharpZipLib.Zip;
using Larvend;
using UnityEditor.iOS;
using System.Linq;

namespace Serialization
{
    public class ProjectHelper
    {
        /// <summary>
        /// When you are trying to create a project:
        /// Create a guid -> Create temporary folder
        /// </summary>
        public static void NewProject()
        {
            Global.CurrentGUID = System.Guid.NewGuid().ToString("D");
            Directory.CreateDirectory(Global.CurrentProjectPath);
        }

        /// <summary>
        /// When you are trying to open a project:
        /// Select a path -> Compress the project file into temp folder -> Deserialize the project
        /// </summary>
        public static void OpenProject()
        {
            var _path = Dialog.OpenFileDialog(Title: "Select File", InitPath: Application.dataPath, Filter: "lpf");
            if (_path == null || !File.Exists(_path)) return;

            try
            {
                var _guid = ZipHelper.UnCompress(_path);
                if (string.IsNullOrEmpty(_guid)) return;

                SerializationHelper.DeSerializeProject(_guid);
            
                EditorUtility.RevealInFinder(Path.Combine(Application.temporaryCachePath, _guid));
            }
            catch (System.Exception _e)
            {
                Debug.LogError("[ProjectHelper.OpenProject]: " + _e.ToString());
                return;
            }
        }

        /// <summary>
        /// When you are trying to save a project:
        /// Select a path -> Serialize the project -> Compress the serialized project folder -> Save to target path
        /// </summary>
        public static void SaveProject()
        {
            var _targetPath = Dialog.SaveFileDialog(Title: "Select Place to Save", InitPath: Application.dataPath, Filter: "lpf");
            if (string.IsNullOrEmpty(_targetPath)) return;

            if (File.Exists(_targetPath)) File.Delete(_targetPath);

            try
            {
                SerializationHelper.SerializeProject();

                List<string> _paths = new List<string>();
                _paths.AddRange(Directory.GetFiles(Global.CurrentProjectPath));
                _paths.AddRange(Directory.GetDirectories(Global.CurrentProjectPath));

                ZipHelper.CompressFilesToZip(_paths.ToArray(), _targetPath);
            }
            catch (System.Exception _e)
            {
                Debug.LogError("[ProjectHelper.SaveProject]: " + _e.ToString());
                return;
            }
        }
    }

    public class ResourceHelper
    {
        public static bool OpenImageResource(string _path, out ImageResource _resource)
        {
            _resource = null;

            if (Global.CurrentGUID == null) return false;
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
            if (_directoryPath == null) _directoryPath = Path.Combine(Global.CurrentProjectPath, $"resources/image");
            if (!Directory.Exists(_directoryPath)) Directory.CreateDirectory(_directoryPath);

            try
            {
                var _imagePath = Path.Combine(_directoryPath, $"{_resource.guid}.png");
                ImageHelper.SaveImage(_resource.texture, _imagePath);

                return true;
            }
            catch (System.Exception _e)
            {
                Debug.LogError("[ResourceHelper.SaveImageResource]: " + _e.ToString());
                return false;
            }
        }
    }

    public class SerializationHelper
    {
        public static void DeSerializeProject(string _guid)
        {
            var _path = Path.Combine(Application.temporaryCachePath, _guid);

            if (!Directory.Exists(_path)) return;

            try
            {
                DeSerializeResources(Path.Combine(_path, "resources"));

                Global.CurrentGUID = _guid;
            }
            catch (System.Exception _e)
            {
                Debug.LogError("[SerializationHelper.DeSerializeProject]: " + _e.ToString());
                return;
            }
        }

        public static void SerializeProject()
        {
            if (string.IsNullOrEmpty(Global.CurrentGUID)) return;

            try
            {
                ResourceManager.SaveAllResources();
            }
            catch (System.Exception _e)
            {
                Debug.LogError("[SerializationHelper.SerializeProject]: " + _e.ToString());
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
                return UnzipFile(File.OpenRead(_filePathName), _outputPath, _password) ? _guid : null;
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

        private static bool UnzipFile(Stream _inputStream, string _outputPath, string _password = null)
        {
            if ((null == _inputStream) || string.IsNullOrEmpty(_outputPath)) return false;

            if (!Directory.Exists(_outputPath))
                Directory.CreateDirectory(_outputPath);

            ZipEntry entry = null;
            using (ZipInputStream zipInputStream = new ZipInputStream(_inputStream))
            {
                if (!string.IsNullOrEmpty(_password))
                    zipInputStream.Password = _password;
    
                while (null != (entry = zipInputStream.GetNextEntry()))
                {
                    if (string.IsNullOrEmpty(entry.Name))
                        continue;
    
                    string filePathName = Path.Combine(_outputPath, entry.Name);

                    if (entry.IsDirectory)
                    {
                        Directory.CreateDirectory(filePathName);
                        continue;
                    }

                    try
                    {
                        using (FileStream fileStream = File.Create(filePathName))
                        {
                            byte[] bytes = new byte[1024];
                            while (true)
                            {
                                int count = zipInputStream.Read(bytes, 0, bytes.Length);
                                if (count > 0)
                                    fileStream.Write(bytes, 0, count);
                                else break;
                            }
                        }
                    }
                    catch (System.Exception _e)
                    {
                        Debug.LogError("[ZipUtility.UnzipFile]: " + _e.ToString());

                        return false;
                    }
                }
            }
    
            return true;
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