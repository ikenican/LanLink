# Renders LAN_Tool_Apple_Flat.svg into app.ico. Rerun only when the SVG changes.
# Handles the SVG subset the icon uses: rect and path with fill/stroke.
Add-Type -AssemblyName PresentationCore, WindowsBase

$svg = [xml](Get-Content -Raw -Encoding UTF8 (Join-Path $PSScriptRoot 'LAN_Tool_Apple_Flat.svg'))
$brushes = New-Object Windows.Media.BrushConverter
$group = New-Object Windows.Media.DrawingGroup

foreach ($n in $svg.svg.ChildNodes) {
    if ($n.LocalName -eq 'rect') {
        $rect = New-Object Windows.Rect ([double]$n.GetAttribute('x')), ([double]$n.GetAttribute('y')), ([double]$n.width), ([double]$n.height)
        $rx = [double]$n.GetAttribute('rx')
        $geometry = New-Object Windows.Media.RectangleGeometry $rect, $rx, $rx
    } elseif ($n.LocalName -eq 'path') {
        $geometry = [Windows.Media.Geometry]::Parse($n.d)
    } else { continue }

    $fill = $null
    if ($n.GetAttribute('fill') -ne 'none') { $fill = $brushes.ConvertFromString($n.GetAttribute('fill')) }
    $pen = $null
    if ($n.GetAttribute('stroke')) {
        $pen = New-Object Windows.Media.Pen $brushes.ConvertFromString($n.GetAttribute('stroke')), ([double]$n.GetAttribute('stroke-width'))
        $pen.StartLineCap = $pen.EndLineCap = $pen.LineJoin = 'Round'
    }
    $group.Children.Add((New-Object Windows.Media.GeometryDrawing $fill, $pen, $geometry))
}

$images = foreach ($size in 16, 20, 24, 32, 40, 48, 64, 256) {
    $visual = New-Object Windows.Media.DrawingVisual
    $dc = $visual.RenderOpen()
    $dc.PushTransform((New-Object Windows.Media.ScaleTransform ($size / 1024.0), ($size / 1024.0)))
    $dc.DrawDrawing($group)
    $dc.Close()
    $bitmap = New-Object Windows.Media.Imaging.RenderTargetBitmap $size, $size, 96, 96, ([Windows.Media.PixelFormats]::Pbgra32)
    $bitmap.Render($visual)
    $encoder = New-Object Windows.Media.Imaging.PngBitmapEncoder
    $encoder.Frames.Add([Windows.Media.Imaging.BitmapFrame]::Create($bitmap))
    $png = New-Object IO.MemoryStream
    $encoder.Save($png)
    ,@($size, $png.ToArray())
}

$file = [IO.File]::Create((Join-Path $PSScriptRoot 'app.ico'))
$w = New-Object IO.BinaryWriter $file
$w.Write([uint16]0); $w.Write([uint16]1); $w.Write([uint16]$images.Count)
$offset = 6 + 16 * $images.Count
foreach ($img in $images) {
    $side = if ($img[0] -eq 256) { 0 } else { $img[0] }   # 0 means 256 in the ICO format
    $w.Write([byte]$side); $w.Write([byte]$side); $w.Write([byte]0); $w.Write([byte]0)
    $w.Write([uint16]1); $w.Write([uint16]32)
    $w.Write([int]$img[1].Length); $w.Write([int]$offset)
    $offset += $img[1].Length
}
foreach ($img in $images) { $w.Write([byte[]]$img[1]) }
$w.Close()
