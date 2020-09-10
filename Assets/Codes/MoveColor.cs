using UnityEngine;
public class MoveColor : MonoBehaviour
{
    private Vector3 posBottles;
    private GameObject objectClone;
    private float distance = 0;

    //Bu script 1. stepteki color objectlerde bulunur.
    private void Start()
    {
        //İlk olarak kamera ile şişelerin bulunduğu parent object arasındaki z eksenindeki mesafe hesaplanır.
        posBottles = GameObject.FindGameObjectWithTag("bottleParent").transform.position;
        distance = Mathf.Abs(Camera.main.transform.position.z - posBottles.z);
    }
    private void OnMouseDown()
    {
        //Bu scriptin bulunduğu objeye tıklandığında bu objeye ait bir clone oluşturur.
        objectClone = Instantiate(gameObject);
    }
    private void OnMouseDrag()
    {
        if (objectClone)
        {
            //Basılı tuttuğumuz sürece pos değeri hesaplanır ve transform işlemi yapılır.
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance));
            pos.z = posBottles.z;
            objectClone.transform.position = pos;
        }
    }
    private void OnMouseUp()
    {
        //Dokunma ya da mouse ile tıklama işlemi bittiğinde oluşturulan clone objecti silinir.
        Destroy(objectClone);
    }
}