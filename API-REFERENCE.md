# VectraMesh – API Reference Guide

**Namespace:** `VectraMesh`  
**Target Frameworks:** `.NET Standard 2.0`, `.NET 8.0+`  
**Math Primitive:** `System.Numerics.Vector2`

---

## Table of Contents
1. [VmEngine](#vmengine)
2. [VmPoint](#vmpoint)
3. [VmSegment](#vmsegment)
4. [VmEdge](#vmedge)
5. [VmTriangle](#vmtriangle)
6. [IVmPresenter](#ivmpresenter)

---

## `VmEngine`

The central coordinator managing points, segments, topology validation (`RecogniseEdges`), and triangulation (`TriangulateEdges`).

### Properties

| Property | Type | Description |
| :--- | :--- | :--- |
| `Presenter` | `IVmPresenter` | Optional UI presenter hook (`null` by default for headless operation). |
| `AllPoints` | `IReadOnlyList<VmPoint>` | All active points currently registered in the engine. |
| `AllSegments` | `IReadOnlyList<VmSegment>` | All active segments currently registered in the engine. |
| `ErrorSegments` | `IReadOnlyList<VmSegment>` | Segments flagged as invalid during the last `RecogniseEdges` pass. |
| `AllEdges` | `IReadOnlyList<VmEdge>` | Closed polygon contours discovered by `RecogniseEdges`, including nested hole edges. |
| `Message` | `string` | Diagnostic error message if `RecogniseEdges` failed (`string.Empty` when valid). |
| `RecognitionInfo` | `string` | Timing summary of the last successful edge recognition pass. |
| `TriangulationInfo` | `string` | Timing and triangle count summary of the last triangulation pass. |

### Events

| Event | Signature | Description |
| :--- | :--- | :--- |
| `EventSegmentsExist` | `Action<bool>` | Raised whenever segments are added or erased, indicating whether `AllSegments.Count > 0`. |

### Geometry Construction & Hit-Testing Methods
* **`VmPoint AddPoint(Vector2 position)`** / **`VmPoint AddPoint(float x, float y)`**  
  Creates and registers a new vertex at the given 2D coordinates.
* **`VmSegment AddSegment(VmPoint startPoint, VmPoint endPoint)`**  
  Connects two existing `VmPoint` instances with a segment and updates segment existence state.
* **`List<VmPoint> CheckPointAtPosition(Vector2 v)`** / **`List<VmPoint> CheckPointAtPosition(float x, float y)`**  
  Returns all points within the engine's snap/hit-test radius of the specified coordinates.
* **`VmSegment CheckSegmentAtPosition(Vector2 v)`** / **`VmSegment CheckSegmentAtPosition(float x, float y)`**  
  Returns the first segment intersecting the hit-test tolerance at the specified coordinates, or `null` if none match.

### Topology Analysis & Triangulation Methods
* **`void RecogniseEdges(bool nestedHolesProhibited = false)`**  
  Validates all segments in batch mode:
  1. Checks for duplicate segments (`"There are doubled segments!"`).
  2. Checks for corner and side intersections (`"There are intersections!"`).
  3. Checks for open polylines / dangling endpoints (`"There are unclosed polygons!"`).
  4. Collects closed loops into `AllEdges`, computes bounding boxes, and assigns `HoleLevel` nesting depths.
  5. If `nestedHolesProhibited` is `true`, flags holes with `HoleLevel > 1` (`"There are nested holes!"`).
* **`void TriangulateEdges(bool withoutGaps = false)`**  
  Triangulates recognized edges into `VmEdge.Triangles` and calls `IVmPresenter.TriangleShow` if a `Presenter` is attached.
  * When `withoutGaps` is `false` (default), only even `HoleLevel` contours (`0`, `2`, `4`...) are filled while odd levels (`1`, `3`...) remain cutouts.
  * When `withoutGaps` is `true`, all recognized contours are triangulated regardless of `HoleLevel`.

### State Cleanup Methods
* **`void ShowErrorSegments()`** – Marks all segments in `ErrorSegments` as error state (`IsError = true`).
* **`void ClearErrorSegments()`** – Resets error flags on `ErrorSegments`, clears the list, and resets `Message`.
* **`void ClearTriangles()`** – Hides and removes generated triangles from all edges while preserving recognized edges.
* **`void ClearEdges()`** – Calls `ClearTriangles()` and clears `AllEdges`.
* **`void ClearAll()`** – Clears edges, error segments, all segments, and all points.

---

## `VmPoint`

Represents a vertex managed by `VmEngine`. Instantiated via `VmEngine.AddPoint(...)`.

### Properties

| Property | Type | Description |
| :--- | :--- | :--- |
| `Position` | `Vector2` | Current 2D coordinates of the point. |
| `Index` | `int` | Current zero-based index of the point in `VmEngine.AllPoints`. |
| `Segments` | `IReadOnlyList<VmSegment>` | All segments currently connected to this point. |
| `PresentationObject` | `object` | Custom UI object returned by `IVmPresenter.PointShow(this)`. |

### Methods
* **`void SetPosition(Vector2 position)`** / **`void SetPosition(float x, float y)`** – Moves the point and updates the direction vectors (`V`) of all connected segments.
* **`void Show()`** – Invokes `IVmPresenter.PointShow(this)` and stores the returned object in `PresentationObject`.
* **`void Hide()`** – Invokes `IVmPresenter.PointHide(this)`.

---

## `VmSegment`

Represents a line segment connecting two `VmPoint` instances. Instantiated via `VmEngine.AddSegment(...)`. Implements `IEquatable<VmSegment>` with direction-independent equality (`(A, B) == (B, A)`).

### Properties

| Property | Type | Description |
| :--- | :--- | :--- |
| `Start` | `VmPoint` | Start vertex of the segment. |
| `End` | `VmPoint` | End vertex of the segment. |
| `S` | `int` | Shorthand for `Start.Index`. |
| `E` | `int` | Shorthand for `End.Index`. |
| `V` | `Vector2` | Direction vector (`End.Position - Start.Position`). |
| `IsError` | `bool` | `true` if flagged during `VmEngine.RecogniseEdges`. |
| `PresentationObject` | `object` | Custom UI object returned by `IVmPresenter.SegmentShow(this)`. |

### Methods
* **`void Erase()`** – Hides the segment, detaches it from `Start` and `End` (automatically deleting orphaned points with zero remaining segments), and removes it from `VmEngine.AllSegments`.
* **`void Show()`** – Invokes `IVmPresenter.SegmentShow(this)` and stores the result in `PresentationObject`.
* **`void Hide()`** – Invokes `IVmPresenter.SegmentHide(this)`.
* **`void Highlight()`** / **`void UnHighlight()`** – Invokes `IVmPresenter.SegmentHighlight(this)` / `SegmentUnhighlight(this)`.
* **`bool Equals(VmSegment other)`** / **`Equals(object obj)`** / **`GetHashCode()`** – Direction-independent value equality (`==` and `!=` operators supported).

---

## `VmEdge`

Represents a closed, non-self-intersecting polygon boundary discovered by `VmEngine.RecogniseEdges()`.

### Properties

| Property | Type | Description |
| :--- | :--- | :--- |
| `Segments` | `IReadOnlyList<VmSegment>` | Ordered boundary segments forming the closed loop. |
| `Triangles` | `IReadOnlyList<VmTriangle>` | Generated triangles for this contour after `TriangulateEdges()`. |
| `Holes` | `IReadOnlyList<VmEdge>` | Direct child hole contours nested immediately inside this edge. |
| `HoleLevel` | `int` | Nesting depth (`0` = outer boundary, `1` = hole, `2` = island inside hole, etc.). |
| `MinX`, `MinY`, `MaxX`, `MaxY` | `float` | Axis-aligned bounding box limits of the contour. |

---

## `VmTriangle`

Represents a single output triangle (`P0`, `P1`, `P2`) produced by `VmEngine.TriangulateEdges()`.

### Properties

| Property | Type | Description |
| :--- | :--- | :--- |
| `P0`, `P1`, `P2` | `VmPoint` | Corner vertices of the triangle. |
| `IsAnalyzed` | `bool` | `true` after `Analyze()` has been called. |
| `AnalysisValue` | `double` | Area-to-Edge Mean Ratio quality score (`1.0` = equilateral triangle, `0.0` = degenerate line). |
| `PresentationObject` | `object` | Custom UI object returned by `IVmPresenter.TriangleShow(this)`. |

### Methods
* **`void Analyze()`** – Computes `AnalysisValue` (`6.92 * Area / (L0² + L1² + L2²)`), sets `IsAnalyzed = true`, and invokes `IVmPresenter.ShowTriangleAnalyzed(this)`.
* **`void Show()`** – Invokes `IVmPresenter.TriangleShow(this)` and stores the result in `PresentationObject`.
* **`void Hide()`** – Invokes `IVmPresenter.TriangleHide(this)`.

---

## `IVmPresenter`

Optional callback interface assigned to `VmEngine.Presenter` to decouple math operations from UI frameworks (WPF, WinUI, MAUI, Unity, etc.).

```csharp
public interface IVmPresenter
{
    object PointShow(VmPoint point);
    void PointHide(VmPoint point);

    object SegmentShow(VmSegment segment);
    void SegmentHide(VmSegment segment);
    void SegmentHighlight(VmSegment segment);
    void SegmentUnhighlight(VmSegment segment);

    object TriangleShow(VmTriangle triangle);
    void TriangleHide(VmTriangle triangle);
    void ShowTriangleAnalyzed(VmTriangle triangle);
}
```
