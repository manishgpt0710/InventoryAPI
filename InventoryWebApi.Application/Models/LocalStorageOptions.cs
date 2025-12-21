using System;
using System.Collections.Generic;

namespace InventoryWebApi.Application.Models;

public sealed class LocalStorageOptions
{
    public string RootPath { get; init; } = default!;
    public string PublicBaseUrl { get; init; } = default!;
    public int MaxFileSizeMB { get; init; } = 5;
    public IReadOnlyList<string> AllowedFileTypes { get; init; } = new List<string>
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };
}
