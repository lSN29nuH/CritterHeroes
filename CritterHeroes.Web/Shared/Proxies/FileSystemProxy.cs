﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CritterHeroes.Web.Domain.Contracts;

namespace CritterHeroes.Web.Shared.Proxies
{
    public class FileSystemProxy : IFileSystem
    {
        private IHttpContext _httpContext;

        public FileSystemProxy(IHttpContext httpContext)
        {
            this._httpContext = httpContext;
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public string CombinePath(params string[] paths)
        {
            return Path.Combine(paths);
        }

        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public string GetFileExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public string MapServerPath(string path)
        {
            return _httpContext.Server.MapPath($"~/{path}");
        }
    }
}
