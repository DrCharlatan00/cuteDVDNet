using cuteDVDCore.Exceptions;
using cuteDVDCore.Models;
using cuteDVDCore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace cuteDVDCore.Services
{
    public class WinDriveFileService : IDriveFileService
    {
        public DriveInfo drive;
        public WinDriveFileService()
        {
            if (OperatingSystem.IsWindows())
            {
                DriveInfo[] driveInfo = DriveInfo.GetDrives();

                foreach (var drive in driveInfo)
                {
                    if (drive.DriveType == DriveType.CDRom)
                    {
                        this.drive = drive;
                    }
                }
            }
        }

        public void CheckDisk()
        {

            DriveInfo[] driveInfo = DriveInfo.GetDrives();

            foreach (var drive in driveInfo)
            {
                if (drive.DriveType == DriveType.CDRom)
                {

                    if (!drive.IsReady)
                    {
                        return;
                    }
                    throw new ExceptionDriveNotReady($"{nameof(CheckDisk)} not found disk or disk not ready");
                }

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

        public async IAsyncEnumerable<ModelDVDFiles>? GetListFiles()
        {
            foreach (var file in Directory.EnumerateFiles(
       drive.Name,
       "*",
       SearchOption.AllDirectories))
            {
                var info = new FileInfo(file);

                yield return new ModelDVDFiles(
                    info.Name,
                    info.Extension,
                    FormatSize(info.Length),
                    info.Directory?.FullName);

                await Task.Yield();
            }

        }

        public async Task<Stream?> GetFileStream(ModelDVDFiles fileModel)
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

            await foreach (var file in GetListFiles())
            {
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




    }
}
