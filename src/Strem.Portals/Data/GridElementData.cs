﻿using System.ComponentModel.DataAnnotations;
using Strem.Portals.Types;

namespace Strem.Portals.Data;

public class GridElementData
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int GridIndex { get; set; }
    public GridElementType ElementType { get; set; }

    [Required] 
    public string Name { get; set; } = string.Empty;
    public ElementStyles DefaultStyles { get; set; } = new ();

    public Dictionary<string, string> Values { get; set; } = new();
}