namespace Assets.Scripts.Helpers
{
    #region INIT

    using UnityEngine;

    public class Line
    {
        private Vector3 from;
        private Vector3 to;

        private GameObject parent;
        private GameObject middle;
        private Transform parentTransform;
        private string name;
        private float width;
        private GameObject prefab;
        public GameObject line;
        
        #endregion

        #region SetUp

        public void SetUp(Vector3 from, Vector3 to, Color color, Transform parent, string name, float width, GameObject prefab)
        {
            this.from = from;
            this.to = to;
            this.parentTransform = parent;
            this.name = name;
            this.width = width;
            this.prefab = prefab;
            this.SetUpGOs(color);
        }

        private void SetUpGOs(Color lineColor)
        {
            //float lineTwoSize = 0.1f;
            parent = new GameObject("LineParent");
            parent.transform.SetParent(this.parentTransform);
            parent.name = this.name;
            var middle = new GameObject("Middle Parent");
            this.middle = middle;
            this.line = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ///this.line = GameObject.Instantiate(this.prefab);
            line.name = "BEAM";
            //var line2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //line2.name = "ACCENT";
            //line2.transform.localScale = new Vector3(1, lineTwoSize, 1);
            //GameObject.Destroy(line.GetComponent<Collider>());
            middle.transform.SetParent(parent.transform);
            line.transform.position = new Vector3(0, 0, 0);
            line.transform.localScale = new Vector3(1, this.width, this.width); 
            parent.transform.position = new Vector3(0.5f, 0, 0);

            line.transform.SetParent(middle.transform);
            //line2.transform.SetParent(middle.transform);
            middle.transform.Rotate(new Vector3(0, 1, 0), 90);
            line.SetShader();
            line.SetColor(lineColor);
            //line2.SetColor(Color.red);
            //line2.SetShader();
            this.Snap();
            //line2.transform.position -= new Vector3(0, 0, 2);
        }

        public void MoveToNewX(float x)
        {
            this.from.x = x;
            this.to.x = x;
            this.Snap();
        }

        public void MoveToNewY(float y)
        {
            this.from.y = y;
            this.to.y = y;
            this.Snap();
        }

        private void Snap()
        {
            this.middle.transform.localScale = new Vector3((this.from - this.to).magnitude, 1, 1);
            parent.transform.position = this.from;
            parent.transform.LookAt(this.to);
        }

        #endregion

        #region INTERFACE

        public void Destroy()
        {
            GameObject.Destroy(this.parentTransform.gameObject);
            GameObject.Destroy(this.parent);
        }

        #endregion
    }
}
