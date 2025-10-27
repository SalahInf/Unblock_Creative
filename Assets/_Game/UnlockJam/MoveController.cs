


//using System.Collections.Generic;
//using UnityEngine;
//using DG.Tweening;
//using System.Collections;
//using UnityEngine.UIElements;

//public class MoveController : MonoBehaviour
//{
//    [SerializeField] GridSpowner grid;

//    // Input and timing
//    [SerializeField] float swipeThreshold = 50f;
//    [SerializeField] float moveDuration = 0.25f;
//    [SerializeField] Ease moveEase = Ease.OutQuad;

//    // Direction matching tolerance for cell.dirWhitColor keys
//    // 0.99 ~= ~8 degrees, increase for stricter match
//    [SerializeField] float dirDotTolerance = 0.99f;

//    // VFX on consume
//    [SerializeField] ParticleSystem destroyVfxPrefab;
//    [SerializeField] bool vfxAutoDestroy = true;
//    // Optional delay before the consume animation/VFX (usually 0 for snappy feedback)
//    [SerializeField] float consumeDelayOverride = 0f;

//    // Staggered delay for placed blocks (not destroyed) within a line
//    // delay for nth placed block = n * staggerStep
//    [SerializeField] float staggerStep = 0.04f;

//    private Vector2 startMousePosition;
//    private Vector2 endMousePosition;

//    enum Dir { None, Left, Right, Up, Down }




//    void Update()
//    {
//        if (Input.GetMouseButtonDown(0))
//        {
//            AudioManager.instance.Play("1");
//            startMousePosition = Input.mousePosition;
//        }

//        if (Input.GetMouseButtonUp(0))
//        {
//            endMousePosition = Input.mousePosition;
//            var dir = DetectSwipeDirection(startMousePosition, endMousePosition, swipeThreshold);
//            if (dir != Dir.None)
//                MoveBlocks(dir);
//        }
//        FingerFolower();
//    }

//    static Dir DetectSwipeDirection(Vector2 start, Vector2 end, float minDist)
//    {
//        var d = end - start;
//        if (d.magnitude < minDist) return Dir.None;
//        return Mathf.Abs(d.x) > Mathf.Abs(d.y)
//            ? (d.x > 0 ? Dir.Right : Dir.Left)
//            : (d.y > 0 ? Dir.Up : Dir.Down);
//    }

//    static Vector3 DirToVec(Dir d)
//    {
//        switch (d)
//        {
//            case Dir.Right: return Vector3.right;
//            case Dir.Left: return Vector3.left;
//            case Dir.Up: return Vector3.up;
//            case Dir.Down: return Vector3.down;
//            default: return Vector3.zero;
//        }
//    }

//    // Try to find a direction key in cell.dirWhitColor that matches the swipe direction (by dot),
//    // return the mapped color for that matched direction.
//    Vector3 want = Vector3.zero;
//    bool TryGetColorForDirection(CellSpring cell, Dir dir, out int mappedColor)
//    {
//        mappedColor = -1;
//        if (cell == null || cell.dirWhitColor == null || cell.dirWhitColor.Count == 0) return false;

//        want = DirToVec(dir);
//        if (want.sqrMagnitude == 0) return false;
//        want.Normalize();

//        foreach (var kvp in cell.dirWhitColor)
//        {
//            var k = kvp.Key;
//            if (k.sqrMagnitude == 0) continue;
//            float dot = Vector3.Dot(k.normalized, want);
//            if (dot >= dirDotTolerance)
//            {
//                mappedColor = kvp.Value;
//                return true;
//            }
//        }
//        return false;
//    }

//    // An edge consumes a block iff its direction key matches the swipe direction and the mapped color equals the block color.
//    bool CanConsumeAtEdge(CellSpring edgeCell, BlockColor block, Dir dir)
//    {
//        if (edgeCell == null || block == null) return false;
//        int colorForDir;
//        if (!TryGetColorForDirection(edgeCell, dir, out colorForDir)) return false;
//        return colorForDir == block.colorBlockIndex;
//    }

//    void MoveBlocks(Dir dir)
//    {
//        int cols = grid.gridCells.cols;
//        int rows = grid.gridCells.rows;

//        switch (dir)
//        {
//            case Dir.Right:
//                for (int y = 0; y < rows; y++)
//                    ProcessLine(cols, i => grid.gridCells[i, y], cols - 1, -1, dir);
//                break;

//            case Dir.Left:
//                for (int y = 0; y < rows; y++)
//                    ProcessLine(cols, i => grid.gridCells[i, y], 0, +1, dir);
//                break;

//            case Dir.Up:
//                for (int x = 0; x < cols; x++)
//                    ProcessLine(rows, i => grid.gridCells[x, i], rows - 1, -1, dir);
//                break;

//            case Dir.Down:
//                for (int x = 0; x < cols; x++)
//                    ProcessLine(rows, i => grid.gridCells[x, i], 0, +1, dir);
//                break;
//        }
//    }

//    // Core line processing with edge-consume chaining and staggered placement
//    void ProcessLine(int length, System.Func<int, CellSpring> getCellAt, int edgeIndex, int stepTowardEdge, Dir dir)
//    {
//        // 1) Collect from edge toward interior
//        List<(CellSpring cell, BlockColor block)> blocks = new List<(CellSpring, BlockColor)>(length);
//        for (int k = 0, i = edgeIndex; k < length; k++, i += stepTowardEdge)
//        {
//            var c = getCellAt(i);
//            if (c.isFull && c.currentblockColor != null)
//                blocks.Add((c, c.currentblockColor));
//        }

//        if (blocks.Count == 0) return;

//        // 2) Clear occupancy (we’ll re-place or destroy)
//        for (int k = 0, i = edgeIndex; k < length; k++, i += stepTowardEdge)
//        {
//            var c = getCellAt(i);
//            c.isFull = false;
//            c.currentblockColor = null;
//        }

//        // 3) Re-pack with edge consume chaining and stagger
//        int writeIndex = edgeIndex;   // where next kept block goes
//        int placedCount = 0;          // for stagger delays of placed blocks

//        while (blocks.Count > 0 && writeIndex >= 0 && writeIndex < length)
//        {
//            var entry = blocks[0];
//            blocks.RemoveAt(0);

//            var targetCell = getCellAt(writeIndex);
//            bool atEdge = (writeIndex == edgeIndex);

//            // Edge consume chaining: do NOT advance writeIndex on consume
//            if (atEdge && CanConsumeAtEdge(targetCell, entry.block, dir))
//            {
//                AudioManager.instance.Play("2");
//                AnimateAndDestroy(entry.block, targetCell, consumeDelayOverride);
//                continue;
//            }

//            // Place the block
//            targetCell.isFull = true;
//            targetCell.currentblockColor = entry.block;
//            entry.block.currentCell = targetCell;

//            var targetPos = targetCell.transform.position;

//            // Staggered delay for placed blocks only
//            float delay = placedCount * Mathf.Max(0f, staggerStep);

//            if (entry.block.transform.position != targetPos)
//            {
//                entry.block.transform
//                    .DOMove(targetPos, moveDuration)
//                    .SetEase(moveEase)
//                    .SetDelay(delay);
//            }

//            placedCount++;
//            writeIndex += stepTowardEdge;

//        }

//        // Safety: destroy any leftover unplaced blocks (shouldn't happen in compression)
//        //for (int i = 0; i < blocks.Count; i++)
//        //    Destroy(blocks[i].block.gameObject);
//    }

//    [SerializeField] float offsetX = 0.5f;
//    [SerializeField] Vector3 scaleFx = Vector3.one;



//    [Header("Shake")]
//    [SerializeField] float duration = 0.4f;
//    [SerializeField] float yStrength = 0.25f;     // how far to shake on Y (in world units)
//    [SerializeField] float scaleStrength = 0.25f;     // how far to shake on Y (in world units)
//    [SerializeField] int vibrato = 20;            // how many shakes
//    [SerializeField] float randomness = 90f;      // randomness of shake
//    [SerializeField] bool fadeOut = true;         // returns smoothly to start when true
//    [SerializeField] bool snapping = false;       // snap to integers (grid-like)

//    [Header("Trigger")]
//    [SerializeField] bool shakeOnStart = false;
//    void AnimateAndDestroy(BlockColor block, CellSpring cell, float extraDelay = 0f)
//    {

//        if (block == null) return;
//        block.transform
//            .DOMove(cell.activeWalls[0].transform.position, moveDuration)
//            .SetEase(moveEase)
//            .SetDelay(Mathf.Max(0f, extraDelay))
//            .OnComplete(() =>
//            {
//                cell.activeWalls[0].transform.GetChild(1).gameObject.SetActive(true);
//                // Spawn consume VFX
//                if (destroyVfxPrefab != null)
//                {
//                    Vector3 targetdir = new Vector3(want.normalized.x, 0, want.normalized.y);
//                    var vfx = Instantiate(destroyVfxPrefab, cell.transform.position + targetdir * offsetX, Quaternion.identity);
//                    ChangeFxMaterial(vfx, cell);
//                    vfx.transform.rotation = Quaternion.LookRotation(targetdir);
//                    vfx.transform.localScale = scaleFx;
//                    vfx.Play();

//                    if (vfxAutoDestroy)
//                    {
//                        var main = vfx.main;
//                        float life = main.duration;
//                        // Add a small buffer
//                        Destroy(vfx.gameObject, life + 0.25f);
//                    }
//                }

//                // Clean any lingering cell references
//                if (block.currentCell != null && block.currentCell.currentblockColor == block)
//                {
//                    block.currentCell.currentblockColor = null;
//                    block.currentCell.isFull = false;
//                }
//                cell.Shake(duration, yStrength, scaleStrength, vibrato, randomness, fadeOut);
//                StartCoroutine(Wait(cell, 0.5f));
//                Destroy(block.gameObject, 0.004f);
//            });
//    }



//    private void ChangeFxMaterial(ParticleSystem p, CellSpring cell)
//    {
//        p.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = new Material(GameManager.Instance.colorsCELL[cell.ColorIndex[cell.isCorner ? 1 : 0]]);
//        p.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().material = new Material(GameManager.Instance.colorsCELL[cell.ColorIndex[cell.isCorner ? 1 : 0]]);
//    }
//    IEnumerator Wait(CellSpring cell, float t)
//    {
//        yield return new WaitForSeconds(t);
//        cell.activeWalls[0].transform.GetChild(1).gameObject.SetActive(false);
//    }


//    [SerializeField] Transform finger;
//    [SerializeField] int mouseButton = 0;    // 0 = left
//    [SerializeField] float smoothTime = 0.06f; // smaller = snappier
//    [SerializeField] float maxSpeed = 100f;
//    Vector3 velocity; // for SmoothDamp
//    Camera cam => Camera.main;
//    void FingerFolower()
//    {
//        Vector3 m = Input.mousePosition;


//        Vector3 target = cam.ScreenToWorldPoint(m);
//        Physics.Raycast(target, Vector3.down, out RaycastHit hitInfo, 100f);

//        finger.position = hitInfo.point + Vector3.up * 3f;
//    }

//}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveController : MonoBehaviour
{
    [SerializeField] GridSpowner grid;

    // Input and timing
    [SerializeField] float swipeThreshold = 50f;
    [SerializeField] float moveDuration = 0.25f;
    [SerializeField] Ease moveEase = Ease.OutQuad;

    // Direction matching tolerance for cell.dirWhitColor keys (Vector3 -> color)
    // 0.99 ~= ~8 degrees, increase for stricter match
    [SerializeField] float dirDotTolerance = 0.99f;

    // VFX on consume
    [SerializeField] ParticleSystem destroyVfxPrefab;
    [SerializeField] bool vfxAutoDestroy = true;
    // Optional base delay before the first consume starts (for a brief settle)
    [SerializeField] float baseConsumeDelay = 0f;
    // Sequential delay between consumed blocks in the SAME line (pop-pop-pop)
    [SerializeField] float consumeStaggerStep = 0.08f;

    // Staggered delay for placed (kept) blocks in the line
    // delay for nth placed block = n * staggerStep
    [SerializeField] float staggerStep = 0.04f;

    // FX placement tuning
    [SerializeField] float offsetX = 0.5f;
    [SerializeField] Vector3 scaleFx = Vector3.one;

    // Shake for the cell/wall on consume
    [Header("Shake")]
    [SerializeField] float duration = 0.4f;
    [SerializeField] float yStrength = 0.25f;
    [SerializeField] float scaleStrength = 0.25f;
    [SerializeField] float scaleduration = 0.25f;
    //[SerializeField] int vibrato = 20;
    //[SerializeField] float randomness = 90f;
    //[SerializeField] bool fadeOut = true;
    //[SerializeField] bool snapping = false;

    // Finger follower (optional)
    [SerializeField] Transform finger;
    [SerializeField] float fingerHoverHeight = 3f;

    private Vector2 startMousePosition;
    private Vector2 endMousePosition;

    enum Dir { None, Left, Right, Up, Down }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Optional audio hook
            if (AudioManager.instance != null) AudioManager.instance.Play("1");
            startMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            endMousePosition = Input.mousePosition;
            var dir = DetectSwipeDirection(startMousePosition, endMousePosition, swipeThreshold);
            if (dir != Dir.None)
                MoveBlocks(dir);
        }

        //FingerFollower();
    }

    static Dir DetectSwipeDirection(Vector2 start, Vector2 end, float minDist)
    {
        var d = end - start;
        if (d.magnitude < minDist) return Dir.None;
        return Mathf.Abs(d.x) > Mathf.Abs(d.y)
            ? (d.x > 0 ? Dir.Right : Dir.Left)
            : (d.y > 0 ? Dir.Up : Dir.Down);
    }

    static Vector3 DirToVec(Dir d)
    {
        switch (d)
        {
            case Dir.Right: return Vector3.right;
            case Dir.Left: return Vector3.left;
            case Dir.Up: return Vector3.up;
            case Dir.Down: return Vector3.down;
            default: return Vector3.zero;
        }
    }

    // Cache of last requested direction vector (used by VFX orientation)
    Vector3 want = Vector3.zero;

    // Try to find a direction key in cell.dirWhitColor that matches the swipe direction (by dot),
    // and return the mapped color for that direction.
    bool TryGetColorForDirection(CellSpring cell, Dir dir, out int mappedColor)
    {
        mappedColor = -1;
        if (cell == null || cell.dirWhitColor == null || cell.dirWhitColor.Count == 0) return false;

        want = DirToVec(dir);
        if (want.sqrMagnitude == 0) return false;
        want.Normalize();

        foreach (var kvp in cell.dirWhitColor)
        {
            var k = kvp.Key;
            if (k.sqrMagnitude == 0) continue;
            float dot = Vector3.Dot(k.normalized, want);
            if (dot >= dirDotTolerance)
            {
                mappedColor = kvp.Value;
                return true;
            }
        }
        return false;
    }

    // Edge consumes a block if direction key matches and mapped color == block color
    bool CanConsumeAtEdge(CellSpring edgeCell, BlockColor block, Dir dir)
    {
        if (edgeCell == null || block == null) return false;
        int colorForDir;
        if (!TryGetColorForDirection(edgeCell, dir, out colorForDir)) return false;
        return colorForDir == block.colorBlockIndex;
    }

    void MoveBlocks(Dir dir)
    {
        int cols = grid.gridCells.cols;
        int rows = grid.gridCells.rows;

        switch (dir)
        {
            case Dir.Right:
                for (int y = 0; y < rows; y++)
                    ProcessLine(cols, i => grid.gridCells[i, y], cols - 1, -1, dir);
                break;

            case Dir.Left:
                for (int y = 0; y < rows; y++)
                    ProcessLine(cols, i => grid.gridCells[i, y], 0, +1, dir);
                break;

            case Dir.Up:
                for (int x = 0; x < cols; x++)
                    ProcessLine(rows, i => grid.gridCells[x, i], rows - 1, -1, dir);
                break;

            case Dir.Down:
                for (int x = 0; x < cols; x++)
                    ProcessLine(rows, i => grid.gridCells[x, i], 0, +1, dir);
                break;
        }
    }

    // Core line processing with:
    // - edge-consume chaining
    // - per-line sequential consume (baseConsumeDelay + n * consumeStaggerStep)
    // - staggered placement for remaining blocks
    void ProcessLine(int length, System.Func<int, CellSpring> getCellAt, int edgeIndex, int stepTowardEdge, Dir dir)
    {
        // 1) Collect from edge toward interior
        List<(CellSpring cell, BlockColor block)> blocks = new List<(CellSpring, BlockColor)>(length);
        for (int k = 0, i = edgeIndex; k < length; k++, i += stepTowardEdge)
        {
            var c = getCellAt(i);
            if (c.isFull && c.currentblockColor != null)
                blocks.Add((c, c.currentblockColor));
        }

        if (blocks.Count == 0) return;

        // 2) Clear occupancy (we’ll re-place or destroy)
        for (int k = 0, i = edgeIndex; k < length; k++, i += stepTowardEdge)
        {
            var c = getCellAt(i);
            c.isFull = false;
            c.currentblockColor = null;
        }

        // 3) Re-pack
        int writeIndex = edgeIndex;   // where next kept block goes
        int placedCount = 0;          // for movement stagger
        int consumedCount = 0;        // for sequential consume timing

        while (blocks.Count > 0 && writeIndex >= 0 && writeIndex < length)
        {
            var entry = blocks[0];
            blocks.RemoveAt(0);

            var targetCell = getCellAt(writeIndex);
            bool atEdge = (writeIndex == edgeIndex);

            // Edge consume chaining: do NOT advance writeIndex when consuming
            if (atEdge && CanConsumeAtEdge(targetCell, entry.block, dir))
            {
                if (AudioManager.instance != null) AudioManager.instance.Play("2");

                float consumeDelay = Mathf.Max(0f, baseConsumeDelay) + consumedCount * Mathf.Max(0f, consumeStaggerStep);
                AnimateAndDestroy(entry.block, targetCell, consumeDelay);
                consumedCount++;
                continue;
            }

            // Place the block
            targetCell.isFull = true;
            targetCell.currentblockColor = entry.block;
            entry.block.currentCell = targetCell;

            var targetPos = targetCell.transform.position;

            // Movement stagger for placed blocks only
            float moveDelay = placedCount * Mathf.Max(0f, staggerStep);

            if (entry.block.transform.position != targetPos)
            {
                entry.block.transform
                    .DOMove(targetPos, moveDuration)
                    .SetEase(moveEase)
                    .SetDelay(moveDelay);
            }

            placedCount++;
            writeIndex += stepTowardEdge;
        }
    }

    void AnimateAndDestroy(BlockColor block, CellSpring cell, float extraDelay = 0f)
    {
        if (block == null || cell == null) return;

        // Move block to the consume anchor (cell.activeWalls[0]) then VFX + destroy

        var anchor = (cell.activeWalls != null && cell.activeWalls.Count > 0) ? cell.activeWalls[0].transform : cell.transform;

        block.transform
            .DOMove(anchor.position, moveDuration)
            .SetEase(moveEase)
            .SetDelay(Mathf.Max(0f, extraDelay))
            .OnComplete(() =>
            {
                // Optional: mark wall state during effect
                //if (cell.activeWalls != null && cell.activeWalls.Count > 0 && cell.activeWalls[0].transform.childCount > 1)
                    

                // Spawn consume VFX
                if (destroyVfxPrefab != null)
                {
                    Vector3 targetdir = new Vector3(want.normalized.x, 0f, want.normalized.y);
                    var vfx = Instantiate(destroyVfxPrefab, cell.transform.position + targetdir * offsetX, Quaternion.identity);
                    ChangeFxMaterial(vfx, cell);
                    vfx.transform.rotation = Quaternion.LookRotation(targetdir);
                    vfx.transform.localScale = scaleFx;
                    vfx.Play();

                    if (vfxAutoDestroy)
                    {
                        var main = vfx.main;
                        float life = main.duration;
                        Destroy(vfx.gameObject, life + 0.25f);
                    }
                }

                // Clean any lingering cell references
                if (block.currentCell != null && block.currentCell.currentblockColor == block)
                {
                    block.currentCell.currentblockColor = null;
                    block.currentCell.isFull = false;
                }

                // Cell/Wall shake feedback
                //  cell.Shake(duration, yStrength, scaleStrength, vibrato, randomness, fadeOut);
                cell.BounceUpDown(cell, duration, scaleduration, yOffset, scaleStrength, ease);
                //StartCoroutine(WaitAndDisableWallFx(cell, 0.5f));
                Destroy(block.gameObject, 0.004f);
            });
    }
    [SerializeField] Ease ease = Ease.OutQuad;
    [SerializeField] float yOffset;
    private void ChangeFxMaterial(ParticleSystem p, CellSpring cell)
    {
        // Assumes GameManager.Instance.colorsCELL is an array of Materials
        int idx = cell.isCorner ? 1 : 0;
        var mat = new Material(GameManager.Instance.colorsCELL[cell.ColorIndex[idx]]);
        var r0 = p.transform.GetChild(0).GetComponent<ParticleSystemRenderer>();
        var r1 = p.transform.GetChild(1).GetComponent<ParticleSystemRenderer>();
        r0.material = mat;
        r1.material = new Material(mat);
    }

    IEnumerator WaitAndDisableWallFx(CellSpring cell, float t)
    {
        yield return new WaitForSeconds(t);
        if (cell.activeWalls != null && cell.activeWalls.Count > 0 && cell.activeWalls[0].transform.childCount > 1)
           cell.activeWalls[0].transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
    }

    // Finger follower (hovering over ground)
    void FingerFollower()
    {
        if (finger == null) return;
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 m = Input.mousePosition;
        // Cast a ray from camera to scene
        Ray ray = cam.ScreenPointToRay(m);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000f))
        {
            finger.position = hitInfo.point + Vector3.up * fingerHoverHeight;
        }
    }
}

/*
Expected types:

class GridSpowner
{
	public CellGrid gridCells; // provides cols, rows and indexer [x,y] -> CellSpring
}

class CellGrid
{
	public int cols;
	public int rows;
	public CellSpring this[int x, int y] { get { ... } }
}

class CellSpring : MonoBehaviour
{
	public bool isFull;
	public BlockColor currentblockColor;
	public bool isCorner;
	public List<int> ColorIndex; // two entries used by ChangeFxMaterial
	public List<Transform> activeWalls;

	// Direction -> color mapping (keys are directions, value is allowed color)
	public Dictionary<Vector3, int> dirWhitColor;

	// Called by controller for wall shake feedback
	public void Shake(float duration, float yStrength, float scaleStrength, int vibrato, float randomness, bool fadeOut)
	{
		// Implement: run DOShakeLocalPosition + DOShakeScale on activeWalls[0] and restore transforms on complete/kill.
	}
}

class BlockColor : MonoBehaviour
{
	public int colorBlockIndex;
	public CellSpring currentCell;
}
*/