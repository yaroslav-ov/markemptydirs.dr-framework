//  
//  Copyright (c) 2009 by Johann Duscher (alias Jonny Dee)
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace DR.IO
{
    namespace Windows
    {
    	public static class SymbolicLinkHelper
        {
            private const UInt32 SymbolicLinkFlagFile = 0;
            private const UInt32 SymbolicLinkFlagDirectory = 1;
    
            [DllImport("kernel32.dll",SetLastError = true)]
            [return: MarshalAs(UnmanagedType.I1)]
            private static extern bool CreateSymbolicLink(string symlinkFileName, string targetFileName, UInt32 flags);
    
            public static bool CreateSymbolicLink(DirectoryInfo targetDirInfo, FileInfo symlinkFileInfo)
            {
                return CreateSymbolicLink(symlinkFileInfo.FullName, targetDirInfo.FullName, SymbolicLinkFlagDirectory);
            }
    
            public static bool CreateSymbolicLink(FileInfo targetFileInfo, FileInfo symlinkFileInfo)
            {
                return CreateSymbolicLink(symlinkFileInfo.FullName, targetFileInfo.FullName, SymbolicLinkFlagFile);
            }

            public static bool IsSymbolicLink(FileSystemInfo symlinkFileSystemInfo)
            {
                // TODO Implement method.
                throw new NotImplementedException("bool IsSymbolicLink(FileSystemInfo symlinkFileSystemInfo)");
            }
    
            public static Uri GetSymbolicLinkTarget(FileSystemInfo symlinkFileSystemInfo)
            {
                // TODO Implement method.
                throw new NotImplementedException("Uri GetSymbolicLinkTarget(FileSystemInfo symlinkFileSystemInfo)");
            }
        }
    }

    namespace Unix
    {
        public static class SymbolicLinkHelper
        {
            private const string MonoPosixAssemblyName = "Mono.Posix, Version=2.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756";
            private const string UnixDirectoryInfoTypeName = "Mono.Unix.UnixDirectoryInfo";
            private const string UnixFileInfoTypeName = "Mono.Unix.UnixFileInfo";
            private const string UnixSymbolicLinkInfoTypeName = "Mono.Unix.UnixSymbolicLinkInfo";
            private const string CreateSymbolicLinkMethodName = "CreateSymbolicLink";
            private const string GetContentsMethodName = "GetContents";
            private const string FullNamePropertyName = "FullName";
            private const string IsSymbolicLinkPropertyName = "IsSymbolicLink";

            private static Type UnixDirectoryInfoType;
            private static Type UnixFileInfoType;
            private static Type UnixSymbolicLinkInfoType;
        	private static bool _initialized = false;

            private static void Init()
            {
				if (_initialized)
					return;

				var assemblyRef = new AssemblyName { Name = MonoPosixAssemblyName };
                var assembly = Assembly.Load(assemblyRef);
                UnixFileInfoType = assembly.GetType(UnixFileInfoTypeName);
                UnixDirectoryInfoType = assembly.GetType(UnixDirectoryInfoTypeName);
                UnixSymbolicLinkInfoType = assembly.GetType(UnixSymbolicLinkInfoTypeName);
            	_initialized = true;
            }
            
            private static void CreateSymbolicLink(Type unixFileSystemInfoType, string targetPath, string symlinkPath)
            {
                var ufi = Activator.CreateInstance(unixFileSystemInfoType, targetPath);
                var method = unixFileSystemInfoType.GetMethod(CreateSymbolicLinkMethodName);
                method.Invoke(ufi, new[] { symlinkPath });
            }
            
            public static bool CreateSymbolicLink(DirectoryInfo targetDirInfo, FileInfo symlinkFileInfo)
            {
                try
                {
                    Init();
                    CreateSymbolicLink(UnixDirectoryInfoType, targetDirInfo.FullName, symlinkFileInfo.FullName);
                    return true;
                }
                catch (Exception ex)
                {
					System.Diagnostics.Debug.WriteLine(ex);
                    return false;
                }
            }
    
            public static bool CreateSymbolicLink(FileInfo targetFileInfo, FileInfo symlinkFileInfo)
            {
                try
                {
                    Init();
                    CreateSymbolicLink(UnixFileInfoType, targetFileInfo.FullName, symlinkFileInfo.FullName);
                    return true;
                }
                catch (Exception ex)
                {
					System.Diagnostics.Debug.WriteLine(ex);
                    return false;
                }
            }

            public static bool IsSymbolicLink(FileSystemInfo symlinkFileSystemInfo)
            {
                Init();
                var usli = Activator.CreateInstance(UnixSymbolicLinkInfoType, symlinkFileSystemInfo.FullName);
                var property = UnixSymbolicLinkInfoType.GetProperty(IsSymbolicLinkPropertyName);
                return (bool)property.GetValue(usli, new object[0]);
            }
    
            public static Uri GetSymbolicLinkTarget(FileSystemInfo symlinkFileSystemInfo)
            {
                Init();
                var usli = Activator.CreateInstance(UnixSymbolicLinkInfoType, symlinkFileSystemInfo.FullName);
                var method = UnixSymbolicLinkInfoType.GetMethod(GetContentsMethodName);
                var ufsi = method.Invoke(usli, new object[0]);
				var property = (PropertyInfo)UnixSymbolicLinkInfoType.GetProperty(FullNamePropertyName);
				return new Uri((string)property.GetValue(ufsi, new object[0]));
            }
        }
    }

    public static class SymbolicLinkHelper
    {
        public static bool CreateSymbolicLink(FileInfo targetFileInfo, FileInfo symlinkFileInfo)
        {
            try
            {
                return Windows.SymbolicLinkHelper.CreateSymbolicLink(targetFileInfo, symlinkFileInfo);
            }
            catch (Exception ex)
            {
				System.Diagnostics.Debug.WriteLine(ex);
            }

			try
			{
				return Unix.SymbolicLinkHelper.CreateSymbolicLink(targetFileInfo, symlinkFileInfo);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}

        	return false;
        }

        public static bool CreateSymbolicLink(DirectoryInfo targetDirInfo, FileInfo symlinkFileInfo)
        {
            try
            {
                return Windows.SymbolicLinkHelper.CreateSymbolicLink(targetDirInfo, symlinkFileInfo);
            }
            catch (Exception ex)
            {
				System.Diagnostics.Debug.WriteLine(ex);
			}

			try
			{
				return Unix.SymbolicLinkHelper.CreateSymbolicLink(targetDirInfo, symlinkFileInfo);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}

			return false;
		}

        public static bool IsSymbolicLink(FileSystemInfo symlinkFileSystemInfo)
        {
            try
            {
                return Windows.SymbolicLinkHelper.IsSymbolicLink(symlinkFileSystemInfo);
            }
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}

			try
			{
				return Unix.SymbolicLinkHelper.IsSymbolicLink(symlinkFileSystemInfo);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}

			return false;
		}

        public static Uri GetSymbolicLinkTarget(FileSystemInfo symlinkFileSystemInfo)
        {
            try
            {
                return Windows.SymbolicLinkHelper.GetSymbolicLinkTarget(symlinkFileSystemInfo);
            }
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}

			try
			{
				return Unix.SymbolicLinkHelper.GetSymbolicLinkTarget(symlinkFileSystemInfo);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}

			return null;
		}
    }
}