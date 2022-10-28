using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class BF_MowingManager : MonoBehaviour
{
    public int precisionValue = 10;
    [Range(0f, 1f)]
    public float marginError = 0.2f;
    private GameObject grassGO;
    private Texture2D noGrassTex;
    private Vector4 noGrassCoordOffset = Vector4.zero;
    private float useVP = 0;
    private Bounds grassBounds;
    [HideInInspector] public List<Vector3> markersPos = new List<Vector3>();
    [HideInInspector] public int totalMarker = 0;
    private bool debugShown = false;

    public void CreateMowingMarker(bool isDebugShown, int precisionValue)
    {
        ClearMarkers();

        grassGO = this.gameObject;
        noGrassTex = (Texture2D)this.GetComponent<MeshRenderer>().sharedMaterial.GetTexture("_NoGrassTex");
        Texture2D newTex = null;
        if (noGrassTex != null)
        {
            newTex = duplicateTexture(noGrassTex);
            noGrassCoordOffset = this.GetComponent<MeshRenderer>().sharedMaterial.GetVector("_MainTex_ST");
        }
        useVP = this.GetComponent<MeshRenderer>().sharedMaterial.GetFloat("_UseVP");
        grassBounds = grassGO.GetComponent<MeshRenderer>().bounds;

        for (int i = 0; i < precisionValue; i++)
        {
            for (int j = 0; j < precisionValue; j++)
            {
                float lerpXValue = i / (float)precisionValue;
                float lerpZValue = j / (float)precisionValue;
                if (isDebugShown)
                {
                    debugShown = true;
                }

                Vector3 markerPosition = new Vector3(Mathf.Lerp(grassBounds.max.x, grassBounds.min.x, lerpZValue), 5, Mathf.Lerp(grassBounds.max.z, grassBounds.min.z, lerpXValue));
                int layerMask = 1 << 0;
                RaycastHit hit;
                if (Physics.Raycast(markerPosition , Vector3.down, out hit, 50, layerMask))
                {
                    if (hit.transform == this.transform)
                    {
                        if (useVP == 0)
                        {
                            if (newTex != null)
                            {
                                if (newTex.GetPixel(Mathf.RoundToInt((hit.textureCoord.x + noGrassCoordOffset.z) * 2048f * noGrassCoordOffset.x), Mathf.RoundToInt((hit.textureCoord.y + noGrassCoordOffset.w) * 2048f * noGrassCoordOffset.y)).r >= 0.2f)
                                {
                                    markersPos.Add(hit.point);
                                }
                            }
                            else
                            {
                                markersPos.Add(hit.point);
                            }
                        }
                        else
                        {
                            Mesh grassMesh = this.GetComponent<MeshFilter>().sharedMesh;

                            if (grassMesh.colors.Length == 0)
                            {
                                markersPos.Add(hit.point);
                            }
                            else
                            {
                                int triIndex = hit.triangleIndex;
                                int vertIndex1 = grassMesh.triangles[triIndex * 3 + 0];
                                if (vertIndex1 < grassMesh.colors.Length)
                                {
                                    if (grassMesh.colors[vertIndex1].g >= 0.2f)
                                    {
                                        markersPos.Add(hit.point);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        totalMarker = markersPos.Count;
    }

    public void changeDebugState(bool isShown)
    {
        debugShown = isShown;
    }

    private void OnDrawGizmos()
    {
        if (debugShown)
        {
            foreach (Vector3 pos in markersPos)
            {
                Gizmos.color = new Color(1, 1, 0, 0.75F);
                Gizmos.DrawWireSphere(pos, 0.3f);
            }
        }
    }

    private void ClearMarkers()
    {
        markersPos.Clear();
        markersPos.TrimExcess();
    }



    Texture2D duplicateTexture(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
}
