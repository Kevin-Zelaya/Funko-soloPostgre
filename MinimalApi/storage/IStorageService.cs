using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using MinimalApi.Errors;

namespace MinimalApi.Storage;

public interface IStorageService
{
    Task<Result<string, DomainError>> SaveFileAsync(IFormFile file, string folder);
    Task<Result<bool, DomainError>> DeleteFileAsync(string filename);
    bool FileExists(string filename);
    string GetFullPath(string filename);
    string GetRelativePath(string filename, string folder = "funkos");
}