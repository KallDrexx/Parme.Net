﻿using System.Numerics;

namespace Parme.Net.Frb;

public class ParticleCamera
{
    public Vector2 Origin { get; set; }
    public int PixelWidth { get; set; }
    public int PixelHeight { get; set; }
    public float HorizontalZoomFactor { get; set; } = 1f;
    public float VerticalZoomFactor { get; set; } = 1f;
}