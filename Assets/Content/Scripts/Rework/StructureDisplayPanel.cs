using UnityEngine;
using UnityEngine.UIElements;

//[RequireComponent(IPanel)]
public abstract class DSDisplayPanel_old : MonoBehaviour, IDisp2, IMem2
{
    protected GameObject NodePrefab;
    protected GameObject LinkPrefab;
    protected bool bDead = false;

    // Create initial panel and all layouts
    public abstract void generatePanel();
    // Update values or positions of already built panel
    public abstract void updatePanel();
    public bool isVisible() { return this.gameObject.activeInHierarchy; }
    public void setVisible(bool _visible) { this.gameObject.SetActive(_visible); }
    public abstract int memSize();
    public abstract int memAddr();
    public bool memIsDead() { return bDead; }
    public void memMarkDead() { setVisible(false); bDead = true; }
}