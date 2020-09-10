using UnityEngine;
public class MovePill : MonoBehaviour
{
    private GameObject bottleParent;
    private Vector3 posBottles;
    private float distance;
    private bool dropped;
    private bool stored;

    //Bu script tüm hap prefablarında vardır. Hapların hareketini ve şişelere depolanmasını sağlamaktadır.
    private void Start()
    {
        dropped = false;
        stored = false;
        
        //bottleParent objesinin z eksenindeki pozisyonuna göre kameraya olan uzaklığı bulunur.
        bottleParent = GameObject.FindGameObjectWithTag("bottleParent");
        posBottles = bottleParent.transform.position;
        distance = Mathf.Abs(Camera.main.transform.position.z - posBottles.z);
    }
    private void OnMouseDown()
    {
        //Dokunulan hapın ilgili componentleri düzenlenir ve dikey durumuna getirilir.
        GetComponent<Rigidbody>().freezeRotation = true;
        GetComponent<CapsuleCollider>().isTrigger = true;
        if(tag != "bottle1Pill")
            transform.eulerAngles = new Vector3(0, 0, 90);
        else
            transform.eulerAngles = new Vector3(0, 0, 0);
    }
    private void OnMouseUp()
    {
        //Dokunmayı bıraktığımızda "dropped" değişkeni true değerini alır ve artık bu hap hareket edemez.
        if (!dropped && !stored)
        {
            dropped = true;

            //Bu hap aşağı düşerken aynı haptan tekrar tepsiye düşmesi için ilgili metotlar çağrılır.
            //Destroy(GetComponent<CapsuleCollider>());
            bottleParent.GetComponent<CreateManager>().FindPill(tag);
            Destroy(gameObject, 2);
        }
    }
    private void OnMouseDrag()
    {
        if (!dropped && !stored)
        {
            //Mouse pozisyonuna göre hapın hareketi sağlanır. Z ekseninde şişeler ile aynı  değere sahip olur.
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance));
            pos.z = posBottles.z;
            transform.position = pos;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //Bu hap şişelerin kapaklarında bulunan collidera değdiğinde "stored" değişkeni true değerini alır.
        //Hapta bulunan Rigidbody componenti sayesinde hap şişenin içine doğru düşmeye başlar.
        if (!dropped && other.gameObject.layer == 8)
            if(!other.GetComponentInParent<BottleManager>().pillConfirmed)
                stored = true; 
    }
    private void OnTriggerExit(Collider other)
    {
        if (stored && other.gameObject.layer == 8)
        {
            //Bu hap kapaktaki colliderdan çıktığında componentleri tekrardan düzenlenir.
            GetComponent<Rigidbody>().freezeRotation = false;
            GetComponent<CapsuleCollider>().isTrigger = false;
            transform.parent = other.transform.parent.transform;

            //Şişede bulunan hap sayısını kontrol eden metoda hap sayısı arttırılarak gönderilir.
            other.GetComponentInParent<BottleManager>().PillCountControl(++other.GetComponentInParent<BottleManager>().pillCount);
            if (other.tag == tag)
            {
                //Şişenin kapağının tagi ile bu hapın tagi aynı ise score 5 puan arttırılır.
                MoveManager.moveManagerClass.score += 5;
            }
            else
            {
                //Şişenin kapağının tagi ile bu hapın tagi farklı ise score 3 puan azaltılır. Bu hap tekrardan tepsiye düşer.
                MoveManager.moveManagerClass.score -= 3;
                bottleParent.GetComponent<CreateManager>().FindPill(tag);
            }
        }
    }
}