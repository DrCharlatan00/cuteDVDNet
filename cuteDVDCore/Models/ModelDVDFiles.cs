using System;
using System.Collections.Generic;
using System.Text;

namespace cuteDVDCore.Models
{
    public record ModelDVDFiles(string Name, string Type, string Size, string? Path = default);
}
