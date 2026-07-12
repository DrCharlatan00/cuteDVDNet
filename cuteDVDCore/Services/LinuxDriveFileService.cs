using cuteDVDCore.Exceptions;
using cuteDVDCore.Models;
using cuteDVDCore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace cuteDVDCore.Services
{
    public class LinuxDriveFileService : IDriveFileService
    {
        public void CheckDisk()
        {
            try
            {
                var mounts = File.ReadAllLines("/proc/mounts");
            }
            catch
            {
                throw new ExceptionDriveNotReady("Drive not ready");
            }
        }

        private static string FormatSize(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB" };

            double size = bytes;
            int unit = 0;

            while (size >= 1024 && unit < units.Length - 1)
            {
                size /= 1024;
                unit++;
            }

            return $"{size:0.##} {units[unit]}";
        }

        public async Task<Stream>? GetFileStream(ModelDVDFiles fileModel)
        {
            try
            {
                CheckDisk();
            }
            catch (ExceptionDriveNotReady ex)
            {
                throw new ExceptionDriveFuncError($"The disk check function {nameof(GetFileStream)} encountered an exception {ex} with the following message: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(GetFileStream)} catch unexpect Exception \n Message: {ex.Message}");
            }

            if (fileModel.Path is null || string.IsNullOrEmpty(fileModel.Path))
            {
                await foreach (var file in GetListFiles())
                {
                    if (file.Name == fileModel.Name) return
                new FileStream(
                    file.Path,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    4096,
                    useAsync: true);
                }
            }
            return new FileStream(
                fileModel.Path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                4096,
                true
                );

            

            

        //    return Task.FromResult<Stream>(
        //new FileStream(
        //    fileModel.Path,
        //    FileMode.Open,
        //    FileAccess.Read,
        //    FileShare.Read,
        //    4096,
        //    useAsync: true));
        }

        public async Task<Stream?> GetFileStream(string fileName)
        {
            try
            {
                CheckDisk();
            }
            catch (ExceptionDriveNotReady ex)
            {
                throw new ExceptionDriveFuncError($"The disk check function {nameof(GetFileStream)} encountered an exception {ex} with the following message: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(GetFileStream)} catch unexpect Exception \n Message: {ex.Message}");
            }
            await foreach (var file in GetListFiles()) {
                if (file.Name == fileName) return
                new FileStream(
                    file.Path,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    4096,
                    useAsync: true);
            }
            return null;
        }

        public async IAsyncEnumerable<ModelDVDFiles>? GetListFiles()
        {
            try
            {
                CheckDisk();
            }
            catch (ExceptionDriveNotReady ex)
            {
                throw new ExceptionDriveFuncError($"The disk check function {nameof(GetListFiles)} encountered an exception {ex} with the following message: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(GetListFiles)} catch unexpect Exception \n Message: {ex.Message}");
            }
            
            var mounts = File.ReadAllLines("/proc/mounts");
            foreach (var line in mounts)
            {

                if (line.StartsWith("/dev/sr"))
                {
                    var filesOnDrive = new List<ModelDVDFiles>();
                    foreach (var file in Directory.EnumerateDirectories(
                        "/dev/sr",
                        "*",
                        SearchOption.AllDirectories)) {
                        var info = new FileInfo(file);

                        yield return new ModelDVDFiles(
                   info.Name,
                   info.Extension,
                   FormatSize(info.Length),
                   info.FullName);
                        await Task.Yield();
                    }
                    

                }
            }
            
        }
    }
}
