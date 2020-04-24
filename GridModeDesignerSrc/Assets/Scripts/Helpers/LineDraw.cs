#region INIT

using UnityEngine;

public class Line
{
    private Vector3 fromV;
    private Vector3 toV;

    private Transform fromT;
    private Transform toT;

    private bool fromIsVector;
    private bool toIsVector;

    private GameObject parent;
    private GameObject middle;
    private GameObject line;

    private bool shouldUpdate = false;

    #endregion

    #region SetUp

    public void SetUp(Vector3 from, Vector3 to, Color lineColor)
    {
        this.SetUpGOs(lineColor);
        this.fromV = from;
        this.toV = to;
        this.fromIsVector = true;
        this.toIsVector = true;
    }

    public void SetUp(Transform from, Transform to, Color lineColor)
    {
        this.SetUpGOs(lineColor);
        this.fromT = from;
        this.toT = to;
        this.fromIsVector = false;
        this.toIsVector = false;
    }

    public void SetUp(Vector3 from, Transform to, Color lineColor)
    {
        this.SetUpGOs(lineColor);
        this.fromV = from;
        this.toT = to;
        this.fromIsVector = true;
        this.toIsVector = false;
    }

    public void SetUp(Transform from, Vector3 to, Color lineColor)
    {
        this.SetUpGOs(lineColor);
        this.fromT = from;
        this.toV = to;
        this.fromIsVector = false;
        this.toIsVector = true;
    }

    /// Check why this.parent.transform.position = new Vector3(0, 1, 0);
    private void SetUpGOs(Color lineColor)
    {
        this.parent = new GameObject("LineParent");
        this.middle = new GameObject("Middle Parent");
        this.line = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

        this.middle.transform.SetParent(this.parent.transform);
        this.line.transform.localScale = new Vector3(0.1f, 1, 0.1f);
        this.line.transform.position = new Vector3(0, 0, 0);
        this.parent.transform.position = new Vector3(0, 1, 0);
        this.line.transform.SetParent(this.middle.transform);
        this.middle.transform.Rotate(new Vector3(1, 0, 0), -90);

        this.line.SetColor(lineColor);
        this.line.SetShader();
    }

    #endregion

    #region PROPS

    private Vector3 From => this.fromIsVector ? this.fromV : this.fromT.position;

    private Vector3 To => this.toIsVector ? this.toV : this.toT.position;

    #endregion

    #region UPDATE

    public void Update(bool force)
    {
        if (this.shouldUpdate == false && force == false)
        {
            return;
        }

        if (this.toIsVector && this.fromIsVector && force == false)
        {
            return;
        }

        this.parent.transform.position = this.From;
        float dist = (this.To - this.From).magnitude;
        this.parent.transform.localScale = new Vector3(1, 1, dist / 2);
        this.parent.transform.LookAt(this.To);
    }

    #endregion

    #region INTERFACE

    public void StartUpdate()
    {
        this.shouldUpdate = true;
    }

    public void StopUpdate()
    {
        this.shouldUpdate = false;
    }

    public void SetInactive()
    {
        this.parent.SetActive(false);
    }

    public void SetActive()
    {
        this.parent.SetActive(true);
    }

    public void Destroy()
    {
        GameObject.Destroy(this.parent);
    }

    #endregion
}


